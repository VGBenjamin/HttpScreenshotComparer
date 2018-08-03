using HttpScreenshotComparer.Core.Configuration;

namespace HttpScreenshotComparer.Core.Browser
{
    public interface IBrowserFactory
    {
        IBrowser GetBrowserFromConfig(IUserConfig userConfig);
    }
}