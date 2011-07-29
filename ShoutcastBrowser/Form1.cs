using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace ShoutcastBrowser
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void webBrowser1_NewWindow(object sender, CancelEventArgs e)
        {
            e.Cancel = true;
        }

        private void webBrowser1_Navigating(object sender, WebBrowserNavigatingEventArgs e)
        {
            this.webBrowser1.Document.Cookie = "Settings=Player~others|Bandwidth~undefined|Codec~ALL";

            if (e.Url.AbsoluteUri.Contains(".pls"))
            {
                try
                {
                    System.Diagnostics.Process.Start("cb0t.exe", "cb0t://radio:" + e.Url.AbsoluteUri);
                }
                catch { }

                e.Cancel = true;
                this.Close();
            }
        }
    }
}
