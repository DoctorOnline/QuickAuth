using System;

namespace QuickAuth.Core.Models
{
    public sealed class CommonSettings
    {
        private string _clientConfigurationsPath = string.Empty;
        public string ClientConfigurationsPath
        {
            get
            {
                return _clientConfigurationsPath;
            }
            set
            {
                _clientConfigurationsPath = Environment.ExpandEnvironmentVariables(value);
            }
        }

        public bool CopyAccessTokenToClipboard { get; set; } = false;
    }
}