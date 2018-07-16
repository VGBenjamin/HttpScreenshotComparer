namespace HttpScreenshotComparer.Core.Browser
{
    public interface IScriptArguments
    {
        string UrlName { get; set; }
        int ScreenWidth { get; set; }
        string ToString();
    }
}