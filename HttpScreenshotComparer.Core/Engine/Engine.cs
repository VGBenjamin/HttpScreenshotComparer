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

        public void Run()
        {
            //Take the screenshots from the browser

            //Compare the images

            //Create the galleries
        }
    }
}
