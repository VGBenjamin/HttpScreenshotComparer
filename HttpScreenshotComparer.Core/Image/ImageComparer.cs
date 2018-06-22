using System;
using System.IO;
using ImageMagick;

namespace HttpScreenshotComparer.Core.Image
{
    public class ImageComparer : IImageComparer
    {
        /// <summary>
        /// Compare two image and save the result of the comparision
        /// </summary>
        /// <param name="sourcePath">The source Image path</param>
        /// <param name="targetPath">The target Image path</param>
        /// <param name="diffSavePath">The path where the result of the comparision will be saved. If null, the result will be not saved</param>
        /// <returns>Returns the percentage of differences between the two images</returns>
        public double Compare(string sourcePath, string targetPath, int fuzziness, string diffSavePath = null, string highlightColor = "#FF0000")
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
                var compareSettigns = new CompareSettings()
                {
                    HighlightColor = new MagickColor(highlightColor),
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
