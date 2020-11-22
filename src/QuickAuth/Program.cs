using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using QuickAuth.Core.Abstractions;
using QuickAuth.Core.Models;
using QuickAuth.Commands;
using QuickAuth.Services;
using Serilog;
using System;
using System.Threading.Tasks;
using QuickAuth.Interfaces;

namespace QuickAuth
{
    class Program
    {
        static async Task<int> Main(string[] args)
        {
            var configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddCommandLine(args)
                .Build();

            var serviceProvider = new ServiceCollection()
                .AddLogging()
                .AddHttpClient()
                .Configure<CommonSettings>(opt => configuration.GetSection(nameof(CommonSettings)).Bind(opt))
                .AddSingleton<ICommand, ClientsCommand>()
                .AddSingleton<IClientsService, ClientsService>()
                .AddSingleton<IAuthService, AuthService>()
                .AddSingleton<App>()
                .BuildServiceProvider();

            Log.Logger = new LoggerConfiguration()
              .MinimumLevel.Information()
              .WriteTo.LiterateConsole()
              .CreateLogger();

            var loggerFactory = serviceProvider.GetService<ILoggerFactory>();
            loggerFactory.AddSerilog();

            var app = serviceProvider.GetRequiredService<App>();
            var result = 1;

            try
            {
                result = await app.ExecuteAsync(args);
            }
            catch (Exception ex)
            {
                result = 0;
                Log.Logger.Error(ex.Message);
            }
#if DEBUG
            Console.ReadKey();
#endif
            return result;
        }
    }
}