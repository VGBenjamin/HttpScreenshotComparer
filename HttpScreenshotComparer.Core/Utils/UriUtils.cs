using System;
using System.Collections.Generic;
using System.Text;

namespace HttpScreenshotComparer.Core.Utils
{
    public static class UriUtils
    {
        public static Uri CombineUri(string baseUri, string relativeOrAbsoluteUri)
        {
            return new Uri(new Uri(baseUri), relativeOrAbsoluteUri);
        }

        public static string CombineUriToString(string baseUri, string relativeOrAbsoluteUri)
        {
            return new Uri(new Uri(baseUri), relativeOrAbsoluteUri).ToString();
        }
    }
}
