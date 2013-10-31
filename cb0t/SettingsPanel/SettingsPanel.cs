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
    public partial class SettingsPanel : UserControl
    {
        private GlobalSettings global_settings { get; set; }

        public SettingsPanel()
        {
            this.InitializeComponent();
        }

        public void CreateSettings()
        {
            this.global_settings = new GlobalSettings();
            this.global_settings.Dock = DockStyle.Fill;
            this.global_settings.AutoScroll = true;
            this.global_settings.Populate();

            this.treeView1.SelectedNode = this.treeView1.Nodes[0];
        }

        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (e.Node == null)
                return;

            while (this.panel1.Controls.Count > 0)
                this.panel1.Controls.RemoveAt(0);

            if (e.Node.Equals(this.treeView1.Nodes[0]))
                this.panel1.Controls.Add(this.global_settings);

            this.treeView1.SelectedNode = null;
        }


    }
}
