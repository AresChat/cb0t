using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Net.Sockets;

namespace cb0t
{
    class Room
    {
        public ChannelButton Button { get; set; }
        public FavouritesListItem Credentials { get; set; }
        public RoomPanel Panel { get; set; }
        public IPEndPoint EndPoint { get; set; }

        private SessionState state = SessionState.Sleeping;
        private int reconnect_count = 0;
        private AresSocket sock = new AresSocket();
        private uint ticks = 0;

        public Room(uint time, FavouritesListItem item)
        {
            this.Credentials = item;
            this.EndPoint = new IPEndPoint(item.IP, item.Port);
            this.ticks = (time - 19);
            this.sock.PacketReceived += this.PacketReceived;
        }

        public void Release()
        {
            this.sock.Disconnect();
            this.sock.PacketReceived -= this.PacketReceived;
            this.sock.Free();
            this.sock = null;
            this.Button.Free();
            this.Button.Dispose();
            this.Button = null;
            this.Panel.Free();
            this.Panel.Dispose();
            this.Panel = null;
        }

        public void SocketTasks(uint time)
        {
            if (this.state == SessionState.Sleeping)
            {
                if (time >= (this.ticks + 20))
                {
                    this.state = SessionState.Connecting;
                    this.sock.Connect(this.EndPoint);                    
                    this.ticks = time;

                    if (this.reconnect_count > 0)
                        this.Panel.ServerText("Connecting to host, please wait... #" + this.reconnect_count);
                    else
                        this.Panel.ServerText("Connecting to host, please wait...");
                }
            }
            else if (this.state == SessionState.Connecting)
            {
                if (this.sock.Connected)
                {
                    this.state = SessionState.Connected;
                    this.ticks = time;
                    this.Panel.ServerText("Connected, handshaking...");
                }
                else if (time >= (this.ticks + 10))
                {
                    this.ticks = time;
                    this.state = SessionState.Sleeping;
                    this.sock.Disconnect();
                    this.reconnect_count++;
                    this.Panel.AnnounceText("Unable to connect");
                }
            }
            else if (!this.sock.Service())
            {
                this.ticks = time;
                this.state = SessionState.Sleeping;
                this.sock.Disconnect();
                this.reconnect_count++;
                this.Panel.AnnounceText("Disconnected (remote connection reset)");
            }
        }

        private void PacketReceived(object sender, PacketReceivedEventArgs e)
        {
            switch (e.Msg)
            {
                default:
                    this.Panel.AnnounceText(e.Msg.ToString());
                    break;
            }
        }


    }
}
