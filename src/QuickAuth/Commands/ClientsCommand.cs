using McMaster.Extensions.CommandLineUtils;
using Microsoft.Extensions.Logging;
using QuickAuth.Core.Abstractions;
using QuickAuth.Interfaces;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace QuickAuth.Commands
{
    internal sealed class ClientsCommand : CommandLineApplication, ICommand
    {
        private readonly IClientsService _clientsService;
        private readonly ILogger<ClientsCommand> _logger;

        public ClientsCommand(IClientsService clientsService, ILogger<ClientsCommand> logger)
        {
            _clientsService = clientsService;
            _logger = logger;

            Name = "clients";
            Description = "Gets list of existing clients";
            HelpOption("-? | -h | --help");
            OnExecuteAsync(ExecuteAsync);
        }

        public async Task<int> ExecuteAsync(CancellationToken cancellationToken)
        {
            try
            {
                var clients = await _clientsService.GetClientsAsync();
                _logger.LogInformation("Clients: {@clients}", clients);
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