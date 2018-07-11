using System;
using System.Collections.Generic;
using System.Text;
using YamlDotNet.Serialization;

namespace HttpScreenshotComparer.Core.Configuration
{
    public class UserConfig
    {
        public string Inherit { get; set; }
        
        public Dictionary<string, string> Urls { get; set; }
        public string TargetDirectory { get; set; }
        public int Fuzziness { get; set; }
        public List<int> ScreenWidth { get; set; }
        public string ScriptFilePath { get; set; }
        public string HighlightColor { get; set; }
        public string GalleryTemplate { get; set; }
        public string Domain { get; set; }
    }

    public class UrlsList : List<UserConfigUrl>
    {
    }

    public class UserConfigUrl
    {
        public string Name { get; set; }
        public string Url { get; set; }
    }

}
