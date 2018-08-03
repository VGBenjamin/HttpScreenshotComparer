using System.Collections.Generic;
using System.Text;
using HttpScreenshotComparer.Core.Configuration;
using Microsoft.Extensions.Logging;

namespace HttpScreenshotComparer.Core.Browser
{
    public class ChromeHeadless : NodeJsScriptBrowser
    {
        public ChromeHeadless(IConfigurationStore configurationManager, ILogger<BrowserBase> logger)
        : base(configurationManager, logger)
        {
        }

    }

}
