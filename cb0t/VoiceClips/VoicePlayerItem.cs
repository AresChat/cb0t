using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace cb0t
{
    class VoicePlayerItem
    {
        public IPEndPoint EndPoint { get; set; }
        public String Sender { get; set; }
        public uint Ident { get; set; }
        public String FileName { get; set; }
        public bool Auto { get; set; }
        public uint ShortCut { get; private set; }
        public bool IsBlack { get; private set; }

        public VoicePlayerItem(uint sc, bool b)
        {
            this.ShortCut = sc;
            this.IsBlack = b;
        }
    }
}
