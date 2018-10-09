namespace HttpScreenshotComparer.Core.Utils
{
    public interface IDirectoryUtils
    {
        string GetLatestDirectoryFromPattern(string pathWithPattern);

        string GetLatestDirectoryFromPattern(string path, string pattern);
        string GenerateDirectoryFromPattern(string pathWithPattern);
    }
}