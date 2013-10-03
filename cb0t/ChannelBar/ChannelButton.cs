using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using System.Net;

namespace cb0t
{
    class ChannelButton : ToolStripButton
    {
        public IPEndPoint EndPoint { get; private set; }
        public String RoomName { get; private set; }

        private Bitmap read;
        private Bitmap unread;
        private bool is_read = true;

        public ChannelButton(FavouritesListItem item)
        {
            this.read = (Bitmap)Properties.Resources.read.Clone();
            this.unread = (Bitmap)Properties.Resources.unread.Clone();
            this.AutoSize = false;
            this.ForeColor = Color.Black;
            this.Image = this.read;
            this.ImageAlign = ContentAlignment.MiddleLeft;
            this.ImageTransparentColor = Color.Magenta;
            this.Size = new Size(172, 28);
            this.Text = String.Empty;
            this.RoomName = item.Name;
            this.ToolTipText = item.Name;
            this.EndPoint = new IPEndPoint(item.IP, item.Port);
            this.TextAlign = ContentAlignment.MiddleLeft;
        }

        public void MakeRead()
        {
            if (!this.is_read)
            {
                this.is_read = true;
                this.Image = this.read;
            }
        }

        public void MakeUnread()
        {
            if (this.is_read)
            {
                this.is_read = false;
                this.Image = this.unread;
            }
        }

        public void Free()
        {
            this.read.Dispose();
            this.unread.Dispose();
        }
    }
}
