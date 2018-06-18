namespace HttpScreenshotComparer.Core.Configuration
{
    public interface IUserConfigStore
    {
        UserConfig ReadUserConfig(string filePath);
    }
}