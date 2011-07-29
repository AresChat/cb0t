using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.IO;

namespace cb0t_chat_client_v2
{
    class ServerPool
    {
        public static Server[] GetServers()
        {
            List<Server> servers = new List<Server>();

            try
            {
                List<byte> list = new List<byte>(File.ReadAllBytes(Settings.folder_path + "servers.dat"));

                while (list.Count >= 4)
                {
                    IPAddress ip = new IPAddress(list.GetRange(0, 4).ToArray());
                    list.RemoveRange(0, 4);
                    ushort port = BitConverter.ToUInt16(list.ToArray(), 0);
                    list.RemoveRange(0, 2);
                    servers.Add(new Server(ip, port));
                }
            }
            catch { }

            if (servers.Count < 10)
            {
                servers.Clear();

                List<byte> list = new List<byte>(Properties.Resources.servers);

                while (list.Count >= 4)
                {
                    IPAddress ip = new IPAddress(list.GetRange(0, 4).ToArray());
                    list.RemoveRange(0, 4);
                    ushort port = BitConverter.ToUInt16(list.ToArray(), 0);
                    list.RemoveRange(0, 2);
                    servers.Add(new Server(ip, port));
                }
            }

            return servers.ToArray();
        }

        public static void Save(IPEndPoint[] servers)
        {
            try
            {
                List<byte> list = new List<byte>();

                foreach (IPEndPoint s in servers)
                {
                    list.AddRange(s.Address.GetAddressBytes());
                    list.AddRange(BitConverter.GetBytes((ushort)s.Port));
                }

                File.WriteAllBytes(Settings.folder_path + "servers.dat", list.ToArray());
            }
            catch { }
        }

        public static ChannelObject[] LastChannelList
        {
            get
            {
                try
                {
                    byte[] cached = File.ReadAllBytes(Settings.folder_path + "cache.dat");

                    if (cached.Length <= 6)
                        throw new Exception();

                    Ares.PacketHandlers.AresDataPacket buffer = new Ares.PacketHandlers.AresDataPacket(cached);
                    List<ChannelObject> ch = new List<ChannelObject>();

                    while (buffer.Remaining() > 6)
                    {
                        ChannelObject obj = new ChannelObject();
                        obj.ip = buffer.ReadIP();
                        obj.port = buffer.ReadInt16();
                        obj.users = buffer.ReadInt16();
                        obj.name = buffer.ReadString();
                        obj.topic = buffer.ReadString();
                        obj.language = buffer.ReadString();
                        obj.server = buffer.ReadString();
                        ch.Add(obj);
                    }

                    return ch.ToArray();
                }
                catch { return null; }
            }
        }

        public static void SaveChannelListCache(ChannelObject[] rooms)
        {
            // ip port users name topic language server
            Ares.PacketHandlers.AresDataPacket buffer = new Ares.PacketHandlers.AresDataPacket();

            foreach (ChannelObject ch in rooms)
            {
                buffer.WriteIP(ch.ip);
                buffer.WriteInt16(ch.port);
                buffer.WriteInt16(ch.users);
                buffer.WriteString(ch.name);
                buffer.WriteString(ch.topic);
                buffer.WriteString(ch.language);
                buffer.WriteString(ch.server);
            }

            if (buffer.GetByteCount() > 6)
            {
                try
                {
                    File.WriteAllBytes(Settings.folder_path + "cache.dat", buffer.ToByteArray());
                }
                catch { }
            }
        }

        public static String ExtractServerName(String version)
        {
            if (version.Contains("sb0t"))
                return "sb0t";

            if (version.Contains("Arca Eclipse"))
                return "AE";

            if (version.Contains("RUNYA"))
                return "Runya";

            if (version.ToUpper().Contains("CALLISTO"))
                return "Callisto";

            if (version.ToUpper().Contains("SURFICTION"))
                return "Surf";

            if (version.ToUpper().Contains("AURA"))
                return "AurA";

            return "Ares";
        }
    }

    class Server
    {
        public IPAddress ip;
        public ushort port;
        public bool reply;

        public Server(String ip, ushort port)
        {
            this.ip = IPAddress.Parse(ip);
            this.port = port;
            this.reply = false;
        }

        public Server(IPAddress ip, ushort port)
        {
            this.ip = ip;
            this.port = port;
            this.reply = false;
        }
    }

    class UdpPacketReader
    {
        private int Position = 0;
        private List<byte> Data = new List<byte>();

        public UdpPacketReader(byte[] bytes)
        {
            this.Data.Clear();
            this.Position = 0;
            this.Data.AddRange(bytes);
        }

        public int Remaining()
        {
            return this.Data.Count - this.Position;
        }

        public byte ReadByte()
        {
            byte tmp = this.Data[this.Position];
            this.Position++;
            return tmp;
        }

        public ushort ReadInt16()
        {
            ushort tmp = BitConverter.ToUInt16(this.Data.ToArray(), this.Position);
            this.Position += 2;
            return tmp;
        }

        public IPAddress ReadIP()
        {
            byte[] tmp = new byte[4];
            Array.Copy(this.Data.ToArray(), this.Position, tmp, 0, tmp.Length);
            this.Position += 4;
            return new IPAddress(tmp);
        }

        public String ReadString()
        {
            ushort len = BitConverter.ToUInt16(this.Data.ToArray(), this.Position);
            this.Position += 2;
            String str = Encoding.UTF8.GetString(this.Data.ToArray(), this.Position, len);
            this.Position += len;
            return str;
        }
    }
}
