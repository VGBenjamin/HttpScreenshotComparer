using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Processing.Transforms;

namespace HttpScreenshotComparer.Core.Image
{
    public class ImageResizer
    {
        public void Resize(string sourcePath, string targetPath, int width, int height)
        {
            using (FileStream output = File.OpenWrite(targetPath))
            using (var image = SixLabors.ImageSharp.Image.Load(sourcePath))
            {
                image.Mutate(x => x.Resize(100, 100));
                image.Save(output, new JpegEncoder());
            }
        }
    }
}
