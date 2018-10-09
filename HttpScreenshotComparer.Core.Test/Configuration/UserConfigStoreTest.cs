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
            var scriptFullPath = $"{currentPath}\\Configuration\\TestFiles\\example1.js";

            //Act
            var result = userConfigStore.ReadUserConfig(configPath);

            //Assert
            result.Should().NotBeNull();

            result.Urls.Should().NotBeNull()
                .And.HaveCount(2);
            result.Urls["url1"].Should().Be("/MyRelativePath1");
            result.Urls["url2"].Should().Be("/fr-fr/MyRelativePath1");

            result.TargetDirectory.Should().Be("d:\\temp\\shots");
            result.SourceDirectory.Should().Be("d:\\temp\\from");
            result.Fuzziness.Should().Be(20);
            result.HighlightColor.Should().Be("FF0000");

            result.ScreenWidth.Should().NotBeNull()
                .And.HaveCount(2);

            result.ScreenWidth[0].Should().Be(1280);
            result.ScreenWidth[1].Should().Be(800);

            result.ScriptFilePath.Should().Be("./example1.js");
            result.GalleryTemplate.Should().Be("./example1.cshtml");
            result.GalleryResult.Should().Be(@"d:\temp\gallery.html");

            result.Domain.Should().Be("www.test.com");

            result.BrowserAsEnum.Should().Be(BrowserEnum.Chrome);
            result.ScriptFileFullPath.Should().Be(scriptFullPath);
        }
    }
}
