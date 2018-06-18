using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using FluentAssertions;
using HttpScreenshotComparer.Core.Configuration;
using Xunit;

namespace HttpScreenshotComparer.Core.Test.Configuration
{
    public class UserConfigStoreTest
    {
        [Fact]
        public void ReadUserConfig_PlainConfig()
        {
            //Assign
            var userConfigStore = new UserConfigStore();
            var currentPath = Directory.GetCurrentDirectory();
            var configPath = $"{currentPath}\\Configuration\\TestFiles\\PlainConfig.yaml";

            //Act
            var result = userConfigStore.ReadUserConfig(configPath);

            //Assert
            result.Should().NotBeNull();

            result.Urls.Should().NotBeNull()
                .And.HaveCount(2);
            result.Urls[0].Name.Should().Be("url1");
            result.Urls[0].Url.Should().Be("/MyRelativePath1");

            result.TargetDirectory.Should().Be("d:\\temp\\shots");
            result.Fuzziness.Should().Be(20);

            result.ScreenWidth.Should().NotBeNull()
                .And.HaveCount(2);

            result.ScreenWidth[0].Should().Be(1280);
            result.ScreenWidth[1].Should().Be(800);

            result.ScriptFilePath.Should().Be("./example1.js");
        }
    }
}
