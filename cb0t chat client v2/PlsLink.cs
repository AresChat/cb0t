using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace cb0t_chat_client_v2
{
    public partial class PlsLink : Form
    {
        public PlsLink()
        {
            InitializeComponent();
        }

        public String Link
        {
            get { return this.textBox1.Text; }
        }
    }
}
