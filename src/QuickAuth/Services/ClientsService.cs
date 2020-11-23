using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using QuickAuth.Core.Abstractions;
using QuickAuth.Core.Models;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace QuickAuth.Services
{
    internal sealed class ClientsService : IClientsService
    {
        private readonly CommonSettings _settings;

        public ClientsService(IOptions<CommonSettings> settings)
        {
            _settings = settings.Value;
        }

        public async Task<Client?> GetClientAsync(string name)
        {
            CreateDirectoryIfNotExists(_settings.ClientConfigurationsPath);
            
            var filePath = Path.Join(_settings.ClientConfigurationsPath, $"{name}.json");

            if (!IsClientExists(filePath))
                return null;

            var result = await File.ReadAllTextAsync(filePath);
            
            return JsonConvert.DeserializeObject<Client>(result);
        }

        public Task<IReadOnlyList<string>> GetClientsAsync()
        {
            CreateDirectoryIfNotExists(_settings.ClientConfigurationsPath);

            var result = Directory.GetFiles(_settings.ClientConfigurationsPath, "*.json")
                .Select(s => s.Replace(_settings.ClientConfigurationsPath + "\\", "").Replace(".json", ""))
                .ToList();

            return Task.FromResult<IReadOnlyList<string>>(result);
        }

        private static bool IsClientsCatalogExists(string path) => Directory.Exists(path);

        private static bool IsClientExists(string path) => File.Exists(path);

        private static void CreateDirectoryIfNotExists(string path)
        {
            if (IsClientsCatalogExists(path))
                return;

            Directory.CreateDirectory(path);
        }
    }
}