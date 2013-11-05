using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Windows.Forms;

namespace cb0t
{
    class ColorMenuItem : Panel
    {
        public ColorMenuItem()
        {
            this.Paint += this.PaintPanel;
        }

        private void PaintPanel(object sender, PaintEventArgs e)
        {
            e.Graphics.Clear(this.BackColor);

            if (this.is_ht)
            {
                Color c = this.BackColor;

                if (c.Equals(Color.Gray))
                    c = Color.Silver;

                c = Color.FromArgb(255 - c.R, 255 - c.G, 255 - c.B);

                using (Pen pen = new Pen(c, 1))
                    e.Graphics.DrawRectangle(pen, new Rectangle(0, 0, this.Width - 3, this.Height - 3));
            }
        }

        private bool is_ht = false;

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
