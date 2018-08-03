using System;
using System.Collections.Generic;
using System.Text;
using HttpScreenshotComparer.Core.Configuration;
using Microsoft.Extensions.Logging;

namespace HttpScreenshotComparer.Core.Browser
{
    public class BrowserFactory : IBrowserFactory
    {
        private readonly IConfigurationStore _configurationManager;
        private readonly ILogger<BrowserBase> _logger;

        public BrowserFactory(IConfigurationStore configurationManager, ILogger<BrowserBase> logger)
        {
            _configurationManager = configurationManager;
            _logger = logger;
        }

        public IBrowser GetBrowserFromConfig(IUserConfig userConfig)
        {
            switch (userConfig.BrowserAsEnum)
            {
                case BrowserEnum.Chrome:
                    return new ChromeHeadless(_configurationManager, _logger);
                case BrowserEnum.Phantom:
                    return new Phantom(_configurationManager, _logger);
                default:
                    throw new ArgumentException($"The argument {nameof(userConfig.BrowserAsEnum)} cannot is not set to a supported value ({userConfig.BrowserAsEnum}).");
            }
        }
    }
}
