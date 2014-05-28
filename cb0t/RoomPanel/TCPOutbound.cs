using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
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
            packet.WriteUInt16(Settings.Port);
            packet.WriteIP("0.0.0.0");
            packet.WriteUInt16(65535);
            packet.WriteUInt32(0);
            packet.WriteString(Settings.GetReg<String>("username", String.Empty), true);
            packet.WriteString(Settings.APP_NAME + " " + Settings.APP_VERSION, true);
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
            ClientFeatures features = (ClientFeatures)0;

            if (Settings.GetReg<bool>("vc_public", true))
            {
                features |= ClientFeatures.CLIENT_SUPPORTS_OPUS_VC;
                features |= ClientFeatures.CLIENT_SUPPORTS_VC;
            }

            if (Settings.GetReg<bool>("vc_private", true))
            {
                features |= ClientFeatures.CLIENT_SUPPORTS_OPUS_PM_VC;
                features |= ClientFeatures.CLIENT_SUPPORTS_PM_VC;
            }

            features |= ClientFeatures.CLIENT_SUPPORTS_HTML;
            packet.WriteByte((byte)features);
            return packet.ToAresPacket(TCPMsg.MSG_CHAT_CLIENT_LOGIN);
        }

        public static byte[] Update(CryptoService c)
        {
            String str;
            TCPPacketWriter packet = new TCPPacketWriter();
            packet.WriteUInt16(0);
            packet.WriteByte(7);
            packet.WriteIP("0.0.0.0");
            packet.WriteUInt16(65535);
            packet.WriteIP(Settings.LocalIP);
            packet.WriteByte((byte)Settings.GetReg<int>("user_age", 0));
            packet.WriteByte((byte)Settings.GetReg<int>("user_gender", 0));
            packet.WriteByte((byte)Settings.GetReg<int>("user_country", 0));
            str = Settings.GetReg<String>("user_region", String.Empty);

            if (str.Length > 30)
                str = str.Substring(0, 30);

            packet.WriteString(str, c);
            return packet.ToAresPacket(TCPMsg.MSG_CHAT_CLIENT_UPDATE_STATUS);
        }

        public static byte[] EnableClips()
        {
            TCPPacketWriter packet = new TCPPacketWriter();
            packet.WriteByte((byte)(Settings.GetReg<bool>("vc_public", true) ? 1 : 0));
            packet.WriteByte((byte)(Settings.GetReg<bool>("vc_private", true) ? 1 : 0));
            byte[] buf = packet.ToAresPacket(TCPMsg.MSG_CHAT_CLIENT_VC_SUPPORTED);
            packet = new TCPPacketWriter();
            packet.WriteBytes(buf);
            return packet.ToAresPacket(TCPMsg.MSG_CHAT_ADVANCED_FEATURES_PROTOCOL);
        }

        public static byte[] BlockCustomNames(bool block)
        {
            TCPPacketWriter packet = new TCPPacketWriter();
            packet.WriteByte((byte)(block ? 1 : 0));
            byte[] buf = packet.ToAresPacket(TCPMsg.MSG_CHAT_CLIENT_BLOCK_CUSTOMNAMES);
            packet = new TCPPacketWriter();
            packet.WriteBytes(buf);
            return packet.ToAresPacket(TCPMsg.MSG_CHAT_ADVANCED_FEATURES_PROTOCOL);
        }

        public static byte[] OnlineStatus(bool away, CryptoService c)
        {
            TCPPacketWriter packet = new TCPPacketWriter();
            packet.WriteString("cb0t_online_status", c);
            packet.WriteByte((byte)(away ? 3 : 1));
            return packet.ToAresPacket(TCPMsg.MSG_CHAT_CLIENT_CUSTOM_DATA_ALL);
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

        public static byte[] Lag(String name, ulong time, CryptoService c)
        {
            TCPPacketWriter packet = new TCPPacketWriter();
            packet.WriteString("cb0t_latency_check", c);
            packet.WriteString(name, c);
            packet.WriteUInt64(time);
            return packet.ToAresPacket(TCPMsg.MSG_CHAT_CLIENT_CUSTOM_DATA);
        }

        public static byte[] CustomCbotData(String target, String ident, byte[] data, CryptoService c)
        {
            TCPPacketWriter packet = new TCPPacketWriter();
            packet.WriteString("cb3_custom_" + ident, c);
            packet.WriteString(target, c);
            packet.WriteBytes(data);
            return packet.ToAresPacket(TCPMsg.MSG_CHAT_CLIENT_CUSTOM_DATA);
        }

        public static byte[] CustomCbotData(String ident, byte[] data, CryptoService c)
        {
            TCPPacketWriter packet = new TCPPacketWriter();
            packet.WriteString("cb3_custom_" + ident, c);
            packet.WriteBytes(data);
            return packet.ToAresPacket(TCPMsg.MSG_CHAT_CLIENT_CUSTOM_DATA_ALL);
        }

        public static byte[] ManualLag(String name, ulong time, CryptoService c)
        {
            TCPPacketWriter packet = new TCPPacketWriter();
            packet.WriteString("cb0t_latency_mcheck", c);
            packet.WriteString(name, c);
            packet.WriteUInt64(time);
            return packet.ToAresPacket(TCPMsg.MSG_CHAT_CLIENT_CUSTOM_DATA);
        }

        public static byte[] Writing(bool writing, CryptoService c)
        {
            TCPPacketWriter packet = new TCPPacketWriter();
            packet.WriteString("cb0t_writing", c);
            packet.WriteByte((byte)(writing ? 2 : 1));
            return packet.ToAresPacket(TCPMsg.MSG_CHAT_CLIENT_CUSTOM_DATA_ALL);
        }

        public static byte[] Nudge(String myName, String target, CryptoService c)
        {
            byte[] temp = Encoding.UTF8.GetBytes("0" + myName);
            temp = Hashlink.e67(temp, 0x5d0);
            temp = Encoding.Default.GetBytes(Convert.ToBase64String(temp));
            TCPPacketWriter packet = new TCPPacketWriter();
            packet.WriteString("cb0t_nudge", c);
            packet.WriteString(target, c);
            packet.WriteBytes(temp);
            return packet.ToAresPacket(TCPMsg.MSG_CHAT_CLIENT_CUSTOM_DATA);
        }

        public static byte[] NudgeReject(String target, CryptoService c)
        {
            TCPPacketWriter packet = new TCPPacketWriter();
            packet.WriteString("cb0t_nudge", c);
            packet.WriteString(target, c);
            packet.WriteBytes(new byte[] { 78, 65, 61, 61 });
            return packet.ToAresPacket(TCPMsg.MSG_CHAT_CLIENT_CUSTOM_DATA);
        }

        public static byte[] Private(String name, String text, CryptoService c)
        {
            TCPPacketWriter packet = new TCPPacketWriter();
            packet.WriteString(name, c);
            packet.WriteString(Helpers.FormatAresColorCodes(text), c, false);
            return packet.ToAresPacket(TCPMsg.MSG_CHAT_CLIENT_PVT);
        }

        public static byte[] CustomPM(String target, String text, CryptoService c)
        {
            TCPPacketWriter packet = new TCPPacketWriter();
            packet.WriteString("cb0t_pm_msg", c);
            packet.WriteString(target, c);
            packet.WriteBytes(PMCrypto.SoftEncrypt(target, Encoding.UTF8.GetBytes(Helpers.FormatAresColorCodes(text))));
            return packet.ToAresPacket(TCPMsg.MSG_CHAT_CLIENT_CUSTOM_DATA);
        }

        public static byte[] Ignore(String name, bool ignore, CryptoService c)
        {
            TCPPacketWriter packet = new TCPPacketWriter();
            packet.WriteByte((byte)(ignore ? 1 : 0));
            packet.WriteString(name, c);
            return packet.ToAresPacket(TCPMsg.MSG_CHAT_CLIENT_IGNORELIST);
        }

        public static byte[] Browse(String target, ushort ident, CryptoService c)
        {
            TCPPacketWriter packet = new TCPPacketWriter();
            packet.WriteUInt16(ident);
            packet.WriteByte(0);
            packet.WriteString(target, c);
            return packet.ToAresPacket(TCPMsg.MSG_CHAT_CLIENT_BROWSE);
        }

        public static byte[] ScribbleRoomFirst(uint size, ushort chunks, byte[] data)
        {
            TCPPacketWriter packet = new TCPPacketWriter();
            packet.WriteUInt32(size);
            packet.WriteUInt16(chunks);
            packet.WriteBytes(data);
            byte[] buf = packet.ToAresPacket(TCPMsg.MSG_CHAT_CLIENT_SCRIBBLEROOM_FIRST);
            packet = new TCPPacketWriter();
            packet.WriteBytes(buf);
            return packet.ToAresPacket(TCPMsg.MSG_CHAT_ADVANCED_FEATURES_PROTOCOL);
        }

        public static byte[] ScribbleRoomChunk(byte[] data)
        {
            TCPPacketWriter packet = new TCPPacketWriter();
            packet.WriteBytes(data);
            byte[] buf = packet.ToAresPacket(TCPMsg.MSG_CHAT_CLIENT_SCRIBBLEROOM_CHUNK);
            packet = new TCPPacketWriter();
            packet.WriteBytes(buf);
            return packet.ToAresPacket(TCPMsg.MSG_CHAT_ADVANCED_FEATURES_PROTOCOL);
        }

        public static byte[] PMScribbleOnce(String target, byte[] data, CryptoService c)
        {
            TCPPacketWriter packet = new TCPPacketWriter();
            packet.WriteString("cb0t_scribble_once", c);
            packet.WriteString(target, c);
            packet.WriteBytes(data);
            return packet.ToAresPacket(TCPMsg.MSG_CHAT_CLIENT_CUSTOM_DATA);
        }

        public static byte[] PMScribbleFirst(String target, byte[] data, CryptoService c)
        {
            TCPPacketWriter packet = new TCPPacketWriter();
            packet.WriteString("cb0t_scribble_first", c);
            packet.WriteString(target, c);
            packet.WriteBytes(data);
            return packet.ToAresPacket(TCPMsg.MSG_CHAT_CLIENT_CUSTOM_DATA);
        }

        public static byte[] PMScribbleChunk(String target, byte[] data, CryptoService c)
        {
            TCPPacketWriter packet = new TCPPacketWriter();
            packet.WriteString("cb0t_scribble_chunk", c);
            packet.WriteString(target, c);
            packet.WriteBytes(data);
            return packet.ToAresPacket(TCPMsg.MSG_CHAT_CLIENT_CUSTOM_DATA);
        }

        public static byte[] PMScribbleLast(String target, byte[] data, CryptoService c)
        {
            TCPPacketWriter packet = new TCPPacketWriter();
            packet.WriteString("cb0t_scribble_last", c);
            packet.WriteString(target, c);
            packet.WriteBytes(data);
            return packet.ToAresPacket(TCPMsg.MSG_CHAT_CLIENT_CUSTOM_DATA);
        }

        public static byte[] PersonalMessage(CryptoService c)
        {
            TCPPacketWriter packet = new TCPPacketWriter();
            String str = Settings.GetReg<String>("personal_message", String.Empty);

            if (str.Length > 100)
                str = str.Substring(0, 100);

            packet.WriteString(Helpers.FormatAresColorCodes(str), c, false);
            return packet.ToAresPacket(TCPMsg.MSG_CHAT_CLIENT_PERSONAL_MESSAGE);
        }

        public static byte[] PersonalMessage(String text, CryptoService c)
        {
            TCPPacketWriter packet = new TCPPacketWriter();
            String str = text;

            if (str.Length > 100)
                str = str.Substring(0, 100);

            packet.WriteString(Helpers.FormatAresColorCodes(str), c, false);
            return packet.ToAresPacket(TCPMsg.MSG_CHAT_CLIENT_PERSONAL_MESSAGE);
        }

        public static byte[] ClearAvatar()
        {
            TCPPacketWriter packet = new TCPPacketWriter();
            return packet.ToAresPacket(TCPMsg.MSG_CHAT_CLIENT_AVATAR);
        }

        public static byte[] Avatar()
        {
            TCPPacketWriter packet = new TCPPacketWriter();
            packet.WriteBytes(cb0t.Avatar.Data);
            return packet.ToAresPacket(TCPMsg.MSG_CHAT_CLIENT_AVATAR);
        }

        public static byte[] Font(bool new_sbot, CryptoService c)
        {
            TCPPacketWriter packet = new TCPPacketWriter();
            packet.WriteByte((byte)Settings.GetReg<int>("user_font_size", 10));
            packet.WriteString(Settings.GetReg<String>("user_font", "Tahoma"), c);
            packet.WriteByte(Helpers.HTMLColorToAresColor("#" + Settings.GetReg<String>("name_color", "000000")));
            packet.WriteByte(Helpers.HTMLColorToAresColor("#" + Settings.GetReg<String>("text_color", "0000FF")));

            if (new_sbot)
            {
                packet.WriteString("#" + Settings.GetReg<String>("name_color", "000000"), c);
                packet.WriteString("#" + Settings.GetReg<String>("text_color", "0000FF"), c);
            }

            byte[] buf = packet.ToAresPacket(TCPMsg.MSG_CHAT_CLIENT_CUSTOM_FONT);
            packet = new TCPPacketWriter();
            packet.WriteBytes(buf);
            return packet.ToAresPacket(TCPMsg.MSG_CHAT_ADVANCED_FEATURES_PROTOCOL);
        }

        public static byte[] SecureAdminLogin(String password, uint cookie, IPAddress ip)
        {
            byte[] result;

            using (SHA1 sha1 = SHA1.Create())
            {
                List<byte> list = new List<byte>();
                list.AddRange(BitConverter.GetBytes(cookie));
                list.AddRange(ip.GetAddressBytes());
                list.AddRange(sha1.ComputeHash(Encoding.UTF8.GetBytes(password)));
                TCPPacketWriter packet = new TCPPacketWriter();
                packet.WriteBytes(sha1.ComputeHash(list.ToArray()));
                result = packet.ToAresPacket(TCPMsg.MSG_CHAT_CLIENT_AUTOLOGIN);
            }

            return result;
        }

        public static byte[] VoiceFirst(uint ident, byte len, byte compresses, uint size, byte[] data)
        {
            TCPPacketWriter packet = new TCPPacketWriter();
            packet.WriteUInt32(ident);
            packet.WriteByte(len);
            packet.WriteByte(compresses);
            packet.WriteUInt32(size);
            packet.WriteBytes(data);
            byte[] buf = packet.ToAresPacket(TCPMsg.MSG_CHAT_CLIENT_VC_FIRST);
            packet = new TCPPacketWriter();
            packet.WriteBytes(buf);
            return packet.ToAresPacket(TCPMsg.MSG_CHAT_ADVANCED_FEATURES_PROTOCOL);
        }

        public static byte[] VoiceChunk(uint ident, byte[] data)
        {
            TCPPacketWriter packet = new TCPPacketWriter();
            packet.WriteUInt32(ident);
            packet.WriteBytes(data);
            byte[] buf = packet.ToAresPacket(TCPMsg.MSG_CHAT_CLIENT_VC_CHUNK);
            packet = new TCPPacketWriter();
            packet.WriteBytes(buf);
            return packet.ToAresPacket(TCPMsg.MSG_CHAT_ADVANCED_FEATURES_PROTOCOL);
        }

        public static byte[] VoiceToFirst(String target, uint ident, byte len, byte compresses, uint size, byte[] data, CryptoService c)
        {
            TCPPacketWriter packet = new TCPPacketWriter();
            packet.WriteString(target, c);
            packet.WriteUInt32(ident);
            packet.WriteByte(len);
            packet.WriteByte(compresses);
            packet.WriteUInt32(size);
            packet.WriteBytes(data);
            byte[] buf = packet.ToAresPacket(TCPMsg.MSG_CHAT_CLIENT_VC_FIRST_TO);
            packet = new TCPPacketWriter();
            packet.WriteBytes(buf);
            return packet.ToAresPacket(TCPMsg.MSG_CHAT_ADVANCED_FEATURES_PROTOCOL);
        }

        public static byte[] VoiceToChunk(String target, uint ident, byte[] data, CryptoService c)
        {
            TCPPacketWriter packet = new TCPPacketWriter();
            packet.WriteString(target, c);
            packet.WriteUInt32(ident);
            packet.WriteBytes(data);
            byte[] buf = packet.ToAresPacket(TCPMsg.MSG_CHAT_CLIENT_VC_CHUNK_TO);
            packet = new TCPPacketWriter();
            packet.WriteBytes(buf);
            return packet.ToAresPacket(TCPMsg.MSG_CHAT_ADVANCED_FEATURES_PROTOCOL);
        }
    }
}
