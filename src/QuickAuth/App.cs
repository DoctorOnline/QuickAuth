using McMaster.Extensions.CommandLineUtils;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
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
        private readonly IAuthService _authService;
        private readonly IClientsService _clientsService;
        private readonly CommonSettings _commonSettings;
        private readonly CommandOption _clientName;
        private readonly ILogger<App> _logger;

        public App(IAuthService authService, IClientsService clientsService, IOptions<CommonSettings> commonSettings, ILogger<App> logger, IEnumerable<ICommand> commands)
        {
            RegisterCommands(commands);

            _commonSettings = commonSettings.Value;
            _authService = authService;
            _clientsService = clientsService;
            _logger = logger;

            Description = "Gets a new token for a specific client";
            HelpOption("-? | -h | --help");

            _clientName = Option("-c | --client <name>",
                                "A client name.",
                                CommandOptionType.SingleValue);

            OnExecuteAsync(ExecuteAsync);
        }

        private void RegisterCommands(IEnumerable<ICommand> commands)
        {
            foreach (var command in commands)
            {
                var commandLineApp = command as CommandLineApplication;

                if (commandLineApp is null)
                {
                    throw new InvalidCastException("Commands must inherit from ICommand and CommandLineApplication");
                }

                Commands.Add(commandLineApp);
            }
        }

        public async Task<int> ExecuteAsync(CancellationToken cancellationToken)
        {
            var client = await _clientsService.GetClientAsync(_clientName.Value());

            if (client is null)
            {
                _logger.LogWarning("Client not found.");
                return 1;
            }

            try
            {
                var token = await _authService.GetTokenAsync(client);

                _logger.LogInformation("Token: {@token}", token);

                if (_commonSettings.CopyAccessTokenToClipboard)
                {
                    await ClipboardService.SetTextAsync($"Bearer {token.AccessToken}");
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