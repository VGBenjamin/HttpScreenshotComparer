using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

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
    }
}
