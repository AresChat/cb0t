using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using System.IO;

namespace cb0t
{
    public partial class ClientSettings : UserControl
    {
        public ClientSettings()
        {
            InitializeComponent();
        }

        private bool setting_up = false;

        public void UpdateTemplate()
        {
            this.label5.Text = StringTemplate.Get(STType.About, 0);
            this.label6.Text = StringTemplate.Get(STType.About, 1);
            this.label1.Text = StringTemplate.Get(STType.Settings, 1);
            this.button1.Text = StringTemplate.Get(STType.ClientSettings, 0);
            this.label4.Text = StringTemplate.Get(STType.ClientSettings, 1) + ":";
            this.label7.Text = StringTemplate.Get(STType.ClientSettings, 2);
        }

        public event EventHandler TemplateChanged;

        public void Populate()
        {
            this.setting_up = true;
            this.label3.Text = "version " + Settings.APP_VERSION;
            String path = Path.Combine(Settings.AppPath, "templates");
            DirectoryInfo dir = new DirectoryInfo(path);
            List<FileInfo> files = new List<FileInfo>(dir.GetFiles("*.txt"));
            files.Sort((x, y) => x.Name.CompareTo(y.Name));

            foreach (FileInfo f in files)
                this.comboBox1.Items.Add(Path.GetFileNameWithoutExtension(f.Name));

            String wanted = Settings.GetReg<String>("template_name", "English");

            for (int i = 0; i < this.comboBox1.Items.Count; i++)
                if (this.comboBox1.Items[i].ToString() == wanted)
                {
                    this.comboBox1.SelectedIndex = i;
                    break;
                }

            this.setting_up = false;
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

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.setting_up)
                return;

            if (this.comboBox1.SelectedIndex > -1)
            {
                Settings.SetReg("template_name", this.comboBox1.SelectedItem.ToString());
                this.TemplateChanged(this, EventArgs.Empty);
            }
        }

    }
}
