using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing.Text;

namespace cb0t
{
    public partial class GlobalSettings : UserControl
    {
        public GlobalSettings()
        {
            InitializeComponent();
        }

        public void UpdateTemplate()
        {
            this.label1.Text = StringTemplate.Get(STType.Settings, 3);
            this.checkBox1.Text = StringTemplate.Get(STType.ChatSettings, 0);
            this.checkBox12.Text = StringTemplate.Get(STType.ChatSettings, 1);
            this.checkBox2.Text = StringTemplate.Get(STType.ChatSettings, 2);
            this.checkBox3.Text = StringTemplate.Get(STType.ChatSettings, 3);
            this.checkBox4.Text = StringTemplate.Get(STType.ChatSettings, 4);
            this.checkBox5.Text = StringTemplate.Get(STType.ChatSettings, 5);
            this.checkBox6.Text = StringTemplate.Get(STType.ChatSettings, 6);
            this.checkBox7.Text = StringTemplate.Get(STType.ChatSettings, 7);
            this.checkBox8.Text = StringTemplate.Get(STType.ChatSettings, 8);
            this.checkBox9.Text = StringTemplate.Get(STType.ChatSettings, 9);
            this.checkBox10.Text = StringTemplate.Get(STType.ChatSettings, 10);
            this.checkBox11.Text = StringTemplate.Get(STType.ChatSettings, 11);
            this.checkBox13.Text = StringTemplate.Get(STType.ChatSettings, 12);
            this.label4.Text = StringTemplate.Get(STType.ChatSettings, 13) + ":";
            this.groupBox1.Text = StringTemplate.Get(STType.ChatSettings, 14);
            this.label3.Text = StringTemplate.Get(STType.ChatSettings, 15) + ":";
            this.label2.Text = StringTemplate.Get(STType.ChatSettings, 16) + ":";
            this.checkBox14.Text = StringTemplate.Get(STType.ChatSettings, 17);
            this.checkBox15.Text = StringTemplate.Get(STType.ChatSettings, 18);
            this.checkBox16.Text = StringTemplate.Get(STType.ChatSettings, 19);
            this.label9.Text = StringTemplate.Get(STType.PersonalSettings, 0) + ":";
            this.label7.Text = StringTemplate.Get(STType.PersonalSettings, 13) + ":";
            this.label10.Text = StringTemplate.Get(STType.Messages, 17) + ":";
        }

        public event EventHandler SpellCheckUpdate;
        public event EventHandler BlockCustomNamesUpdate;
        private bool setting_up_font = false;

        public void Populate()
        {
            this.comboBox2.SelectedIndex = Settings.GetReg<int>("spell_checker", 0);
            this.checkBox1.Checked = Settings.GetReg<bool>("can_timestamp", false);
            this.checkBox2.Checked = Settings.GetReg<bool>("can_emoticon", true);
            this.checkBox3.Checked = Settings.GetReg<int>("crypto", 250) == 250;
            this.checkBox4.Checked = Settings.GetReg<bool>("can_write", true);
            this.checkBox5.Checked = Settings.GetReg<bool>("can_receive_pms", true);
            this.checkBox6.Checked = Settings.GetReg<bool>("receive_nudge", true);
            this.checkBox7.Checked = Settings.GetReg<bool>("receive_scribbles", true);
            this.checkBox8.Checked = Settings.GetReg<bool>("lag_check", true);
            this.checkBox9.Checked = Settings.GetReg<bool>("block_redirect", false);
            this.checkBox10.Checked = Settings.GetReg<bool>("block_cls", false);
            this.checkBox11.Checked = Settings.GetReg<bool>("block_popups", false);
            this.checkBox12.Checked = Settings.GetReg<bool>("back_bg", false);
            this.checkBox13.Checked = Settings.GetReg<bool>("block_friend_popup", false);
            this.checkBox14.Checked = Settings.GetReg<bool>("receive_ppl_fonts", true);
            this.checkBox15.Checked = Settings.GetReg<bool>("block_custom_names", false);
            this.checkBox16.Checked = Settings.GetReg<bool>("can_html", true);
            Settings.CanHTML = Settings.GetReg<bool>("can_html", true);

            InstalledFontCollection fonts = new InstalledFontCollection();

            for (int i = 0; i < fonts.Families.Length; i++)
                this.comboBox1.Items.Add(fonts.Families[i].Name);

            String str = Settings.GetReg<String>("global_font", "Tahoma");
            this.setting_up_font = true;

            for (int i = 0; i < this.comboBox1.Items.Count; i++)
                if ((String)this.comboBox1.Items[i] == str)
                {
                    this.comboBox1.SelectedIndex = i;
                    break;
                }

            this.setting_up_font = false;
            this.numericUpDown1.Value = Settings.GetReg<int>("global_font_size", 10);

            this.panel2.BackColor = this.AresColorCodeToColorObject(Settings.GetReg<int>("defc_name", 1));
            this.panel1.BackColor = this.AresColorCodeToColorObject(Settings.GetReg<int>("defc_pm", 1));
            this.panel4.BackColor = this.AresColorCodeToColorObject(Settings.GetReg<int>("defc_text", 12));
            this.panel3.BackColor = this.AresColorCodeToColorObject(Settings.GetReg<int>("defc_emote", 6));
            this.panel8.BackColor = this.AresColorCodeToColorObject(Settings.GetReg<int>("defc_join", 3));
            this.panel7.BackColor = this.AresColorCodeToColorObject(Settings.GetReg<int>("defc_part", 7));
            this.panel6.BackColor = this.AresColorCodeToColorObject(Settings.GetReg<int>("defc_server", 2));
            this.panel5.BackColor = this.AresColorCodeToColorObject(Settings.GetReg<int>("defc_announce", 4));
        }

