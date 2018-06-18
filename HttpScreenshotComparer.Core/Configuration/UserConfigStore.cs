using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using HttpScreenshotComparer.Core.Utils;
using YamlDotNet.RepresentationModel;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace HttpScreenshotComparer.Core.Configuration
{
    public class UserConfigStore : IUserConfigStore
    {
        public UserConfig ReadUserConfig(string filePath)
        {
            var path = PathUtils.MapPath(filePath);

            using (var textReader = File.OpenText(path))
            {
                /*var yaml = new YamlStream();
                yaml.Load(textReader);*/

                var deserializer = new DeserializerBuilder()
                    .WithNamingConvention(new CamelCaseNamingConvention())
                    .Build();

                return deserializer.Deserialize<UserConfig>(textReader.ReadToEnd());
            }
        }
    }
}
