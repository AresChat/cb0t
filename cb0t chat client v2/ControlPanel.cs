using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Text;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace cb0t_chat_client_v2
{
    public partial class ControlPanel : UserControl
    {
        public ControlPanel()
        {
            InitializeComponent();
        }

        private bool setting_up = true;
        public bool send_font_packet = false;

        internal delegate void DownloadHashlinkDelegate(ChannelObject cObj);
        internal event DownloadHashlinkDelegate OnHashlinkDownloaded;

        internal delegate void DCPortChangedDelegate();
        internal event DCPortChangedDelegate OnDCPortChanged;

        internal delegate void AvatarProtocolPacketDelegate(byte[] packet);
        internal event AvatarProtocolPacketDelegate OnAvatarProtocolPacket;

        internal delegate void EnableCustomNamesDelegate(bool enabled);
        internal event EnableCustomNamesDelegate OnEnableCustomNames;

        internal delegate void SendCustomEmotePacketHandler(byte[] packet);
        internal event SendCustomEmotePacketHandler SendCustomEmotePacket;

        private void button1_Click(object sender, EventArgs e) // hashlink
        {
            String str = textBox1.Text;
            ChannelObject _obj = Hashlink.DecodeHashlink(str);

            if (_obj != null)
            {
                _obj.topic = _obj.name;
                this.OnHashlinkDownloaded(_obj);
            }
        }

        private void PopulateFontBox()
        {
            InstalledFontCollection fonts = new InstalledFontCollection();

            for (int i = 0; i < fonts.Families.Length; i++)
            {
                this.comboBox10.Items.Add(fonts.Families[i].Name);
                this.comboBox11.Items.Add(fonts.Families[i].Name);
                Settings.font_list.Add(fonts.Families[i].Name);
            }

            this.comboBox10.Text = Settings.font_name;
            this.comboBox11.Text = Settings.p_font_name;
            this.numericUpDown1.Value = Settings.font_size;
            this.numericUpDown2.Value = Settings.p_font_size;

            for (int i = 0; i < 16; i++)
            {
                this.comboBox12.Items.Add(i);
                this.comboBox13.Items.Add(i);
            }

            this.comboBox12.SelectedIndex = Settings.p_name_col;
            this.comboBox13.SelectedIndex = Settings.p_text_col;
        }

        public void PopulateSettings()
        {
            this.setting_up = true;
            this.textBox2.Text = Settings.my_username;
            this.checkBox1.Checked = Settings.black_background;
            this.checkBox2.Checked = Settings.enable_emoticons;
            this.checkBox3.Checked = Settings.enable_timestamps;
            this.checkBox4.Checked = Settings.auto_add_rooms_to_favourites;
            this.checkBox5.Checked = Settings.receive_pm;
            this.checkBox7.Checked = Settings.receive_nudges;
            this.checkBox8.Checked = Settings.enable_pm_reply;
            this.checkBox9.Checked = Settings.enable_dc_reply;
            this.checkBox10.Checked = Settings.dc_enabled;
            this.textBox3.Text = Settings.pm_reply;
            this.textBox4.Text = Settings.dc_reply;
            this.textBox5.Text = Settings.dc_port.ToString();
            this.checkBox11.Checked = Settings.winamp_enabled;
            this.checkBox12.Checked = Settings.send_to_tray;
            this.checkBox13.Checked = Settings.dc_auto_accept;
            this.textBox7.Text = Settings.share_file_msg;
            this.checkBox14.Checked = Settings.share_file_on;
            this.checkBox17.Checked = Settings.scribble_enabled;
            this.checkBox18.Checked = Settings.auto_load;
            this.checkBox19.Checked = Settings.show_userlist_song;
            this.checkBox6.Checked = Settings.enable_custom_emotes;
            this.checkBox11.Checked = Settings.enable_clips;
            this.checkBox19.Checked = Settings.receive_private_clips;
            this.checkBox33.Checked = Settings.record_quality;
            this.comboBox14.SelectedIndex = 0;

            foreach (String nstr in Settings.notify_msg)
                if (nstr.Length > 0)
                    this.listBox1.Items.Add(nstr);

            foreach (String nstr in Settings.pm_notify_msg)
                if (nstr.Length > 0)
                    this.listBox2.Items.Add(nstr);

            this.checkBox15.Checked = (Settings.notify_msg.Length > 0 && Settings.notify_on);
            this.checkBox16.Checked = Settings.notify_sound;
            this.comboBox4.SelectedIndex = Settings.age;
            this.comboBox1.SelectedIndex = Settings.sex;
            this.comboBox3.SelectedIndex = Settings.country;
            this.textBox9.Text = Settings.region;
            this.textBox10.Text = Settings.personal_message;
            Avatar.Load();
            this.pictureBox2.Image = Avatar.avatar_big;

            this.comboBox6.SelectedIndex = 0;
            this.comboBox7.SelectedIndex = 0;
            this.comboBox9.SelectedIndex = 0;
            this.checkBox20.Checked = Settings.allow_events;
            this.checkBox21.Checked = Settings.allow_events_flood_check;
            this.checkBox22.Checked = Settings.send_custom_data;
            this.checkBox23.Checked = Settings.enable_custom_names;
            this.checkBox24.Checked = Settings.ignore_cls;
            this.checkBox26.Checked = Settings.receive_custom_fonts;
            this.checkBox25.Checked = Settings.enable_my_custom_font;
            this.checkBox27.Checked = Settings.enable_whois_writing;
            this.checkBox28.Checked = Settings.enable_clips;
            this.checkBox29.Checked = Settings.receive_private_clips;
            this.checkBox30.Checked = Settings.record_quality;
            this.checkBox31.Checked = Settings.pm_sound;
            this.checkBox32.Checked = Settings.pm_notify;
            this.PopulateFontBox();

            this.cEmoteList1.LoadEmoticons();

            this.ResetEventTab();

            ChatEventObject[] chatevents = ChatEvents.GetItems();

            if (chatevents.Length > 0)
            {
                foreach (ChatEventObject obj in chatevents)
                {
                    if (obj._event == "OnConnect")
                        this.listView1.Items.Add(new ListViewItem(new String[] { obj._event, obj._room, obj._result }));
                    else
                        this.listView1.Items.Add(new ListViewItem(new String[] { obj._event, obj._room, "if (" + obj._variable + "." + obj._condition + "(" + obj._argument + ")) " + obj._result }));
                }
            }

            MenuOptionObject[] menuoptions = MenuOptions.GetItems();

            if (menuoptions.Length > 0)
                foreach (MenuOptionObject obj in menuoptions)
                    this.listView2.Items.Add(new ListViewItem(new String[] { obj.name, obj.text }));

            Ignores.Load();

            foreach (IgnoresItem i in Ignores.CurrentList)
                this.listView3.Items.Add(new ListViewItem(new String[] { i.type, i.condition, i.value }));

            this.comboBox2.SelectedIndex = 0;
            this.comboBox5.SelectedIndex = 0;

            this.setting_up = false;

            this.emoticonDesigner1.Location = new Point(this.emoticonDesigner1.Location.X + 16, this.emoticonDesigner1.Location.Y);
        }

        private void textBox2_Leave(object sender, EventArgs e) // name change
        {
            if (Encoding.UTF8.GetByteCount(this.textBox2.Text) < 2)
                this.textBox2.Text = "cb0t user";

            while (Encoding.UTF8.GetByteCount(this.textBox2.Text) > 20)
                this.textBox2.Text = this.textBox2.Text.Substring(0, this.textBox2.Text.Length - 1);

            Settings.my_username = this.textBox2.Text;

            if (!this.setting_up)
                Settings.UpdateRecords();
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e) // black background
        {
            Settings.black_background = checkBox1.Checked;

            if (!this.setting_up)
                Settings.UpdateRecords();
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e) // enable emoticons
        {
            Settings.enable_emoticons = checkBox2.Checked;

            if (!this.setting_up)
                Settings.UpdateRecords();
        }

        private void checkBox3_CheckedChanged(object sender, EventArgs e) // enable timestamps
        {
            Settings.enable_timestamps = checkBox3.Checked;

            if (!this.setting_up)
                Settings.UpdateRecords();
        }

        private void checkBox4_CheckedChanged(object sender, EventArgs e) // auto add rooms to favourites
        {
            Settings.auto_add_rooms_to_favourites = checkBox4.Checked;

            if (!this.setting_up)
                Settings.UpdateRecords();
        }

        private void checkBox5_CheckedChanged(object sender, EventArgs e) // receive pms
        {
            Settings.receive_pm = checkBox5.Checked;

            if (!this.setting_up)
                Settings.UpdateRecords();
        }

        private void checkBox7_CheckedChanged(object sender, EventArgs e) // receive nudges
        {
            Settings.receive_nudges = checkBox7.Checked;

            if (!this.setting_up)
                Settings.UpdateRecords();
        }

        private void checkBox8_CheckedChanged(object sender, EventArgs e) // enable pm auto response
        {
            Settings.enable_pm_reply = checkBox8.Checked;

            if (!this.setting_up)
                Settings.UpdateRecords();
        }

        private void checkBox9_CheckedChanged(object sender, EventArgs e) // enable dc auto response
        {
            Settings.enable_dc_reply = checkBox9.Checked;

            if (!this.setting_up)
                Settings.UpdateRecords();
        }

        private void textBox3_Leave(object sender, EventArgs e) // pm response changed
        {
            String[] lines = this.textBox3.Text.Split(new String[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);

            if (lines.Length > 4)
                lines = new List<String>(lines).GetRange(0, 4).ToArray();

            Settings.pm_reply = String.Join("\r\n", lines);
            this.textBox3.Text = Settings.pm_reply;

            if (!this.setting_up)
                Settings.UpdateRecords();
        }

        private void textBox4_Leave(object sender, EventArgs e) // dc response changed
        {
            String[] lines = this.textBox4.Text.Split(new String[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);

            if (lines.Length > 4)
                lines = new List<String>(lines).GetRange(0, 4).ToArray();

            Settings.dc_reply = String.Join("\r\n", lines);
            this.textBox4.Text = Settings.dc_reply;

            if (!this.setting_up)
                Settings.UpdateRecords();
        }

        private void checkBox10_CheckedChanged(object sender, EventArgs e) // enable dc
        {
            Settings.dc_enabled = this.checkBox10.Checked;

            if (!this.setting_up)
                Settings.UpdateRecords();
        }

        private void textBox5_Leave(object sender, EventArgs e) // dc port
        {
            if (ushort.TryParse(this.textBox5.Text, out Settings.dc_port))
            {
                if (!this.setting_up)
                    Settings.UpdateRecords();

                this.OnDCPortChanged();
            }
            else
            {
                this.textBox5.Text = Settings.dc_port.ToString();
            }
        }

        private void checkBox12_CheckedChanged(object sender, EventArgs e) // send to tray
        {
            Settings.send_to_tray = this.checkBox12.Checked;

            if (!this.setting_up)
                Settings.UpdateRecords();
        }

        private void checkBox13_CheckedChanged(object sender, EventArgs e) // dc auto accept files
        {
            Settings.dc_auto_accept = this.checkBox13.Checked;

            if (!this.setting_up)
                Settings.UpdateRecords();
        }

        private void checkBox14_CheckedChanged(object sender, EventArgs e) // share file enabled
        {
            Settings.share_file_on = this.checkBox14.Checked;

            if (!this.setting_up)
                Settings.UpdateRecords();
        }

        private void textBox7_Leave(object sender, EventArgs e) // share file msg
        {
            String[] lines = this.textBox7.Text.Split(new String[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);

            if (lines.Length > 4)
                lines = new List<String>(lines).GetRange(0, 4).ToArray();

            for (int i = 0; i < lines.Length; i++)
                while (Encoding.UTF8.GetByteCount(lines[i]) > 50)
                    lines[i] = lines[i].Substring(0, lines[i].Length - 1);

            Settings.share_file_msg = String.Join("\r\n", lines);
            this.textBox7.Text = Settings.share_file_msg;

            if (!this.setting_up)
                Settings.UpdateRecords();
        }

        private void checkBox15_CheckedChanged(object sender, EventArgs e) // notification enabled
        {
            Settings.notify_on = this.checkBox15.Checked;

            if (!this.setting_up)
                Settings.UpdateRecords();
        }

        private void checkBox16_CheckedChanged(object sender, EventArgs e) // notification sound
        {
            Settings.notify_sound = this.checkBox16.Checked;

            if (!this.setting_up)
                Settings.UpdateRecords();
        }

        private void button3_Click(object sender, EventArgs e) // add new trigger word
        {
            String text = this.textBox8.Text.Trim();
            this.textBox8.Clear();

            if (this.listBox1.Items.Contains(text))
                return;

            if (text.Length == 0)
                return;

            this.listBox1.Items.Add(text);

            List<String> temp = new List<String>();

            foreach (String item in this.listBox1.Items)
                temp.Add(item);

            if (this.checkBox15.Checked)
                Settings.notify_on = true;

            Settings.notify_msg = temp.ToArray();

            if (!this.setting_up)
                Settings.UpdateRecords();
        }

        private void removeToolStripMenuItem_Click(object sender, EventArgs e) // delete trigger word
        {
            if (this.listBox1.SelectedIndex > -1)
            {
                String text = (String)this.listBox1.SelectedItem;
                this.listBox1.Items.Remove(this.listBox1.SelectedItem);

                List<String> temp = new List<String>();

                foreach (String item in this.listBox1.Items)
                    temp.Add(item);

                if (temp.Count == 0)
                    Settings.notify_on = false;

                Settings.notify_msg = temp.ToArray();

                if (!this.setting_up)
                    Settings.UpdateRecords();
            }
        }

        private void button4_Click(object sender, EventArgs e) // clear avatar
        {
            Avatar.Clear();
            this.pictureBox2.Image = Avatar.avatar_big;
        }

        private void button2_Click(object sender, EventArgs e) // update avatar
        {
            OpenFileDialog f = new OpenFileDialog();
            f.Multiselect = false;
            f.Filter = "Supported file types (*.bmp;*.jpg;*.png)|*.bmp;*.jpg;*.png|Bitmap files (*.bmp)|*.bmp|Jpeg files (*.jpg)|*.jpg|PNG files (*.png)|*.png";

            if (f.ShowDialog() == DialogResult.OK)
            {
                Avatar.Update(f.FileName);
                this.pictureBox2.Image = Avatar.avatar_big;

                if (Avatar.avatar_small != null)
                    this.OnAvatarProtocolPacket(Packets.AvatarPacket());
            }
        }

        private void comboBox4_SelectionChangeCommitted(object sender, EventArgs e) // age changed
        {
            Settings.age = (byte)this.comboBox4.SelectedIndex;

            if (!this.setting_up)
                Settings.UpdateRecords();
        }

        private void comboBox1_SelectionChangeCommitted(object sender, EventArgs e)
        {
            Settings.sex = (byte)this.comboBox1.SelectedIndex;

            if (!this.setting_up)
                Settings.UpdateRecords();
        }

        private void comboBox3_SelectionChangeCommitted(object sender, EventArgs e)
        {
            Settings.country = (byte)this.comboBox3.SelectedIndex;

            if (!this.setting_up)
                Settings.UpdateRecords();
        }

        private void textBox9_Leave(object sender, EventArgs e)
        {
            String text = this.textBox9.Text.Trim();

            if (text.Length > 20)
            {
                text = text.Substring(0, 20);
                this.textBox9.Text = text;
            }

            if (Settings.region != text)
            {
                Settings.region = text;

                if (!this.setting_up)
                    Settings.UpdateRecords();
            }
        }

        private void textBox10_Leave(object sender, EventArgs e) // personal message
        {
            String text = this.textBox10.Text.Trim();

            if (text.Length > 150)
            {
                text = text.Substring(0, 100);
                this.textBox10.Text = text;
            }

            if (Settings.personal_message != text)
            {
                Settings.personal_message = text;

                if (!this.setting_up)
                    Settings.UpdateRecords();

                this.OnAvatarProtocolPacket(Packets.PersonalMessagePacket());
            }
        }

        private void checkBox17_CheckedChanged(object sender, EventArgs e)
        {
            Settings.scribble_enabled = this.checkBox17.Checked;

            if (!this.setting_up)
                Settings.UpdateRecords();
        }

        private void checkBox18_CheckedChanged(object sender, EventArgs e) // auto load
        {
            Settings.UpdateAutoLoad(this.checkBox18.Checked);
        }

        private void checkBox19_CheckedChanged(object sender, EventArgs e)
        {
            Settings.show_userlist_song = this.checkBox19.Checked;

            if (!this.setting_up)
                Settings.UpdateRecords();
        }

        private void listView1_Resize(object sender, EventArgs e)
        {
            if (this.listView1.Columns.Count > 2)
                this.listView1.Columns[2].Width = this.listView1.Size.Width - 184;
        }

        private void button5_Click(object sender, EventArgs e) // add a chat event
        {
            if (this.textBox12.Text.Length == 0)
                return;

            ChatEventObject obj = new ChatEventObject();
            obj._event = (String)this.comboBox6.SelectedItem;
            obj._argument = this.textBox11.Text;
            obj._condition = (String)this.comboBox9.SelectedItem;
            obj._result = this.textBox12.Text;
            obj._room = (String)this.comboBox8.SelectedItem;
            obj._variable = (String)this.comboBox7.SelectedItem;

            if (obj._event == "OnJoin" && obj._result.Contains("whisper"))
            {
                this.ResetEventTab();
                return;
            }

            ChatEvents.Add(obj);

            if (obj._event == "OnConnect")
                this.listView1.Items.Add(new ListViewItem(new String[] { obj._event, obj._room, obj._result }));
            else
                this.listView1.Items.Add(new ListViewItem(new String[] { obj._event, obj._room, "if (" + obj._variable + "." + obj._condition + "(" + obj._argument + ")) " + obj._result }));

            this.ResetEventTab();
        }

        public void ResetEventTab()
        {
            this.comboBox6.SelectedIndex = 0;
            this.comboBox7.SelectedIndex = 0;
            this.comboBox9.SelectedIndex = 0;
            this.textBox11.Clear();
            this.textBox12.Clear();
            this.comboBox8.Items.Clear();
            this.comboBox8.Items.Add("Any");

            foreach (String str in Settings.favourites)
                comboBox8.Items.Add(str);

            this.comboBox8.SelectedIndex = 0;
        }

        public void ResetMenuOptionsTab()
        {
            this.textBox13.Clear();
            this.textBox14.Clear();
        }

        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.tabControl1.SelectedIndex == 5)
                this.ResetEventTab();

            if (this.tabControl1.SelectedIndex == 6)
                this.ResetMenuOptionsTab();
        }

        private void comboBox6_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (comboBox6.SelectedIndex)
            {
                case 0: // onjoin
                case 1: // onpart
                    comboBox7.Items.Clear();
                    comboBox7.Items.AddRange(new String[] { "Name", "External IP", "Local IP", "Port" });
                    comboBox7.SelectedIndex = 0;
                    label27.Text = "Variables: +name +eip +lip +port";
                    break;

                case 2: // onconnect
                    comboBox7.Enabled = false;
                    comboBox9.Enabled = false;
                    textBox11.Enabled = false;
                    label27.Text = String.Empty;
                    break;

                case 3: // ontext
                case 4: // onemote
                case 5: // onpm
                    comboBox7.Items.Clear();
                    comboBox7.Items.AddRange(new String[] { "Name", "External IP", "Local IP", "Port", "Text" });
                    comboBox7.SelectedIndex = 0;
                    label27.Text = "Variables: +name +eip +lip +port +text";
                    break;
            }

            if (comboBox6.SelectedIndex != 2)
            {
                comboBox7.Enabled = true;
                comboBox9.Enabled = true;
                textBox11.Enabled = true;
            }
        }

        private void removeToolStripMenuItem1_Click(object sender, EventArgs e) // remove chat event
        {
            if (this.listView1.SelectedItems.Count > 0)
            {
                int i = this.listView1.SelectedItems[0].Index;
                ChatEvents.RemoveAt(i);
                this.listView1.Items.RemoveAt(i);
            }
        }

        private void checkBox20_CheckedChanged(object sender, EventArgs e) // allow events
        {
            Settings.allow_events = this.checkBox20.Checked;

            if (!this.setting_up)
                Settings.UpdateRecords();
        }

        private void listView2_Resize(object sender, EventArgs e)
        {
            if (this.listView2.Columns.Count == 2)
                this.listView2.Columns[1].Width = this.listView2.Size.Width - 114;
        }

        private void button6_Click(object sender, EventArgs e) // add custom menu option
        {
            if (this.textBox13.Text.Length == 0 || this.textBox14.Text.Length == 0)
                return;

            MenuOptionObject obj = new MenuOptionObject();
            obj.name = this.textBox13.Text;
            obj.text = this.textBox14.Text;
            MenuOptions.Add(obj);
            this.listView2.Items.Add(new ListViewItem(new String[] { obj.name, obj.text }));
            this.ResetMenuOptionsTab();
        }

        private void removeToolStripMenuItem2_Click(object sender, EventArgs e) // remove custom menu option
        {
            if (this.listView2.SelectedItems.Count > 0)
            {
                int i = this.listView2.SelectedItems[0].Index;
                MenuOptions.RemoveAt(i);
                this.listView2.Items.RemoveAt(i);
            }
        }

        private void checkBox21_CheckedChanged(object sender, EventArgs e) // flood protection
        {
            Settings.allow_events_flood_check = this.checkBox21.Checked;

            if (!this.setting_up)
                Settings.UpdateRecords();
        }

        private void checkBox22_CheckedChanged(object sender, EventArgs e)
        {
            Settings.send_custom_data = this.checkBox22.Checked;

            if (!this.setting_up)
                Settings.UpdateRecords();
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try
            {
                System.Diagnostics.Process.Start("http://oobe.arca-eclipse.com/");
            }
            catch { }
        }

        private void checkBox23_CheckedChanged(object sender, EventArgs e) // allow custom names
        {
            Settings.enable_custom_names = this.checkBox23.Checked;

            if (!this.setting_up)
            {
                Settings.UpdateRecords();
                this.OnEnableCustomNames(this.checkBox23.Checked);
            }
        }

        private void comboBox10_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!this.setting_up)
                Settings.font_name = this.comboBox10.Text;
        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            if (!this.setting_up)
                Settings.font_size = (int)this.numericUpDown1.Value;
        }

        private void comboBox11_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!this.setting_up)
            {
                Settings.p_font_name = this.comboBox11.Text;

                if (Settings.enable_my_custom_font)
                    this.send_font_packet = true;
            }
        }

        private void numericUpDown2_ValueChanged(object sender, EventArgs e)
        {
            if (!this.setting_up)
            {
                Settings.p_font_size = (int)this.numericUpDown2.Value;

                if (Settings.enable_my_custom_font)
                    this.send_font_packet = true;
            }
        }

        private void checkBox24_CheckedChanged(object sender, EventArgs e) // ignore cls
        {
            if (!this.setting_up)
                Settings.ignore_cls = this.checkBox24.Checked;
        }

        private void checkBox26_CheckedChanged(object sender, EventArgs e) // receive custom fonts
        {
            if (!this.setting_up)
                Settings.receive_custom_fonts = this.checkBox26.Checked;
        }

        private void checkBox25_CheckedChanged(object sender, EventArgs e) // enable my custom font
        {
            if (!this.setting_up)
            {
                Settings.enable_my_custom_font = this.checkBox25.Checked;
                this.send_font_packet = true;
            }
        }

        private void comboBox12_DrawItem(object sender, DrawItemEventArgs e)
        {
            if (e.Index > -1)
            {
                Color c = this.GetColorFromCode(e.Index);
                e.Graphics.FillRectangle(new SolidBrush(c), e.Bounds);
            }
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
                default: return Color.White;
            }
        }

        private void comboBox12_SelectedIndexChanged(object sender, EventArgs e) // name
        {
            if (!this.setting_up)
            {
                Settings.p_name_col = this.comboBox12.SelectedIndex;
                this.send_font_packet = true;
            }
        }

        private void comboBox13_SelectedIndexChanged(object sender, EventArgs e) // text
        {
            if (!this.setting_up)
            {
                Settings.p_text_col = this.comboBox13.SelectedIndex;
                this.send_font_packet = true;
            }
        }

        private void checkBox27_CheckedChanged(object sender, EventArgs e)
        {
            if (!this.setting_up)
                Settings.enable_whois_writing = this.checkBox27.Checked;
        }

        public bool send_vc_support_packet = false;

        private void checkBox31_CheckedChanged(object sender, EventArgs e) // play pm notify sound
        {
            if (!this.setting_up)
                Settings.pm_sound = this.checkBox31.Checked;
        }

        private void checkBox32_CheckedChanged(object sender, EventArgs e) // enable pm notify
        {
            if (!this.setting_up)
                Settings.pm_notify = this.checkBox32.Checked;
        }

        private void button7_Click(object sender, EventArgs e)
        {
            String text = this.textBox15.Text;
            this.textBox15.Clear();

            if (this.listBox2.Items.Contains(text))
                return;

            if (text.Length == 0)
                return;

            this.listBox2.Items.Add(text);

            List<String> temp = new List<String>();

            foreach (String item in this.listBox2.Items)
                temp.Add(item);

            Settings.pm_notify_msg = temp.ToArray();

            if (!this.setting_up)
                Settings.UpdateRecords();
        }

        private void removeToolStripMenuItem3_Click(object sender, EventArgs e)
        {
            if (this.listBox2.SelectedIndex > -1)
            {
                String text = (String)this.listBox2.SelectedItem;
                this.listBox2.Items.Remove(this.listBox2.SelectedItem);

                List<String> temp = new List<String>();

                foreach (String item in this.listBox2.Items)
                    temp.Add(item);

                Settings.pm_notify_msg = temp.ToArray();

                if (!this.setting_up)
                    Settings.UpdateRecords();
            }
        }

        private ColorDialog cpick = new ColorDialog();

        private void comboBox14_SelectedIndexChanged(object sender, EventArgs e) // change emote size
        {
            this.emoticonDesigner1.Mode = this.comboBox14.SelectedIndex == 0 ? EmoteDesignMode.Size16x16 : EmoteDesignMode.Size32x32;
        }

        private void button10_Click(object sender, EventArgs e) // add designed emote
        {
            if (this.cEmoteList1.SelectedEmoticon == -1)
            {
                MessageBox.Show("Select a target slot", "cb0t custom emoticons", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            CEmoteItem item = (CEmoteItem)this.cEmoteList1.Items[this.cEmoteList1.SelectedEmoticon];

            if (item.Image != null)
            {
                DialogResult dr = MessageBox.Show("Are you sure you want to overwrite the existing emoticon in this slot?",
                    "cb0t custom emoticons", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (dr == DialogResult.No)
                    return;
                else
                {
                    this.SendCustomEmotePacket(Packets.CustomEmoteDelete(item.Shortcut));
                    CustomEmotes.RemoveItem(item.Shortcut);
                    this.cEmoteList1.DeleteItem(this.cEmoteList1.SelectedEmoticon);
                }
            }

            String kshortcut = String.Empty;

            using (CEmoticonShortcutA dr = new CEmoticonShortcutA())
            {
                while (true)
                {
                    if (dr.ShowDialog() == DialogResult.OK)
                    {
                        kshortcut = dr.KeyboardShortcut;

                        if (kshortcut.Length == 0)
                        {
                            MessageBox.Show("Keyboard shortcut must be at least 1 letter long", "cb0t custom emoticons", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            continue;
                        }

                        for (int i = 0; i < 16; i++)
                        {
                            if (((CEmoteItem)this.cEmoteList1.Items[i]).Shortcut != null)
                            {
                                if (((CEmoteItem)this.cEmoteList1.Items[i]).Shortcut == kshortcut)
                                {
                                    MessageBox.Show("Keyboard shortcut already in use", "cb0t custom emoticons", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                    continue;
                                }
                            }
                        }
                    }
                    else kshortcut = null;

                    break;
                }
            }

            if (kshortcut == null)
            {
                CustomEmotes.Save();
                return;
            }

            item = new CEmoteItem();
            item.Shortcut = kshortcut;
            item.Image = this.emoticonDesigner1.GetEmoticon;
            item.Size = this.emoticonDesigner1.Mode == EmoteDesignMode.Size16x16 ? 16 : 32;

            this.SendCustomEmotePacket(Packets.CustomEmoteItem(item));
            CustomEmotes.AddItem(item);
            CustomEmotes.Save();
            this.cEmoteList1.AddItem(this.cEmoteList1.SelectedEmoticon, item);
        }

        private void button8_Click(object sender, EventArgs e) // delete designed emote
        {
            this.emoticonDesigner1.ClearCanvas(true);
        }

        private void button9_Click(object sender, EventArgs e) // change color color
        {
            if (this.cpick.CustomColors[0] != ColorTranslator.ToOle(Color.Magenta))
                this.cpick.CustomColors = new int[] { ColorTranslator.ToOle(Color.Magenta) };

            if (this.cpick.ShowDialog() == DialogResult.OK)
            {
                this.button9.BackColor = this.cpick.Color;
                this.emoticonDesigner1.ForeColor = this.cpick.Color;
            }
        }

        private void button12_Click(object sender, EventArgs e) // import emoticon from file
        {
            if (this.cEmoteList1.SelectedEmoticon == -1)
            {
                MessageBox.Show("Select a target slot", "cb0t custom emoticons", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            CEmoteItem item = (CEmoteItem)this.cEmoteList1.Items[this.cEmoteList1.SelectedEmoticon];

            if (item.Image != null)
            {
                DialogResult dr = MessageBox.Show("Are you sure you want to overwrite the existing emoticon in this slot?",
                    "cb0t custom emoticons", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (dr == DialogResult.No)
                    return;
                else
                {
                    this.SendCustomEmotePacket(Packets.CustomEmoteDelete(item.Shortcut));
                    CustomEmotes.RemoveItem(item.Shortcut);
                    this.cEmoteList1.DeleteItem(this.cEmoteList1.SelectedEmoticon);
                }
            }

            item = new CEmoteItem();
            CustomEmotes.ImportEmoticon(ref item.Image, out item.Size);

            if (item.Size > -1)
            {
                String kshortcut = String.Empty;

                using (CEmoticonShortcutA dr = new CEmoticonShortcutA())
                {
                    while (true)
                    {
                        if (dr.ShowDialog() == DialogResult.OK)
                        {
                            kshortcut = dr.KeyboardShortcut;

                            if (kshortcut.Length == 0)
                            {
                                MessageBox.Show("Keyboard shortcut must be at least 1 letter long", "cb0t custom emoticons", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                continue;
                            }

                            for (int i = 0; i < 16; i++)
                            {
                                if (((CEmoteItem)this.cEmoteList1.Items[i]).Shortcut != null)
                                {
                                    if (((CEmoteItem)this.cEmoteList1.Items[i]).Shortcut == kshortcut)
                                    {
                                        MessageBox.Show("Keyboard shortcut already in use", "cb0t custom emoticons", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                        continue;
                                    }
                                }
                            }
                        }
                        else kshortcut = null;

                        break;
                    }
                }

                if (kshortcut != null)
                {
                    item.Shortcut = kshortcut;
                    CustomEmotes.AddItem(item);
                    this.cEmoteList1.AddItem(this.cEmoteList1.SelectedEmoticon, item);
                    this.SendCustomEmotePacket(Packets.CustomEmoteItem(item));
                }
            }
            else MessageBox.Show("Could not import this file", "cb0t custom emoticons", MessageBoxButtons.OK, MessageBoxIcon.Error);

            CustomEmotes.Save();
        }

        private void button11_Click(object sender, EventArgs e) // unset emoticon slot
        {
            if (this.cEmoteList1.SelectedEmoticon == -1)
            {
                MessageBox.Show("Select a target slot", "cb0t custom emoticons", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            CEmoteItem item = (CEmoteItem)this.cEmoteList1.Items[this.cEmoteList1.SelectedEmoticon];

            if (item.Image != null)
            {
                DialogResult dr = MessageBox.Show("Are you sure you want to unset the existing emoticon in this slot?",
                    "cb0t custom emoticons", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (dr == DialogResult.No)
                    return;
                else
                {
                    this.SendCustomEmotePacket(Packets.CustomEmoteDelete(item.Shortcut));
                    CustomEmotes.RemoveItem(item.Shortcut);
                    CustomEmotes.Save();
                    this.cEmoteList1.DeleteItem(this.cEmoteList1.SelectedEmoticon);
                }
            }
        }

        private void checkBox6_CheckedChanged(object sender, EventArgs e) // enable custom emotes
        {
            if (!this.setting_up)
                Settings.enable_custom_emotes = this.checkBox6.Checked;
        }

        private void linkLabel2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try
            {
                System.Diagnostics.Process.Start("http://oobe.arca-eclipse.com/");
            }
            catch { }
        }

        private void checkBox11_CheckedChanged_1(object sender, EventArgs e) // public vc
        {
            if (!this.setting_up)
            {
                Settings.enable_clips = this.checkBox11.Checked;
                this.send_vc_support_packet = true;
            }
        }

        private void checkBox19_CheckedChanged_1(object sender, EventArgs e) // private vc
        {
            if (!this.setting_up)
            {
                Settings.receive_private_clips = this.checkBox19.Checked;
                this.send_vc_support_packet = true;
            }
        }

        private void checkBox33_CheckedChanged(object sender, EventArgs e) // hq vc
        {
            if (!this.setting_up)
                Settings.record_quality = this.checkBox30.Checked;
        }

        private void button13_Click(object sender, EventArgs e)
        {
            if (this.textBox16.Text.Length < 2)
                return;

            IgnoresItem i = new IgnoresItem();
            i.value = this.textBox16.Text;
            i.type = this.comboBox5.SelectedItem.ToString();
            i.condition = this.comboBox2.SelectedItem.ToString();
            this.textBox16.Clear();
            this.comboBox5.SelectedIndex = 0;
            this.comboBox2.SelectedIndex = 0;
            this.listView3.Items.Add(new ListViewItem(new String[] { i.type, i.condition, i.value }));
            Ignores.AddItem(i);
        }

        private void removeToolStripMenuItem4_Click(object sender, EventArgs e)
        {
            if (this.listView3.SelectedIndices.Count > 0)
            {
                int i = this.listView3.SelectedIndices[0];
                this.listView3.Items.RemoveAt(i);
                Ignores.RemoveItem(i);
            }
        }

    }
}
