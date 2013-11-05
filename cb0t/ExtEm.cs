using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using System.Drawing.Drawing2D;

namespace cb0t
{
    class ExtEm : Panel
    {
        private Bitmap preview { get; set; }

        public String ShortcutText { get; private set; }

        public ExtEm(ExtendedEmoticon em)
        {
            this.ShortcutText = em.ShortcutText.ToLower();

            int org_width = em.Img.Width;
            int org_height = em.Img.Height;

            if (org_width <= 50 && org_height <= 50)
            {
                this.preview = new Bitmap(org_width, org_height);

                using (Graphics g = Graphics.FromImage(this.preview))
                {
                    g.Clear(Color.White);
                    g.DrawImage(em.Img, new Point(0, 0));
                }
            }
            else
            {
                double ratioX = (double)50 / org_width;
                double ratioY = (double)50 / org_height;
                double ratio = Math.Min(ratioX, ratioY);

                int new_width = (int)(org_width * ratio);
                int new_height = (int)(org_height * ratio);

                this.preview = new Bitmap(new_width, new_height);

                using (Graphics g = Graphics.FromImage(this.preview))
                {
                    g.Clear(Color.White);
                    g.DrawImage(em.Img, 0, 0, new_width, new_height);
                }
            }

            this.Paint += this.PaintPreview;
        }

        private bool is_ht = false;

        private void PaintPreview(object sender, PaintEventArgs e)
        {
            e.Graphics.Clear(Color.White);

            int x = 25 - (int)(double)((double)this.preview.Width / 2);
            int y = 25 - (int)(double)((double)this.preview.Height / 2);

            e.Graphics.DrawImage(this.preview, new Point(x, y));

            if (this.is_ht)
                using (Pen pen = new Pen(Color.DimGray, 1))
                    e.Graphics.DrawRectangle(pen, new Rectangle(0, 0, 49, 49));
        }

        protected override void OnMouseEnter(EventArgs e)
        {
            base.OnMouseEnter(e);

            if (!this.is_ht)
            {
                this.is_ht = true;
                this.Invalidate();
            }
        }

        protected override void OnMouseClick(MouseEventArgs e)
        {
            if (this.is_ht)
            {
                this.is_ht = false;
                this.Invalidate();
            }

            base.OnMouseClick(e);
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);

            if (this.is_ht)
            {
                this.is_ht = false;
                this.Invalidate();
            }
        }
    }
}
