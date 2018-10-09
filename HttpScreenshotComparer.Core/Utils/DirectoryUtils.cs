using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using HttpScreenshotComparer.Core.Logging;
using Microsoft.Extensions.Logging;

namespace HttpScreenshotComparer.Core.Utils
{
    public class DirectoryUtils : IDirectoryUtils
    {
        private readonly IDirectoryProvider _directoryProvider;
        private readonly ILogger<DirectoryUtils> _logger;

        private Dictionary<string, PatternDetails> _patternDetailsDictionnary = new Dictionary<string, PatternDetails>();

        class PatternDetails
        {
            public string DatePattern { get; set; }
            public Regex Regex { get; set; }
        }

        public DirectoryUtils(IDirectoryProvider directoryProvider, ILogger<DirectoryUtils> logger)
        {
            _directoryProvider = directoryProvider;
            _logger = logger;
        }

        public string GetLatestDirectoryFromPattern(string pathWithPattern)
        {
            var full = pathWithPattern;
            if (full.EndsWith('\\') || full.EndsWith('/'))
                full = full.Remove((full.Length - 1));

            var last = Path.GetFileName(full);

            return GetLatestDirectoryFromPattern(full.Remove(full.Length - last.Length), last);
        }

        public string GenerateDirectoryFromPattern(string pathWithPattern)
        {
            var regex = new Regex("^(.*)#(.+)#(.*)$");
            var match = regex.Match(pathWithPattern);
            if (!match.Success)
            {
                return pathWithPattern;
            }

            return $"{match.Groups[1].Value}{DateTime.Now.ToString(match.Groups[2].Value)}{match.Groups[3].Value}";
        }


        public virtual string GetLatestDirectoryFromPattern(string path, string pattern)
        {
            var directories = from directory in _directoryProvider.GetSubDirectories(path)
                            select new
                            {
                                Directory = directory,
                                Date = GetDirectoryDateTimeFromPattern(directory, pattern)
                            };

            return directories.OrderByDescending(d => d.Date)
                .FirstOrDefault()?.Directory;
        }

        internal string GetPatternPart(string path)
        {
            return Regex.Match(path, "#.+?#")?.Value?.Replace("#", string.Empty);
        }

        internal DateTime GetDirectoryDateTimeFromPattern(string directory, string pattern)
        {
            if (string.IsNullOrWhiteSpace(directory))
                return DateTime.MinValue;
            if (string.IsNullOrWhiteSpace(pattern))
                return DateTime.MinValue;

            PatternDetails patternDetails;
            if (_patternDetailsDictionnary.ContainsKey(pattern))
                patternDetails = _patternDetailsDictionnary[pattern];
            else
            {
                patternDetails = new PatternDetails();

                patternDetails.DatePattern = Regex.Replace(pattern, ".*?#(.+?)#.*", "$1");
                var patternWithoutDatePart = Regex.Replace(pattern, "#.+?#", "_________"); //First replace the date format to avoid the escape
                var escapedRegex = Regex.Escape(patternWithoutDatePart); //Escape the rest of the expression to avoid issues with the reserved characters
                var patternAsRegex = escapedRegex.Replace("_________", "(.+)");
                patternDetails.Regex = new Regex(patternAsRegex);

                _patternDetailsDictionnary.Add(pattern, patternDetails);
            }

            var match = patternDetails.Regex.Match(directory);
            if (!match.Success)
            {
                _logger.LogDebug(EventIds.FolderDateParsing, $"Skip the directory '{directory}' because it does not match the pattern: '{pattern}'");
                return DateTime.MinValue;
            }

            var dateString = match.Groups[1].Value;
            if (!DateTime.TryParseExact(dateString, patternDetails.DatePattern, CultureInfo.CurrentCulture, DateTimeStyles.None, out var directoryDate))
            {
                _logger.LogError(EventIds.FolderDateParsing, $"The directory '{directory}' match the pattern: '{pattern}' but the extracted date: '{dateString}' cannot be parsed with the format: '{patternDetails.DatePattern}'");
                return DateTime.MinValue;
            }

            return directoryDate;
        }

        internal string PatternToRegex(string pattern)
        {
            if (string.IsNullOrEmpty(pattern))
                return string.Empty;

            string regexPattern = string.Empty;
            string buffer = string.Empty;
            foreach (var character in pattern.ToCharArray())
            {
                if (character != buffer.LastOrDefault())
                {
                    regexPattern += ToRegexExpression(buffer);
                    buffer = string.Empty;
                }

                buffer += character;
            }
            regexPattern += ToRegexExpression(buffer);
            return $"^({regexPattern})$";
        }

