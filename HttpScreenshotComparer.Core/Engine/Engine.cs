using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HttpScreenshotComparer.Core.Browser;
using HttpScreenshotComparer.Core.Configuration;
using HttpScreenshotComparer.Core.GalleryGenerator;
using HttpScreenshotComparer.Core.Image;
using Microsoft.Extensions.Logging;

namespace HttpScreenshotComparer.Core.Engine
{
    public class Engine
    {
        private readonly IImageComparer _imageComparer;
        private readonly IBrowser _browser;
        private readonly IConfigurationStore _configurationStore;
        private readonly IUserConfigStore _userConfigStore;
        private readonly ILogger _logger;

        public Engine(IImageComparer imageComparer, IBrowser browser, IConfigurationStore configurationStore, IUserConfigStore userConfigStore, ILogger logger)
        {
            _imageComparer = imageComparer;
            _browser = browser;
            _configurationStore = configurationStore;
            _userConfigStore = userConfigStore;
            _logger = logger;
        }

        public void Run(ExecutionOptions options)
        {
            if (options == null)
                throw new ArgumentException("The options parameter is required", nameof(options));

            List<string> errors;
            if (!options.IsValid(out errors))
            {
                var error = errors.Aggregate((current, next) => $"{current}{Environment.NewLine}{next}");
                throw new ArgumentException($"Some options are not valid. Errors: {error}", nameof(options));
            }

            var userConfig = _userConfigStore.ReadUserConfig(options.ConfigMappedPath);

            if (options.Mode == ExecutionMode.Screenshot)
                TakeScreenshots(options, userConfig);
            else if (options.Mode == ExecutionMode.Compare)
                CompareScreenshots(options, userConfig);            
        }

        protected void CompareScreenshots(ExecutionOptions options, IUserConfig userConfig)
        {
            var galleryModel = new GalleryModel
            {
                SourceDirectory = userConfig.SourceDirectory,
                TargetDirectory = userConfig.TargetDirectory,                
            };

            var parallelOptions = new ParallelOptions()
            {
                MaxDegreeOfParallelism = userConfig.NumberOfThreads <= 0 ? 10 : userConfig.NumberOfThreads
            };
            
            Parallel.ForEach(Directory.GetFiles(userConfig.SourceDirectory), parallelOptions, (sourceFile) =>
            {
                var targetFile = sourceFile.Replace(userConfig.SourceDirectory, userConfig.TargetDirectory);
                var diffFile = sourceFile.Replace(userConfig.SourceDirectory, userConfig.ResultDirectory);

                //Check if the diff file already exist and delete it if necessary
                CheckIfDiffFileExistAndDeleteItIsNecessary(diffFile);

                if (File.Exists(targetFile))
                {
                    //Compare the images
                    var percentageOfDifference = _imageComparer.Compare(sourceFile, targetFile, userConfig.Fuzziness, diffFile, userConfig.HighlightColor);

                    var urlTuple = GetUrlTupleFromUserConfig(userConfig, sourceFile);

                    galleryModel.Lines.Add(new GalleryLine()
                    {
                        DifferenceRate = percentageOfDifference,
                        DifferencesImage = diffFile,
                        SourceImage = sourceFile,
                        TargetImage = targetFile,
                        Name = urlTuple?.Key,
                        Url = urlTuple?.Value
                    });
                }
                else
                {
                    _logger.LogError($"The file '{sourceFile}' exist in the source but is not present in the target: '{targetFile}'");
                }
            });

            //Create the galleries
        }

        internal KeyValuePair<string, string>? GetUrlTupleFromUserConfig(IUserConfig userConfig, string fileFullPath)
        {
            if (string.IsNullOrEmpty(fileFullPath))
                return null;

            if(userConfig == null)
                throw new ArgumentNullException(nameof(userConfig));

            var fileName = Path.GetFileName(fileFullPath);

            return userConfig.Urls?.FirstOrDefault(keyValue =>
                keyValue.Key.Equals(fileName, StringComparison.InvariantCultureIgnoreCase));
        }

        private void CheckIfDiffFileExistAndDeleteItIsNecessary(string diffFile)
        {
            if (File.Exists(diffFile))
            {
                try
                {
                    File.Delete(diffFile);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Cannot delete the file: '{diffFile}' to save the new result");
                    throw;
                }
            }
        }

        //Take the screenshots from the browser
        protected void TakeScreenshots(ExecutionOptions options, IUserConfig userConfig)
        {
            var parallelOptions = new ParallelOptions()
            {
                MaxDegreeOfParallelism = userConfig.NumberOfThreads <= 0 ? 10 : userConfig.NumberOfThreads
            };
            
            Parallel.ForEach(userConfig.Urls, parallelOptions, (url) =>
            {
                foreach (var width in userConfig.ScreenWidth)
                {
                    var argument = new ScriptArguments()
                    {
                        ScreenWidth = width,
                        UrlName = url.Key
                    };
                    _browser.ExecuteScript(url.Value, argument); //TODO : add the parameters
                }
                
            });
        }
    }
}
