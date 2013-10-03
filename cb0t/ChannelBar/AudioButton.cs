using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Windows.Forms;

namespace cb0t
{
    class AudioButton : ToolStripButton
    {
        public Bitmap icon;

        public AudioButton()
        {
            this.icon = (Bitmap)Properties.Resources.audio.Clone();
            this.AutoSize = false;
            this.ForeColor = Color.Black;
            this.Image = this.icon;
            this.ImageAlign = ContentAlignment.MiddleCenter;
            this.ImageTransparentColor = Color.Magenta;
            this.Size = new Size(28, 28);
            this.Text = String.Empty;
            this.ToolTipText = "Audio";
            this.TextAlign = ContentAlignment.MiddleCenter;
            this.DisplayStyle = ToolStripItemDisplayStyle.Image;
        }
    }
}
