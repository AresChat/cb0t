using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace cb0t
{
    class VoicePlayer
    {
        public static List<VoicePlayerInboundItem> Inbound { get; set; }
        

        public static void Init()
        {
            Inbound = new List<VoicePlayerInboundItem>();
        }

        public static void QueueItem(VoicePlayerItem item)
        {

        }
    }
}
