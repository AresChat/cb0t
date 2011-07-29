using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace cb0t_chat_client_v2
{
    public partial class CEmoticonSizeA : Form
    {
        public CEmoticonSizeA()
        {
            InitializeComponent();
        }

        public int EmoticonSize
        {
            get
            {
                if (this.radioButton1.Checked)
                    return 16;
                else if (this.radioButton2.Checked)
                    return 32;
                else return 48;
            }
        }

        private void radioButton1_Click(object sender, EventArgs e)
        {
            // 16
            this.radioButton1.Checked = true;
            this.radioButton2.Checked = false;
            this.radioButton3.Checked = false;
        }

        private void radioButton2_Click(object sender, EventArgs e)
        {
            // 32
            this.radioButton1.Checked = false;
            this.radioButton2.Checked = true;
            this.radioButton3.Checked = false;
        }

        private void radioButton3_Click(object sender, EventArgs e)
        {
            // 48
            this.radioButton1.Checked = false;
            this.radioButton2.Checked = false;
            this.radioButton3.Checked = true;
        }
    }
}
