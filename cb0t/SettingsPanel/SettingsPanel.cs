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
        private ClientSettings client_settings { get; set; }
        private GlobalSettings global_settings { get; set; }
        private HashlinkSettings hashlink_settings { get; set; }
        private PersonalSettings personal_settings { get; set; }
        private AudioSettings audio_settings { get; set; }
        private FilterSettings filter_settings { get; set; }
        private MenuSettings menu_settings { get; set; }
        private PrivacySettings privacy_settings { get; set; }

        public event EventHandler JoinFromHashlinkClicked;
        public event EventHandler SpellCheckUpdate2;
        public event EventHandler OnTemplateChanged;
        public event EventHandler BlockCustomNamesUpdate2;

        public SettingsPanel()
        {
            this.InitializeComponent();
            this.treeView1.ExpandAll();
        }

        public void UpdateTemplate()
        {
            this.client_settings.UpdateTemplate();
            this.global_settings.UpdateTemplate();
            this.hashlink_settings.UpdateTemplate();
            this.personal_settings.UpdateTemplate();
            this.audio_settings.UpdateTemplate();
            this.filter_settings.UpdateTemplate();
            this.menu_settings.UpdateTemplate();
            this.privacy_settings.UpdateTemplate();

            for (int i = 0; i < this.treeView1.Nodes.Count; i++)
                this.treeView1.Nodes[i].Text = StringTemplate.Get(STType.Settings, (i * 2));
        }

        public void CreateSettings()
        {
            this.client_settings = new ClientSettings();
            this.client_settings.TemplateChanged += this.TemplateChanged;
            this.client_settings.Dock = DockStyle.Fill;
            this.client_settings.AutoScroll = true;
            this.client_settings.Populate();
            this.global_settings = new GlobalSettings();
            this.global_settings.Dock = DockStyle.Fill;
            this.global_settings.AutoScroll = true;
            this.global_settings.SpellCheckUpdate += this.SpellCheckUpdate;
            this.global_settings.BlockCustomNamesUpdate += this.BlockCustomNamesUpdate;
            this.global_settings.Populate();
            this.hashlink_settings = new HashlinkSettings();
            this.hashlink_settings.Dock = DockStyle.Fill;
            this.hashlink_settings.AutoScroll = true;
            this.hashlink_settings.Populate();
            this.hashlink_settings.JoinFromHashlink += this.JoinFromHashlink;
            this.personal_settings = new PersonalSettings();
            this.personal_settings.Dock = DockStyle.Fill;
            this.personal_settings.AutoScroll = true;
            this.personal_settings.Populate();
            this.audio_settings = new AudioSettings();
            this.audio_settings.Dock = DockStyle.Fill;
            this.audio_settings.AutoScroll = true;
            this.audio_settings.Populate();
            this.filter_settings = new FilterSettings();
            this.filter_settings.Dock = DockStyle.Fill;
            this.filter_settings.AutoScroll = true;
            this.filter_settings.Populate();
            this.menu_settings = new MenuSettings();
            this.menu_settings.Dock = DockStyle.Fill;
            this.menu_settings.AutoScroll = true;
            this.menu_settings.Populate();
            this.privacy_settings = new PrivacySettings();
            this.privacy_settings.Dock = DockStyle.Fill;
            this.privacy_settings.AutoScroll = true;
            this.privacy_settings.Populate();

            this.treeView1.SelectedNode = this.treeView1.Nodes[0];
        }

        private void BlockCustomNamesUpdate(object sender, EventArgs e)
        {
            if (this.BlockCustomNamesUpdate2 != null)
                this.BlockCustomNamesUpdate2(sender, e);
        }

        private void TemplateChanged(object sender, EventArgs e)
        {
            this.OnTemplateChanged(sender, e);
        }

        private void SpellCheckUpdate(object sender, EventArgs e)
        {
            this.SpellCheckUpdate2(sender, e);
        }

        private void JoinFromHashlink(object sender, EventArgs e)
        {
            this.JoinFromHashlinkClicked(sender, e);
        }

        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (e.Node == null)
                return;

            while (this.panel1.Controls.Count > 0)
                this.panel1.Controls.RemoveAt(0);

            if (e.Node.Equals(this.treeView1.Nodes[0]))
                this.panel1.Controls.Add(this.client_settings);
            else if (e.Node.Equals(this.treeView1.Nodes[1]))
                this.panel1.Controls.Add(this.global_settings);
            else if (e.Node.Equals(this.treeView1.Nodes[2]))
                this.panel1.Controls.Add(this.hashlink_settings);
            else if (e.Node.Equals(this.treeView1.Nodes[3]))
                this.panel1.Controls.Add(this.personal_settings);
            else if (e.Node.Equals(this.treeView1.Nodes[4]))
                this.panel1.Controls.Add(this.audio_settings);
            else if (e.Node.Equals(this.treeView1.Nodes[5]))
                this.panel1.Controls.Add(this.filter_settings);
            else if (e.Node.Equals(this.treeView1.Nodes[6]))
                this.panel1.Controls.Add(this.menu_settings);
            else if (e.Node.Equals(this.treeView1.Nodes[7]))
                this.panel1.Controls.Add(this.privacy_settings);
        }

        private void treeView1_BeforeCollapse(object sender, TreeViewCancelEventArgs e)
        {
            e.Cancel = true;
        }
    }
}
