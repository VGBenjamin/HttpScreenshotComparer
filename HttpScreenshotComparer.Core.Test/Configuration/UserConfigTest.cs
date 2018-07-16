using System;
using System.Collections.Generic;
using System.Text;
using FluentAssertions;
using HttpScreenshotComparer.Core.Configuration;
using Xunit;

namespace HttpScreenshotComparer.Core.Test.Configuration
{
    public class UserConfigTest
    {
        [Fact]
        public void SourceDirectoryReplacedWithDateToken()
        {
            //Assign
            var input = "c:\\temp\\hello-#yyyyMMdd#-coucou";
            var expected = $"c:\\temp\\hello-{DateTime.Now.ToString("yyyyMMdd")}-coucou";

            //Act
            var userConfig = new UserConfig();
            userConfig.SourceDirectory = input;

            //Assert
            userConfig.SourceDirectoryReplaced.Should().Be(expected);
        }

        [Fact]
        public void SourceDirectoryReplacedWithDateTwoToken()
        {
            //Assign
            var input = "c:\\temp\\hello-#yyyyMMdd#-coucou-#yyyy#";
            var expected = $"c:\\temp\\hello-{DateTime.Now.ToString("yyyyMMdd")}-coucou-{DateTime.Now.ToString("yyyy")}";

            //Act
            var userConfig = new UserConfig();
            userConfig.SourceDirectory = input;

            //Assert
            userConfig.SourceDirectoryReplaced.Should().Be(expected);
        }
    }
}
