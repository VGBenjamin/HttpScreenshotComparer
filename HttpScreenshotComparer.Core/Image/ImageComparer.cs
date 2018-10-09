using System;
using System.IO;
using System.Text.RegularExpressions;
using HttpScreenshotComparer.Core.Logging;
using ImageMagick;
using Microsoft.Extensions.Logging;

namespace HttpScreenshotComparer.Core.Image
{
    public class ImageComparer : IImageComparer
    {
        private readonly ILogger<ImageComparer> _logger;

        public ImageComparer(ILogger<ImageComparer> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Compare two image and save the result of the comparision
        /// </summary>
        /// <param name="sourcePath">The source Image path</param>
        /// <param name="targetPath">The target Image path</param>
        /// <param name="diffSavePath">The path where the result of the comparision will be saved. If null, the result will be not saved</param>
        /// <returns>Returns the percentage of differences between the two images</returns>
        public double Compare(string sourcePath, string targetPath, int fuzziness, string diffSavePath = null, string highlightColor = "FF0000")
        {
            if (string.IsNullOrEmpty(sourcePath))
                throw new ArgumentException("The parameter is required", nameof(sourcePath));

            if (string.IsNullOrEmpty(targetPath))
                throw new ArgumentException("The parameter is required", nameof(targetPath));

            if (fuzziness < 0)
                throw new ArgumentException("The parameter must be a positive integer", nameof(fuzziness));

            double distortionPercentage = -1;

            using (var magickSource = new MagickImage(sourcePath))
            using (var magickTarget = new MagickImage(targetPath))
            using (var diffImage = new MagickImage())
            {
                MagickColor magicHighlightColor;
                try
                {
                    highlightColor = Regex.Replace(highlightColor, "[0-9A-F]{3,6}", "#$0");
                    magicHighlightColor = new MagickColor(highlightColor);
                }
                catch
                {
                    _logger.LogError(EventIds.InvalidColor, $"The color '{highlightColor}' is not a valid color. You can use html colors withou prefixed or a color name.");
                    throw;
                }


                var compareSettigns = new CompareSettings()
                {
                    HighlightColor = magicHighlightColor,
                    //LowlightColor = new MagickColor("blue"),
                    //MasklightColor = new MagickColor("yellow"),
                    Metric = ErrorMetric.Absolute
                };

                if(fuzziness > 0)
                    magickSource.ColorFuzz = new Percentage(fuzziness);

                //Distortion = the number Of Pixel of differences
                var distortion = magickSource.Compare(magickTarget, compareSettigns, diffImage);
                var imageSize = magickSource.BaseHeight * magickSource.BaseWidth;
                distortionPercentage = (100.0 / imageSize) * distortion;

                if (!string.IsNullOrEmpty(diffSavePath))
                {
                    if (File.Exists(diffSavePath)) //If already exist remove it
                    {
                        File.Delete(diffSavePath);
                    }
                    diffImage.Write(diffSavePath);
                }
            }
            return distortionPercentage;
        }
    }
}
