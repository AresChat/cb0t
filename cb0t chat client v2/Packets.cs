using System;
using System.Collections.Generic;
using System.Text;
using System.Security.Cryptography;
using Ares.PacketHandlers;

namespace cb0t_chat_client_v2
{
    class Packets
    {
        public delegate void SendPacketDelegate(byte[] packet);

        public static byte[] DisableCustomNames(bool yes)
        {
            AresDataPacket packet = new AresDataPacket();
            packet.WriteString("cb0t_no_custom_names", true);
            byte[] buf = packet.ToAresPacket(yes ? (byte)202 : (byte)203);
            packet = new AresDataPacket();
            packet.WriteBytes(buf);
            return packet.ToAresPacket(250);
        }

        public static byte[] CustomEmoteItem(CEmoteItem item)
        {
            AresDataPacket packet = new AresDataPacket();
            packet.WriteString(item.Shortcut);
            packet.WriteByte((byte)item.Size);
            packet.WriteBytes(item.Image);
            byte[] buf = packet.ToAresPacket(221);
            packet = new AresDataPacket();
            packet.WriteBytes(buf);
            return packet.ToAresPacket(250);
        }

        public static byte[] CustomEmoteFlag()
        {
            return new byte[] { 3, 0, 250, 0, 0, 220 };
        }

        public static byte[] CustomEmoteDelete(String shortcut)
        {
            AresDataPacket packet = new AresDataPacket();
            packet.WriteString(shortcut);
            byte[] buf = packet.ToAresPacket(222);
            packet = new AresDataPacket();
            packet.WriteBytes(buf);
            return packet.ToAresPacket(250);
        }

        public static byte[] EnableClips(bool main, bool pvt)
        {
            AresDataPacket packet = new AresDataPacket();
            packet.WriteByte(main ? (byte)1 : (byte)0);
            packet.WriteByte(pvt ? (byte)1 : (byte)0);
            byte[] buf = packet.ToAresPacket(205);
            packet = new AresDataPacket();
            packet.WriteBytes(buf);
            return packet.ToAresPacket(250);
        }

        public static byte[] VCIgnore(String name)
        {
            AresDataPacket packet = new AresDataPacket();
            packet.WriteString(name);
            byte[] buf = packet.ToAresPacket(210);
            packet = new AresDataPacket();
            packet.WriteBytes(buf);
            return packet.ToAresPacket(250);
        }

        public static byte[] LoginPacket()
        {
            AresDataPacket packet = new AresDataPacket();
            packet.WriteGuid(Settings.my_guid);
            packet.WriteInt16(666);
            packet.WriteByte(0);
            packet.WriteInt16(Settings.dc_port); // dc port
            packet.WriteIP("0.0.0.0");
            packet.WriteInt16(0);
            packet.WriteInt32(0); // speed no longer needed
            packet.WriteString(Settings.my_username, true);
            packet.WriteString("cb0t 2.69 client", true);
            packet.WriteIP(Settings.local_ip);
            packet.WriteIP(Settings.external_ip); // external
            packet.WriteByte(7); // browse + zlib
            packet.WriteBytes(new byte[] { 0, 0, 0 });
            packet.WriteByte(Settings.age);
            packet.WriteByte(Settings.sex);
            packet.WriteByte(Settings.country);
            packet.WriteString(Settings.region);
            return packet.ToAresPacket(2);
        }

        public static byte[] UpdatePacket()
        {
            AresDataPacket packet = new AresDataPacket();
            packet.WriteInt16(666);
            packet.WriteByte(7); // browse + zlib
            packet.WriteIP("0.0.0.0");
            packet.WriteInt16(0);
            packet.WriteIP(Settings.external_ip); // external
            packet.WriteBytes(new byte[] { 0, 0, 0 });
            packet.WriteByte(Settings.age);
            packet.WriteByte(Settings.sex);
            packet.WriteByte(Settings.country);
            packet.WriteString(Settings.region);
            return packet.ToAresPacket(4);
        }

