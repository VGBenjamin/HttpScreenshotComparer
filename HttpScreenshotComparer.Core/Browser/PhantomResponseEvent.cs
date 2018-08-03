using System;
using System.Collections.Generic;
using System.Text;

namespace HttpScreenshotComparer.Core.Browser
{
    public class BrowserResponseEventArgs : EventArgs
    {
        public string Response { get; set; }

        public BrowserResponseEventArgs(string response)
        {
            Response = response;
        }
    }
}
