using System;
using System.Collections.Generic;
using System.Linq;
using CommandLine;
using CommandLine.Text;
using HttpScreenshotComparer.Core.Browser;
using HttpScreenshotComparer.Core.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;

namespace HttpScreenshotComparer
{
    class Program
    {
        /// <summary>
        /// https://github.com/commandlineparser/commandline
        /// </summary>
        

        private static bool _isError = false;
        private static ExecutionOptions UserOptions;

        static void Main(string[] args)
        {
            CommandLine.Parser.Default.ParseArguments<ExecutionOptions>(args)
                    .WithParsed<ExecutionOptions>(opts => RunOptionsAndReturnExitCode(opts))
                    .WithNotParsed<ExecutionOptions>((errs) => HandleParseError(errs));

            if (!_isError)
            {
                // Adding all environment variables into IConfiguration.
                IConfiguration config = new ConfigurationBuilder()
                    .AddJsonFile("appsettings.json")
                    // .AddEnvironmentVariables()
                    .Build();

                // create service collection
                var serviceCollection = new ServiceCollection();
                var serviceProvider = ConfigureServices(serviceCollection, config);

                var userConfigStore = serviceProvider.GetService<IUserConfigStore>();
                var userConfig = userConfigStore.ReadUserConfig(UserOptions.ConfigFile);

                var scriptPath = "D:\\Progz\\phantom2.5\\node_modules\\phantomjs25-beta\\lib\\phantom\\bin\\cap.js";

                var phantom = serviceProvider.GetRequiredService<Phantom>();
                phantom.OutputDataReceived += (sender, eventArgs) =>
                {
                    Console.WriteLine(eventArgs.Response);
                };

                phantom.ErrorDataReceived += (sender, eventArgs) =>
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine(eventArgs.Response);
                    Console.ResetColor();
                };

                phantom.ExecuteScript(scriptPath, new ScriptArguments()
                {
                    
                });
            }
          
            Console.WriteLine("END!");
            Console.ReadKey();
        }

        private static void HandleParseError(IEnumerable<Error> errs)
        {
            if (errs?.Count() > 0)
            {
                Console.WriteLine($"The command parameters are incorrect. Errors: ");
                foreach (var error in errs)
                {
                    Console.WriteLine(error);                    
                }

                _isError = true;
            }            
        }

        private static void RunOptionsAndReturnExitCode(ExecutionOptions opts)
        {
            UserOptions = opts;
        }

        private static IServiceProvider ConfigureServices(ServiceCollection services, IConfiguration config)
        {
            //Runner is the custom class
            services.AddTransient<Phantom>();

            services.AddSingleton<ILoggerFactory, LoggerFactory>();
            services.AddSingleton(typeof(ILogger<>), typeof(Logger<>));
            services.AddSingleton<IConfigurationStore, ConfigurationStore>();
            services.AddSingleton<IUserConfigStore, UserConfigStore>();
            services.AddSingleton<IConfiguration>(config);

            services.AddLogging((builder) => builder.SetMinimumLevel(LogLevel.Trace));

            var serviceProvider = services.BuildServiceProvider();

            var loggerFactory = serviceProvider.GetRequiredService<ILoggerFactory>();

            //configure NLog
            loggerFactory.AddNLog(new NLogProviderOptions { CaptureMessageTemplates = true, CaptureMessageProperties = true });
            NLog.LogManager.LoadConfiguration("nlog.config");

            return serviceProvider;


        }
    }
}
