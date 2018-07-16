namespace HttpScreenshotComparer.Core.Configuration
{
    public interface IUserConfigStore
    {
        IUserConfig ReadUserConfig(string filePath);
    }
}