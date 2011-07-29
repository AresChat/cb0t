using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace cb0t_chat_client_v2
{
    class ContextMenuRenderer : ToolStripRenderer
    {
        protected override void OnRenderToolStripBackground(ToolStripRenderEventArgs e)
        {
            GraphicsPath p = RoundedRectangle.Create(e.AffectedBounds.X, e.AffectedBounds.Y, e.AffectedBounds.Width, e.AffectedBounds.Height);
            e.Graphics.FillPath(new LinearGradientBrush(e.AffectedBounds, Color.White, Color.Gainsboro, LinearGradientMode.ForwardDiagonal), p);
            p = RoundedRectangle.Create(e.AffectedBounds.X, e.AffectedBounds.Y, e.AffectedBounds.Width - 1, e.AffectedBounds.Height - 1);
            e.Graphics.DrawPath(new Pen(Brushes.Silver, 1), p);
        }

        protected override void OnRenderMenuItemBackground(ToolStripItemRenderEventArgs e)
        {
            if (e.Item.Text.StartsWith("Option"))
                return;

            Graphics g = e.Graphics;

            if (e.Item.Selected)
            {
                GraphicsPath p = RoundedRectangle.Create(4, 1, e.Item.Width - 8, e.Item.Height - 2);
                LinearGradientBrush b = new LinearGradientBrush(new Rectangle(4, 1, e.Item.Width - 8, e.Item.Height - 2), Color.White, Color.FromArgb(194, 211, 232), LinearGradientMode.Vertical);
                g.FillPath(b, p);
                g.DrawPath(new Pen(Brushes.Gainsboro, 1), p);
            }
        }

    }
}
