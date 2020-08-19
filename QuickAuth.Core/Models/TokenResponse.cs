using System;

namespace QuickAuth.Core.Models
{
    public sealed class TokenResponse
    {
        public string AccessToken { get; set; } = string.Empty;

        public string RefreshToken { get; set; } = string.Empty;

        public DateTime ExpireDate { get; set; }
    }
}