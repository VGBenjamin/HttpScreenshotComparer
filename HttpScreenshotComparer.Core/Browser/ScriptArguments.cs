namespace HttpScreenshotComparer.Core.Browser
{
    public class ScriptArguments : IScriptArguments
    {
        public string UrlName { get; set; }
        public int ScreenWidth { get; set; }
        public override string ToString() => $"{UrlName} {ScreenWidth}";
    }
}