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

                        case '+':
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
