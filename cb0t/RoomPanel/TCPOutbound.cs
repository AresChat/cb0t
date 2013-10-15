using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace cb0t
{
    class TCPOutbound
    {
        public static byte[] Login()
        {
            String str;
            TCPPacketWriter packet = new TCPPacketWriter();
            packet.WriteGuid(Settings.Guid);
            packet.WriteUInt16(0);
            packet.WriteByte((byte)Settings.GetReg<int>("crypto", 250));
            packet.WriteUInt16(0);
            //packet.WriteIP("0.0.0.0");
            packet.WriteBytes(new byte[] { 0x7b, 0xff, 0x7b, 0xff });
            packet.WriteUInt16(65535);
            packet.WriteUInt32(0);
            packet.WriteString("placebo", true);
            packet.WriteString("cb0t 3.00a", true);
            packet.WriteIP(Settings.LocalIP);
            packet.WriteIP(Settings.LocalIP);
            packet.WriteByte(7);
            packet.WriteBytes(new byte[] { 0, 0, 0 });
            packet.WriteByte((byte)Settings.GetReg<int>("user_age", 0));
            packet.WriteByte((byte)Settings.GetReg<int>("user_gender", 0));
            packet.WriteByte((byte)Settings.GetReg<int>("user_country", 0));
            str = Settings.GetReg<String>("user_region", String.Empty);

            if (str.Length > 30)
                str = str.Substring(0, 30);

            packet.WriteString(str);
            packet.WriteByte((byte)(1 | 2 | 4 | 8));
            return packet.ToAresPacket(TCPMsg.MSG_CHAT_CLIENT_LOGIN);
        }

        public static byte[] Public(String text, CryptoService c)
        {
            TCPPacketWriter packet = new TCPPacketWriter();
            packet.WriteString(Helpers.FormatAresColorCodes(text), c, false);
            return packet.ToAresPacket(TCPMsg.MSG_CHAT_CLIENT_PUBLIC);
        }

        public static byte[] Emote(String text, CryptoService c)
        {
            TCPPacketWriter packet = new TCPPacketWriter();
            packet.WriteString(Helpers.FormatAresColorCodes(text), c, false);
            return packet.ToAresPacket(TCPMsg.MSG_CHAT_CLIENT_EMOTE);
        }

        public static byte[] Command(String text, CryptoService c)
        {
            TCPPacketWriter packet = new TCPPacketWriter();
            packet.WriteString(Helpers.FormatAresColorCodes(text), c, false);
            return packet.ToAresPacket(TCPMsg.MSG_CHAT_CLIENT_COMMAND);
        }


    }
}
