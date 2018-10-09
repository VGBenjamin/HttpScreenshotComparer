using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using ImageMagick;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

namespace HttpScreenshotComparer.Core.Image
{
    public class ImageResizer : IImageResizer
    {
        public void Resize(string sourcePath, string targetPath, int width, int height)
        {
            if (string.IsNullOrEmpty(sourcePath))
                throw new ArgumentException("The parameter is required", nameof(sourcePath));

            if (string.IsNullOrEmpty(targetPath))
                throw new ArgumentException("The parameter is required", nameof(targetPath));

            if (width <= 0)
                throw new ArgumentException("The parameter must be a positive integer", nameof(width));

            if (height <= 0)
                throw new ArgumentException("The parameter must be a positive integer", nameof(height));

            using (var magickSource = new MagickImage(sourcePath))
            {
                magickSource.Resize(new MagickGeometry(width, height));
                if (File.Exists(targetPath)) //If already exist remove it
                {
                    File.Delete(targetPath);
                }

                magickSource.Write(targetPath);
            }
        }
    }
}
