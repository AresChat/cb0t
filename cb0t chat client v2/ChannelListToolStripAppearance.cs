using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace cb0t_chat_client_v2
{
    class ChannelListToolStripAppearance : ToolStripRenderer
    {
        protected override void OnRenderToolStripBackground(ToolStripRenderEventArgs e)
        {
            Graphics g = e.Graphics;
            g.FillRectangle(new SolidBrush(SystemColors.Control), e.AffectedBounds);
        }

        protected override void OnRenderButtonBackground(ToolStripItemRenderEventArgs e)
        {
            if (e.Item.Selected)
            {
                GraphicsPath p = RoundedRectangle.Create(2, 2, e.Item.Width - 5, e.Item.Height - 5);
                e.Graphics.DrawPath(new Pen(Brushes.Gray, (float)0.5), p);
            }

            if (e.Item.Pressed)
            {
                GraphicsPath p = RoundedRectangle.Create(2, 2, e.Item.Width - 5, e.Item.Height - 5);
                e.Graphics.DrawPath(new Pen(Brushes.Silver, (float)0.5), p);
            }
        }
    }
}
