using System;
using System.Collections.Generic;
using System.Text;

namespace HttpScreenshotComparer.Core.Configuration
{
    public class UserConfig
    {
        public string Inherit { get; set; }
        public List<UserConfigUrl> Urls { get; set; }
        public string TargetDirectory { get; set; }
        public int Fuzziness { get; set; }
        public List<int> ScreenWidth { get; set; }
        public string ScriptFilePath { get; set; }
    }

    public class UserConfigUrl
    {
        public string Name { get; set; }
        public string Url { get; set; }
    }

}
