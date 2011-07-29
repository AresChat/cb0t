using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Drawing;
using System.Windows.Forms;

namespace cb0t_chat_client_v2
{
    class DCAvatar : UserControl
    {
        public DCAvatar()
        {
            this.DoubleBuffered = true;
        }

        private Bitmap avatar = null;

        public void UpdateImage(byte[] data)
        {
            this.avatar = new Bitmap(new MemoryStream(data));
            this.Invalidate();
        }

        public void UpdateImage(Bitmap data)
        {
            this.avatar = (Bitmap)data.Clone();
            this.Invalidate();
        }

        protected override void OnPaintBackground(PaintEventArgs e)
        {
            Rectangle r = e.ClipRectangle;
            Graphics g = e.Graphics;

            int cr = 230;
            int cg = 230;
            int cb = 230;
            bool decrement = true;

            Color c = Color.FromArgb(cr, cg, cb);

            g.FillRectangle(Brushes.Gainsboro, r);

            for (int x = 0; x < 8; x++) // make a nice faded border for avatar to sit in
            {
                g.DrawRectangle(new Pen(new SolidBrush(c), 1), new Rectangle(r.X + x, r.Y + x, r.Width - 1 - (x * 2), r.Height - 1 - (x * 2)));

                if (decrement)
                {
                    cr -= 40;
                    cg -= 40;
                    cb -= 40;

                    if (cr < 70)
                    {
                        cr = 40;
                        cg = 40;
                        cb = 40;
                        decrement = false;
                    }
                }
                else
                {
                    cr += 60;
                    cg += 60;
                    cb += 60;
                }

                c = Color.FromArgb(cr, cg, cb);
            }

            if (this.avatar != null)
            {
                g.DrawImage(this.avatar, new RectangleF(r.X + 8, r.Y + 8, 94, 94));
                g.DrawRectangle(new Pen(Brushes.LightGoldenrodYellow, 1), new Rectangle(r.X + 8, r.Y + 8, 93, 93));
            }
        }
    }
}
