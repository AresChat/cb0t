using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace cb0t
{
    public enum ServerFeatures : byte
    {
        SERVER_SUPPORTS_PVT = 1,
        SERVER_SUPPORTS_SHARING = 2,
        SERVER_SUPPORTS_COMPRESSION = 4,
        SERVER_SUPPORTS_VC = 8,
        SERVER_SUPPORTS_OPUS_VC = 16,
        SERVER_SUPPORTS_ROOM_SCRIBBLES = 32,
        SERVER_SUPPORTS_PM_SCRIBBLES = 64,
        SERVER_SUPPORTS_HTML = 128
    }
}
