using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Diagnostics;

namespace cb0t
{
    public partial class ChannelListPanel : UserControl
    {
        public ChannelListPanel()
        {
            this.InitializeComponent();
            this.contextMenuStrip2.Items.Insert(4, new ToolStripLabel("Admin password:"));
            this.toolStripButton1.Image = (Bitmap)Properties.Resources.refresh.Clone();
            this.toolStrip1.Renderer = new ChannelListBar();
        }

        private bool setting_up = false;

        public void Create()
        {
            this.setting_up = true;
            this.filter_lang = (RoomLanguage)Enum.Parse(typeof(RoomLanguage), Settings.GetReg<String>("filter_lang", "Any"));
   
            for (int i=0;i<this.toolStripComboBox1.Items.Count;i++)
                if (this.toolStripComboBox1.Items[i].ToString() == this.filter_lang.ToString())
                {
                    this.toolStripComboBox1.SelectedIndex = i;
                    break;
                }

            int fav_split = Settings.GetReg<int>("clist_pos", -1);

            if (fav_split > 0 && (this.splitContainer1.ClientSize.Height - fav_split) > 0)
                this.splitContainer1.SplitterDistance = (this.splitContainer1.ClientSize.Height - fav_split);

            this.LoadCache();
            this.LoadFavourites();
            this.setting_up = false;
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
        public event EventHandler<OpenChannelEventArgs> OpenChannel;

        private List<ChannelListItem> full_channel_list = new List<ChannelListItem>();
        private List<ChannelListItem> part_channel_list = new List<ChannelListItem>();
        private List<ChannelListViewItem> gfx_items = new List<ChannelListViewItem>();
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
                                if (rooms[i].Lang == this.filter_lang || this.filter_lang == RoomLanguage.Any)
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
                    RoomLanguage last_lang = this.filter_lang;

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
                        else if (last_lang != this.filter_lang)
                        {
                            last_lang = this.filter_lang;
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
                            this.SaveCache();

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
                list = null;
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
        private RoomLanguage filter_lang = RoomLanguage.Any;
        private bool reloading_list = false;

        private void toolStripTextBox1_TextChanged(object sender, EventArgs e)
        {
            if (this.filter_text.Length > 0)
                this.filter_text.Remove(0, this.filter_text.Length);

            this.filter_text.Append(this.toolStripTextBox1.Text);

            if (!this.reloading_list)
                this.FilterResults();
        }

        private void toolStripComboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!this.setting_up)
            {
                this.filter_lang = (RoomLanguage)Enum.Parse(typeof(RoomLanguage), this.toolStripComboBox1.SelectedItem.ToString());
                Settings.SetReg("filter_lang", this.filter_lang.ToString());

                if (!this.reloading_list)
                    this.FilterResults();
            }
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
                        if (this.full_channel_list[i].Lang == this.filter_lang || this.filter_lang == RoomLanguage.Any)
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

        private void SaveCache()
        {
            if (this.full_channel_list.Count > 10)
            {
                List<byte> list = new List<byte>();

                foreach (ChannelListItem i in this.full_channel_list)
                {
                    list.AddRange(i.IP.GetAddressBytes());
                    list.AddRange(BitConverter.GetBytes(i.Port));
                    list.Add((byte)i.Lang);
                    list.AddRange(BitConverter.GetBytes((ushort)Encoding.UTF8.GetByteCount(i.Name)));
                    list.AddRange(Encoding.UTF8.GetBytes(i.Name));
                    list.AddRange(BitConverter.GetBytes((ushort)Encoding.UTF8.GetByteCount(i.Topic)));
                    list.AddRange(Encoding.UTF8.GetBytes(i.Topic));
                    list.AddRange(BitConverter.GetBytes(i.Users));
                }

                try { File.WriteAllBytes(Settings.DataPath + "cache.dat", list.ToArray()); }
                catch { }

                list.Clear();
                list = null;
            }
        }

        private void LoadCache()
        {
            List<byte> list = new List<byte>();

            try { list.AddRange(File.ReadAllBytes(Settings.DataPath + "cache.dat")); }
            catch { }

            if (list.Count > 0)
            {
                UdpPacketReader buf = new UdpPacketReader(list.ToArray());
                list.Clear();
                list = null;

                while (buf.Remaining() > 0)
                {
                    ChannelListItem item = new ChannelListItem();
                    item.IP = buf.ReadIP();
                    item.Port = buf.ReadUInt16();
                    item.Lang = (RoomLanguage)buf.ReadByte();
                    item.Name = buf.ReadString();
                    item.Topic = buf.ReadString();
                    item.Users = buf.ReadUInt16();
                    this.full_channel_list.Add(item);
                    ChannelListViewItem vitem = new ChannelListViewItem();
                    this.gfx.RenderChannelListItem(vitem, item);
                    this.gfx_items.Add(vitem);
                }

                buf = null;
                this.FilterResults();
            }
        }

        private void splitContainer1_SplitterMoved(object sender, SplitterEventArgs e)
        {
            if (!this.setting_up)
            {
                int fav_split = Settings.GetReg<int>("clist_pos", -1);

                if (this.splitContainer1.Panel2.Height != fav_split)
                    Settings.SetReg("clist_pos", this.splitContainer1.Panel2.Height + 4);
            }
        }

        private List<FavouritesListItem> favs = new List<FavouritesListItem>();
        private List<ChannelListViewItem> g_favs = new List<ChannelListViewItem>();

        private void LoadFavourites()
        {
            List<byte> list = new List<byte>();

            try { list.AddRange(File.ReadAllBytes(Settings.DataPath + "favourites.dat")); }
            catch { }

            if (list.Count > 0)
            {
                UdpPacketReader buf = new UdpPacketReader(list.ToArray());
                list.Clear();
                list = null;

                while (buf.Remaining() > 0)
                {
                    FavouritesListItem item = new FavouritesListItem();
                    item.AutoJoin = buf.ReadByte() == 1;
                    item.IP = buf.ReadIP();
                    item.Port = buf.ReadUInt16();
                    item.Name = buf.ReadString();
                    item.Topic = buf.ReadString();
                    item.Password = buf.ReadString();
                    this.favs.Add(item);
                    ChannelListViewItem vitem = new ChannelListViewItem();
                    this.gfx.RenderChannelListItem(vitem, item);
                    this.g_favs.Add(vitem);
                    this.channelListView2.Items.Add(vitem);
                }

                buf = null;
            }
        }

        private void SaveFavourites()
        {
            List<byte> list = new List<byte>();

            foreach (FavouritesListItem i in this.favs)
            {
                list.Add((byte)(i.AutoJoin ? 1 : 0));
                list.AddRange(i.IP.GetAddressBytes());
                list.AddRange(BitConverter.GetBytes(i.Port));
                list.AddRange(BitConverter.GetBytes((ushort)Encoding.UTF8.GetByteCount(i.Name)));
                list.AddRange(Encoding.UTF8.GetBytes(i.Name));
                list.AddRange(BitConverter.GetBytes((ushort)Encoding.UTF8.GetByteCount(i.Topic)));
                list.AddRange(Encoding.UTF8.GetBytes(i.Topic));
                list.AddRange(BitConverter.GetBytes((ushort)Encoding.UTF8.GetByteCount(i.Password)));
                list.AddRange(Encoding.UTF8.GetBytes(i.Password));
            }

            try { File.WriteAllBytes(Settings.DataPath + "favourites.dat", list.ToArray()); }
            catch { }

            list.Clear();
            list = null;
        }

        private void contextMenuStrip1_Opening(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (this.channelListView1.SelectedItems.Count < 1)
                e.Cancel = true;
        }

        private void contextMenuStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            if (this.channelListView1.SelectedItems.Count > 0)
                if (e.ClickedItem.Tag != null)
                {
                    String button = e.ClickedItem.Tag.ToString();

                    switch (button)
                    {
                        case "1":
                            this.ExportHashlink(this.part_channel_list[this.channelListView1.SelectedIndices[0]].ToFavouritesItem());
                            break;

                        case "2":
                            this.AddToFavourites(this.part_channel_list[this.channelListView1.SelectedIndices[0]]);
                            break;
                    }
                }
        }

