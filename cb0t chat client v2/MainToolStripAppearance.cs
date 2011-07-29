using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace cb0t_chat_client_v2
{
    class MainToolStripAppearance : ToolStripRenderer
    {
        public enum SelectedTab : byte
        {
            Chat = 0,
            ControlPanel = 1,
            AudioPlayer = 2,
            IRC = 3
        };

        public SelectedTab Selected;

        public MainToolStripAppearance()
        {
            this.Selected = SelectedTab.Chat;
        }

        protected override void OnRenderToolStripBackground(ToolStripRenderEventArgs e)
        {
            using (LinearGradientBrush brush1 = new LinearGradientBrush(e.AffectedBounds, Color.SteelBlue, Color.White, LinearGradientMode.Vertical))
                e.Graphics.FillRectangle(brush1, e.AffectedBounds);

            using (LinearGradientBrush brush2 = new LinearGradientBrush(e.AffectedBounds, Color.White, Color.SteelBlue, LinearGradientMode.Vertical))
                e.Graphics.FillRectangle(brush2, new Rectangle(e.AffectedBounds.X, e.AffectedBounds.Y + (e.AffectedBounds.Height / 2),
                    e.AffectedBounds.Width, e.AffectedBounds.Height / 2));

            using (Pen pen = new Pen(Brushes.SteelBlue, 1))
                e.Graphics.DrawLine(pen, new Point(e.AffectedBounds.X, e.AffectedBounds.Y + e.AffectedBounds.Height - 1),
                    new Point(e.AffectedBounds.X + e.AffectedBounds.Width, e.AffectedBounds.Y + e.AffectedBounds.Height - 1));
        }

        protected override void OnRenderButtonBackground(ToolStripItemRenderEventArgs e)
        {
            bool currently_selected = ((this.Selected == SelectedTab.Chat && e.Item.Text == "Chat") ||
                (this.Selected == SelectedTab.ControlPanel && e.Item.Text == "Control Panel") ||
                (this.Selected == SelectedTab.AudioPlayer && e.Item.Text == "Audio Player"));

            if (!currently_selected)
            {
                if (e.Item.Selected)
                {
                    using (GraphicsPath p = RoundedRectangle.Create(2, 2, e.Item.Width - 5, e.Item.Height - 5))
                        e.Graphics.DrawPath(new Pen(Brushes.AliceBlue, (float)0.5), p);
                }
            }
            else
            {
                using (GraphicsPath p = RoundedRectangle.Create(2, 2, e.Item.Width - 5, e.Item.Height - 5))
                {
                    using (SolidBrush brush = new SolidBrush(Color.AliceBlue))
                        e.Graphics.FillPath(brush, p);

                    using (Pen pen = new Pen(Brushes.DarkOliveGreen, 1))
                        e.Graphics.DrawPath(pen, p);
                }
            }
        }
    }
}
