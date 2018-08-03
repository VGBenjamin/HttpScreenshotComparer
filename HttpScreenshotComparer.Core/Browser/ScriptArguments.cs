namespace HttpScreenshotComparer.Core.Browser
{
    public class ScriptArguments : IScriptArguments
    {
        public string UrlName { get; set; }
        public string Url { get; set; }
        public int ScreenWidth { get; set; }
        public string TargetPath { get; set; }
        public override string ToString() => $"'{TargetPath}' '{UrlName}' '{Url}' {ScreenWidth}";
    }
}