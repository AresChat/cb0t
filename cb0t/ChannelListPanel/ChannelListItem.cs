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
        public String Topic { get; set; }
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
            this.Topic = topic;
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
            this.Topic = packet.ReadString();
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
            return f;
        }
    }
}
