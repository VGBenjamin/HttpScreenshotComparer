using System;
using System.Diagnostics;
using System.IO;
using HttpScreenshotComparer.Core.Configuration;
using Microsoft.Extensions.Logging;

namespace HttpScreenshotComparer.Core.Browser
{
    public abstract class BrowserBase : IBrowser
    {
        protected readonly IConfigurationStore _configurationManager;
        protected readonly ILogger<BrowserBase> _logger;

        public event EventHandler<BrowserResponseEventArgs> OutputDataReceived;
        public event EventHandler<BrowserResponseEventArgs> ErrorDataReceived;

        protected BrowserBase(IConfigurationStore configurationManager, ILogger<BrowserBase> logger)
        {
            _configurationManager = configurationManager;
            _logger = logger;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="scriptPath"></param>
        /// <param name="arguments">Arguments separated by a space character</param>
        public void ExecuteScript(string scriptPath, IScriptArguments arguments)
        {
            try
            {
                var process = new System.Diagnostics.Process();
                var startInfo = new System.Diagnostics.ProcessStartInfo
                {
                    WindowStyle = System.Diagnostics.ProcessWindowStyle.Maximized,
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    FileName = GetFilePath(),
                    Arguments = $"{scriptPath} {arguments.ToString()}"
                };

                EnsureTargetFolderExist(arguments.TargetPath);

                //* Set your output and error (asynchronous) handlers
                process.OutputDataReceived += new DataReceivedEventHandler(HandleOutputData);
                process.ErrorDataReceived += (sender, args) => ErrorDataReceived?.Invoke(this, new BrowserResponseEventArgs(args.Data));

                _logger.LogDebug($"Calling executable: {startInfo.FileName} {startInfo.Arguments}");

                process.StartInfo = startInfo;
                process.Start();

                process.BeginOutputReadLine();
                process.BeginErrorReadLine();
                process.WaitForExit();
            }
            catch (Exception ex)
            {
                _logger.LogError("Error when calling phantomjs", ex);
                throw;
            }

        }

        protected virtual void EnsureTargetFolderExist(string argumentsTargetPath)
        {
            var folder = Path.GetDirectoryName(argumentsTargetPath);
            if (!Directory.Exists(folder))
            {
                _logger.LogDebug($"Creating the target directory: {folder}");
                Directory.CreateDirectory(folder);
            }
        }

        protected abstract string GetFilePath();

        protected void HandleOutputData(object sender, DataReceivedEventArgs args)
        {
            OutputDataReceived?.Invoke(this, new BrowserResponseEventArgs(args.Data));
        }
    }
}