        public static String GetDefaultColorString(DefaultColorType type, bool black)
        {
            int i = 0;

            if (type == DefaultColorType.Server)
            {
                i = Settings.GetReg<int>("defc_server", 2);

                if ((black && i == 1) || (!black && i == 0))
                    i = 2;
            }
            else if (type == DefaultColorType.Join)
            {
                i = Settings.GetReg<int>("defc_join", 3);

                if ((black && i == 1) || (!black && i == 0))
                    i = 3;
            }
            else if (type == DefaultColorType.Part)
            {
                i = Settings.GetReg<int>("defc_part", 7);

                if ((black && i == 1) || (!black && i == 0))
                    i = 7;
            }

            return i < 10 ? ("\x00030" + i) : ("\x0003" + i);
        }

        public static int GetDefaultColorInt(DefaultColorType type, bool black)
        {
            int i = 0;

            if (type == DefaultColorType.Announce)
            {
                i = Settings.GetReg<int>("defc_announce", 4);

                if ((black && i == 1) || (!black && i == 0))
                    i = 4;
            }
            else if (type == DefaultColorType.Server)
            {
                i = Settings.GetReg<int>("defc_server", 2);

                if ((black && i == 1) || (!black && i == 0))
                    i = 2;
            }
            else if (type == DefaultColorType.Emote)
            {
                i = Settings.GetReg<int>("defc_emote", 6);

                if ((black && i == 1) || (!black && i == 0))
                    i = 6;
            }
            else if (type == DefaultColorType.Text)
            {
                i = Settings.GetReg<int>("defc_text", 12);

                if ((black && i == 1) || (!black && i == 0))
                    i = 12;
            }
            else if (type == DefaultColorType.Name)
            {
                i = Settings.GetReg<int>("defc_name", 1);

                if (black && i == 1)
                    i = 0;
                else if (!black && i == 0)
                    i = 1;
            }
            else if (type == DefaultColorType.PM)
            {
                i = Settings.GetReg<int>("defc_pm", 1);

                if (black && i == 1)
                    i = 0;
                else if (!black && i == 0)
                    i = 1;
            }

            return i;
        }

        public enum DefaultColorType
        {
            Announce,
            Server,
            Emote,
            Text,
            Name,
            PM,
            Join,
            Part
        }

        public void ColorChangeCallback(String panel, int color)
        {
            switch (panel)
            {
                case "panel2":
                    Settings.SetReg("defc_name", color);
                    this.panel2.BackColor = this.AresColorCodeToColorObject(color);
                    break;

                case "panel1":
                    Settings.SetReg("defc_pm", color);
                    this.panel1.BackColor = this.AresColorCodeToColorObject(color);
                    break;

                case "panel4":
                    Settings.SetReg("defc_text", color);
                    this.panel4.BackColor = this.AresColorCodeToColorObject(color);
                    break;

                case "panel3":
                    Settings.SetReg("defc_emote", color);
                    this.panel3.BackColor = this.AresColorCodeToColorObject(color);
                    break;

                case "panel8":
                    Settings.SetReg("defc_join", color);
                    this.panel8.BackColor = this.AresColorCodeToColorObject(color);
                    break;

                case "panel7":
                    Settings.SetReg("defc_part", color);
                    this.panel7.BackColor = this.AresColorCodeToColorObject(color);
                    break;

                case "panel6":
                    Settings.SetReg("defc_server", color);
                    this.panel6.BackColor = this.AresColorCodeToColorObject(color);
                    break;

                case "panel5":
                    Settings.SetReg("defc_announce", color);
                    this.panel5.BackColor = this.AresColorCodeToColorObject(color);
                    break;
            }
        }