        public static Byte[] FakeFilePacket(String filename)
        {
            AresDataPacket packet = new AresDataPacket();
            packet.WriteByte(0);
            packet.WriteInt32(69);
            packet.WriteInt16(4);
            packet.WriteString("cb0t123456789012345", false);
            packet.WriteByte((byte)filename.Length);
            packet.WriteInt16((ushort)(filename.Length + filename.Length + 4));
            packet.WriteByte((byte)filename.Length);
            packet.WriteByte(15);
            packet.WriteString(filename, false);
            packet.WriteByte((byte)filename.Length);
            packet.WriteByte((byte)1);
            packet.WriteString(filename, false);
            return packet.ToAresPacket(50);
        }

        public static byte[] TextPacket(String text)
        {
            AresDataPacket packet = new AresDataPacket();

            if (text.Length > 250)
                text = text.Substring(0, 250);

            packet.WriteString(Helpers.FormatAresColorCodes(text), false);
            return packet.ToAresPacket(10);
        }

        public static byte[] EmotePacket(String text)
        {
            AresDataPacket packet = new AresDataPacket();

            if (text.Length > 250)
                text = text.Substring(0, 250);

            packet.WriteString(Helpers.FormatAresColorCodes(text), false);
            return packet.ToAresPacket(11);
        }

        public static byte[] FontPacket()
        {
            AresDataPacket packet = new AresDataPacket();
            packet.WriteByte((byte)Settings.p_font_size);
            packet.WriteString(Settings.p_font_name, true);
            packet.WriteByte((byte)Settings.p_name_col);
            packet.WriteByte((byte)Settings.p_text_col);
            byte[] buf = packet.ToAresPacket(204);
            packet = new AresDataPacket();
            packet.WriteBytes(buf);
            return packet.ToAresPacket(250);
        }

        public static byte[] FontOffPacket()
        {
            AresDataPacket packet = new AresDataPacket();
            packet.WriteBytes(new byte[] { 0, 0 });
            byte[] buf = packet.ToAresPacket(204);
            packet = new AresDataPacket();
            packet.WriteBytes(buf);
            return packet.ToAresPacket(250);
        }

        public static byte[] PMPacket(String name, String text)
        {
            AresDataPacket packet = new AresDataPacket();

            if (text.Length > 200)
                text = text.Substring(0, 200);

            packet.WriteString(name);
            packet.WriteString(Helpers.FormatAresColorCodes(text), false);
            return packet.ToAresPacket(25);
        }

        public static byte[] CommandPacket(String text)
        {
            AresDataPacket packet = new AresDataPacket();

            if (text.Length > 200)
                text = text.Substring(0, 200);

            packet.WriteString(Helpers.FormatAresColorCodes(text), false);
            return packet.ToAresPacket(74);
        }

        public static byte[] BrowseRequestPacket(String target, ushort ident, byte type)
        {
            AresDataPacket packet = new AresDataPacket();
            packet.WriteInt16(ident);
            packet.WriteByte(type);
            packet.WriteString(target, false);
            return packet.ToAresPacket(52);
        }

        public static byte[] IgnoreUserPacket(String name, bool ignore)
        {
            AresDataPacket packet = new AresDataPacket();
            packet.WriteByte(ignore ? (byte)1 : (byte)0);
            packet.WriteString(name);
            return packet.ToAresPacket(45);
        }

        public static byte[] NudgePacket(String source, String target)
        {
            byte[] temp = Encoding.UTF8.GetBytes("0" + source);
            temp = AresCryptography.e67(temp, 1488);
            temp = Encoding.Default.GetBytes(Convert.ToBase64String(temp));

            AresDataPacket packet = new AresDataPacket();
            packet.WriteString("cb0t_nudge");
            packet.WriteString(target);
            packet.WriteBytes(temp);
            return packet.ToAresPacket(200);
        }

        public static byte[] NudgeRejectPacket(String target)
        {
            byte[] temp = Encoding.UTF8.GetBytes("1");
            temp = AresCryptography.e67(temp, 1488);
            temp = Encoding.Default.GetBytes(Convert.ToBase64String(temp));

            AresDataPacket packet = new AresDataPacket();
            packet.WriteString("cb0t_nudge");
            packet.WriteString(target);
            packet.WriteBytes(temp);
            return packet.ToAresPacket(200);
        }

