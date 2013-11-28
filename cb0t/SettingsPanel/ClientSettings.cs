using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;

namespace cb0t
{
    public partial class ClientSettings : UserControl
    {
        public ClientSettings()
        {
            InitializeComponent();
        }

        public void UpdateTemplate()
        {
            this.label5.Text = StringTemplate.Get(STType.About, 0);
            this.label6.Text = StringTemplate.Get(STType.About, 1);
            this.label1.Text = StringTemplate.Get(STType.Settings, 1);
            this.button1.Text = StringTemplate.Get(STType.ClientSettings, 0);
            this.label4.Text = StringTemplate.Get(STType.ClientSettings, 1) + ":";
            this.label7.Text = StringTemplate.Get(STType.ClientSettings, 2);
        }

        public void Populate()
        {
            this.label3.Text = "version " + Settings.APP_VERSION;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                Process.Start("explorer.exe", Settings.DataPath);
            }
            catch { }
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try
            {
                Process.Start("http://cb0t.codeplex.com/");
            }
            catch { }
        }

    }
}
