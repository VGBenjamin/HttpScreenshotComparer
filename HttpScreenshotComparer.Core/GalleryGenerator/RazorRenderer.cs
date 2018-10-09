using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using HttpScreenshotComparer.Core.Logging;
using Microsoft.Extensions.Logging;
using RazorLight;

namespace HttpScreenshotComparer.Core.GalleryGenerator
{
    /// <summary>
    /// This class allow you to personalize the html from an object
    /// </summary>
    public class RazorRenderer : IRazorRenderer
    {
        private readonly ILogger<RazorRenderer> _logger;

        public RazorRenderer(ILogger<RazorRenderer> logger)
        {
            _logger = logger;
        }

        public virtual string[] GetViewContent(string viewPath)
        {
            return System.IO.File.ReadAllLines(viewPath);
        }

        /// <summary>
        /// Render a view from a model by using the razor syntax
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="viewPath"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public  string Render<T>(string viewPath, T model)
        {
            if(string.IsNullOrEmpty(viewPath))
                throw new ArgumentNullException(nameof(viewPath));

            if(model == null)
                throw new ArgumentNullException(nameof(model));

            var key = $"{viewPath.ToLowerInvariant()}-{typeof(T)}";
            var engine = new RazorLightEngineBuilder()
                .UseMemoryCachingProvider()
                .Build();

            var cacheResult = engine.TemplateCache.RetrieveTemplate(key);
            string result;
            if (cacheResult.Success)
            {
                result = engine.RenderTemplateAsync(cacheResult.Template.TemplatePageFactory(), model).Result;
            }
            else
            {
                var template = string.Join("",
                    GetViewContent(viewPath)
                        .Where(l => !l.Contains(typeof(T).ToString())));//Remove @model in the template razor cshtml

                result = engine.CompileRenderAsync(key, template, model).Result;
            }

            return result;
        }

        public void RenderAndSave<T>(string viewPath, T model, string targetFile)
        {
            var result = Render(viewPath, model);
            if (string.IsNullOrEmpty(result))
            {
                _logger.LogError(EventIds.RazorRenderingResultIsEmpty, "The result of the razor rendering is null or empty", this);
            }
            else
            {
                try
                {
                    if (File.Exists(result))
                    {
                        File.Delete(result);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(EventIds.CannotDeleteOldGaleryFile, ex, $"Cannot delete the already existing file for the gallery: '{result}'", this);
                    throw;
                }

                try
                {                   
                    System.IO.File.WriteAllText(targetFile, result);
                }
                catch (Exception ex)
                {
                    _logger.LogError(EventIds.RazorRenderingResultIsEmpty, ex, $"Cannot save the result of the razor rendering to '{targetFile}'", this);
                    throw;
                }
            }
        }
    }
}
