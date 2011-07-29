using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Windows.Forms;

namespace cb0t_chat_client_v2
{
    class WhoIsWriting : Panel
    {
        private bool black_background;
        private String _text;
        private ulong _latency;
        private Font f;
        private int tick;
        private bool hq;

        public WhoIsWriting()
        {
            this.DoubleBuffered = true;
            this.black_background = Settings.black_background;
            this.f = new Font("Verdana", 8.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            this._latency = 0;
            this.tick = -1;
            this.hq = false;
        }

        public void SetTicks(int t, bool qual)
        {
            this.tick = t;
            this.hq = qual;
            this.Invalidate();
        }

        public void SetText(String text)
        {
            if (!String.IsNullOrEmpty(text))
                this._text = "typing: " + text;
            else
                this._text = null;

            this.Invalidate();
        }

        public void SetLatency(ulong latency)
        {
            this._latency = latency;

            if (String.IsNullOrEmpty(this._text))
                this.Invalidate();
        }

        public void Fix()
        {
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
                else if (Settings.enable_whois_writing)
                {
                    if (!String.IsNullOrEmpty(this._text))
                    {
                        e.Graphics.DrawImage(AresImages.Writing, new RectangleF(0, 0, 15, 15));
                        int x_pos = 14;
                        char[] letters = this._text.ToCharArray();

                        for (int i = 0; i < letters.Length; i++)
                        {
                            if (letters[i] == ' ')
                                x_pos += 2;
                            else
                            {
                                int char_width = (int)Math.Round((double)e.Graphics.MeasureString(letters[i].ToString(), this.f, 100, StringFormat.GenericTypographic).Width);

                                if ((char_width + x_pos) > e.ClipRectangle.Width)
                                    break;

                                using (SolidBrush brush = new SolidBrush(this.black_background ? Color.White : Color.Black))
                                    e.Graphics.DrawString(letters[i].ToString(), this.f, brush, new PointF(x_pos, 1));

                                x_pos += char_width;
                            }
                        }
                    }
                    else if (this._latency > 0)
                    {
                        if (this._latency < 200)
                            e.Graphics.DrawImage(AresImages.RedStar_NoFiles, new RectangleF(0, 0, 18, 14));
                        else if (this._latency < 500)
                            e.Graphics.DrawImage(AresImages.RedStar_Files, new RectangleF(0, 0, 18, 14));
                        else if (this._latency < 1000)
                            e.Graphics.DrawImage(AresImages.BlueStar_NoFiles, new RectangleF(0, 0, 18, 14));
                        else if (this._latency < 2000)
                            e.Graphics.DrawImage(AresImages.BlueStar_Files, new RectangleF(0, 0, 18, 14));
                        else if (this._latency < 5000)
                            e.Graphics.DrawImage(AresImages.GreenStar_NoFiles, new RectangleF(0, 0, 18, 14));
                        else
                            e.Graphics.DrawImage(AresImages.GreenStar_Files, new RectangleF(0, 0, 18, 14));

                        using (SolidBrush brush = new SolidBrush(this.black_background ? Color.White : Color.Black))
                            e.Graphics.DrawString("lag: " + this._latency + " milliseconds", this.f, brush, new PointF(18, 1));
                    }
                }
            }
            catch { }
        }
    }
}
