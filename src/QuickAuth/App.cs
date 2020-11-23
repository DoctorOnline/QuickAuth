using McMaster.Extensions.CommandLineUtils;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using QuickAuth.Commands;
using QuickAuth.Core.Abstractions;
using QuickAuth.Core.Models;
using QuickAuth.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using TextCopy;

namespace QuickAuth
{
    internal sealed class App : CommandLineApplication
    {
        private readonly CommandOption _clientName;
        private readonly CommonSettings _commonSettings;
        private readonly IClientsService _clientsService;
        private readonly IAuthService _authService;
        private readonly ILogger<App> _logger;

        public App(IClientsService clientsService, IAuthService authService, IOptions<CommonSettings> commonSettings, ILogger<App> logger, IEnumerable<ICommand> commands)
        {
            _clientsService = clientsService;
            _commonSettings = commonSettings.Value;
            _authService = authService;
            _logger = logger;

            foreach (var command in commands)
            {
                if (command is not CommandLineApplication commandLineApp)
                {
                    throw new InvalidCastException("Commands must inherit from ICommand and CommandLineApplication");
                }

                Commands.Add(commandLineApp);
            }

            _clientName = Option("-c | --client <name>",
                                    "A client name",
                                CommandOptionType.SingleValue);

            Name = "token";
            Description = "Gets a new access token for a specific client";
            HelpOption("-? | -h | --help");
            OnExecuteAsync(ExecuteAsync);
        }

        public async Task<int> ExecuteAsync(CancellationToken cancellationToken)
        {
            var clientName = _clientName.Value() ?? string.Empty;

            if (string.IsNullOrWhiteSpace(clientName))
            {
                ShowHelp(true);
                return 1;
            }

            var client = await _clientsService.GetClientAsync(clientName);

            if (client is null)
            {
                _logger.LogInformation("Client not found.");
                return 1;
            }

            try
            {
                var token = await _authService.GetTokenAsync(client);

                _logger.LogInformation("Token: {@token}", token);

                if (_commonSettings.CopyAccessTokenToClipboard)
                {
                    await ClipboardService.SetTextAsync($"Bearer {token.AccessToken}", cancellationToken);
                    _logger.LogInformation("Token has been copied to clipboard.");
                }

                return 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return ex.HResult;
            }
        }
    }
}