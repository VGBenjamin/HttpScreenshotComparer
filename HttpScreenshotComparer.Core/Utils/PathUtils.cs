using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace HttpScreenshotComparer.Core.Utils
{
    public static class PathUtils
    {
        public static string MapPath(string path)
        {
            if (String.IsNullOrEmpty(path))
                return path;

            path = path.Replace("file:///", "");

            if (path.StartsWith(".") || path.StartsWith("~"))
                path = path.Remove(0, 1);

            if(path.StartsWith('\\'))
            {
                var assembly = System.Reflection.Assembly.GetEntryAssembly();
                var assemblyPath = new Uri(assembly.CodeBase).AbsolutePath;
                var root = System.IO.Path.GetDirectoryName(assemblyPath);

                path = Path.Combine(root, path);
            }

            return path;
        }

        /// <summary>
        /// Strip illegal chars and reserved words from a candidate filename (should not include the directory path)
        /// </summary>
        /// <remarks>
        /// http://stackoverflow.com/questions/309485/c-sharp-sanitize-file-name
        /// </remarks>
        public static string SanitizeFileName(string filename)
        {
            var invalidChars = Regex.Escape(new string(Path.GetInvalidFileNameChars()));
            var invalidReStr = string.Format(@"[{0}]+", invalidChars);

            var reservedWords = new[]
            {
                "CON", "PRN", "AUX", "CLOCK$", "NUL", "COM0", "COM1", "COM2", "COM3", "COM4",
                "COM5", "COM6", "COM7", "COM8", "COM9", "LPT0", "LPT1", "LPT2", "LPT3", "LPT4",
                "LPT5", "LPT6", "LPT7", "LPT8", "LPT9"
            };

            var sanitisedNamePart = Regex.Replace(filename, invalidReStr, "_");
            foreach (var reservedWord in reservedWords)
            {
                var reservedWordPattern = string.Format("^{0}\\.", reservedWord);
                sanitisedNamePart = Regex.Replace(sanitisedNamePart, reservedWordPattern, "_reservedWord_.", RegexOptions.IgnoreCase);
            }

            return sanitisedNamePart;
        }

    }
}
