using System.IO;

namespace HttpScreenshotComparer.Core.Utils
{
    public class DirectoryProvider : IDirectoryProvider
    {
        public virtual string[] GetSubDirectories(string path) => Directory.GetDirectories(path);
    }
}