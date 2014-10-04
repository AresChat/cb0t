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

            if (e.ToolStrip is ToolStripDropDownMenu)
                return;

            // topic
            if (this.TopicText != null)
            {
                Rectangle r = new Rectangle(2, (e.ToolStrip.Height / 2) - 9, (e.ToolStrip.Width - 60), 20);
                char[] letters = this.TopicText.ToCharArray();
                Color fg = Color.Black;
                bool bold = false, italic = false, underline = false, back_color_required = false;
                int x = r.X, y = r.Y, max_x = (r.Width + 2), clen = 0, tcol = 0;
                EmojiItem emoji_item;

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

                        case (char)35:
                        case (char)48:
                        case (char)49:
                        case (char)50:
                        case (char)51:
                        case (char)52:
                        case (char)53:
                        case (char)54:
                        case (char)55:
                        case (char)56:
                        case (char)57:
                        case (char)169:
                        case (char)174:
                        case (char)55356:
                        case (char)55357:
                        case (char)8252:
                        case (char)8265:
                        case (char)8482:
                        case (char)8505:
                        case (char)8596:
                        case (char)8597:
                        case (char)8598:
                        case (char)8599:
                        case (char)8600:
                        case (char)8601:
                        case (char)8617:
                        case (char)8618:
                        case (char)8986:
                        case (char)8987:
                        case (char)9193:
                        case (char)9194:
                        case (char)9195:
                        case (char)9196:
                        case (char)9200:
                        case (char)9203:
                        case (char)9410:
                        case (char)9642:
                        case (char)9643:
                        case (char)9654:
                        case (char)9664:
                        case (char)9723:
                        case (char)9724:
                        case (char)9725:
                        case (char)9726:
                        case (char)9728:
                        case (char)9729:
                        case (char)9742:
                        case (char)9745:
                        case (char)9748:
                        case (char)9749:
                        case (char)9757:
                        case (char)9786:
                        case (char)9800:
                        case (char)9801:
                        case (char)9802:
                        case (char)9803:
                        case (char)9804:
                        case (char)9805:
                        case (char)9806:
                        case (char)9807:
                        case (char)9808:
                        case (char)9809:
                        case (char)9810:
                        case (char)9811:
                        case (char)9824:
                        case (char)9827:
                        case (char)9829:
                        case (char)9830:
                        case (char)9832:
                        case (char)9851:
                        case (char)9855:
                        case (char)9875:
                        case (char)9888:
                        case (char)9889:
                        case (char)9898:
                        case (char)9899:
                        case (char)9917:
                        case (char)9918:
                        case (char)9924:
                        case (char)9925:
                        case (char)9934:
                        case (char)9940:
                        case (char)9962:
                        case (char)9970:
                        case (char)9971:
                        case (char)9973:
                        case (char)9978:
                        case (char)9981:
                        case (char)9986:
                        case (char)9989:
                        case (char)9992:
                        case (char)9993:
                        case (char)9994:
                        case (char)9995:
                        case (char)9996:
                        case (char)9999:
                        case (char)10002:
                        case (char)10004:
                        case (char)10006:
                        case (char)10024:
                        case (char)10035:
                        case (char)10036:
                        case (char)10052:
                        case (char)10055:
                        case (char)10060:
                        case (char)10062:
                        case (char)10067:
                        case (char)10068:
                        case (char)10069:
                        case (char)10071:
                        case (char)10084:
                        case (char)10133:
                        case (char)10134:
                        case (char)10135:
                        case (char)10145:
                        case (char)10160:
                        case (char)10175:
                        case (char)10548:
                        case (char)10549:
                        case (char)11013:
                        case (char)11014:
                        case (char)11015:
                        case (char)11035:
                        case (char)11036:
                        case (char)11088:
                        case (char)11093:
                        case (char)12336:
                        case (char)12349:
                        case (char)12951:
                        case (char)12953:
                            emoji_item = Emoji.GetEmoji24(letters, i);

                            if (emoji_item != null)
                            {
                                if ((x + 15) >= max_x)
                                {
                                    x += 15;
                                    break;
                                }

                                e.Graphics.DrawImage(emoji_item.Image, new RectangleF(x + 3, y + 2, 16, 16));
                                x += 18;
                                i += (emoji_item.Length - 1);
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
