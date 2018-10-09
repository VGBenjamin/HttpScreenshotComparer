using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Logging;

namespace HttpScreenshotComparer.Core.Logging
{
    public static class EventIds
    {
        public static EventId GeneralUnexpectedException { get; } = new EventId(1, "A5C6F92F-F4B7-4FE1-B591-8774ACD56296");
        public static EventId UserConfigReadException { get; } = new EventId(2, "2138A75C-4AE5-400F-A2C0-DB051F4D2819");
        public static EventId BrowserErrorMessage { get; } = new EventId(3, "E6263C23-8B60-4FB1-B463-5A3A6472E347");
        public static EventId FolderDateParsing { get; } = new EventId(4, "861564E3-7309-4D45-8AA5-74D4A7A7D9FF");
        public static EventId InvalidColor { get; } = new EventId(5, "7804C9EC-6B93-44D1-A738-02FFAF2B56A8");
        public static EventId CannotCreateeResutFolder { get; } = new EventId(6, "15FB3737-D8D9-4FF9-A6CC-3A5851756AF1");
        public static EventId RazorRenderingResultIsEmpty { get; } = new EventId(7, "A86D0EA6-DBE2-43D3-B3D8-BBE32D67A15E");
        public static EventId CannotDeleteOldGaleryFile { get; } = new EventId(8, "9D0C0CD0-CBF5-4FD7-9CBF-B70824C5C96B");
    }
}