        private void AddToFavourites(ChannelListItem room)
        {
            if (this.favs.Find(x => x.IP.Equals(room.IP) && x.Port == room.Port) != null)
                return;

            FavouritesListItem f = room.ToFavouritesItem();
            this.favs.Add(f);
            ChannelListViewItem vitem = new ChannelListViewItem();
            this.gfx.RenderChannelListItem(vitem, f);
            this.g_favs.Add(vitem);
            this.channelListView2.Items.Add(vitem);
            this.SaveFavourites();
        }

        public void AddToFavourites(FavouritesListItem room)
        {
            if (this.favs.Find(x => x.IP.Equals(room.IP) && x.Port == room.Port) != null)
                return;

            this.favs.Add(room);
            ChannelListViewItem vitem = new ChannelListViewItem();
            this.gfx.RenderChannelListItem(vitem, room);
            this.g_favs.Add(vitem);
            this.channelListView2.Items.Add(vitem);
            this.SaveFavourites();
        }

        private void ExportHashlink(FavouritesListItem room)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(room.Name);
            sb.Append("arlnk://");
            sb.AppendLine(Hashlink.EncodeHashlink(room));

            try
            {
                File.WriteAllText(Settings.DataPath + "hashlink.txt", sb.ToString());
                Process.Start("notepad.exe", Settings.DataPath + "hashlink.txt");
            }
            catch { }
        }

