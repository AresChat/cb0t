using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace cb0t.Scripting
{
    class JSOutboundTextItem
    {
        public String Name { get; set; }
        public IPEndPoint EndPoint { get; set; }
        public String Text { get; set; }
        public JSOutboundTextItemType Type { get; set; }
    }

    enum JSOutboundTextItemType
    {
        Command,
        Emote,
        Public,
        Private,
        Link,
        ChatScreenCallback
    }
}
