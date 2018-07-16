using System;
using System.Collections.Generic;
using System.Text;
using FluentAssertions;
using HttpScreenshotComparer.Core.Configuration;
using Xunit;

namespace HttpScreenshotComparer.Core.Test.Engine
{
    public class EngineTest
    {
        #region GetUrlFromUserConfig

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public void GetUrlFromUserConfig_WithEmptyFilePath(string filePath)
        {
            //Assign
            var engine = new Core.Engine.Engine(null, null, null, null, null);

            //Act
            var result = engine.GetUrlTupleFromUserConfig(null, filePath);

            //Assert
            result.Should().Be("#");
        }

        [Fact]
        public void GetUrlFromUserConfig_WithNullUserConfig()
        {
            //Assign
            var engine = new Core.Engine.Engine(null, null, null, null, null);

            //Act
            var ex = Assert.Throws<ArgumentNullException>(() => engine.GetUrlTupleFromUserConfig(null, "fddfddfdf"));

            //Assert
            ex.Should().NotBeNull();
            ex.ParamName.Should().Be("userConfig");
        }

        [Theory]
        [InlineData("url1", "http://www.url1.com")]
        [InlineData("url2", "http://www.url2.com")]
        [InlineData("url5", null)]
        public void GetUrlFromUserConfig_WithFilePath(string searchedUrl, string expected)
        {
            //Assign
            var engine = new Core.Engine.Engine(null, null, null, null, null);
            var userConfig = new UserConfig()
            {
                Urls = new Dictionary<string, string>()
                {
                    {"url1", "http://www.url1.com"},
                    {"url2", "http://www.url2.com"},
                    {"url3", "http://www.url3.com"},
                }
            };

            //Act
            var result = engine.GetUrlTupleFromUserConfig(userConfig, searchedUrl);

            //Assert
            result?.Value.Should().Be(expected);
        }
        #endregion
    }
}
