using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using FluentAssertions;
using HttpScreenshotComparer.Core.Utils;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Internal;
using Moq;
using Xunit;

namespace HttpScreenshotComparer.Core.Test.Utils
{
    public class DirectoryUtilsTest
    {
        #region DirectoryUtil
        [Theory]
        [InlineData("c:\\temp\\hello", "")]
        [InlineData("c:\\temp\\hello-#yyyyMMdd-hhmm#", "yyyyMMdd-hhmm")]
        public void GetPatternPart_Test(string path, string expected)
        {
            //Assign
            var directoryUtils = new DirectoryUtils(null, null);
            //Act
            var result = directoryUtils.GetPatternPart(path);
            //Assert
            result.Should().Be(expected);
        }
        #endregion


        #region GetDirectoryDateTimeFromPattern
        public static IEnumerable<object[]> GetDirectoryDateTimeFromPattern_Test_Data()
        {           
            yield return new object[] { null, null, default(DateTime) };
            yield return new object[] { string.Empty, string.Empty, default(DateTime) };
            yield return new object[] { "c:\\temp\\hello-20180905-0740", "hello-#yyyyMMdd-hhmm#", new DateTime(2018, 9, 5, 7, 40, 0) };
            yield return new object[] { "c:\\temp\\hello-20180905-074005-bla", "hello-#yyyyMMdd-hhmmss#-bla", new DateTime(2018, 9, 5, 7, 40, 5) };
            yield return new object[] { "c:\\temp\\hello-20180905-bla", "hello-#yyyyMMdd#-bla", new DateTime(2018, 9, 5) };
        }

        [Theory]
        [MemberData(nameof(GetDirectoryDateTimeFromPattern_Test_Data))]
        public void GetDirectoryDateTimeFromPattern_Test(string path, string pattern, DateTime expected)
        {
            //Assign
            /*var directoryProvider = new Mock<DirectoryProvider>();
            directoryProvider.Setup(dp => dp.GetSubDirectories(It.IsAny<string>()))
                .Returns(new string[]
                {
                    "c:\\temp\\hello-20180830",
                    "c:\\temp\\20180831",
                    "c:\\temp\\hello-20180831"
                });
            var directoryUtils = new DirectoryUtils(directoryProvider.Object);*/
            var logger = new Mock<ILogger<DirectoryUtils>>();
            /*logger.Setup(l => l.LogDebug(It.IsAny<string>()))
                .Verifiable();
            logger.Setup(l => l.LogDebug(It.IsAny<string>()));*/

            var directoryUtils = new DirectoryUtils(null, null);

            //Act
            var result = directoryUtils.GetDirectoryDateTimeFromPattern(path, pattern);

            //Assert
            result.Should().Be(expected);
        }

        [Fact]
        public void GetDirectoryDateTimeFromPattern_PatternDoesNotMatch()
        {
            //Assign
            string path = "c:\\temp\\hello-20180905-bla";
            string pattern = "hello-#yyyyMMdd#-XXX";
            var expected = DateTime.MinValue;
            
            var logger = new Mock<ILogger<DirectoryUtils>>();
            /*logger.Setup(l => l.Log(It.Is(l => l == LogLevel.Debug), It.IsAny<EventId>(), It.IsAny<FormattedLogValues>(), It.IsAny<Exception>(), It.IsAny<Func<FormattedLogValues, Exception, string>>()))
                .Verifiable();
      */
            var directoryUtils = new DirectoryUtils(null, logger.Object);

            //Act
            var result = directoryUtils.GetDirectoryDateTimeFromPattern(path, pattern);

            //Assert
            result.Should().Be(expected);
            //logger.Verify();
        }

