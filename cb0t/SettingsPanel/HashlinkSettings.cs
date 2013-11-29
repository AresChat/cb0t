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
    public partial class HashlinkSettings : UserControl
    {
        public HashlinkSettings()
        {
            InitializeComponent();
        }

        public void UpdateTemplate()
        {
            this.label1.Text = StringTemplate.Get(STType.Settings, 5);
            this.button1.Text = StringTemplate.Get(STType.HashlinkSettings, 0);
        }

        public event EventHandler JoinFromHashlink;

        public void Populate()
        {
            this.textBox1.Text = Settings.GetReg<String>("last_hashlink", String.Empty);
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            Settings.SetReg("last_hashlink", this.textBox1.Text);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            String hashlink = this.textBox1.Text.Trim(' ', '\r', '\n');

            if (hashlink.StartsWith("arlnk://"))
                hashlink = hashlink.Substring(8);

            DecryptedHashlink h = Hashlink.DecodeHashlink(hashlink);

            if (h != null)
                this.JoinFromHashlink(h, EventArgs.Empty);
            else MessageBox.Show("Invalid hashlink",
                "cb0t", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}
