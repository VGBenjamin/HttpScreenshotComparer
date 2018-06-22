using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using FluentAssertions;
using HttpScreenshotComparer.Core.Image;
using ImageMagick;
using Xunit;

namespace HttpScreenshotComparer.Core.Test.Image
{
    public class ImageResizerTest
    {
        [Fact]
        public void Resize()
        {
            //Assign
            var currentPath = Directory.GetCurrentDirectory();
            var source = $"{currentPath}\\Image\\TestFiles\\smile.bmp";
            var target = $"{currentPath}\\Image\\TestFiles\\Result_resized.bmp";
            var imageResizer = new ImageResizer();

            //Act
            imageResizer.Resize(source, target, 50, 50);

            //Assert
            File.Exists(target).Should().BeTrue();

            using (var img = System.Drawing.Image.FromFile(target))
            {
                img.Height.Should().Be(50);
                img.Width.Should().Be(31);
            }
        }

        [Theory]
        [InlineData(null, "$imagesPath$\\result_argument.bmp", 50, 50)]
        [InlineData("$imagesPath$\\smile.bmp", null, 50, 50)]
        [InlineData("$imagesPath$\\smile.bmp", "$imagesPath$\\result_argument.bmp", 0, 50)]
        [InlineData("$imagesPath$\\smile.bmp", "$imagesPath$\\result_argument.bmp", -1, 50)]
        [InlineData("$imagesPath$\\smile.bmp", "$imagesPath$\\result_argument.bmp", 50, -1)]
        public void Resize_ArgumentsExceptions(string sourcePath, string targetPath, int width, int height)
        {
            //Assign
            var currentPath = Directory.GetCurrentDirectory();
            var imagesPath = $"{currentPath}\\Image\\TestFiles";

            if (!string.IsNullOrEmpty(sourcePath))
                sourcePath = sourcePath.Replace("$imagesPath$", imagesPath);

            if (!string.IsNullOrEmpty(targetPath))
                targetPath = targetPath.Replace("$imagesPath$", imagesPath);

            var imageResizer = new ImageResizer();

            //Act
            Exception ex = Assert.Throws<ArgumentException>(() => imageResizer.Resize(sourcePath, targetPath, width, height));

            //Assert
            ex.Should().NotBeNull();
        }

        [Theory]

        [InlineData("$imagesPath$\\NOT_EXISTING_FILE.bmp", "$imagesPath$\\result_argument.bmp", 50, 50)]
        public void Resize_IncorrectFilePaths(string sourcePath, string targetPath, int width, int height)
        { 
            //Assign
            var currentPath = Directory.GetCurrentDirectory();
            var imagesPath = $"{currentPath}\\Image\\TestFiles";

            if (!string.IsNullOrEmpty(sourcePath))
                sourcePath = sourcePath.Replace("$imagesPath$", imagesPath);

            if (!string.IsNullOrEmpty(targetPath))
                targetPath = targetPath.Replace("$imagesPath$", imagesPath);

            var imageResizer = new ImageResizer();

            //Act
            Exception ex = Assert.Throws<MagickBlobErrorException>(() => imageResizer.Resize(sourcePath, targetPath, width, height));

            //Assert
            ex.Should().NotBeNull();
        }
    }
}
