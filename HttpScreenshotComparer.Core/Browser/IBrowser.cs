using System;
using System.Collections.Generic;
using System.Text;

namespace HttpScreenshotComparer.Core.Browser
{
    public interface IBrowser
    {
        event EventHandler<BrowserResponseEventArgs> OutputDataReceived;
        event EventHandler<BrowserResponseEventArgs> ErrorDataReceived;

        void ExecuteScript(string scriptPath, IScriptArguments arguments);
    }
}
