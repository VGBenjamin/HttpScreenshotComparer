using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using FluentAssertions;
using HttpScreenshotComparer.Core.Image;
using ImageMagick;
using Xunit;

namespace HttpScreenshotComparer.Core.Test.Image.Comparision
{
    public class ImageComparerTest
    {
        [Theory]
        [InlineData("Equals", "smile.bmp", "smile.bmp", 0, 0)]
        [InlineData("Green", "smile.bmp", "smile_green.bmp", 1.764, 1.764)]
        [InlineData("Sad", "smile.bmp", "sad.bmp", 50, 90)]
        [InlineData("Shifted", "smile.bmp", "smile_shifted.bmp", 50, 60)]
        [InlineData("fuzz_5_percent", "smile.bmp", "smile_fuzz.bmp", 0.5, 0.6, 5)]
        [InlineData("fuzz_11_ercent", "smile.bmp", "smile_fuzz.bmp", 0.07, 0.09, 11)]

        public void Compare_Images(string testName, string sourceImage, string targetImage, double expectedDifferenceMin, double expectedDifferenceMax, int fuzz = 0)
        {            

            //Assign
            var currentPath = Directory.GetCurrentDirectory();
            var source = $"{currentPath}\\Image\\TestFiles\\{sourceImage}";
            var target = $"{currentPath}\\Image\\TestFiles\\{targetImage}";
            var diff = $"{currentPath}\\Image\\TestFiles\\Result_{testName}.bmp";

            var imageComparer = new ImageComparer();

            //Act
            var difference = imageComparer.Compare(source, target, fuzz, diff);

            //Assert
            difference.Should().BeGreaterOrEqualTo(expectedDifferenceMin);
            difference.Should().BeLessOrEqualTo(expectedDifferenceMax);
        }


        [Theory]
        [InlineData(null, "$imagesPath$\\smile.bmp", 50, "$imagesPath$\\compare_result_argument.bmp")]
        [InlineData("$imagesPath$\\smile.bmp", null, 50, "$imagesPath$\\compare_result_argument.bmp")]
        [InlineData("$imagesPath$\\smile.bmp", "$imagesPath$\\smile.bmp", -1, "$imagesPath$\\compare_result_argument.bmp")]
        public void Compare_ArgumentsExceptions(string sourcePath, string targetPath, int fuzziness, string diffSavePath)
        {
            //Assign
            var currentPath = Directory.GetCurrentDirectory();
            var imagesPath = $"{currentPath}\\Image\\TestFiles";

            if (!string.IsNullOrEmpty(sourcePath))
                sourcePath = sourcePath.Replace("$imagesPath$", imagesPath);

            if (!string.IsNullOrEmpty(targetPath))
                targetPath = targetPath.Replace("$imagesPath$", imagesPath);

            if (!string.IsNullOrEmpty(diffSavePath))
                diffSavePath = diffSavePath.Replace("$imagesPath$", imagesPath);

            var imageComparer = new ImageComparer();
            
            //Act
            Exception ex = Assert.Throws<ArgumentException>(() => imageComparer.Compare(sourcePath, targetPath, fuzziness, diffSavePath));

            //Assert
            ex.Should().NotBeNull();
        }

        [Theory]

        [InlineData("$imagesPath$\\NOT_EXISTING.bmp", "$imagesPath$\\smile.bmp", 50, "$imagesPath$\\compare_result_argument.bmp")]
        [InlineData("$imagesPath$\\smile.bmp", "$imagesPath$\\NOT_EXISTING.bmp", 50, "$imagesPath$\\compare_result_argument.bmp")]
        public void Compare_IncorrectFilePaths(string sourcePath, string targetPath, int fuzziness, string diffSavePath)
        {
            //Assign
            var currentPath = Directory.GetCurrentDirectory();
            var imagesPath = $"{currentPath}\\Image\\TestFiles";

            if (!string.IsNullOrEmpty(sourcePath))
                sourcePath = sourcePath.Replace("$imagesPath$", imagesPath);

            if (!string.IsNullOrEmpty(targetPath))
                targetPath = targetPath.Replace("$imagesPath$", imagesPath);

            if (!string.IsNullOrEmpty(diffSavePath))
                diffSavePath = targetPath.Replace("$imagesPath$", imagesPath);

            var imageComparer = new ImageComparer();

            //Act
            Exception ex = Assert.Throws<MagickBlobErrorException>(() => imageComparer.Compare(sourcePath, targetPath, fuzziness, diffSavePath));

            //Assert
            ex.Should().NotBeNull();
        }
    }
}
