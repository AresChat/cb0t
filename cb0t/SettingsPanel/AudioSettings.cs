using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace cb0t
{
    public partial class AudioSettings : UserControl
    {
        public AudioSettings()
        {
            InitializeComponent();
        }

        public void UpdateTemplate()
        {
            this.label1.Text = StringTemplate.Get(STType.Settings, 9);
            
        }

        public void Populate()
        {
            String str;

            foreach (String s in AudioHelpers.GetRecordDevices())
                this.comboBox6.Items.Add(s);

            if (this.comboBox6.Items.Count > 0)
            {
                str = Settings.GetReg<String>("vc_rec_device", String.Empty);
                this.comboBox6.SelectedIndex = 0;

                for (int i = 0; i < this.comboBox6.Items.Count; i++)
                    if (this.comboBox6.Items[i].ToString() == str)
                    {
                        this.comboBox6.SelectedIndex = i;
                        break;
                    }
            }

            foreach (String s in AudioHelpers.GetPlaybackDevices())
                this.comboBox1.Items.Add(s);

            if (this.comboBox1.Items.Count > 0)
            {
                str = Settings.GetReg<String>("vc_play_device", String.Empty);
                this.comboBox1.SelectedIndex = 0;

                for (int i = 0; i < this.comboBox1.Items.Count; i++)
                    if (this.comboBox1.Items[i].ToString() == str)
                    {
                        this.comboBox1.SelectedIndex = i;
                        break;
                    }
            }

            this.comboBox2.SelectedIndex = Settings.GetReg<int>("m_listener_index", 0);
            this.checkBox1.Checked = Settings.GetReg<bool>("vc_public", true);
            this.checkBox2.Checked = Settings.GetReg<bool>("vc_private", true);
            this.checkBox3.Checked = Settings.GetReg<bool>("show_song", true);
            this.checkBox4.Checked = Settings.GetReg<bool>("can_popup_sound", true);
            this.checkBox5.Checked = Settings.GetReg<bool>("can_narrate", false);
            this.checkBox6.Checked = Settings.GetReg<bool>("can_opus", true);
        }

        private void comboBox6_SelectedIndexChanged(object sender, EventArgs e)
        {
            Settings.SetReg("vc_rec_device", this.comboBox6.SelectedItem.ToString());
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            Settings.SetReg("vc_play_device", this.comboBox1.SelectedItem.ToString());
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            Settings.SetReg("vc_public", this.checkBox1.Checked);
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            Settings.SetReg("vc_private", this.checkBox2.Checked);
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.checkBox3.Enabled = this.comboBox2.SelectedIndex > 0;
            Settings.SetReg("m_listener_index", this.comboBox2.SelectedIndex);
        }

        private void checkBox3_CheckedChanged(object sender, EventArgs e)
        {
            Settings.SetReg("show_song", this.checkBox3.Checked);
        }

        private void checkBox4_CheckedChanged(object sender, EventArgs e)
        {
            Settings.SetReg("can_popup_sound", this.checkBox4.Checked);
        }

        private void checkBox5_CheckedChanged(object sender, EventArgs e)
        {
            Settings.SetReg("can_narrate", this.checkBox5.Checked);
        }

        private void checkBox6_CheckedChanged(object sender, EventArgs e)
        {
            Settings.SetReg("can_opus", this.checkBox6.Checked);
        }
    }
}
