using System.Collections.Generic;

namespace QuickAuth.Core.Models
{
    public sealed class Client
    {
        public string AuthUrl { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;

        public IDictionary<string, string> Headers { get; set; } = new Dictionary<string, string>();
    }
}