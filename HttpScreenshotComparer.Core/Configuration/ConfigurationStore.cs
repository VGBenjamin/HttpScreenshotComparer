using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Configuration;

namespace HttpScreenshotComparer.Core.Configuration
{
    public class ConfigurationStore : IConfigurationStore
    {
        private readonly IConfiguration _configuration;

        public ConfigurationStore(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string PhantomJsExePath => _configuration["httpScreenshotComparer:phantomjs:exePath"];
    }
}
