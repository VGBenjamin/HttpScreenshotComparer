namespace HttpScreenshotComparer.Core.GalleryGenerator
{
    public interface IRazorRenderer
    {
        string Render<T>(string viewPath, T model);
        void RenderAndSave<T>(string viewPath, T model, string targetFile);
    }
}