        private void panel2_Click(object sender, EventArgs e)
        {
            String panel = ((Panel)sender).Name;

            SharedUI.CMenu.StartPosition = FormStartPosition.Manual;
            SharedUI.CMenu.Location = new Point(MousePosition.X - 40, MousePosition.Y - 150);
            SharedUI.CMenu.SetCallback(this, panel);
            SharedUI.CMenu.Show();
        }

        private Color AresColorCodeToColorObject(int code)
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

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            Settings.SetReg("can_timestamp", this.checkBox1.Checked);
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            Settings.SetReg("can_emoticon", this.checkBox2.Checked);
        }

        private void checkBox3_CheckedChanged(object sender, EventArgs e)
        {
            Settings.SetReg("crypto", (int)(this.checkBox3.Checked ? 250 : 0));
        }

        private void checkBox4_CheckedChanged(object sender, EventArgs e)
        {
            Settings.SetReg("can_write", this.checkBox4.Checked);

            if (!this.checkBox4.Checked)
                RoomPool.Rooms.ForEach(x => x.CancelWritingStatus());
        }

        private void checkBox5_CheckedChanged(object sender, EventArgs e)
        {
            Settings.SetReg("can_receive_pms", this.checkBox5.Checked);
        }

        private void checkBox6_CheckedChanged(object sender, EventArgs e)
        {
            Settings.SetReg("receive_nudge", this.checkBox6.Checked);
        }

        private void checkBox7_CheckedChanged(object sender, EventArgs e)
        {
            Settings.SetReg("receive_scribbles", this.checkBox7.Checked);
        }

        private void checkBox8_CheckedChanged(object sender, EventArgs e)
        {
            Settings.SetReg("lag_check", this.checkBox8.Checked);
        }

        private void checkBox9_CheckedChanged(object sender, EventArgs e)
        {
            Settings.SetReg("block_redirect", this.checkBox9.Checked);
        }

        private void checkBox10_CheckedChanged(object sender, EventArgs e)
        {
            Settings.SetReg("block_cls", this.checkBox10.Checked);
        }

        private void checkBox11_CheckedChanged(object sender, EventArgs e)
        {
            Settings.SetReg("block_popups", this.checkBox11.Checked);
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.comboBox1.SelectedIndex > -1)
            {
                Settings.SetReg("global_font", (String)this.comboBox1.SelectedItem);

                if (!this.setting_up_font)
                    RoomPool.Rooms.ForEach(x => x.Panel.UpdateFont());
            }
        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            Settings.SetReg("global_font_size", (int)this.numericUpDown1.Value);
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.comboBox2.SelectedIndex > -1)
            {
                Settings.SetReg("spell_checker", this.comboBox2.SelectedIndex);

                SpellChecker.Load();
                this.SpellCheckUpdate(this.comboBox2.SelectedIndex > 0, null);
            }
        }

        private void checkBox12_CheckedChanged(object sender, EventArgs e)
        {
            Settings.SetReg("back_bg", this.checkBox12.Checked);
        }

        private void checkBox13_CheckedChanged(object sender, EventArgs e)
        {
            Settings.SetReg("block_friend_popup", this.checkBox13.Checked);
        }

        private void checkBox14_CheckedChanged(object sender, EventArgs e)
        {
            Settings.SetReg("receive_ppl_fonts", this.checkBox14.Checked);
        }

        private void checkBox15_CheckedChanged(object sender, EventArgs e)
        {
            Settings.SetReg("block_custom_names", this.checkBox15.Checked);

            if (this.BlockCustomNamesUpdate != null)
                this.BlockCustomNamesUpdate(null, EventArgs.Empty);
        }

        private void checkBox16_CheckedChanged(object sender, EventArgs e)
        {
            Settings.SetReg("can_html", this.checkBox16.Checked);
            Settings.CanHTML = this.checkBox16.Checked;
        }

        
    }
}
