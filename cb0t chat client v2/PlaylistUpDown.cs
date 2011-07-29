using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Windows.Forms;

namespace cb0t_chat_client_v2
{
    class PlaylistUpDown : UserControl
    {
        public event EventHandler UpClicked;
        public event EventHandler DownClicked;

        protected override void OnPaint(PaintEventArgs e)
        {
            if (AudioControllerImages.arrow_down != null)
            {
                e.Graphics.DrawImage(AudioControllerImages.arrow_up, new Point(0, 0));
                e.Graphics.DrawImage(AudioControllerImages.arrow_down, new Point(14, 0));
            }
        }

        protected override void OnMouseClick(MouseEventArgs e)
        {
            if (e.X >= 0 && e.X <= 12)
            {
                if (this.UpClicked != null)
                    this.UpClicked(this, new EventArgs());
            }
            else if (this.DownClicked != null)
                this.DownClicked(this, new EventArgs());
        }
    }
}
