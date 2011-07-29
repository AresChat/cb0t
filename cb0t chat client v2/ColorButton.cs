using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Windows.Forms;

namespace cb0t_chat_client_v2
{
    class ColorButton : UserControl
    {
        private Color bcol = Color.Red;

        public ColorButton()
        {
            this.DoubleBuffered = true;
        }

        public Color SelectedColor
        {
            get
            {
                return this.bcol;
            }
            set
            {
                this.bcol = value;
                this.Invalidate();
            }
        }

        protected override void OnPaintBackground(PaintEventArgs e)
        {
            Rectangle r = e.ClipRectangle;
            Graphics g = e.Graphics;
            
            g.FillRectangle(new SolidBrush(this.bcol), r);
            g.DrawRectangle(new Pen(Brushes.Black, 1), new Rectangle(r.X, r.Y, r.Width - 1, r.Height - 1));
        }
    }
}
