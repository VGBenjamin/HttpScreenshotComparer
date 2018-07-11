using System;
using System.Collections.Generic;
using System.Text;

namespace HttpScreenshotComparer.Core.GalleryGenerator
{
    public class GalleryModel
    {
        public string OriginalDomain { get; set; }
        public string TargetDomain { get; set; }
        public List<GalleryLine> Lines { get; set; }
    }
    public class GalleryLine
    {
        public string Url { get; set; }
        public string OriginalImage { get; set; }
        public string TargetImage { get; set; }
        public string DifferencesImage { get; set; }
        public double DifferenceRate { get; set; }

    }
}
