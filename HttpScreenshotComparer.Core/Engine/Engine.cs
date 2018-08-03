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
using HttpScreenshotComparer.Core.Logging;
using HttpScreenshotComparer.Core.Utils;
using Microsoft.Extensions.Logging;

namespace HttpScreenshotComparer.Core.Engine
{
    public class Engine : IEngine
    {
        private readonly IImageComparer _imageComparer;
        private readonly IConfigurationStore _configurationStore;
        private readonly IUserConfigStore _userConfigStore;
        private readonly ILogger<Engine> _logger;
        private readonly IBrowserFactory _browserFactory;

        public Engine(IImageComparer imageComparer, IConfigurationStore configurationStore, IUserConfigStore userConfigStore, ILogger<Engine> logger, IBrowserFactory browserFactory)
        {
            _imageComparer = imageComparer;
            _configurationStore = configurationStore;
            _userConfigStore = userConfigStore;
            _logger = logger;
            _browserFactory = browserFactory;
        }

        public void Run(ExecutionOptions options)
        {
            _logger.LogTrace("Starting the engine...");

            if (options == null)
                throw new ArgumentException("The options parameter is required", nameof(options));

            List<string> errors;
            if (!options.IsValid(out errors))
            {
                var error = errors.Aggregate((current, next) => $"{current}{Environment.NewLine}{next}");
                throw new ArgumentException($"Some options are not valid. Errors: {error}", nameof(options));
            }

            IUserConfig userConfig;
            try
            {
                
                userConfig = _userConfigStore.ReadUserConfig(options.ConfigMappedPath);
            }
            catch (Exception ex)
            {
                _logger.LogError(EventIds.UserConfigReadException, ex, "Error when reading the user configuration yaml file");
                throw;
            }

            if (options.Mode == ExecutionMode.Screenshot)
                TakeScreenshots(options, userConfig);
            else if (options.Mode == ExecutionMode.Compare)
                CompareScreenshots(options, userConfig);            
        }

        protected void CompareScreenshots(ExecutionOptions options, IUserConfig userConfig)
        {
            _logger.LogDebug("Starting the compare mode");

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
            _logger.LogDebug("Starting the screenshot mode");

            var parallelOptions = new ParallelOptions()
            {
                MaxDegreeOfParallelism = userConfig.NumberOfThreads <= 0 ? 10 : userConfig.NumberOfThreads
            };

            var browser = _browserFactory.GetBrowserFromConfig(userConfig);
            
            Parallel.ForEach(userConfig.Urls, parallelOptions, (url) =>
            {
                _logger.LogInformation($"Screenshoting: '{url}'");

                foreach (var width in userConfig.ScreenWidth)
                {
                    var argument = new ScriptArguments()
                    {
                        ScreenWidth = width,
                        UrlName = url.Key,
                        Url =  UriUtils.CombineUriToString(userConfig.Domain, url.Value),
                        TargetPath = GetTargetPath(userConfig, url.Key, width)
                    };
                    browser.ErrorDataReceived += (sender, args) =>
                    {
                        _logger.LogError(EventIds.BrowserErrorMessage, $"BROWSER ERROR : {args.Response}");
                    };
                    browser.OutputDataReceived += (sender, args) =>
                    {
                        _logger.LogInformation(EventIds.BrowserErrorMessage, $"BROWSER : {args.Response}");
                    };
                    browser.ExecuteScript(userConfig.ScriptFileFullPath, argument);
                }                
            });
        }

        private string GetTargetPath(IUserConfig userConfig, string urlName, int width)
        {
            var filename = PathUtils.SanitizeFileName($"{urlName}_{width}.jpg");
            var filePath = Path.Combine(userConfig.TargetDirectoryReplaced, filename);
            return PathUtils.MapPath(filePath);
        }
    }
}
