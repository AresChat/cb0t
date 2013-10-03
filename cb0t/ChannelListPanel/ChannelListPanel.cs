using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.Net;
using System.Net.Sockets;
using System.IO;

namespace cb0t
{
    public partial class ChannelListPanel : UserControl
    {
        public ChannelListPanel()
        {
            this.InitializeComponent();
            this.toolStripButton1.Image = (Bitmap)Properties.Resources.refresh;
            this.toolStrip1.Renderer = new ChannelListBar();
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {

        }

        public bool Terminate { get; set; }
        private Queue<ChannelListItem> socket_found = new Queue<ChannelListItem>();

        public void RefreshList()
        {
            new Thread(new ThreadStart(() =>
            {
                this.Terminate = false;
                byte[] raw = null;

                if (File.Exists(Settings.DataPath + "servers.dat"))
                    raw = File.ReadAllBytes(Settings.DataPath + "servers.dat");
                else
                    raw = File.ReadAllBytes(Settings.AppPath + "servers.dat");

                if (raw != null)
                {
                    List<byte> list = new List<byte>(raw);
                    List<IPEndPoint> to_send = new List<IPEndPoint>();

                    while (list.Count >= 6)
                    {
                        IPAddress ip = new IPAddress(list.GetRange(0, 4).ToArray());
                        list.RemoveRange(0, 4);
                        ushort port = BitConverter.ToUInt16(list.ToArray(), 0);
                        list.RemoveRange(0, 2);
                        to_send.Add(new IPEndPoint(ip, port));
                    }

                    Socket sock = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                    sock.Blocking = false;

                    List<IPEndPoint> sent_items = new List<IPEndPoint>();
                    List<ChannelListItem> recv_items = new List<ChannelListItem>();

                    EndPoint recv_ep = new IPEndPoint(IPAddress.Any, 0);
                    uint time = Settings.Time;
                    uint last_push = time;

                    while (true)
                    {
                        if (this.Terminate)
                            return;

                        uint now = Settings.Time;

                        if (now > last_push)
                        {
                            last_push = now;

                            foreach (ChannelListItem c in recv_items)
                                this.socket_found.Enqueue(c);

                            recv_items.Clear();
                        }

                        if (now > (time + 15))
                        {
                            this.toolStrip1.BeginInvoke((Action)(() => this.toolStripButton1.Enabled = true));
                            sock.Close();
                        //    this.SaveServers();
                          //  this.searching = false;
                            //this.SearchEnded(this, EventArgs.Empty);
                            return;
                        }

                        if (to_send.Count > 0)
                        {
                            try { sock.SendTo(new byte[] { 2 }, to_send[0]); }
                            catch { }

                            sent_items.Add(to_send[0]);
                            to_send.RemoveAt(0);
                            time = now;
                        }

                        while (sock.Available > 0)
                        {
                            byte[] buf = new byte[4096];
                            int size = 0;

                            try { size = sock.ReceiveFrom(buf, ref recv_ep); }
                            catch { }

                            if (size > 0)
                                if (buf[0] == 3)
                                {
                                    UdpPacketReader packet = new UdpPacketReader(buf.Skip(1).Take(size - 1).ToArray());
                                    ChannelListItem item = null;
                                    time = now;

                                    try { item = new ChannelListItem((IPEndPoint)recv_ep, packet); }
                                    catch { }

                                    if (item != null)
                                    {
                                        recv_items.Add(item);

                                        foreach (IPEndPoint e in item.Servers)
                                            if (!to_send.Contains(e))
                                                if (!sent_items.Contains(e))
                                                    to_send.Add(e);
                                    }
                                }
                        }

                        Thread.Sleep(50);
                    }
                }
            })).Start();
        }


    }
}
