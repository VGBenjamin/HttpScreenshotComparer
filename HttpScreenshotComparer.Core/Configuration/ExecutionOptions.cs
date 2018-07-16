using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using CommandLine;
using HttpScreenshotComparer.Core.Utils;

namespace HttpScreenshotComparer.Core.Configuration
{
    public class ExecutionOptions
    {
        [Option('c', "config", Required = true, HelpText = "The config file who contains your settings")]
        public string ConfigFile { get; set; }

        private string _configMappedPath;
        public string ConfigMappedPath => _configMappedPath ?? (_configMappedPath = PathUtils.MapPath(ConfigFile));

        [Option('r', "domain", Required = false, HelpText = "The domain to capture. If specify this will override the setting from the yaml config file.")]
        public string Domain { get; set; }

        [Option('m', "mode", Required = true, HelpText = "The mode of execution. It can be screenshot of compare")]
        public ExecutionMode Mode { get; set; }

        public bool IsValid(out List<string> errors)
        {
            errors = new List<string>();

            if (string.IsNullOrEmpty(ConfigFile))            
                errors.Add("The config file option is required");

            if(!File.Exists(ConfigMappedPath))
                errors.Add("The config file does not exist");

            if(string.IsNullOrEmpty(Domain))
                errors.Add("The domain is required");

            if (Mode == default(ExecutionMode))
                errors.Add("The execution mode is required");

            return errors.Any();
        }
    }

    public enum ExecutionMode   
    {
        Unknown,
        Screenshot,
        Compare
    }
}
