using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace FormEx.SystemMenuEx
{
    public class SystemMenuItem : EventArgs, ISystemMenuItem, IDisposable
    {
        public String Text { get; private set; }
        public IntPtr Image { get; private set; }
        public int Index { get; set; }

        internal uint Ident { get; set; }

        public SystemMenuItem(String text, IntPtr img)
        {
            this.Text = text;
            this.Image = img;
        }

        public SystemMenuItem(String text, Image img)
        {
            this.Text = text;

            if (img == null)
                this.Image = IntPtr.Zero;
            else
            {
                using (Bitmap bmp = new Bitmap(16, 16))
                using (Graphics g = Graphics.FromImage(bmp))
                {
                    Rectangle rec = new Rectangle(0, 0, 16, 16);

                    using (SolidBrush brush = new SolidBrush(SystemColors.Control))
                        g.FillRectangle(brush, rec);

                    g.DrawImage(img, new Point(0, 0));
                    this.Image = bmp.GetHbitmap();
                }
            }
        }

        public void Dispose()
        {
            // release image
        }
    }
}
