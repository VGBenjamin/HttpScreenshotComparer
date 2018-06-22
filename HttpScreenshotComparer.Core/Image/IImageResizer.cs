namespace HttpScreenshotComparer.Core.Image
{
    public interface IImageResizer
    {
        void Resize(string sourcePath, string targetPath, int width, int height);
    }
}