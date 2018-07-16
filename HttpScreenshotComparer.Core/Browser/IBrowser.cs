using System;
using System.Collections.Generic;
using System.Text;

namespace HttpScreenshotComparer.Core.Browser
{
    public interface IBrowser
    {
        void ExecuteScript(string scriptPath, IScriptArguments arguments);
    }
}
