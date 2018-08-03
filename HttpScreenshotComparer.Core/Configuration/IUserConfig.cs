using System.Collections.Generic;

namespace HttpScreenshotComparer.Core.Configuration
{
    public interface IUserConfig
    {
        string Domain { get; set; }
        int Fuzziness { get; set; }
        string GalleryTemplate { get; set; }
        string HighlightColor { get; set; }
        int NumberOfThreads { get; set; }
        List<int> ScreenWidth { get; set; }
        string ScriptFilePath { get; set; }
        string ScriptFileFullPath { get; set; }

        string TargetDirectory { get; set; }
        string SourceDirectory { get; set; }
        string SourceDirectoryReplaced { get; }

        string ResultDirectory { get; set; }        
        Dictionary<string, string> Urls { get; set; }
        string TargetDirectoryReplaced { get; }

        string Browser { get; }
        BrowserEnum BrowserAsEnum { get; }

    }
}