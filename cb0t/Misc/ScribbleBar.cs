using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace cb0t
{
    class ScribbleBar : ToolStripRenderer
    {
        private Pen outline = new Pen(new SolidBrush(Color.FromArgb(103, 104, 106)), 1);
        public ScribbleBrush BrushWeight { get; set; }

        public ScribbleBar()
        {
            this.BrushWeight = ScribbleBrush.Thin;
        }

        protected override void OnRenderToolStripBackground(ToolStripRenderEventArgs e)
        {
            Rectangle bounds = new Rectangle(0, 0, e.ToolStrip.Width, e.ToolStrip.Height);

            using (LinearGradientBrush brush = new LinearGradientBrush(bounds, Color.Gainsboro, Color.WhiteSmoke, LinearGradientMode.Vertical))
                e.Graphics.FillRectangle(brush, bounds);
        }

        protected override void OnRenderSeparator(ToolStripSeparatorRenderEventArgs e)
        {
            Rectangle r = e.Item.Bounds;
            e.Graphics.DrawLine(this.outline, new Point((r.Width / 2) - 1, 6), new Point((r.Width / 2) - 1, (r.Height - 6)));
        }

        protected override void OnRenderLabelBackground(ToolStripItemRenderEventArgs e)
        {
            Rectangle r = new Rectangle(2, 2, 12, 12);

            using (SolidBrush sb = new SolidBrush(e.Item.BackColor))
                e.Graphics.FillRectangle(sb, r);

            e.Graphics.DrawRectangle(this.outline, new Rectangle(2, 2, 11, 11));
        }

        protected override void OnRenderButtonBackground(ToolStripItemRenderEventArgs e)
        {
            if (e.Item.Selected || e.Item.Pressed)
            {
                using (GraphicsPath path = new Rectangle(1, 1, e.Item.Width - 3, e.Item.Height - 3).Rounded(2))
                using (Pen pen = new Pen(Color.Silver))
                    e.Graphics.DrawPath(pen, path);
            }
        }
    }

    enum ScribbleBrush
    {
        Thin,
        Medium,
        Thick
    }
}
