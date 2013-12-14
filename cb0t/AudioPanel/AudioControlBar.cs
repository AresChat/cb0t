using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace cb0t
{
    class AudioControlBar : ToolStripRenderer
    {
        private Color bg1 = Color.FromArgb(238, 241, 224);
        private Color bg2 = Color.FromArgb(223, 227, 202);
        private Pen bgp = new Pen(Color.Gray);
        private SolidBrush npt = new SolidBrush(Color.FromArgb(28, 28, 28));

        public String DisplayText { get; set; }

        public AudioControlBar()
        {
            this.DisplayText = String.Empty;
        }

        protected override void OnRenderToolStripBackground(ToolStripRenderEventArgs e)
        {
            Rectangle bounds = new Rectangle(0, 0, e.ToolStrip.Width, e.ToolStrip.Height);

            if (e.ToolStrip is ToolStripDropDownMenu)
            {
                using (LinearGradientBrush brush = new LinearGradientBrush(bounds, Color.WhiteSmoke, Color.Gainsboro, LinearGradientMode.Vertical))
                    e.Graphics.FillRectangle(brush, bounds);

                return;
            }
            else
            {
                if (Aero.CanAero)
                    e.Graphics.Clear(Color.Transparent);
                else
                {
                    using (LinearGradientBrush brush = new LinearGradientBrush(bounds, Color.Gainsboro, Color.Silver, LinearGradientMode.Vertical))
                        e.Graphics.FillRectangle(brush, bounds);
                }
            }

            bounds = new Rectangle(155, 3, e.ToolStrip.Width - 156, e.ToolStrip.Height - 6);

            if (bounds.Width <= 0)
                return;

            using (GraphicsPath path = bounds.Rounded(2))
            {
                using (LinearGradientBrush brush = new LinearGradientBrush(bounds, this.bg1, this.bg2, LinearGradientMode.Vertical))
                    e.Graphics.FillPath(brush, path);

                e.Graphics.DrawPath(this.bgp, path);
            }

            int max_width = bounds.Width - 20;
            int x = 10;
            char[] letters = this.DisplayText.ToCharArray();
            int eval;

            foreach (char c in letters)
            {
                if (c == ' ')
                {
                    x += 3;
                    continue;
                }

                eval = (int)Math.Round(e.Graphics.MeasureString(c.ToString(), e.ToolStrip.Font, 32, StringFormat.GenericTypographic).Width);

                if ((x + eval) >= max_width)
                    break;

                e.Graphics.DrawString(c.ToString(), e.ToolStrip.Font, this.npt, new PointF(x + bounds.X, 5));
                x += eval;
            }
        }

        protected override void OnRenderButtonBackground(ToolStripItemRenderEventArgs e)
        {
            if (e.Item.Selected || e.Item.Pressed)
            {
                using (GraphicsPath path = new Rectangle(1, 1, e.Item.Width - 3, e.Item.Height - 3).Rounded(2))
                using (Pen pen = new Pen(Color.Black))
                    e.Graphics.DrawPath(pen, path);
            }
        }

        protected override void OnRenderDropDownButtonBackground(ToolStripItemRenderEventArgs e)
        {
            if (e.Item.Selected || e.Item.Pressed)
            {
                using (GraphicsPath path = new Rectangle(1, 1, e.Item.Width - 3, e.Item.Height - 3).Rounded(2))
                using (Pen pen = new Pen(Color.Black))
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