        internal string ToRegexExpression(string patternChar)
        {
            switch (patternChar)
            {
                case "d":
                    return "([1-9])|([1-2][0-9])|(3[0-1])";
                case "dd":
                    return "[0-3][0-9]";
                case "ddd":
                    return GetAbbreviatedDaysOfWeekRegex();
                case "dddd":
                    return GetDaysOfWeekRegex();
                case "f":
                    return "[0-9]";
                case "ff":
                    return "[0-9]{2}";
                case "fff":
                    return "[0-9]{3}";
                case "ffff":
                    return "[0-9]{4}";
                case "fffff":
                    return "[0-9]{5}";
                case "ffffff":
                    return "[0-9]{6}";
                case "fffffff":
                    return "[0-9]{7}";
                case "ffffffff":
                    return "[0-9]{8}";
                case "F":
                    return "[1-9]?";
                case "FF":
                    return "([1-9][0-9])?";
                case "FFF":
                    return "[1-9][0-9]{2})?";
                case "FFFF":
                    return "[1-9][0-9]{3})?";
                case "FFFFF":
                    return "[1-9][0-9]{4})?";
                case "FFFFFF":
                    return "[1-9][0-9]{5})?";
                case "FFFFFFF":
                    return "[1-9][0-9]{6})?";
                case "FFFFFFFF":
                    return "[1-9][0-9]{7})?";
                case "h":
                    return "(1[0-2])|([0-9])";
                case "hh":
                    return "(1[0-2])|(0[0-9])";
                case "H":
                    return "(2[0-4])|(1?[0-9])";
                case "HH":
                    return "(2[0-4])|(0|1[0-9])";
                case "m":
                case "s":
                    return "[0-9]|([1-5][0-9])";
                case "mm":
                case "ss":
                    return "[0-5][0-9]";
                case "M":
                    return "[0-9]|(1[0-2])";
                case "MM":
                    return "(0[0-9])|(1[0-2])";
                case "MMM":
                    return GetAbbreviatedMonthRegex();
                case "MMMM":
                    return GetMonthRegex();
                case "y":
                    return "[1-9]?[0-9]";
                case "yy":
                    return "[0-9][0-9]";
                case "yyy":
                    return "[1-2]?[0-9]{3}";
                case "yyyy":
                    return "0[0-2][0-9]{3}";
                default:
                    return patternChar;
                    //TODO: K, g, gg, t, tt, z, zz, zzz

            }
        }

        private string GetAbbreviatedDaysOfWeekRegex()
        {
            return $"({CultureInfo.CurrentCulture.DateTimeFormat.GetAbbreviatedDayName(DayOfWeek.Tuesday)})|" +
                   $"({CultureInfo.CurrentCulture.DateTimeFormat.GetAbbreviatedDayName(DayOfWeek.Thursday)})|" +
                   $"({CultureInfo.CurrentCulture.DateTimeFormat.GetAbbreviatedDayName(DayOfWeek.Wednesday)})|" +
                   $"({CultureInfo.CurrentCulture.DateTimeFormat.GetAbbreviatedDayName(DayOfWeek.Friday)})|" +
                   $"({CultureInfo.CurrentCulture.DateTimeFormat.GetAbbreviatedDayName(DayOfWeek.Monday)})|" +
                   $"({CultureInfo.CurrentCulture.DateTimeFormat.GetAbbreviatedDayName(DayOfWeek.Saturday)})|" +
                   $"({CultureInfo.CurrentCulture.DateTimeFormat.GetAbbreviatedDayName(DayOfWeek.Sunday)})";
        }

        private string GetDaysOfWeekRegex()
        {
            return $"({CultureInfo.CurrentCulture.DateTimeFormat.GetAbbreviatedDayName(DayOfWeek.Tuesday)})|" +
                   $"({CultureInfo.CurrentCulture.DateTimeFormat.GetAbbreviatedDayName(DayOfWeek.Thursday)})|" +
                   $"({CultureInfo.CurrentCulture.DateTimeFormat.GetAbbreviatedDayName(DayOfWeek.Wednesday)})|" +
                   $"({CultureInfo.CurrentCulture.DateTimeFormat.GetAbbreviatedDayName(DayOfWeek.Friday)})|" +
                   $"({CultureInfo.CurrentCulture.DateTimeFormat.GetAbbreviatedDayName(DayOfWeek.Monday)})|" +
                   $"({CultureInfo.CurrentCulture.DateTimeFormat.GetAbbreviatedDayName(DayOfWeek.Saturday)})|" +
                   $"({CultureInfo.CurrentCulture.DateTimeFormat.GetAbbreviatedDayName(DayOfWeek.Sunday)})";
        }

        private string GetAbbreviatedMonthRegex()
        {
            string regex = string.Empty;
            for (int i = 1; i < 12; i++)
            {
                regex += $"({CultureInfo.CurrentCulture.DateTimeFormat.GetAbbreviatedMonthName(i)})|";
            }
            regex += $"({CultureInfo.CurrentCulture.DateTimeFormat.GetAbbreviatedMonthName(12)})";
            return regex;
        }

        private string GetMonthRegex()
        {
            string regex = string.Empty;
            for (int i = 1; i < 12; i++)
            {
                regex += $"({CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(i)})|";
            }
            regex += $"({CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(12)})";
            return regex;
        }
    }
}
