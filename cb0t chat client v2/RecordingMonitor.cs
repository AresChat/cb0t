using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Windows.Forms;

namespace cb0t_chat_client_v2
{
    class RecordingMonitor : Panel
    {
        private bool black_background;
        private int tick;
        private Font f;
        private bool hq;

        public RecordingMonitor()
        {
            this.DoubleBuffered = true;
            this.black_background = Settings.black_background;
            this.f = new Font("Verdana", 8.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            this.tick = -1;
            this.hq = false;
        }

        public void SetTicks(int t, bool qual)
        {
            this.tick = t;
            this.hq = qual;
            this.Invalidate();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            try
            {
                e.Graphics.FillRectangle(this.black_background ? Brushes.Black : Brushes.White, new Rectangle(0, 0, e.ClipRectangle.Width, e.ClipRectangle.Height));

                if (this.tick > -1)
                {
                    e.Graphics.DrawImage(AresImages.GrayStar_NoFiles, new RectangleF(0, 0, 15, 15));

                    using (SolidBrush brush = new SolidBrush(this.black_background ? Color.White : Color.Black))
                        e.Graphics.DrawString("RECORDING [" + ((this.hq ? 15 : 25) - this.tick) + " seconds remaining]", this.f, brush, new PointF(16, 1));
                }
            }
            catch { }
        }

    }
}
