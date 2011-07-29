using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using Ares.PacketHandlers;
using ZLib;

namespace cb0t_chat_client_v2
{
    class Hashlink
    {
        public static String EncodeHashlink(ChannelObject item)
        {
            AresDataPacket packet = new AresDataPacket();
            packet.WriteBytes(new byte[20]);
            packet.WriteString("CHATCHANNEL");
            packet.WriteIP(item.ip);
            packet.WriteInt16(item.port);
            packet.WriteIP(Helpers.RandomIPAddress());
            packet.WriteString(item.name);
            packet.WriteByte(0);
            byte[] bytes = packet.ToByteArray();
            bytes = Zlib.Compress(bytes);
            bytes = AresCryptography.e67(bytes, 28435);
            return Convert.ToBase64String(bytes);
        }

        public static ChannelObject DecodeHashlink(String item)
        {
            ChannelObject chitem = new ChannelObject();

            item = item.Trim();

            if (item.StartsWith("arlnk://"))
                item = item.Substring(8);

            try
            {
                if (item.ToUpper().StartsWith("CHATROOM:")) // not encrypted
                {
                    item = item.Substring(9);
                    int split = item.IndexOf(":");

                    if (!IPAddress.TryParse(item.Substring(0, split), out chitem.ip))
                        chitem.dyndns = item.Substring(0, split);

                    item = item.Substring(split + 1);
                    split = item.IndexOf("|");
                    chitem.port = ushort.Parse(item.Substring(0, split));
                    chitem.name = item.Substring(split + 1).Replace("%20", " ");
                    chitem.topic = String.Empty;
                    return chitem;
                }
                else // encrypted
                {
                    byte[] bytes = Convert.FromBase64String(item);
                    bytes = AresCryptography.d67(bytes, 28435);
                    bytes = Zlib.Decompress(bytes, false);
                    AresDataPacket packet = new AresDataPacket(bytes);
                    packet.SkipBytes(32);
                    chitem.ip = packet.ReadIP();
                    chitem.port = packet.ReadInt16();
                    packet.SkipBytes(4);
                    chitem.name = packet.ReadString();
                    chitem.topic = String.Empty;
                    return chitem;
                }
            }
            catch // badly formed hashlink
            {
                return null;
            }
        }
    }
}
