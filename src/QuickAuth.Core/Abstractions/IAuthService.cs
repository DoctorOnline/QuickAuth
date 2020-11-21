using QuickAuth.Core.Models;
using System.Threading.Tasks;

namespace QuickAuth.Core.Abstractions
{
    public interface IAuthService
    {
        Task<TokenResponse> GetTokenAsync(Client client);
    }
}