using System;
using System.Collections.Generic;
using System.Text;
using HttpScreenshotComparer.Core.Browser;
using HttpScreenshotComparer.Core.Configuration;
using HttpScreenshotComparer.Core.Image;

namespace HttpScreenshotComparer.Core.Engine
{
    public class Engine
    {
        private readonly IImageComparer _imageComparer;
        private readonly IBrowser _browser;
        private readonly IConfigurationStore _configurationStore;

        public Engine(IImageComparer imageComparer, IBrowser browser, IConfigurationStore configurationStore)
        {
            _imageComparer = imageComparer;
            _browser = browser;
            _configurationStore = configurationStore;
        }

        public void Run(ExecutionOptions options)
        {
            if (options == null)
                throw new ArgumentException("The options parameter is required", nameof(options));

            if (string.IsNullOrEmpty(options.ConfigFile))
                throw new ArgumentException("The config file option is required", nameof(options));

            if (string.IsNullOrEmpty(options.ConfigFile))
                throw new ArgumentException("The config file option is required", nameof(options));


            //Take the screenshots from the browser

            //Compare the images

            //Create the galleries
        }
    }
}