        private void contextMenuStrip2_Opening(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (this.channelListView2.SelectedItems.Count < 1)
                e.Cancel = true;
            else
            {
                this.autoJoinToolStripMenuItem.Checked = this.favs[this.channelListView2.SelectedIndices[0]].AutoJoin;
                this.toolStripTextBox2.Text = this.favs[this.channelListView2.SelectedIndices[0]].Password;
            }
        }

        private void contextMenuStrip2_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            if (this.channelListView2.SelectedItems.Count > 0)
                if (e.ClickedItem.Tag != null)
                {
                    String button = e.ClickedItem.Tag.ToString();

                    switch (button)
                    {
                        case "1":
                            this.ExportHashlink(this.favs[this.channelListView2.SelectedIndices[0]]);
                            break;

                        case "2":
                            this.RemoveFromFavourites(this.channelListView2.SelectedIndices[0]);
                            break;

                        case "3":
                            this.ChangeAutoJoin(this.channelListView2.SelectedIndices[0]);
                            break;
                    }
                }
        }

        private void RemoveFromFavourites(int index)
        {
            this.channelListView2.Items.RemoveAt(index);
            this.g_favs[index].Dispose();
            this.g_favs.RemoveAt(index);
            this.favs.RemoveAt(index);
            this.SaveFavourites();
        }

        private void ChangeAutoJoin(int index)
        {
            this.favs[index].AutoJoin = !this.favs[index].AutoJoin;
            this.SaveFavourites();
        }

        private void contextMenuStrip2_Closing(object sender, ToolStripDropDownClosingEventArgs e)
        {
            if (this.channelListView2.SelectedIndices.Count > 0)
            {
                FavouritesListItem item = this.favs[this.channelListView2.SelectedIndices[0]];
                String pass = this.toolStripTextBox2.Text;

                if (item.Password != pass)
                {
                    item.Password = pass;
                    this.SaveFavourites();
                }
            }
        }

        private void channelListView1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
                if (this.channelListView1.SelectedItems.Count > 0)
                    this.OpenChannel(null, new OpenChannelEventArgs { Room = this.part_channel_list[this.channelListView1.SelectedIndices[0]].ToFavouritesItem() });
        }

        private void channelListView2_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
                if (this.channelListView2.SelectedItems.Count > 0)
                    this.OpenChannel(null, new OpenChannelEventArgs { Room = this.favs[this.channelListView2.SelectedIndices[0]] });
        }

        public FavouritesListItem[] GetAutoJoinRooms()
        {
            return this.favs.FindAll(x => x.AutoJoin).ToArray();
        }
    }
}
