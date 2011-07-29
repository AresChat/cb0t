using System;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Collections.Generic;

namespace cb0t_chat_client_v2
{
    class ChannelList : ListView
    {
        public delegate void ChannelClickedDelegate(ChannelObject cObj);
        public delegate void ChannelTotalUpdateDelegate(String text);
        public event ChannelClickedDelegate OnChannelClicked;
        public event ChannelClickedDelegate OnChannelAddToFavourites;
        public event ChannelTotalUpdateDelegate OnChannelTotalUpdated;

        public bool terminate = false;
        public String Query = String.Empty;

        private List<ChannelObject> current_list = new List<ChannelObject>();
        private List<ChannelObject> full_list = new List<ChannelObject>();
        private ChannelListSortingItem sort_item = ChannelListSortingItem.None;
        private bool udp_busy = false;
        private Thread thread;
        
        public void RefreshList(String query)
        {
            if (this.udp_busy)
                return; // already refreshing

            this.Items.Clear();
            this.full_list.Clear();
            this.current_list.Clear();
            this.udp_busy = true;
            this.OnChannelTotalUpdated("Searching...");
            this.thread = new Thread(new ThreadStart(this.Worker));
            this.thread.Start();
        }

        private void Worker()
        {
            Thread.Sleep(1000);
            Server[] nodes = ServerPool.GetServers();

            List<IPEndPoint> to_send = new List<IPEndPoint>();

            foreach (Server n in nodes)
                to_send.Add(new IPEndPoint(n.ip, n.port));

            List<IPEndPoint> have_sent = new List<IPEndPoint>();
            List<IPEndPoint> successful_receives = new List<IPEndPoint>();

            List<ChannelObject> recently_received = new List<ChannelObject>();
            uint last_action = Time;
            uint last_publish = Time;
            EndPoint ep = new IPEndPoint(IPAddress.Any, 0);

            Socket sock = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            sock.Blocking = false;

            while (true)
            {
                if (this.terminate) return;
                uint now = Time;

                if (last_publish < now) // send some channels
                {
                    last_publish = now;

                    if (recently_received.Count > 0)
                    {
                        this.AddChannelsCrossThreaded(recently_received.ToArray());
                        recently_received.Clear();
                    }
                }

                if ((last_action + 15) < now) // game over
                {
                    this.udp_busy = false;
                    ServerPool.SaveChannelListCache(this.full_list.ToArray());
                    ServerPool.Save(successful_receives.ToArray());
                    this.AddChannelsCrossThreaded(recently_received.ToArray());
                    return;
                }

                if (to_send.Count > 0) // send
                {
                    try
                    {
                        sock.SendTo(new byte[] { 2 }, to_send[0]);
                        have_sent.Add(to_send[0]);
                    }
                    catch { }

                    to_send.RemoveAt(0);
                    last_action = now;
                }

                while (sock.Available > 0) // receive
                {
                    try
                    {
                        byte[] buffer = new byte[8192];
                        int buffer_size = sock.ReceiveFrom(buffer, ref ep);

                        if (buffer_size > 0)
                        {
                            last_action = now;

                            byte[] tmp = new byte[buffer_size];
                            Array.Copy(buffer, tmp, buffer_size);

                            UdpPacketReader packet = new UdpPacketReader(tmp);

                            if (packet.ReadByte() == 3)
                            {
                                successful_receives.Add((IPEndPoint)ep);
                                ChannelObject co = new ChannelObject();
                                co.ip = ((IPEndPoint)ep).Address;
                                co.port = packet.ReadInt16();
                                co.users = packet.ReadInt16();
                                co.name = packet.ReadString();
                                co.topic = packet.ReadString();
                                co.language = ((ChannelLanguage)packet.ReadByte()).ToString();
                                co.server = ServerName(packet.ReadString());
                                recently_received.Add(co);

                                if (packet.Remaining() > 1)
                                {
                                    packet.ReadByte();

                                    while (packet.Remaining() >= 6)
                                    {
                                        IPAddress ip = packet.ReadIP();
                                        ushort port = packet.ReadInt16();

                                        if (to_send.Find(x => x.Address.Equals(ip)) == null)
                                            if (have_sent.Find(x => x.Address.Equals(ip)) == null)
                                                to_send.Add(new IPEndPoint(ip, port));
                                    }
                                }
                            }
                        }
                    }
                    catch { break; }
                }

                Thread.Sleep(50);
            }
        }