        public static byte[] PasswordPacket(String password)
        {
            AresDataPacket packet = new AresDataPacket();
            packet.WriteString(password, false);
            return packet.ToAresPacket(82);
        }

        public static byte[] AutoLoginPasswordPacket(ChannelObject cObj)
        {
            List<byte> temp = new List<byte>();
            temp.AddRange(SHA1.Create().ComputeHash(Encoding.UTF8.GetBytes(cObj.password)));
            temp.InsertRange(0, cObj.ip.GetAddressBytes());
            temp.InsertRange(0, BitConverter.GetBytes(cObj.cookie));

            AresDataPacket packet = new AresDataPacket();
            packet.WriteBytes(SHA1.Create().ComputeHash(temp.ToArray()));
            return packet.ToAresPacket(7);
        }

        public static byte[] PersonalMessagePacket()
        {
            AresDataPacket packet = new AresDataPacket();
            packet.WriteString(Settings.personal_message, false);
            return packet.ToAresPacket(13);
        }

        public static byte[] UserlistSongPacket(String text)
        {
            AresDataPacket packet = new AresDataPacket();
            packet.WriteByte(7);
            packet.WriteString(text, false);
            return packet.ToAresPacket(13);
        }

        public static byte[] AvatarPacket()
        {
            AresDataPacket packet = new AresDataPacket();
            packet.WriteBytes(Avatar.avatar_small);
            return packet.ToAresPacket(9);
        }

        public static byte[] ScribbleOncePacket(String target, byte[] data)
        {
            AresDataPacket packet = new AresDataPacket();
            packet.WriteString("cb0t_scribble_once");
            packet.WriteString(target);
            packet.WriteBytes(data);
            return packet.ToAresPacket(200);
        }

        public static byte[] ScribbleFirstPacket(String target, byte[] data)
        {
            AresDataPacket packet = new AresDataPacket();
            packet.WriteString("cb0t_scribble_first");
            packet.WriteString(target);
            packet.WriteBytes(data);
            return packet.ToAresPacket(200);
        }

        public static byte[] ScribbleChunkPacket(String target, byte[] data)
        {
            AresDataPacket packet = new AresDataPacket();
            packet.WriteString("cb0t_scribble_chunk");
            packet.WriteString(target);
            packet.WriteBytes(data);
            return packet.ToAresPacket(200);
        }

        public static byte[] ScribbleLastPacket(String target, byte[] data)
        {
            AresDataPacket packet = new AresDataPacket();
            packet.WriteString("cb0t_scribble_last");
            packet.WriteString(target);
            packet.WriteBytes(data);
            return packet.ToAresPacket(200);
        }

        public static byte[] ScribbleRejectedPacket(String target)
        {
            AresDataPacket packet = new AresDataPacket();
            packet.WriteString("cb0t_scribble_reject");
            packet.WriteString(target);
            packet.WriteByte(1);
            return packet.ToAresPacket(200);
        }

        public static byte[] LagCheckPacket(String name, ulong time)
        {
            AresDataPacket packet = new AresDataPacket();
            packet.WriteString("cb0t_lag_check");
            packet.WriteString(name);
            packet.WriteInt64(time);
            return packet.ToAresPacket(200);
        }

        public static byte[] LatencyCheckPacket(String name)
        {
            AresDataPacket packet = new AresDataPacket();
            packet.WriteString("cb0t_latency_check");
            packet.WriteString(name);
            TimeSpan ts = (DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0));
            packet.WriteInt64((ulong)ts.TotalMilliseconds);
            return packet.ToAresPacket(200);
        }

        public static byte[] WritingPacket(bool writing)
        {
            AresDataPacket packet = new AresDataPacket();
            packet.WriteString("cb0t_writing");
            packet.WriteByte(writing ? (byte)2 : (byte)1);
            return packet.ToAresPacket(201);
        }

