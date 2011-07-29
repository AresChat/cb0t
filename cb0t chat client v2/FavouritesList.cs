using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Windows.Forms;
using System.Xml;
using System.Net;

namespace cb0t_chat_client_v2
{
    class FavouritesList : ListView
    {
        private ContextMenuStrip menu = new ContextMenuStrip();

        public FavouritesList()
        {
            this.Columns.Add("Name");
            this.Columns[0].Width = 170;
            this.Columns.Add("Topic");
            this.Columns[1].Width = (this.Width - 198);

            this.OwnerDraw = true;
            this.DoubleBuffered = true;
            this.View = View.Details;
            this.FullRowSelect = true;
            this.MultiSelect = false;
            this.CurrentWidth = this.Width;
            this.CurrentHover = -1;
            this.CurrentHoverClear = -1;

            this.ContextMenuStrip = this.menu;
            this.menu.Items.Add("Export hashlink");
            this.menu.Items[0].Click += new EventHandler(this.OnExportHashlink);
            this.menu.Items.Add("Remove from favourites");
            this.menu.Items[1].Click += new EventHandler(this.OnRemoveFromFavourites);
            this.menu.Items.Add(new ToolStripSeparator());
            this.menu.Items.Add("Auto start");
            this.menu.Items[3].Click += new EventHandler(this.OnAutoStartStatusChanged);
            this.menu.Items.Add(new ToolStripSeparator());
            this.menu.Items.Add(new ToolStripLabel("Auto login password:"));
            this.menu.Items.Add(new ToolStripTextBox());
            this.menu.Opening += new System.ComponentModel.CancelEventHandler(this.OnMenuOpening);
            this.menu.Closing += new ToolStripDropDownClosingEventHandler(this.OnMenuClosing);
            this.menu.Opacity = 0.9;
            this.menu.ForeColor = Color.Black;
            this.menu.Renderer = new ContextMenuRenderer();
        }

        public event ChannelList.ChannelClickedDelegate OnChannelClicked;

        private List<ChannelObject> full_list = new List<ChannelObject>();

        public void AddRoom(ChannelObject cObj, bool save)
        {
            if (this.full_list.Contains(cObj))
                return;

            if (String.IsNullOrEmpty(cObj.topic))
                cObj.topic = cObj.name;

            this.full_list.Add(cObj);
            this.Items.Add(new ListViewItem(new String[] { cObj.name, cObj.topic }, 0));

            if (save)
                this.UpdateRecords();
        }

        public void UpdateTopic(ChannelObject cObj)
        {
            for (int i = 0; i < this.full_list.Count; i++)
            {
                if (this.full_list[i].Equals(cObj))
                {
                    this.full_list[i].topic = cObj.topic;
                    this.UpdateTopic(i);
                }
            }
        }

        private delegate void UpdateTopicDelegate(int index);

