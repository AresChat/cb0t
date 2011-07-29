using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace cb0t_chat_client_v2
{
    public partial class CEmoticonShortcutA : Form
    {
        public CEmoticonShortcutA()
        {
            InitializeComponent();
        }

        public String KeyboardShortcut
        {
            get { return this.textBox1.Text; }
        }
    }
}
