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
        private Font t_font = new Font("Tahoma", 10F, FontStyle.Regular, GraphicsUnit.Point, ((byte)(0)));
        private SolidBrush close_brush { get; set; }

        public Topic()
        {
            this.close_brush = new SolidBrush(Color.Red);
        }

        public String TopicText { get; set; }

        public void Free()
        {
            this.close_brush.Dispose();
            this.close_brush = null;
            this.t_font.Dispose();
            this.t_font = null;
        }

        protected override void OnRenderToolStripBackground(ToolStripRenderEventArgs e)
        {
            Rectangle bounds = new Rectangle(0, 0, e.ToolStrip.Width, e.ToolStrip.Height);

            using (LinearGradientBrush brush = new LinearGradientBrush(bounds, Color.Gainsboro, Color.WhiteSmoke, LinearGradientMode.Vertical))
                e.Graphics.FillRectangle(brush, bounds);

            if (e.ConnectedArea.Width > 0)
                return;

            // topic
            if (this.TopicText != null)
            {
                Rectangle r = new Rectangle(2, (e.ToolStrip.Height / 2) - 9, (e.ToolStrip.Width - 60), 20);
                char[] letters = this.TopicText.ToCharArray();
                Color fg = Color.Black;
                bool bold = false, italic = false, underline = false, back_color_required = false;
                int x = r.X, y = r.Y, max_x = (r.Width + 2), clen = 0, tcol = 0;

                for (int i = 0; i < letters.Length; i++)
                {
                    switch (letters[i])
                    {
                        case '\x0006':
                            bold = !bold;
                            break;

                        case '\x0007':
                            underline = !underline;
                            break;

                        case '\x0009':
                            italic = !italic;
                            break;

                        case '\x0003':
                            if (letters.Length >= (i + 3))
                            {
                                if (int.TryParse(this.TopicText.Substring(i + 1, 2), out tcol))
                                {
                                    fg = this.GetColorFromCode(tcol);
                                    i += 2;
                                }
                                else goto default;
                            }
                            else goto default;
                            break;

                        case '\x0005':
                            if (letters.Length >= (i + 3))
                            {
                                if (int.TryParse(this.TopicText.Substring(i + 1, 2), out tcol))
                                {
                                    Color back_color = this.GetColorFromCode(tcol);
                                    back_color_required = true;
                                    i += 2;

                                    if ((max_x - x) > 0)
                                        using (SolidBrush brush = new SolidBrush(back_color))
                                            e.Graphics.FillRectangle(brush, new Rectangle(x + 2, y, max_x - x, 20));
                                }
                                else goto default;
                            }
                            else goto default;
                            break;

                        case ' ':
                            x += underline ? 2 : (bold ? 4 : 3);

                            if (x >= max_x)
                                break;

                            if (underline)
                                using (Font font = new Font(this.t_font, this.CreateFont(bold, italic, underline)))
                                using (SolidBrush brush = new SolidBrush(fg))
                                    e.Graphics.DrawString(" ", font, brush, new PointF(x, (y + 1)));
                            break;

                        case '(':
                        case ':':
                        case ';':
                            Emotic emote_index = Emoticons.FindEmoticon(this.TopicText.Substring(i).ToUpper());

                            if (emote_index != null)
                            {
                                if ((x + 15) >= max_x)
                                {
                                    x += 15;
                                    break;
                                }

                                e.Graphics.DrawImage(Emoticons.emotic[emote_index.Index], new RectangleF(x + 3, y + 2, 16, 16));
                                x += 18;
                                i += (emote_index.Shortcut.Length - 1);
                                break;
                            }
                            else goto default;

                        default:
                            using (Font font = new Font(this.t_font, this.CreateFont(bold, italic, underline)))
                            {
                                clen = (int)Math.Round((double)e.Graphics.MeasureString(letters[i].ToString(), font, 100, StringFormat.GenericTypographic).Width);

                                if ((x + clen) >= max_x)
                                {
                                    x += clen;
                                    break;
                                }

                                using (SolidBrush brush = new SolidBrush(fg))
                                    e.Graphics.DrawString(letters[i].ToString(), font, brush, new PointF(x, (y + 1)));

                                x += clen;
                            }
                            break;
                    }

                    if (x >= max_x)
                        break;
                }

                if (back_color_required)
                    if ((max_x - x) > 0)
                    {
                        Rectangle excess = new Rectangle((x + 2), 0, (max_x - x), e.ToolStrip.Height);

                        using (LinearGradientBrush lb = new LinearGradientBrush(excess, Color.Gainsboro, Color.WhiteSmoke, LinearGradientMode.Vertical))
                            e.Graphics.FillRectangle(lb, excess);
                    }
            }

            // close
            if (this.close_hottracking)
            {
                Rectangle rec = new Rectangle(e.ToolStrip.Width - 19, 7, 12, 10);
                e.Graphics.FillRectangle(this.close_brush, rec);
            }
        }

        private FontStyle CreateFont(bool bold, bool italic, bool underline)
        {
            FontStyle f = bold ? FontStyle.Bold : FontStyle.Regular;

            if (italic)
                f |= FontStyle.Italic;

            if (underline)
                f |= FontStyle.Underline;

            return f;
        }

        private Color GetColorFromCode(int code)
        {
            switch (code)
            {
                case 1: return Color.Black;
                case 2: return Color.Navy;
                case 3: return Color.Green;
                case 4: return Color.Red;
                case 5: return Color.Maroon;
                case 6: return Color.Purple;
                case 7: return Color.Orange;
                case 8: return Color.Yellow;
                case 9: return Color.Lime;
                case 10: return Color.Teal;
                case 11: return Color.Aqua;
                case 12: return Color.Blue;
                case 13: return Color.Fuchsia;
                case 14: return Color.Gray;
                case 15: return Color.Silver;
            }

            return Color.White;
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
