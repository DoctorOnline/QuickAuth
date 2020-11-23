using QuickAuth.Core.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace QuickAuth.Core.Abstractions
{
    public interface IClientsService
    {
        Task<IReadOnlyList<string>> GetClientsAsync();

        Task<Client?> GetClientAsync(string name);
    }
}