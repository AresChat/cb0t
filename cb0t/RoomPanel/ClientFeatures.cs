using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace cb0t
{
    public enum ClientFeatures : byte
    {
        CLIENT_SUPPORTS_VC = 1,
        CLIENT_SUPPORTS_PM_VC = 2,
        CLIENT_SUPPORTS_OPUS_VC = 4,
        CLIENT_SUPPORTS_OPUS_PM_VC = 8,
        CLIENT_SUPPORTS_HTML = 16
    }
}
