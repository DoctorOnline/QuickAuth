using McMaster.Extensions.CommandLineUtils;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using QuickAuth.Core.Abstractions;
using QuickAuth.Core.Models;
using QuickAuth.Interfaces;
using System;
using System.Threading;
using System.Threading.Tasks;
using TextCopy;

namespace QuickAuth.Commands
{
    internal sealed class TokenCommand : CommandLineApplication, ICommand
    {
        private readonly CommandOption _clientName;
        private readonly IClientsService _clientsService;
        private readonly IAuthService _authService;
        private readonly ILogger<ClientsCommand> _logger;

        private readonly CommonSettings _commonSettings;
        public TokenCommand(IAuthService authService, IOptions<CommonSettings> commonSettings, IClientsService clientsService, ILogger<ClientsCommand> logger)
        {
            _clientsService = clientsService;
            _commonSettings = commonSettings.Value;
            _authService = authService;
            _logger = logger;
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