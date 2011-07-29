using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;

namespace cb0t_chat_client_v2
{
    public partial class AudioOptionsScreen : Form
    {
        private bool setting_up = false;

        public AudioOptionsScreen()
        {
            InitializeComponent();
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try
            {
                Process.Start("http://www.last.fm/home");
            }
            catch { }
        }

        private void checkBox3_CheckedChanged(object sender, EventArgs e)
        {
            AudioSettings.voice_mute = this.checkBox3.Checked;

            if (!this.setting_up)
                AudioSettings.Save();
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            AudioSettings.winamp = this.comboBox1.SelectedIndex == 1;

            if (!this.setting_up)
                AudioSettings.Save();
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            AudioSettings.show_album_art = this.checkBox2.Checked;

            if (!this.setting_up)
                AudioSettings.Save();
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            AudioSettings.show_in_userlist = this.checkBox1.Checked;

            if (!this.setting_up)
                AudioSettings.Save();
        }

        public void UpdateNPText()
        {
            AudioSettings.np_text = this.textBox1.Text;

            if (!this.setting_up)
                AudioSettings.Save();
        }

        public void SetupValues()
        {
            this.setting_up = true;
            this.checkBox1.Checked = AudioSettings.show_in_userlist;
            this.checkBox2.Checked = AudioSettings.show_album_art;
            this.checkBox3.Checked = AudioSettings.voice_mute;
            this.checkBox4.Checked = AudioSettings.unicode_effect;
            this.textBox1.Text = AudioSettings.np_text;
            this.comboBox1.SelectedIndex = AudioSettings.winamp ? 1 : 0;
            this.setting_up = false;
        }

        private void checkBox4_CheckedChanged(object sender, EventArgs e)
        {
            AudioSettings.unicode_effect = this.checkBox4.Checked;
            
            if (!this.setting_up)
                AudioSettings.Save();
        }
        
    }
}
