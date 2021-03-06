﻿using System;
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
        public IUserConfig ReadUserConfig(string filePath)
        {
            var path = PathUtils.MapPath(filePath);

            using (var textReader = File.OpenText(path))
            {
                /*var yaml = new YamlStream();
                yaml.Load(textReader);*/

                var deserializer = new DeserializerBuilder()
                    .WithNamingConvention(new CamelCaseNamingConvention())
                    //.WithTypeConverter(new UrlsTypeConverter())
                    .Build();

                var text = textReader.ReadToEnd();
                var deserialized = deserializer.Deserialize<UserConfig>(text);
                deserialized.ScriptFileFullPath =
                    Path.GetFullPath($"{Path.GetDirectoryName(filePath)}{PathUtils.MapPath(deserialized.ScriptFilePath)}");
                deserialized.GalleryTemplateFullPath =
                    Path.GetFullPath($"{Path.GetDirectoryName(filePath)}{PathUtils.MapPath(deserialized.GalleryTemplate)}");
                return deserialized;
            }
        }
    }
}
