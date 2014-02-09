using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace cb0t
{
    class ChannelListItem
    {
        public String Name { get; set; }
        public String StrippedName { get; set; }
        public String Topic { get; set; }
        public String StrippedTopic { get; set; }
        public ushort Port { get; set; }
        public IPAddress IP { get; set; }
        public IPEndPoint[] Servers { get; set; }
        public ushort Users { get; set; }
        public RoomLanguage Lang { get; set; }

        public ChannelListItem()
        {
            this.Servers = new IPEndPoint[] { };
        }

        public ChannelListItem(String name, String topic, IPAddress ip, ushort port)
        {
            this.Name = name;
            StringBuilder sb = new StringBuilder();
            int i;

            foreach (char c in this.Name.ToUpper().ToCharArray())
            {
                i = (int)c;

                if ((i >= 65 && i <= 90) || (i >= 48 && i <= 57))
                    sb.Append(c);
            }

            this.StrippedName = sb.ToString();
            this.Topic = topic;
            this.StrippedTopic = Helpers.StripColors(Helpers.FormatAresColorCodes(this.Topic)).ToUpper();
            this.Port = port;
            this.IP = ip;
            this.Servers = new IPEndPoint[] { };
            this.Users = 0;
            this.Lang = RoomLanguage.English;
        }

        public ChannelListItem(IPEndPoint ep, UdpPacketReader packet)
        {
            this.IP = ep.Address;
            this.Port = packet.ReadUInt16();
            this.Users = packet.ReadUInt16();
            this.Name = packet.ReadString();
            StringBuilder sb = new StringBuilder();
            int i2;

            foreach (char c in this.Name.ToUpper().ToCharArray())
            {
                i2 = (int)c;

                if ((i2 >= 65 && i2 <= 90) || (i2 >= 48 && i2 <= 57))
                    sb.Append(c);
            }

            this.StrippedName = sb.ToString();
            this.Topic = packet.ReadString();
            this.StrippedTopic = Helpers.StripColors(Helpers.FormatAresColorCodes(this.Topic)).ToUpper();
            this.Lang = (RoomLanguage)packet.ReadByte();
            packet.ReadString();
            byte count = packet.ReadByte();

            if (count > 0)
            {
                List<IPEndPoint> servers = new List<IPEndPoint>();

                for (int i = 0; i < count; i++)
                {
                    IPAddress ip = packet.ReadIP();
                    ushort port = packet.ReadUInt16();
                    servers.Add(new IPEndPoint(ip, port));
                }

                this.Servers = servers.ToArray();
            }
            else this.Servers = new IPEndPoint[] { };
        }

        public FavouritesListItem ToFavouritesItem()
        {
            FavouritesListItem f = new FavouritesListItem();
            f.IP = this.IP;
            f.Name = this.Name;
            f.Port = this.Port;
            f.Topic = this.Topic;
            f.CountString = this.Users.ToString();
            return f;
        }
    }
}
