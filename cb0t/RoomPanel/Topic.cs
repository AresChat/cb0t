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
            Rectangle bounds = new Rectangle(0, 0, e.ToolStrip.Width, e.ToolStrip.Height);

            using (LinearGradientBrush brush = new LinearGradientBrush(bounds, Color.Gainsboro, Color.WhiteSmoke, LinearGradientMode.Vertical))
                e.Graphics.FillRectangle(brush, bounds);

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
            if (e.Item.Selected || e.Item.Pressed)
            {
                using (GraphicsPath path = new Rectangle(1, 1, e.Item.Width - 3, e.Item.Height - 3).Rounded(2))
                using (Pen pen = new Pen(Color.Silver))
                    e.Graphics.DrawPath(pen, path);
            }

            if (e.Item.Selected != this.close_hottracking)
            {
                this.close_hottracking = !this.close_hottracking;
                e.ToolStrip.Invalidate();
            }
        }

        protected override void OnRenderDropDownButtonBackground(ToolStripItemRenderEventArgs e)
        {
            if (e.Item.Selected || e.Item.Pressed)
            {
                using (GraphicsPath path = new Rectangle(1, 1, e.Item.Width - 3, e.Item.Height - 3).Rounded(2))
                using (Pen pen = new Pen(Color.Silver))
                    e.Graphics.DrawPath(pen, path);
            }
        }

        protected override void OnRenderItemText(ToolStripItemTextRenderEventArgs e)
        {
            e.TextColor = e.Item.Selected ? Color.Red : Color.Black;
            base.OnRenderItemText(e);
        }
    }
}
