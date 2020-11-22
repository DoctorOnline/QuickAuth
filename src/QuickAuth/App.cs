using McMaster.Extensions.CommandLineUtils;
using Microsoft.Extensions.Logging;
using QuickAuth.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace QuickAuth
{
    internal sealed class App : CommandLineApplication
    {
        public App(ILogger<App> logger, IEnumerable<ICommand> commands)
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

            OnExecuteAsync((ct) =>
            {
                ShowHelp(true);
                return Task.FromResult(0);
            });
        }
    }
}