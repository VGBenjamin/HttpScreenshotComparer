using HttpScreenshotComparer.Core.Configuration;
using Microsoft.Extensions.Logging;

namespace HttpScreenshotComparer.Core.Browser
{
    public class NodeJsScriptBrowser : BrowserBase
    {
        public NodeJsScriptBrowser(IConfigurationStore configurationManager, ILogger<BrowserBase> logger)
            : base(configurationManager, logger)
        {
        }

        protected override string GetFilePath() => "Node";
    }
}