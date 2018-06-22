using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using YamlDotNet.Core;
using YamlDotNet.Core.Events;
using YamlDotNet.Serialization;

namespace HttpScreenshotComparer.Core.Configuration.Yaml
{

    internal sealed class UrlsTypeConverter : IYamlTypeConverter
    {
        private static readonly Type _contentCategoryNodeType = typeof(UrlsList);

        public bool Accepts(Type type)
        {
            return type == _contentCategoryNodeType;
        }

        public object ReadYaml(IParser parser, Type type)
        {
            if (!Accepts(type))
                return null;

            if (!(parser.Current is SequenceStart))
            {
                throw new InvalidDataException("Invalid YAML content.");
            }

            parser.MoveNext(); // skip the sequence start

            var urlList = new UrlsList();
            do
            {
                urlList.Add(this.GetScalarValue(parser));
                parser.MoveNext();
            } while (!(parser.Current is SequenceEnd));

            return urlList;
        }

        private UserConfigUrl GetScalarValue(IParser parser)
        {
            MappingStart mappingStart = parser.Current as MappingStart;

            if (mappingStart == null)
            {
                throw new InvalidDataException("Failed to retrieve scalar value.");
            }
 
            // You could replace the above null check with parser.Expect<Scalar> which will throw its own exception

            return new UserConfigUrl() {Name = "?", Url = "" };
        }

        public void WriteYaml(IEmitter emitter, object value, Type type)
        {
        }
    }
}
