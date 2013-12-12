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
        }

        public event EventHandler SpellCheckUpdate;
        public event EventHandler BlockCustomNamesUpdate;

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

            InstalledFontCollection fonts = new InstalledFontCollection();

            for (int i = 0; i < fonts.Families.Length; i++)
                this.comboBox1.Items.Add(fonts.Families[i].Name);

            String str = Settings.GetReg<String>("global_font", "Tahoma");

            for (int i = 0; i < this.comboBox1.Items.Count; i++)
                if ((String)this.comboBox1.Items[i] == str)
                {
                    this.comboBox1.SelectedIndex = i;
                    break;
                }

            this.numericUpDown1.Value = Settings.GetReg<int>("global_font_size", 10);
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
                Settings.SetReg("global_font", (String)this.comboBox1.SelectedItem);
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
    }
}
