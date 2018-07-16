using System;
using System.Collections.Generic;
using System.Text;

namespace HttpScreenshotComparer.Core.GalleryGenerator
{
    public class GalleryModel
    {
        public string SourceDirectory { get; set; }
        public string TargetDirectory { get; set; }
        public List<GalleryLine> Lines { get; set; }
    }
    public class GalleryLine
    {
        public string Url { get; set; }
        public string SourceImage { get; set; }
        public string TargetImage { get; set; }
        public string DifferencesImage { get; set; }
        public double DifferenceRate { get; set; }
        public string Name { get; set; }
    }
}
