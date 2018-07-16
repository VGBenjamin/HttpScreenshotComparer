using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using HttpScreenshotComparer.Core.Configuration;
using Microsoft.Extensions.Logging;

namespace HttpScreenshotComparer.Core.Browser
{
    public class Phantom : IBrowser
    {
        private readonly IConfigurationStore _configurationManager;
        private readonly ILogger<Phantom> _logger;

        //public delegate void SampleEventHandler(object sender, EventArgs e);
        public event EventHandler<PhantomResponseEventArgs> OutputDataReceived;
        public event EventHandler<PhantomResponseEventArgs> ErrorDataReceived;

        public Phantom(IConfigurationStore configurationManager, ILogger<Phantom> logger)
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
                    WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden,
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    FileName = _configurationManager.PhantomJsExePath,
                    Arguments = $"{scriptPath} {arguments.ToString()}"
                };

                //* Set your output and error (asynchronous) handlers
                process.OutputDataReceived += (sender, args) => OutputDataReceived?.Invoke(this, new PhantomResponseEventArgs(args.Data));
                process.ErrorDataReceived += (sender, args) => ErrorDataReceived?.Invoke(this, new PhantomResponseEventArgs(args.Data));

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
    }
}
