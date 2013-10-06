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

        public void Create()
        {
            for (int i=0;i<this.toolStripComboBox1.Items.Count;i++)
                if (this.toolStripComboBox1.Items[i].ToString() == this.lang_pref.ToString())
                {
                    this.toolStripComboBox1.SelectedIndex = i;
                    break;
                }
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            this.toolStripButton1.Enabled = false;
            this.full_channel_list.Clear();
            this.part_channel_list.Clear();
            this.channelListView1.Items.Clear();
            this.gfx_items.ForEach(x => x.Dispose());
            this.gfx_items.Clear();
            this.RefreshList();
        }

        public bool Terminate { get; set; }
        public event EventHandler<ChannelListLabelChangedEventArgs> LabelChanged;

        private List<ChannelListItem> full_channel_list = new List<ChannelListItem>();
        private List<ChannelListItem> part_channel_list = new List<ChannelListItem>();
        private List<ChannelListViewItem> gfx_items = new List<ChannelListViewItem>();
        private RoomLanguage lang_pref = RoomLanguage.Any;
        private ChannelListTopicRenderer gfx = new ChannelListTopicRenderer();

        private void CheckNewItems(ChannelListItem[] rooms)
        {
            try
            {
                this.channelListView1.BeginInvoke((Action)(() =>
                {
                    String text = this.filter_text.ToString().ToUpper();

                    for (int i = 0; i < rooms.Length; i++)
                        if (rooms[i].Users > 0)
                        {
                            this.full_channel_list.Add(rooms[i]);
                            ChannelListViewItem item = new ChannelListViewItem();
                            this.gfx.RenderChannelListItem(item, rooms[i]);
                            this.gfx_items.Add(item);

                            if (rooms[i].Name.ToUpper().Contains(text))
                            {
                                this.part_channel_list.Add(rooms[i]);
                                this.channelListView1.Items.Add(this.gfx_items[this.gfx_items.Count - 1]);
                            }
                        }

                    if (this.full_channel_list.Count == this.part_channel_list.Count)
                        this.LabelChanged(null, new ChannelListLabelChangedEventArgs { Text = "Searching (" + this.full_channel_list.Count + ")" });
                    else
                        this.LabelChanged(null, new ChannelListLabelChangedEventArgs { Text = "Searching (" + this.part_channel_list.Count + "/" + this.full_channel_list.Count + ")" });
                }));
            }
            catch { }
        }

        public void RefreshList()
        {
            new Thread(new ThreadStart(() =>
            {
                this.reloading_list = true;
                this.LabelChanged(null, new ChannelListLabelChangedEventArgs { Text = "Searching..." });
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
                    String last_filter = this.filter_text.ToString();

                    while (true)
                    {
                        if (this.Terminate)
                            return;

                        uint now = Settings.Time;

                        if (last_filter != this.filter_text.ToString())
                        {
                            last_filter = this.filter_text.ToString();
                            this.FilterResults();
                        }

                        if (now > last_push)
                        {
                            last_push = now;

                            if (recv_items.Count > 0)
                            {
                                this.CheckNewItems(recv_items.ToArray());
                                recv_items.Clear();
                            }
                        }

                        if (now > (time + 15))
                        {
                            this.toolStrip1.BeginInvoke((Action)(() => this.toolStripButton1.Enabled = true));

                            try
                            {
                                sock.Close();
                                sock = null;
                            }
                            catch { }

                            this.reloading_list = false;
                            this.SaveServers();

                            if (this.full_channel_list.Count == this.part_channel_list.Count)
                                this.LabelChanged(null, new ChannelListLabelChangedEventArgs { Text = "Channels (" + this.full_channel_list.Count + ")" });
                            else
                                this.LabelChanged(null, new ChannelListLabelChangedEventArgs { Text = "Channels (" + this.part_channel_list.Count + "/" + this.full_channel_list.Count + ")" });

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

        private void SaveServers()
        {
            if (this.full_channel_list.Count > 10)
            {
                List<byte> list = new List<byte>();

                foreach (ChannelListItem i in this.full_channel_list)
                {
                    list.AddRange(i.IP.GetAddressBytes());
                    list.AddRange(BitConverter.GetBytes(i.Port));
                }

                try { File.WriteAllBytes(Settings.DataPath + "servers.dat", list.ToArray()); }
                catch { }

                list.Clear();
            }
            else
            {
                try
                {
                    if (File.Exists(Settings.DataPath + "servers.dat"))
                        File.Delete(Settings.DataPath + "servers.dat");
                }
                catch { }
            }
        }

        private StringBuilder filter_text = new StringBuilder();
        private bool reloading_list = false;

        private void toolStripTextBox1_TextChanged(object sender, EventArgs e)
        {
            if (this.filter_text.Length > 0)
                this.filter_text.Remove(0, this.filter_text.Length);

            this.filter_text.Append(this.toolStripTextBox1.Text);

            if (!this.reloading_list)
                this.FilterResults();
        }

        private void FilterResults()
        {
            if (this.channelListView1.InvokeRequired)
                this.channelListView1.BeginInvoke((Action)(() => this.FilterResults()));
            else
            {
                String text = this.filter_text.ToString().ToUpper();
                this.channelListView1.BeginUpdate();
                this.channelListView1.Items.Clear();
                this.part_channel_list.Clear();

                for (int i = 0; i < this.full_channel_list.Count; i++)
                    if (this.full_channel_list[i].Name.ToUpper().Contains(text))
                    {
                        this.part_channel_list.Add(this.full_channel_list[i]);
                        this.channelListView1.Items.Add(this.gfx_items[i]);
                    }

                this.channelListView1.EndUpdate();

                if (this.reloading_list)
                {
                    if (this.full_channel_list.Count == this.part_channel_list.Count)
                        this.LabelChanged(null, new ChannelListLabelChangedEventArgs { Text = "Searching (" + this.full_channel_list.Count + ")" });
                    else
                        this.LabelChanged(null, new ChannelListLabelChangedEventArgs { Text = "Searching (" + this.part_channel_list.Count + "/" + this.full_channel_list.Count + ")" });
                }
                else
                {
                    if (this.full_channel_list.Count == this.part_channel_list.Count)
                        this.LabelChanged(null, new ChannelListLabelChangedEventArgs { Text = "Channels (" + this.full_channel_list.Count + ")" });
                    else
                        this.LabelChanged(null, new ChannelListLabelChangedEventArgs { Text = "Channels (" + this.part_channel_list.Count + "/" + this.full_channel_list.Count + ")" });
                }
            }
        }


    }
}