        private void UpdateTopic(int index)
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke(new UpdateTopicDelegate(this.UpdateTopic), index);
            }
            else
            {
                this.Items[index].SubItems[1].Text = this.full_list[index].topic;
                this.UpdateRecords();
            }
        }

        public void Populate()
        {
            this.Items.Clear();
            this.full_list.Clear();

            try
            {
                FileStream f = new FileStream(Settings.folder_path + "favourites.xml", FileMode.Open);
                XmlReader xml = XmlReader.Create(new StreamReader(f));

                xml.MoveToContent();
                xml.ReadSubtree().ReadToFollowing("favourites");

                while (xml.ReadToFollowing("item"))
                {
                    ChannelObject _obj = new ChannelObject();

                    xml.ReadSubtree().ReadToFollowing("name");
                    _obj.name = Encoding.UTF8.GetString(Convert.FromBase64String(xml.ReadElementContentAsString()));

                    xml.ReadToFollowing("topic");
                    _obj.topic = Encoding.UTF8.GetString(Convert.FromBase64String(xml.ReadElementContentAsString()));

                    xml.ReadToFollowing("ip");
                    _obj.ip = new IPAddress(Convert.FromBase64String(xml.ReadElementContentAsString()));

                    xml.ReadToFollowing("port");

                    if (!ushort.TryParse(xml.ReadElementContentAsString(), out _obj.port))
                        continue;

                    xml.ReadToFollowing("autostart");

                    if (bool.TryParse(xml.ReadElementContentAsString(), out _obj.auto_start))
                        this.AddRoom(_obj, false);

                    xml.ReadToFollowing("password");
                    _obj.password = Encoding.UTF8.GetString(Convert.FromBase64String(xml.ReadElementContentAsString()));
                }

                xml.Close();
                f.Flush();
                f.Close();
            }
            catch { }

            List<String> names = new List<String>();

            foreach (ChannelObject _obj in this.full_list)
                if (!names.Contains(_obj.name))
                    names.Add(_obj.name);

            Settings.favourites = names.ToArray();

            foreach (ChannelObject _obj in this.full_list)
                if (_obj.auto_start)
                    this.OnChannelClicked(_obj);
        }

        private void UpdateRecords()
        {
            try
            {
                XmlWriterSettings appearance = new XmlWriterSettings();
                appearance.Indent = true;
                XmlWriter xml = XmlWriter.Create(Settings.folder_path + "favourites.xml", appearance);

                xml.WriteStartDocument();
                xml.WriteStartElement("favourites");

                foreach (ChannelObject _obj in this.full_list)
                {
                    xml.WriteStartElement("item");
                    xml.WriteElementString("name", Convert.ToBase64String(Encoding.UTF8.GetBytes(_obj.name)));
                    xml.WriteElementString("topic", Convert.ToBase64String(Encoding.UTF8.GetBytes(_obj.topic)));
                    xml.WriteElementString("ip", Convert.ToBase64String(_obj.ip.GetAddressBytes()));
                    xml.WriteElementString("port", _obj.port.ToString());
                    xml.WriteElementString("autostart", _obj.auto_start.ToString());
                    xml.WriteElementString("password", Convert.ToBase64String(Encoding.UTF8.GetBytes(_obj.password)));
                    xml.WriteEndElement();
                }

                xml.WriteEndElement();
                xml.WriteEndDocument();
                xml.Flush();
                xml.Close();
            }
            catch { }

            List<String> names = new List<String>();

            foreach (ChannelObject _obj in this.full_list)
                if (!names.Contains(_obj.name))
                    names.Add(_obj.name);

            Settings.favourites = names.ToArray();
        }

        private void OnRemoveFromFavourites(object sender, EventArgs e)
        {
            if (this.SelectedItems.Count > 0)
            {
                int i = this.SelectedItems[0].Index;
                this.full_list.RemoveAt(i);
                this.Items.RemoveAt(i);
                this.UpdateRecords();
            }
        }

        private void OnExportHashlink(object sender, EventArgs e)
        {
            if (this.SelectedItems.Count > 0)
            {
                ChannelObject _obj = this.full_list[this.SelectedItems[0].Index];
                Helpers.ExportHashlink(_obj);
            }
        }

        private void OnAutoStartStatusChanged(object sender, EventArgs e)
        {
            if (this.SelectedItems.Count > 0)
            {
                ChannelObject _obj = this.full_list[this.SelectedItems[0].Index];
                _obj.auto_start = !_obj.auto_start;
                this.UpdateRecords();
            }
        }

        private void OnMenuOpening(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (this.SelectedItems.Count > 0)
            {
                ChannelObject _obj = this.full_list[this.SelectedItems[0].Index];
                ((ToolStripMenuItem)this.menu.Items[3]).Checked = _obj.auto_start;
                ((ToolStripTextBox)this.menu.Items[6]).Text = _obj.password;
            }
            else
            {
                e.Cancel = true;
            }
        }

        private void OnMenuClosing(object sender, ToolStripDropDownClosingEventArgs e)
        {
            if (this.SelectedItems.Count > 0)
            {
                int i = this.SelectedItems[0].Index;

                if (((ToolStripTextBox)this.menu.Items[6]).Text != this.full_list[i].password)
                {
                    this.full_list[i].password = ((ToolStripTextBox)this.menu.Items[6]).Text;
                    this.UpdateRecords();
                }
            }
        }

        protected override void OnMouseDoubleClick(MouseEventArgs e)
        {
            if (this.SelectedItems.Count > 0)
            {
                ChannelObject _obj = this.full_list[this.SelectedItems[0].Index];
                this.OnChannelClicked(_obj);
            }
            else
            {
                base.OnMouseDoubleClick(e);
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
                    e.NewWidth = (this.Width - 198);
                    break;
            }
        }

        protected override void OnResize(EventArgs e)
        {
            if (this.CurrentWidth != this.Width)
            {
                this.CurrentWidth = this.Width;
                this.Columns[1].Width = (this.Width - 198);
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

        protected override void OnDrawColumnHeader(DrawListViewColumnHeaderEventArgs e)
        {
            e.DrawDefault = true;
        }

        protected override void OnSelectedIndexChanged(EventArgs e)
        {
            base.OnSelectedIndexChanged(e);

            if (this.SelectedIndices.Count > 0)
                this.Invalidate(this.Items[this.SelectedIndices[0]].Bounds);
        }

        private int CurrentWidth { get; set; }
        private int CurrentHover { get; set; }
        private int CurrentHoverClear { get; set; }

        private SolidBrush hover_brush = new SolidBrush(Color.LightGray);
        private SolidBrush clear_brush = new SolidBrush(SystemColors.Window);
        private SolidBrush selected_brush = new SolidBrush(Color.CornflowerBlue);

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
            if (e.ColumnIndex == 0)
            {
                e.Graphics.DrawImage(AresImages.Chat, new RectangleF(e.Bounds.X, e.Bounds.Y + 1, 14, 14));
                e.DrawText();
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
}