        public void PopulateWithLastKnownList()
        {
            ChannelObject[] cached = ServerPool.LastChannelList;

            if (cached != null)
            {
                this.full_list = new List<ChannelObject>(cached);
                this.current_list = new List<ChannelObject>(cached);
                this.BeginUpdate();
                this.AddChannels(cached);
                this.EndUpdate();
                this.OnChannelTotalUpdated("Channels (" + this.full_list.Count + ")");
            }
        }

        public void SearchList(String query)
        {
            if (this.full_list.Count > 0)
            {
                this.Query = query.ToUpper();
                this.BeginUpdate();
                this.Items.Clear();

                this.current_list.Clear();

                this.current_list.AddRange(this.full_list.FindAll(
                    delegate(ChannelObject x)
                    {
                        return x.topic.ToUpper().Contains(this.Query) ||
                            x.name.ToUpper().Contains(this.Query) ||
                            x.server.ToUpper().Contains(this.Query);
                    }));

                this.AddChannels(this.current_list.ToArray());
                this.EndUpdate();

                if (this.current_list.Count != this.full_list.Count)
                    this.OnChannelTotalUpdated((this.udp_busy ? "Searching..." : "Channels") + " (" + this.current_list.Count + "/" + this.full_list.Count + ")");
                else
                    this.OnChannelTotalUpdated((this.udp_busy ? "Searching..." : "Channels") + " (" + this.full_list.Count + ")");
            }
        }

        private int CurrentWidth { get; set; }
        private int CurrentHover { get; set; }
        private int CurrentHoverClear { get; set; }

        private SolidBrush hover_brush = new SolidBrush(Color.LightGray);
        private SolidBrush clear_brush = new SolidBrush(SystemColors.Window);
        private SolidBrush selected_brush = new SolidBrush(Color.CornflowerBlue);

        public ChannelList()
        {
            this.Columns.Add("Name");
            this.Columns[0].Width = 170;
            this.Columns.Add("Server");
            this.Columns[1].Width = 68;
            this.Columns.Add("Users");
            this.Columns[2].Width = 45;
            this.Columns.Add("Language");
            this.Columns[3].Width = 72;
            this.Columns.Add("Topic");
            this.Columns[4].Width = 335;

            this.OwnerDraw = true;
            this.DoubleBuffered = true;
            this.View = View.Details;
            this.FullRowSelect = true;
            this.MultiSelect = false;
            this.CurrentWidth = this.Width;
            this.CurrentHover = -1;
            this.CurrentHoverClear = -1;

            this.ContextMenuStrip = new ContextMenuStrip();
            this.ContextMenuStrip.Items.Add("Export hashlink");
            this.ContextMenuStrip.Items.Add("Add to favourites");
            this.ContextMenuStrip.Items[0].Click += new EventHandler(this.ShowHashlink);
            this.ContextMenuStrip.Items[1].Click += new EventHandler(this.AddToFavourites);
        }

        private void ShowHashlink(object sender, EventArgs e)
        {
            if (this.SelectedItems.Count > 0)
                Helpers.ExportHashlink(this.current_list[this.SelectedIndices[0]]);
        }

        private void AddToFavourites(object sender, EventArgs e)
        {
            if (this.SelectedItems.Count > 0)
                this.OnChannelAddToFavourites(this.current_list[this.SelectedIndices[0]]);
        }

        private static uint Time
        {
            get
            {
                TimeSpan ts = (DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0));
                return (uint)ts.TotalSeconds;
            }
        }

        private static String ServerName(String org)
        {
            if (org.Contains("[DS]"))
                return "DS";

            if (org.Contains("sb0t"))
                return "sb0t";

            if (org.Contains("Arca Eclipse"))
                return "AE";

            if (org.Contains("RUNYA"))
                return "Runya";

            if (org.ToUpper().Contains("CALLISTO"))
                return "Callisto";

            if (org.ToUpper().Contains("SURFICTION"))
                return "Surf";

            if (org.ToUpper().Contains("AURA"))
                return "AurA";

            return "Ares";
        }

