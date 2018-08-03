using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using CommandLine;
using CommandLine.Text;
using HttpScreenshotComparer.Core.Browser;
using HttpScreenshotComparer.Core.Configuration;
using HttpScreenshotComparer.Core.Engine;
using HttpScreenshotComparer.Core.GalleryGenerator;
using HttpScreenshotComparer.Core.Image;
using HttpScreenshotComparer.Core.Logging;
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
        private static ExecutionOptions _userOptions;

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

                //var userConfigStore = serviceProvider.GetService<IUserConfigStore>();
                //var userConfig = userConfigStore.ReadUserConfig();

                var logger = serviceProvider.GetService<ILogger<Program>>();

                try
                {
                    var engine = serviceProvider.GetService<IEngine>();
                    engine.Run(_userOptions);
                }
                catch (Exception ex)
                {
                    logger.LogError(EventIds.GeneralUnexpectedException, ex, "Unexpected exception");
                    throw;
                }

                NLog.LogManager.Shutdown();
            }

            Console.WriteLine("END!");
            Console.ReadKey();
        }

        private static void HandleParseError(IEnumerable<Error> errors)
        {
            if (errors?.Count() > 0)
            {
                Console.WriteLine($"The command parameters are incorrect. Errors: ");
                foreach (var error in errors)
                {
                    Console.WriteLine(error);                    
                }

                _isError = true;
            }            
        }

        private static void RunOptionsAndReturnExitCode(ExecutionOptions opts)
        {
            _userOptions = opts;
        }

        private static IServiceProvider ConfigureServices(ServiceCollection services, IConfiguration config)
        {
            //Runner is the custom class
            services.AddSingleton<ILoggerFactory, LoggerFactory>();
            services.AddSingleton(typeof(ILogger<>), typeof(Logger<>));

            services.AddSingleton<IConfigurationStore, ConfigurationStore>();
            services.AddSingleton<IUserConfigStore, UserConfigStore>();
            services.AddSingleton<IConfiguration>(config);
            services.AddSingleton<IEngine, Engine>();
            services.AddSingleton<IBrowser, Phantom>();
            services.AddSingleton<IRazorRenderer, RazorRenderer>();
            services.AddSingleton<IImageComparer, ImageComparer>();
            services.AddSingleton<IImageResizer, ImageResizer>();
            services.AddSingleton<IBrowserFactory, BrowserFactory>();

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
