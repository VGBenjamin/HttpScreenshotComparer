using System;
using System.Collections.Generic;
using System.Text;
using FluentAssertions;
using HttpScreenshotComparer.Core.Utils;
using Xunit;

namespace HttpScreenshotComparer.Core.Test.Utils
{
    public class UriUtilsTest
    {
        [Theory]
        [InlineData("http://www.my.domain/", "relative/path", "http://www.my.domain/relative/path")]
        [InlineData("http://www.my.domain", "relative/path", "http://www.my.domain/relative/path")]
        [InlineData("http://www.my.domain/something/other/", "relative/path", "http://www.my.domain/something/other/relative/path")]
        [InlineData("http://www.my.domain/something/other", "/absolute/path", "http://www.my.domain/absolute/path")]
        [InlineData("http://localhost:8091/something/other", "/absolute/path", "http://localhost:8091/absolute/path")]

        public void UriUtilsTestCombineUriToString(string basePath, string additionalPath, string expected)
        {
            //Assign
            //Act
            var result = UriUtils.CombineUriToString(basePath, additionalPath);
            //Assert
            result.Should().Be(expected);
        }
    }
}