        public static byte[] OnlineStatusPacket()
        {
            AresDataPacket packet = new AresDataPacket();
            packet.WriteString("cb0t_online_status");
            packet.WriteByte((byte)Settings.my_status);
            return packet.ToAresPacket(201);
        }

        // Direct Chat

        public static byte[] DCSessionRequestString()
        {
            AresDataPacket packet = new AresDataPacket();
            packet.WriteString("CHAT CONNECT/0.1", false);
            packet.WriteBytes(new byte[] { 13, 10, 13, 10 });

            return packet.ToByteArray();
        }

        public static byte[] DCVersionString()
        {
            AresDataPacket packet = new AresDataPacket();
            packet.WriteString("CHAT/0.1 200 OK", false);
            packet.WriteBytes(new byte[] { 13, 10, 13, 10 });

            return packet.ToByteArray();
        }

        public static byte[] DCMyInfo()
        {
            AresDataPacket packet = new AresDataPacket();
            packet.WriteString("|01:0.0.0.0:0|" + Settings.external_ip.ToString() + ":" + Settings.dc_port, false);
            packet.WriteByte(10);
            packet.WriteString("|02:" + Settings.my_username, false);
            packet.WriteByte(10);
            packet.WriteString("|03:2.1.1.3035", false);
            packet.WriteByte(10);
            packet.WriteBytes(new byte[] { 0, 10, 0, 29 });
            packet.WriteIP(Settings.external_ip);
            packet.WriteInt16(Settings.dc_port);
            packet.WriteIP(Settings.local_ip);
            packet.WriteByte(0);
            packet.WriteByte((byte)(Settings.my_username.Length + 1));
            packet.WriteBytes(new byte[] { 0, 26 });
            packet.WriteString(Settings.my_username);
            packet.WriteBytes(new byte[] { 0, 5, 0, 12 });
            packet.WriteIP(Settings.local_ip);
            packet.WriteByte(1);

            return packet.ToByteArray();
        }

        public static byte[] DCPing()
        {
            return new AresDataPacket().ToAresDCPacket(10);
        }

        public static byte[] DCPong()
        {
            AresDataPacket packet = new AresDataPacket();
            packet.WriteIP(Settings.external_ip);
            packet.WriteInt16(Settings.dc_port);
            packet.WriteIP(Settings.local_ip);

            return packet.ToAresDCPacket(30);
        }

        public static byte[] DCTextMessage(String text)
        {
            AresDataPacket packet = new AresDataPacket();
            packet.WriteString(text, false);

            return packet.ToAresDCPacket(1);
        }

        public static byte[] DCSendFileRequest(ulong filesize, ushort referral, uint rnd, String filename)
        {
            AresDataPacket packet = new AresDataPacket();
            packet.WriteInt64(filesize);
            packet.WriteInt16(referral);
            packet.WriteInt32(rnd);
            packet.WriteString(filename);
            packet.WriteByte(0);

            return packet.ToAresDCPacket(2);
        }

        public static byte[] DCAcceptFileRequest(String filename)
        {
            AresDataPacket packet = new AresDataPacket();
            packet.WriteBytes(new byte[8]);
            packet.WriteString(filename);
            packet.WriteByte(0);

            return packet.ToAresDCPacket(3);
        }

        public static byte[] DCDeclineFileRequest(String filename)
        {
            AresDataPacket packet = new AresDataPacket();
            packet.WriteString(filename);

            return packet.ToAresDCPacket(4);
        }

        public static byte[] DCFileChunk(ushort referral, byte[] data)
        {
            AresDataPacket packet = new AresDataPacket();
            packet.WriteInt16(referral);
            packet.WriteBytes(data);

            return packet.ToAresDCPacket(5);
        }

        public static byte[] DCAvatarChunkPacket(byte[] data)
        {
            AresDataPacket packet = new AresDataPacket();
            packet.WriteBytes(data);
            return packet.ToAresDCPacket(6);
        }

        public static byte[] DCAvatarEndPacket()
        {
            return new AresDataPacket().ToAresDCPacket(6);
        }

    }
}