        private delegate void SomeChannelsHandler(object r);
        private void AddChannelsCrossThreaded(object r)
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke(new SomeChannelsHandler(this.AddChannelsCrossThreaded), r);
            }
            else
            {
                ChannelObject[] rooms = (ChannelObject[])r;

                this.full_list.AddRange(rooms);

                foreach (ChannelObject x in rooms)
                {
                    if (x.topic.ToUpper().Contains(this.Query) ||
                            x.name.ToUpper().Contains(this.Query) ||
                            x.server.ToUpper().Contains(this.Query))
                    {
                        this.current_list.Add(x);
                        this.Items.Add(new ListViewItem(new String[] { x.name, x.server, x.users.ToString(), x.language, x.topic }));
                    }
                }

                if (this.current_list.Count != this.full_list.Count)
                    this.OnChannelTotalUpdated((this.udp_busy ? "Searching..." : "Channels") + " (" + this.current_list.Count + "/" + this.full_list.Count + ")");
                else
                    this.OnChannelTotalUpdated((this.udp_busy ? "Searching..." : "Channels") + " (" + this.full_list.Count + ")");
            }
        }

        private void AddChannels(ChannelObject[] rooms)
        {
            foreach (ChannelObject i in rooms)
                this.Items.Add(new ListViewItem(new String[] { i.name, i.server, i.users.ToString(), i.language, i.topic }));
        }

        protected override void OnMouseDoubleClick(MouseEventArgs e)
        {
            if (this.SelectedItems.Count > 0)
            {
                ChannelObject _obj = this.current_list[this.SelectedItems[0].Index];
                this.OnChannelClicked(_obj);
            }
            else base.OnMouseDoubleClick(e);
        }

        protected override void OnKeyPress(KeyPressEventArgs e)
        {
            if (e.KeyChar == 13)
            {
                if (this.SelectedItems.Count > 0)
                {
                    ChannelObject _obj = this.current_list[this.SelectedItems[0].Index];
                    this.OnChannelClicked(_obj);
                }

                e.Handled = true;
            }
            else base.OnKeyPress(e);
        }

        protected override void OnColumnClick(ColumnClickEventArgs e)
        {
            if (this.current_list.Count > 1)
            {
                if ((ChannelListSortingItem)e.Column == this.sort_item)
                {
                    this.full_list.Reverse();
                    this.SearchList(this.Query);
                }
                else
                {
                    this.sort_item = (ChannelListSortingItem)e.Column;

                    switch (this.sort_item)
                    {
                        case ChannelListSortingItem.Name:
                            this.full_list.Sort(delegate(ChannelObject x, ChannelObject y) { return String.Compare(x.name, y.name); });
                            break;

                        case ChannelListSortingItem.Server:
                            this.full_list.Sort(delegate(ChannelObject x, ChannelObject y) { return String.Compare(x.server, y.server); });
                            break;

                        case ChannelListSortingItem.Users:
                            this.full_list.Sort(delegate(ChannelObject x, ChannelObject y) { return x.users - y.users; });
                            break;

                        case ChannelListSortingItem.Language:
                            this.full_list.Sort(delegate(ChannelObject x, ChannelObject y) { return String.Compare(x.language, y.language); });
                            break;

                        case ChannelListSortingItem.Topic:
                            this.full_list.Sort(delegate(ChannelObject x, ChannelObject y) { return String.Compare(x.topic, y.topic); });
                            break;
                    }

                    this.SearchList(this.Query);
                }
            }
        }

        protected override void OnColumnWidthChanging(ColumnWidthChangingEventArgs e)
        {
            e.Cancel = true;

            switch (e.ColumnIndex)
            {
                case 0:
                    e.NewWidth = 170;
                    break;

                case 1:
                    e.NewWidth = 68;
                    break;

                case 2:
                    e.NewWidth = 45;
                    break;

                case 3:
                    e.NewWidth = 72;
                    break;

                case 4:
                    e.NewWidth = (this.Width - 383);
                    break;
            }
        }

        protected override void OnResize(EventArgs e)
        {
            if (this.CurrentWidth != this.Width)
            {
                this.CurrentWidth = this.Width;
                this.Columns[4].Width = (this.Width - 383);
            }
        }

        protected override void OnDrawColumnHeader(DrawListViewColumnHeaderEventArgs e)
        {
            e.DrawDefault = true;
        }

        protected override void OnMouseMove(MouseEventArgs e) // hot tracking
        {
            base.OnMouseMove(e);

            ListViewItem item = this.GetItemAt(e.X, e.Y);

            if (item != null)
            {
                if (item.Index == this.CurrentHover)
                    return;

                if (this.CurrentHover > -1)
                {
                    if (this.Items.Count > this.CurrentHover)
                    {
                        this.CurrentHoverClear = this.CurrentHover;

                        if (this.Items.Count > this.CurrentHoverClear)
                            this.Invalidate(this.Items[this.CurrentHoverClear].Bounds);

                        if (!item.Selected)
                        {
                            this.CurrentHover = item.Index;
                            this.Invalidate(this.Items[this.CurrentHover].Bounds);
                        }
                        else this.CurrentHover = -1;
                    }
                }
                else
                {
                    if (!item.Selected)
                    {
                        this.CurrentHover = item.Index;
                        this.Invalidate(this.Items[this.CurrentHover].Bounds);
                    }
                    else this.CurrentHover = -1;
                }
            }
            else if (this.CurrentHover > -1)
            {
                this.CurrentHoverClear = this.CurrentHover;
                this.CurrentHover = -1;

                if (this.Items.Count > this.CurrentHoverClear)
                    this.Invalidate(this.Items[this.CurrentHoverClear].Bounds);
            }
        }

        protected override void OnMouseLeave(EventArgs e) // cancel hot tracking
        {
            base.OnMouseLeave(e);

            if (this.CurrentHover > -1)
            {
                int i = this.CurrentHover;
                this.CurrentHover = -1;

                if (this.Items.Count > i)
                    this.Invalidate(this.Items[i].Bounds);
            }
        }

        protected override void OnSelectedIndexChanged(EventArgs e)
        {
            base.OnSelectedIndexChanged(e);

            if (this.SelectedIndices.Count > 0)
                this.Invalidate(this.Items[this.SelectedIndices[0]].Bounds);
        }

        protected override void OnDrawItem(DrawListViewItemEventArgs e)
        {
            if (e.Item.Selected)
            {
                e.Graphics.FillRectangle(this.selected_brush, new Rectangle(e.Bounds.X, e.Bounds.Y, e.Bounds.Width, e.Bounds.Height - 2));
            }
            else
            {
                if (e.Item.Index == this.CurrentHover)
                    e.Graphics.FillRectangle(this.hover_brush, new Rectangle(e.Bounds.X, e.Bounds.Y, e.Bounds.Width, e.Bounds.Height - 2));
                else if (e.Item.Index == this.CurrentHoverClear)
                {
                    e.Graphics.FillRectangle(this.clear_brush, new Rectangle(e.Bounds.X, e.Bounds.Y, e.Bounds.Width, e.Bounds.Height - 2));
                    this.CurrentHoverClear = -1;
                }
            }
        }

        protected override void OnDrawSubItem(DrawListViewSubItemEventArgs e)
        {
            if (e.ColumnIndex != 4)
            {
                if (e.ColumnIndex == 0)
                {
                    e.Graphics.DrawImage(AresImages.Chat, new RectangleF(e.Bounds.X, e.Bounds.Y + 1, 14, 14));
                    e.DrawText();
                }
                else e.DrawText(TextFormatFlags.Default);
            }
            else //topic
            {
                char[] letters = e.SubItem.Text.ToCharArray();

                Color org_back_color = e.Item.Selected ? Color.CornflowerBlue : (e.Item.Index == this.CurrentHover ? Color.LightGray : SystemColors.Window);
                Color fore_color = Color.Black;
                bool bold = false, italic = false, underline = false, back_color_required = false;
                int x = 0;
                int color_finder;

                for (int i = 0; i < letters.Length; i++)
                {
                    switch (letters[i])
                    {
                        case '\x0006': // bold
                            bold = !bold;
                            break;

                        case '\x0007': // underline
                            underline = !underline;
                            break;

                        case '\x0009': // italic
                            italic = !italic;
                            break;

                        case '\x0003': // fore color
                            if (letters.Length >= (i + 3))
                            {
                                if (int.TryParse(e.SubItem.Text.Substring(i + 1, 2), out color_finder))
                                {
                                    fore_color = this.GetColorFromCode(color_finder);
                                    i += 2;
                                }
                                else goto default;
                            }
                            else goto default;
                            break;

                        case '\x0005': // back color
                            if (letters.Length >= (i + 3))
                            {
                                if (int.TryParse(e.SubItem.Text.Substring(i + 1, 2), out color_finder))
                                {
                                    Color back_color = this.GetColorFromCode(color_finder);
                                    back_color_required = true;
                                    i += 2;

                                    using (SolidBrush brush = new SolidBrush(back_color))
                                        e.Graphics.FillRectangle(brush, new Rectangle(e.SubItem.Bounds.X + x + 2, e.SubItem.Bounds.Y, e.SubItem.Bounds.Width - x - 2, e.SubItem.Bounds.Height - 2));
                                }
                                else goto default;
                            }
                            else goto default;
                            break;

                        case ' ': // space
                            x += underline ? 1 : (bold ? 3 : 2);

                            if (x > (e.SubItem.Bounds.Width - 1))
                                break;

                            if (underline)
                                using (Font font = new Font(e.Item.Font, this.CreateFont(bold, italic, underline)))
                                using (SolidBrush brush = new SolidBrush(fore_color))
                                    e.Graphics.DrawString(" ", font, brush, new PointF(e.SubItem.Bounds.X + x, e.SubItem.Bounds.Y));
                            break;

                        case '+':
                        case '(':
                        case ':':
                        case ';': // emoticons
                            int emote_index = EmoticonFinder.GetRTFEmoticonFromKeyboardShortcut(e.SubItem.Text.Substring(i).ToUpper());

                            if (emote_index > -1)
                            {
                                if ((x + 15) > (e.SubItem.Bounds.Width - 1))
                                {
                                    x += 15;
                                    break;
                                }

                                e.Graphics.DrawImage(AresImages.TransparentEmoticons[emote_index], new RectangleF(e.SubItem.Bounds.X + x + 2, e.SubItem.Bounds.Y, 15, 15));
                                x += 15;
                                i += (EmoticonFinder.last_emote_length - 1);
                                break;
                            }
                            else goto default;

                        default: // text
                            using (Font font = new Font(e.Item.Font, this.CreateFont(bold, italic, underline)))
                            {
                                int width = (int)Math.Round((double)e.Graphics.MeasureString(letters[i].ToString(), font, 100, StringFormat.GenericTypographic).Width);

                                if ((x + width) > (e.SubItem.Bounds.Width - 1))
                                {
                                    x += width;
                                    break;
                                }

                                using (SolidBrush brush = new SolidBrush(fore_color))
                                    e.Graphics.DrawString(letters[i].ToString(), font, brush, new PointF(e.SubItem.Bounds.X + x, e.SubItem.Bounds.Y));

                                x += width;
                            }
                            break;
                    }

                    if (x > (e.SubItem.Bounds.Width - 1)) // run out of space - stop drawing!!
                        return;
                }

                if (back_color_required) // trim excess background because the topic is shorter than the column width
                    if ((x + 2) < e.SubItem.Bounds.Width)
                        using (SolidBrush brush = new SolidBrush(org_back_color))
                            e.Graphics.FillRectangle(brush, new Rectangle(e.SubItem.Bounds.X + x + 2, e.SubItem.Bounds.Y, e.SubItem.Bounds.Width - x - 2, e.SubItem.Bounds.Height - 2));
            }
        }

        private FontStyle CreateFont(bool bold, bool italic, bool underline)
        {
            FontStyle f = bold ? FontStyle.Bold : FontStyle.Regular;

            if (italic)
                f |= FontStyle.Italic;

            if (underline)
                f |= FontStyle.Underline;

            return f;
        }

        private Color GetColorFromCode(int code)
        {
            switch (code)
            {
                case 1: return Color.Black;
                case 2: return Color.Navy;
                case 3: return Color.Green;
                case 4: return Color.Red;
                case 5: return Color.Maroon;
                case 6: return Color.Purple;
                case 7: return Color.Orange;
                case 8: return Color.Yellow;
                case 9: return Color.Lime;
                case 10: return Color.Teal;
                case 11: return Color.Aqua;
                case 12: return Color.Blue;
                case 13: return Color.Fuchsia;
                case 14: return Color.Gray;
                case 15: return Color.Silver;
            }

            return Color.White;
        }
    }

    enum ChannelLanguage : byte
    {
        English = 10,
        Arabic = 11,
        Chinese = 12,
        Czech = 14,
        Danish = 15,
        Dutch = 16,
        Japanese = 17,
        Kirghiz = 19,
        Polish = 20,
        Portuguese = 21,
        Slovak = 22,
        Spanish = 23,
        Swedish = 25,
        Turkish = 26,
        Finnish = 27,
        French = 28,
        German = 29,
        Italian = 30,
        Russian = 31
    }

    enum ChannelListSortingItem
    {
        Name = 0,
        Server = 1,
        Users = 2,
        Language = 3,
        Topic = 4,
        None = 5
    }
}
