using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace cb0t
{
    class PacketReceivedEventArgs : EventArgs
    {
        public TCPPacketReader Packet { get; set; }
        public TCPMsg Msg { get; set; }
    }
}
