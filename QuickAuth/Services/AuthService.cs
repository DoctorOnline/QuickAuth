using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using QuickAuth.Core.Abstractions;
using QuickAuth.Core.Models;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace QuickAuth.Services
{
    internal sealed class AuthService : IAuthService
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public AuthService(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<TokenResponse> GetTokenAsync(Client client)
        {
            using var httpClient = _httpClientFactory.CreateClient();

            foreach (var header in client.Headers)
            {
                httpClient.DefaultRequestHeaders.Add(header.Key, header.Value);
            }

            var content = new StringContent(JsonConvert.SerializeObject(
                new { client.Username, client.Password }), 
                Encoding.UTF8, 
                "application/json"
            );

            var response = await httpClient.PostAsync(client.AuthUrl, content);

            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<TokenResponse>(responseContent);

                return result;
            }

            throw new Exception($"{(int)response.StatusCode}: {response.ReasonPhrase}");
        }
    }
}