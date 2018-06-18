using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using FluentAssertions;
using HttpScreenshotComparer.Core.Utils;
using Xunit;

namespace HttpScreenshotComparer.Core.Test.Utils
{
    public class PathUtilsTest
    {

        public static IEnumerable<object[]> GetPaths()
        {
            var currentPath = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);

            yield return new object[] { null, null };
            yield return new object[] { string.Empty, string.Empty };
            yield return new object[] { ".\\test.ps1", $"\\test.ps1" };
            yield return new object[] { "~\\test.ps1", $"\\test.ps1" };
            yield return new object[] { "\\test.ps1", $"\\test.ps1" };
            yield return new object[] { "c:\\test.ps1", $"c:\\test.ps1" };
        }

        [Theory]
        [MemberData(nameof(GetPaths))]
        public void MapPath_Test(string input, string expected)
        {
            //Assign
            //Act
            var result = PathUtils.MapPath(input);
            //Assert
            result.Should().Be(expected);
        }

    }
}
