using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;

namespace cb0t
{
    class AresSocket
    {
        private Socket sock = null;
        private List<byte> data_in = new List<byte>();
        private List<byte[]> data_out = new List<byte[]>();
        private List<byte[]> trickle_out = new List<byte[]>();
        private int health = 0;
        private int avail = 0;

        public event EventHandler<PacketReceivedEventArgs> PacketReceived;

        public int SockCode { get; set; }

        public void Free()
        {
            this.data_in.Clear();
            this.data_in = new List<byte>();
            this.data_out.Clear();
            this.data_out = new List<byte[]>();
            this.trickle_out.Clear();
            this.trickle_out = new List<byte[]>();
        }

        public void DequeueTrickle()
        {
            if (this.trickle_out.Count > 0)
            {
                this.Send(this.trickle_out[0]);
                this.trickle_out.RemoveAt(0);
            }
        }

        public void SendTrickle(byte[] data)
        {
            this.trickle_out.Add(data);
        }

        public void Clear()
        {
            this.data_in.Clear();
            this.data_out.Clear();
            this.trickle_out.Clear();
        }

        public void Disconnect()
        {
            if (this.sock != null)
            {
                try { this.sock.Disconnect(false); }
                catch { }
                try { this.sock.Shutdown(SocketShutdown.Both); }
                catch { }
                try { this.sock.Close(); }
                catch { }

                this.sock = null;
            }
        }

        public void Connect(IPEndPoint ep)
        {
            this.health = 0;
            this.data_in.Clear();
            this.data_out.Clear();
            this.trickle_out.Clear();
            this.sock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            this.sock.Blocking = false;

            try { this.sock.Connect(ep); }
            catch { }
        }

        public bool Connected
        {
            get
            {
                if (this.sock == null)
                    return false;

                return this.sock.Poll(0, SelectMode.SelectWrite);
            }
        }

        public void Send(byte[] data)
        {
            this.data_out.Add(data);
        }

        public void SendPriority(byte[] data)
        {
            this.data_out.Insert(0, data);
        }

        public bool Service(uint time)
        {
            while (this.data_out.Count > 0)
            {
                try
                {
                    this.sock.Send(this.data_out[0]);
                    this.data_out.RemoveAt(0);
                }
                catch { break; }
            }

            byte[] buf = new byte[8192];
            SocketError e = SocketError.Success;
            int size = 0;
            bool success = true;

            try
            {
                this.avail = this.sock.Available;

                if (this.avail > 8192)
                    this.avail = 8192;

                size = this.sock.Receive(buf, 0, this.avail, SocketFlags.None, out e);
            }
            catch { }

            if (size == 0)
            {
                if (e == SocketError.WouldBlock)
                    this.health = 0;
                else if (this.health++ > 3)
                {
                    this.SockCode = (int)e;
                    success = false;
                }
            }
            else
            {
                this.health = 0;
                this.data_in.AddRange(buf.Take(size));
            }

            while (this.data_in.Count >= 3)
            {
                ushort len = BitConverter.ToUInt16(this.data_in.ToArray(), 0);

                if (this.data_in.Count >= (len + 3))
                {
                    byte id = this.data_in[2];
                    byte[] packet = this.data_in.GetRange(3, len).ToArray();
                    this.data_in.RemoveRange(0, (len + 3));

                    PacketReceivedEventArgs args = new PacketReceivedEventArgs
                    {
                        Msg = (TCPMsg)id,
                        Packet = new TCPPacketReader(packet),
                        Time = time
                    };

                    if (args.Msg == TCPMsg.MSG_CHAT_CLIENTCOMPRESSED)
                    {
                        packet = Zip.Decompress(packet);

                        if (packet != null)
                            if (packet.Length > 0)
                                this.data_in.InsertRange(0, packet);
                    }
                    else
                    {
                        try { this.PacketReceived(null, args); }
                        catch { }
                    }
                }
                else break;
            }

            return success;
        }
    }
}
