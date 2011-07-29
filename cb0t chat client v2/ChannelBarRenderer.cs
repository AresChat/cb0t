using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace cb0t_chat_client_v2
{
    class ChannelBarRenderer : ToolStripRenderer
    {
        public int selected_tab = -1;

        private Bitmap tab_close;
        private Bitmap tab_close_hidden;

        public ChannelBarRenderer()
        {
            if (AresImages.tab_close != null)
                this.tab_close = (Bitmap)AresImages.tab_close.Clone();

            if (AresImages.tab_close_hidden != null)
                this.tab_close_hidden = (Bitmap)AresImages.tab_close_hidden.Clone();
        }

        protected override void OnRenderToolStripBackground(ToolStripRenderEventArgs e)
        {
            for (int i = 0; i < e.ToolStrip.Height; i += 25)
            {
                Rectangle r = new Rectangle(e.AffectedBounds.X, e.AffectedBounds.Y + i, e.AffectedBounds.Width, 25);

                using (LinearGradientBrush brush1 = new LinearGradientBrush(r, Color.SteelBlue, Color.White, LinearGradientMode.Vertical))
                    e.Graphics.FillRectangle(brush1, r);

                using (LinearGradientBrush brush2 = new LinearGradientBrush(r, Color.White, Color.SteelBlue, LinearGradientMode.Vertical))
                    e.Graphics.FillRectangle(brush2, new Rectangle(r.X, r.Y + (r.Height / 2), r.Width, r.Height / 2));
            }

            for (int i = 0; i < e.ToolStrip.Height; i += 25)
                if (i > 0)
                    using (Pen pen = new Pen(Brushes.DimGray, 3))
                        e.Graphics.DrawLine(pen, new Point(e.AffectedBounds.X, e.AffectedBounds.Y + i - 1),
                            new Point(e.AffectedBounds.X + e.AffectedBounds.Width, e.AffectedBounds.Y + i - 1));

            using (Pen pen = new Pen(Brushes.Gray, 1))
                e.Graphics.DrawLine(pen, new Point(e.AffectedBounds.X, e.AffectedBounds.Y + (e.AffectedBounds.Height - 1)),
                    new Point(e.AffectedBounds.X + e.AffectedBounds.Width, e.AffectedBounds.Y + (e.AffectedBounds.Height - 1)));
        }

        protected override void OnRenderButtonBackground(ToolStripItemRenderEventArgs e)
        {
            int id = (int)e.Item.Tag;

            if (id == this.selected_tab)
            {
                using (GraphicsPath p = RoundedRectangle.Create(0, 0, e.Item.Width - 4, e.Item.Height - 1))
                {
                    using (SolidBrush brush = new SolidBrush(Color.AliceBlue))
                        e.Graphics.FillPath(brush, p);

                    e.Graphics.DrawPath(new Pen(Brushes.DarkOliveGreen, 1), p);
                }
            }
            else
            {
                if (e.Item.Selected) // hot tracking
                {
                    using (GraphicsPath p = RoundedRectangle.Create(0, 0, e.Item.Width - 4, e.Item.Height - 1))
                        e.Graphics.DrawPath(new Pen(Brushes.AliceBlue, (float)0.5), p);
                }
            }

            if (id > -1)
                if (e.Item.Selected)
                    e.Graphics.DrawImage(this.tab_close, new Point(e.Item.Width - 20, 3));
                else e.Graphics.DrawImage(this.tab_close_hidden, new Point(e.Item.Width - 20, 3));
        }

    }
}
