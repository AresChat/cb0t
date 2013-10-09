using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace cb0t
{
    class ButtonBar : ToolStripRenderer
    {
        protected override void OnRenderToolStripBackground(ToolStripRenderEventArgs e)
        {
            Rectangle bounds = new Rectangle(0, 0, e.ToolStrip.Width, e.ToolStrip.Height);

            using (LinearGradientBrush brush = new LinearGradientBrush(bounds, Color.WhiteSmoke, Color.Gainsboro, LinearGradientMode.Vertical))
                e.Graphics.FillRectangle(brush, bounds);
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
}
