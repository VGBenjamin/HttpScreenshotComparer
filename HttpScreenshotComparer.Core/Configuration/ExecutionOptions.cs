using System;
using System.Collections.Generic;
using System.Text;
using CommandLine;

namespace HttpScreenshotComparer.Core.Configuration
{
    public class ExecutionOptions
    {
        [Option('c', "config", Required = true, HelpText = "The config file who contains your settings")]
        public string ConfigFile { get; set; }


        [Option('r', "domain", Required = false, HelpText = "The domain to capture. If specify this will override the setting from the yaml config file.")]
        public string ReferenceDomain { get; set; }
    }
}
