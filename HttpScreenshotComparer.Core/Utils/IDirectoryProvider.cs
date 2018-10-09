using System;
using System.Collections.Generic;
using System.Text;

namespace HttpScreenshotComparer.Core.Utils
{
    public interface IDirectoryProvider
    {
        string[] GetSubDirectories(string path);
    }
}
