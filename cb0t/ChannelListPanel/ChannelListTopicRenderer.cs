using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace cb0t
{
    class ChannelListTopicRenderer
    {
        private Font t_font = new Font("Tahoma", 9F, FontStyle.Regular, GraphicsUnit.Point, ((byte)(0)));

        public void RenderChannelListItem(ChannelListViewItem item, ChannelListItem org)
        {
            this.RenderChannelListItem(item, org.ToFavouritesItem());
        }

        public void RenderChannelListItem(ChannelListViewItem item, FavouritesListItem org)
        {
            using (Bitmap bmp = new Bitmap(154, 15))
            using (Graphics g = Graphics.FromImage(bmp))
            {
                g.Clear(Color.White);
                
                String text = org.Name;
                char[] letters = text.ToCharArray();
                int x = 0;

                for (var i = 0; i < letters.Length; i++)
                {
                    switch (letters[i])
                    {
                        case ' ':
                            x += 3;
                            break;

                        default:
                            int w = (int)Math.Round((double)g.MeasureString(letters[i].ToString(), this.t_font, 100, StringFormat.GenericTypographic).Width);

                            if ((w + x) < 154)
                                g.DrawString(letters[i].ToString(), this.t_font, Brushes.Black, new PointF(x, 0));

                            x += w;
                            break;
                    }

                    if (x >= 154)
                        break;
                }

                item.NameImg = new Bitmap(x + 2, 15);

                using (Graphics g2 = Graphics.FromImage(item.NameImg))
                {
                    g2.Clear(Color.Transparent);
                    g2.DrawImage(bmp, new Point(0, 0));
                }
            }

            using (Bitmap bmp = new Bitmap(35, 15))
            using (Graphics g = Graphics.FromImage(bmp))
            {
                g.Clear(Color.White);

                String text = org.CountString;

                if (String.IsNullOrEmpty(text))
                    text = String.Empty;

                char[] letters = text.ToCharArray();
                int x = 0;

                for (var i = 0; i < letters.Length; i++)
                {
                    switch (letters[i])
                    {
                        case ' ':
                            x += 3;
                            break;

                        default:
                            int w = (int)Math.Round((double)g.MeasureString(letters[i].ToString(), this.t_font, 100, StringFormat.GenericTypographic).Width);

                            if ((w + x) < 35)
                                g.DrawString(letters[i].ToString(), this.t_font, Brushes.Black, new PointF(x, 0));

                            x += w;
                            break;
                    }

                    if (x >= 35)
                        break;
                }

                item.CountImg = new Bitmap(x + 2, 15);

                using (Graphics g2 = Graphics.FromImage(item.CountImg))
                {
                    g2.Clear(Color.Transparent);
                    g2.DrawImage(bmp, new Point(0, 0));
                }
            }

            using (Bitmap bmp = new Bitmap(1024, 15))
            using (Graphics g = Graphics.FromImage(bmp))
            {
                g.Clear(Color.White);
                g.CompositingQuality = CompositingQuality.HighQuality;
                g.InterpolationMode = InterpolationMode.HighQualityBilinear;

                String text = org.Topic;
                char[] letters = text.ToCharArray();
                int x = 0;
                bool bold = false, italic = false, underline = false;
                Color fore_color = Color.Black;
                int color_finder;
                EmojiItem emoji_item;

                for (var i = 0; i < letters.Length; i++)
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
                                if (int.TryParse(text.Substring(i + 1, 2), out color_finder))
                                {
                                    fore_color = this.GetColorFromCode(color_finder);
                                    i += 2;
                                }
                                else goto default;
                            }
                            else goto default;
                            break;

                        case '\x0005':
                            if (letters.Length >= (i + 3))
                            {
                                if (int.TryParse(text.Substring(i + 1, 2), out color_finder))
                                {
                                    Color back_color = this.GetColorFromCode(color_finder);
                                    i += 2;

                                    using (SolidBrush brush = new SolidBrush(back_color))
                                        g.FillRectangle(brush, new Rectangle(x + 2, 0, 1024 - x - 2, 15));
                                }
                                else goto default;
                            }
                            else goto default;
                            break;

                        case ' ':
                            x += underline ? 2 : (bold ? 4 : 3);

                            if (underline)
                                using (Font font = new Font(this.t_font, this.CreateFont(bold, italic, underline)))
                                using (SolidBrush brush = new SolidBrush(fore_color))
                                    g.DrawString(" ", font, brush, new PointF(x, 0));
                            break;

                        case '(':
                        case ':':
                        case ';':
                            Emotic em = Emoticons.FindEmoticon(text.Substring(i).ToUpper());

                            if (em != null)
                            {
                                g.DrawImage(Emoticons.emotic[em.Index], new RectangleF(x + 2, 0, 15, 15));
                                x += 15;
                                i += (em.Shortcut.Length - 1);
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
                                g.DrawImage(emoji_item.Image, new RectangleF(x + 2, 0, 15, 15));
                                x += 15;
                                i += (emoji_item.Length - 1);
                                break;
                            }
                            else goto default;

                        default:
                            using (Font f = new Font(this.t_font, this.CreateFont(bold, italic, underline)))
                            using (SolidBrush brush = new SolidBrush(fore_color))
                            {
                                int w = (int)Math.Round((double)g.MeasureString(letters[i].ToString(), f, 100, StringFormat.GenericTypographic).Width);
                                g.DrawString(letters[i].ToString(), f, brush, new PointF(x, 0));
                                x += w;
                            }
                            break;
                    }
                }

                item.TopicImg = new Bitmap(x + 2, 15);

                using (Graphics g2 = Graphics.FromImage(item.TopicImg))
                {
                    g2.Clear(Color.Transparent);
                    g2.DrawImage(bmp, new Point(0, 0));
                }
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
    }
}
