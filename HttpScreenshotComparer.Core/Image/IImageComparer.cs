namespace HttpScreenshotComparer.Core.Image
{
    public interface IImageComparer
    {
        double Compare(string sourcePath, string targetPath, int fuzziness, string diffSavePath = null, string highlightColor = "#FF0000");
    }
}