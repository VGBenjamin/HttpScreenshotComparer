namespace HttpScreenshotComparer.Core.Browser
{
    public interface IScriptArguments
    {
        string UrlName { get; set; }
        string Url { get; set; }
        int ScreenWidth { get; set; }    
        string TargetPath { get; set; }
        string ToString();
    }
}