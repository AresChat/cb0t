using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace cb0t
{
    class Topic : ToolStripRenderer
    {
        private bool close_hottracking = false;

        private SolidBrush close_brush { get; set; }

        public Topic()
        {
            this.close_brush = new SolidBrush(Color.Red);
        }

        public void Free()
        {
            this.close_brush.Dispose();
        }

        protected override void OnRenderToolStripBackground(ToolStripRenderEventArgs e)
        {
            e.Graphics.Clear(Color.WhiteSmoke);

            // topic
            Rectangle topic_bounds = new Rectangle(4, (e.ToolStrip.Height / 2) - 9, (e.ToolStrip.Width - 60), 20);

            // close
            if (this.close_hottracking)
            {
                Rectangle rec = new Rectangle(e.ToolStrip.Width - 19, 7, 12, 10);
                e.Graphics.FillRectangle(this.close_brush, rec);
            }
        }

        protected override void OnRenderButtonBackground(ToolStripItemRenderEventArgs e)
        {
            if (e.Item.Selected != this.close_hottracking)
            {
                this.close_hottracking = !this.close_hottracking;
                e.ToolStrip.Invalidate();
            }
        }

        protected override void OnRenderItemText(ToolStripItemTextRenderEventArgs e)
        {
            e.TextColor = e.Item.Selected ? Color.Red : Color.Black;
            base.OnRenderItemText(e);
        }
    }
}
