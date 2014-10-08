using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace cb0t
{
    class UserListBoxItem
    {
        public User Owner { get; private set; }

        public UserListBoxItem(User owner)
        {
            this.Owner = owner;
        }

        public void Draw(DrawItemEventArgs e, bool selected, bool tracked, bool is_black)
        {
            Color item_bg = is_black ? Color.Black : Color.White;

            if (tracked)
                item_bg = is_black ? Color.DimGray : Color.Gainsboro;
            if (selected)
                item_bg = is_black ? Color.DarkBlue : Color.LightSteelBlue;

            using (SolidBrush brush = new SolidBrush(item_bg))
                e.Graphics.FillRectangle(brush, e.Bounds);

            if (this.Owner.AvatarBytes.Length > 0)
            {
                using (MemoryStream ms = new MemoryStream(this.Owner.AvatarBytes))
                using (Bitmap bmp = new Bitmap(ms))
                    e.Graphics.DrawImage(bmp, new Point(e.Bounds.X + 1, e.Bounds.Y + 2));
            }
            else
            {
                using (MemoryStream ms = new MemoryStream(Avatar.DefaultAvatar))
                using (Bitmap bmp = new Bitmap(ms))
                    e.Graphics.DrawImage(bmp, new Point(e.Bounds.X + 1, e.Bounds.Y + 2));
            }

            int img_x = 58;

            Color name_color = is_black ? Color.White : Color.Black;

            if (this.Owner.Level == 3)
                name_color = Color.Red;
            else if (this.Owner.Level == 2)
                name_color = is_black ? Color.Lime : Color.Green;
            else if (this.Owner.Level == 1)
                name_color = is_black ? Color.Aqua : Color.Blue;

            if (this.Owner.HasFiles)
            {
                using (MemoryStream ms = new MemoryStream(Avatar.BrowseIcon))
                using (Bitmap bmp = new Bitmap(ms))
                    e.Graphics.DrawImage(bmp, new Rectangle(e.Bounds.X + img_x, e.Bounds.Y + 7, 14, 14));

                img_x += 15;
            }

            if (this.Owner.IsAway)
            {
                using (MemoryStream ms = new MemoryStream(Avatar.AwayIcon))
                using (Bitmap bmp = new Bitmap(ms))
                    e.Graphics.DrawImage(bmp, new Rectangle(e.Bounds.X + img_x, e.Bounds.Y + 7, 14, 14));

                img_x += 15;
            }

            if (this.Owner.SupportsVC)
            {
                using (MemoryStream ms = new MemoryStream(is_black ? Avatar.VoiceIconBlack : Avatar.VoiceIcon))
                using (Bitmap bmp = new Bitmap(ms))
                    e.Graphics.DrawImage(bmp, new Rectangle(e.Bounds.X + img_x, e.Bounds.Y + 7, 14, 14));

                img_x += 15;
            }

            using (Font name_font = new Font(e.Font, FontStyle.Bold))
            using (SolidBrush brush = new SolidBrush(name_color))
            {
                char[] n_letters = this.Owner.Name.ToCharArray();
                int n_x_pos = e.Bounds.X + img_x;
                int n_y_pos = e.Bounds.Y + 7;
                int n_max_width = (n_x_pos + e.Bounds.Width);

                for (int i = 0; i < n_letters.Length; i++)
                {
                    switch (n_letters[i])
                    {
                        case ' ':
                            n_x_pos += 4;
                            break;

                        default:
                            try
                            {
                                int w = (int)Math.Round((double)e.Graphics.MeasureString(n_letters[i].ToString(), name_font, 100, StringFormat.GenericTypographic).Width);

                                if ((n_x_pos + w) < n_max_width)
                                    e.Graphics.DrawString(n_letters[i].ToString(), name_font, brush, new PointF(n_x_pos, n_y_pos));

                                n_x_pos += w;
                            }
                            catch { break; }
                            break;
                    }

                    if (n_x_pos >= n_max_width)
                        break;
                }
            }

            try
            {
                using (SolidBrush brush = new SolidBrush(is_black ? Color.WhiteSmoke : Color.Gray))
                    e.Graphics.DrawString(this.Owner.ToASLString(), e.Font, brush, new PointF(e.Bounds.X + 58, e.Bounds.Y + 24));
            }
            catch { }

            String text = this.Owner.PersonalMessage;
            bool is_song = false;

            if (text.StartsWith("\x0007"))
            {
                text = Helpers.StripColors(text);

                if (is_black)
                    text = "\x0007\x000311" + text;
                else
                    text = "\x0007\x000312" + text;

                is_song = true;
            }

            if (!String.IsNullOrEmpty(text))
            {
                char[] letters = text.ToCharArray();
                bool bold = false, italic = false, underline = false;
                int x = e.Bounds.X + 58, y = e.Bounds.Y + 38;
                int max_width = (e.Bounds.X + e.Bounds.Width);
                Color fore_color = is_black ? Color.WhiteSmoke : Color.Gray;
                int color_finder;
                EmojiItem emojiitem;

                if (is_song)
                {
                    using (MemoryStream ms = new MemoryStream(is_black ? Avatar.MusicIconBlack : Avatar.MusicIcon))
                    using (Bitmap bmp = new Bitmap(ms))
                        e.Graphics.DrawImage(bmp, new RectangleF(x, y, 14, 14));

                    x += 15;
                }

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
                                if (int.TryParse(text.Substring(i + 1, 2), out color_finder))
                                {
                                    fore_color = this.GetColorFromCode(color_finder);
                                    i += 2;
                                }
                                else goto default;
                            else goto default;
                            break;

                        case '\x0005':
                            if (letters.Length >= (i + 3))
                                if (int.TryParse(text.Substring(i + 1, 2), out color_finder))
                                    i += 2;
                                else goto default;
                            else goto default;
                            break;

                        case ' ':
                            x += underline ? 2 : (bold ? 4 : 3);
                            break;

                        case '(':
                        case ':':
                        case ';':
                            Emotic em = Emoticons.FindEmoticon(text.Substring(i).ToUpper());

                            if (em != null && !is_song)
                            {
                                if ((x + 15) < max_width)
                                    e.Graphics.DrawImage(Emoticons.emotic[em.Index], new RectangleF(x + 3, y, 14, 14));

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
                            emojiitem = Emoji.GetEmoji24(letters, i);

                            if (emojiitem != null && !is_song)
                            {
                                if ((x + 15) < max_width)
                                    e.Graphics.DrawImage(emojiitem.Image, new RectangleF(x + 3, y, 14, 14));

                                x += 15;
                                i += (emojiitem.Length - 1);
                                break;
                            }
                            else goto default;

                        default:
                            using (Font f = new Font(e.Font, this.CreateFont(bold, italic, underline)))
                            using (SolidBrush brush = new SolidBrush(fore_color))
                            {
                                try
                                {
                                    int w = (int)Math.Round((double)e.Graphics.MeasureString(letters[i].ToString(), f, 100, StringFormat.GenericTypographic).Width);

                                    if ((x + w) < max_width)
                                        e.Graphics.DrawString(letters[i].ToString(), f, brush, new PointF(x, y));

                                    x += w;
                                }
                                catch { break; }
                            }
                            break;
                    }

                    if (x >= max_width)
                        break;
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