        [Fact]
        public void GetDirectoryDateTimeFromPattern_DateNotParsable()
        {
            //Assign
            string path = "c:\\temp\\hello-201809-bla";
            string pattern = "hello-#yyyyMMdd#-XXX";
            var expected = DateTime.MinValue;

            var logger = new Mock<ILogger<DirectoryUtils>>();
            /*logger.Setup(l => l.LogError(It.IsAny<string>()))
                .Verifiable();*/

            var directoryUtils = new DirectoryUtils(null, logger.Object);

            //Act
            var result = directoryUtils.GetDirectoryDateTimeFromPattern(path, pattern);

            //Assert
            result.Should().Be(expected);
            //logger.Verify();
        }
        #endregion

        #region ToRegexExpression
        [Theory]
        [InlineData("d", "1", true)]
        [InlineData("d", "01", false)]
        [InlineData("d", "10", true)]
        [InlineData("d", "12", true)]
        [InlineData("d", "30", true)]
        [InlineData("d", "31", true)]
        [InlineData("d", "32", false)]
        [InlineData("d", "40", false)]

        [InlineData("dd", "01", true)]
        [InlineData("dd", "11", true)]
        [InlineData("dd", "21", true)]
        [InlineData("dd", "1", false)]
        [InlineData("dd", "40", false)]

        public void ToRegexExpression_Test(string pattern, string input, bool shouldWork)
        {
            //Assign
            var directoryUtils = new DirectoryUtils(null, null);
            //Act
            var regex = new Regex(directoryUtils.PatternToRegex(pattern));

            //Assert
            regex.IsMatch(input).Should().Be(shouldWork, $"{input} should { (shouldWork ? "" : "not") } match the regex: {regex}");
        }

        #endregion

        [Fact]
        public void GetLatestDirectoryFromPattern_Test()
        {
            //Assign
            var logger = new Mock<ILogger<DirectoryUtils>>();

            var directoryProvider = new Mock<DirectoryProvider>();
            directoryProvider.Setup(dp => dp.GetSubDirectories(It.IsAny<string>()))
                .Returns(new string[]
                {
                    "c:\\temp\\hello-20180830",
                    "c:\\temp\\20180831",
                    "c:\\temp\\hello-20180831"
                });
            var directoryUtils = new DirectoryUtils(directoryProvider.Object, logger.Object);

            string pattern = "hello-#yyyyMMdd#";

            //Act
            var latestDirectory = directoryUtils.GetLatestDirectoryFromPattern("c:\\temp", pattern);

            //Assert
            latestDirectory.Should().Be("c:\\temp\\hello-20180831");
        }

        [Fact]
        public void GetLatestDirectoryFromPattern_TestPathAndPattern()
        {
            //Assign
            var directoryUtils = new Mock<DirectoryUtils>(null, null);
            directoryUtils.Setup(m => m.GetLatestDirectoryFromPattern(It.IsAny<string>(), It.IsAny<string>()))
                .Returns((string d, string a) => string.Empty);
                

            //Act
            directoryUtils.Object.GetLatestDirectoryFromPattern("c:\\temp\\hello-#YYYYMMdd");

            //Assert
            directoryUtils.Verify(m => m.GetLatestDirectoryFromPattern(It.IsAny<string>(), It.IsAny<string>()), Times.Once);

        }

        #region GenerateDirectoryFromPattern

        [Fact]
        public void GenerateDirectoryFromPattern_WithPattern()
        {
            //Assign
            var directoryUtils = new DirectoryUtils(null, null);

            //Act
            var result = directoryUtils.GenerateDirectoryFromPattern("c:\\temp\\hello-#yyyyMMdd#");

            //Assert
            result.Should().Be($"c:\\temp\\hello-{DateTime.Now.ToString("yyyyMMdd")}");
        }

        [Fact]
        public void GenerateDirectoryFromPattern_WithoutPattern()
        {
            //Assign
            var directoryUtils = new DirectoryUtils(null, null);

            //Act
            var result = directoryUtils.GenerateDirectoryFromPattern("c:\\temp\\hello");

            //Assert
            result.Should().Be($"c:\\temp\\hello");
        }
        #endregion
    }
}
