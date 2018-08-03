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
    }
}
