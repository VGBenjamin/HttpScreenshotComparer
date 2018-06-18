using System;
using System.Collections.Generic;
using System.Text;

namespace HttpScreenshotComparer.Core.Browser
{
    public class PhantomResponseEventArgs : EventArgs
    {
        public string Response { get; set; }

        public PhantomResponseEventArgs(string response)
        {
            Response = response;
        }
    }
}
