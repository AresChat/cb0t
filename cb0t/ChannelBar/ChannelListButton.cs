using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using System.Net;

namespace cb0t
{
    class ChannelListButton : ToolStripButton
    {
        public Bitmap icon;

        public ChannelListButton()
        {
            this.icon = (Bitmap)Properties.Resources.clist.Clone();
            this.AutoSize = false;
            this.ForeColor = Color.Black;
            this.Image = this.icon;
            this.ImageAlign = ContentAlignment.MiddleLeft;
            this.ImageTransparentColor = Color.Magenta;
            this.Size = new Size(172, 28);
            this.Text = "Channels";
            this.ToolTipText = "Channels";
            this.TextAlign = ContentAlignment.MiddleLeft;
        }
    }
}
