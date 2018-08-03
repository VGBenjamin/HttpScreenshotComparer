using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using HttpScreenshotComparer.Core.Configuration;
using Microsoft.Extensions.Logging;

namespace HttpScreenshotComparer.Core.Browser
{
    public class Phantom : BrowserBase
    {
        public Phantom(IConfigurationStore configurationManager, ILogger<BrowserBase> logger)
            : base(configurationManager, logger)
        {
        }

        protected override string GetFilePath() => _configurationManager.PhantomJsExePath;
    }
}
