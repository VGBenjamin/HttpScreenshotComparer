using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using YamlDotNet.Serialization;

namespace HttpScreenshotComparer.Core.Configuration
{
    public class UserConfig : IUserConfig
    {
        //public string Inherit { get; set; }
        
        public Dictionary<string, string> Urls { get; set; }
        public string SourceDirectory { get; set; }

        private string _sourceDirectoryReplaced;
        public string SourceDirectoryReplaced 
            => _sourceDirectoryReplaced ?? (_sourceDirectoryReplaced = ReplaceTokens(SourceDirectory));

        protected string ReplaceTokens(string input)
        {
            return Regex.Replace(input, "#(?<token>.+?)#", match =>
            {
                return DateTime.Now.ToString(match.Groups["token"].Value);
            });
        }

        public string TargetDirectory { get; set; }

        private string _targetDirectoryReplaced;
        public string TargetDirectoryReplaced
            => _targetDirectoryReplaced ?? (_targetDirectoryReplaced = ReplaceTokens(TargetDirectory));

        public string ResultDirectory { get; set; }

        public int Fuzziness { get; set; }
        public List<int> ScreenWidth { get; set; }
        public string ScriptFilePath { get; set; }

        public string ScriptFileFullPath { get; set; }

        public string HighlightColor { get; set; }
        public string GalleryTemplate { get; set; }
        public string GalleryTemplateFullPath { get; set; }
        public string GalleryResult { get; set; }

        public string Domain { get; set; }
        public int NumberOfThreads { get; set; }

        private string _browser;
        public string Browser
        {
            get => _browser;
            set
            {
                BrowserEnum val = default(BrowserEnum);
                if (!Enum.TryParse(value, true, out val))
                {
                    throw new ArgumentException($"The value {value} is not a correct value for the parameter '{Browser}'");
                }
                BrowserAsEnum = val;
                _browser = value;
            }
        }
        public BrowserEnum BrowserAsEnum { get; private set; }
    }
}
