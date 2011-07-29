using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace cb0t_chat_client_v2
{
    class UserlistItem : ListViewItem
    {
        public String UserName { get; private set; }

        public UserlistItem(String username)
        {
            this.UserName = username;
        }

        public void PaintItem(UserObject userobj, DrawListViewSubItemEventArgs e, int CurrentHover, bool black_background, ref Bitmap def_av)
        {
            int x = 1;
            int y = 1;

            if (userobj.avatar != null)
                e.Graphics.DrawImage(userobj.avatar, new Point(e.Bounds.X + 1, e.Bounds.Y + 1));
            else
                e.Graphics.DrawImage(def_av, new Point(e.Bounds.X + 1, e.Bounds.Y + 1));

            x += 56;

            if (userobj.writing)
                e.Graphics.DrawImage(AresImages.Writing, new Point(e.Bounds.X + x, e.Bounds.Y + 6));

            String str = "\x0006";

            switch (userobj.level)
            {
                case 0:
                    str += black_background ? "\x000300" : "\x000301";
                    break;

                case 1:
                    str += black_background ? "\x000311" : "\x000312";
                    break;

                case 2:
                    str += black_background ? "\x000309" : "\x000303";
                    break;

                case 3:
                    str += "\x000304";
                    break;
            }

            str += Helpers.StripColors(userobj.name);

            if (!String.IsNullOrEmpty(userobj.personal_message))
            {
                str += "\x0006 " + (black_background ? "\x000315- " : "\x000314- ");

                if (userobj.is_song) // song
                    str += "\x0001\x0007\x000312" + Helpers.StripColors(userobj.personal_message);
                else
                    str += userobj.personal_message;
            }

            this.PaintText(str, userobj.writing ? (x + 18) : x, y, e, e.Item.Font, CurrentHover, black_background);

            using (Font f = new Font("Verdana", 7F, FontStyle.Regular, GraphicsUnit.Point, 0))
            {
                y += 15;
                this.PaintText((black_background ? "\x000300" : "") + "\x0009Files: " + userobj.files, userobj.writing ? (x + 18) : x, y, e, f, CurrentHover, black_background);
                y += 13;
                this.PaintText((black_background ? "\x000300" : "") + "\x0009ASL: " + userobj.ToASLString(), x, y, e, f, CurrentHover, black_background);

                if (userobj.status != Settings.OnlineStatus.online)
                {
                    y += 13;

                    switch (userobj.status)
                    {
                        case Settings.OnlineStatus.be_back_later:
                            e.Graphics.DrawImage(AresImages.mini_away, new Point(e.Bounds.X + x + 2, e.Bounds.Y + y + 1));
                            this.PaintText((black_background ? "\x000300" : "") + "\x0009Away", x + 14, y, e, f, CurrentHover, black_background);
                            break;

                        case Settings.OnlineStatus.do_not_disturb:
                            e.Graphics.DrawImage(AresImages.mini_busy, new Point(e.Bounds.X + x + 2, e.Bounds.Y + y + 1));
                            this.PaintText((black_background ? "\x000300" : "") + "\x0009Busy", x + 14, y, e, f, CurrentHover, black_background);
                            break;

                        case Settings.OnlineStatus.sleeping:
                            e.Graphics.DrawImage(AresImages.mini_sleep, new Point(e.Bounds.X + x + 2, e.Bounds.Y + y + 1));
                            this.PaintText((black_background ? "\x000300" : "") + "\x0009Sleeping", x + 14, y, e, f, CurrentHover, black_background);
                            break;
                    }
                }
                else if (userobj.can_vc_public)
                {
                    y += 13;
                    e.Graphics.DrawImage(AresImages.mini_ignore, new Point(e.Bounds.X + x + 2, e.Bounds.Y + y + 1));
                    this.PaintText((black_background ? "\x000300" : "") + "\x0009Voice enabled", x + 14, y, e, f, CurrentHover, black_background);
                }
            }
        }

        private void PaintText(String text, int start_x, int start_y, DrawListViewSubItemEventArgs e, Font orgFont, int CurrentHover, bool blackground)
        {
            char[] letters = text.ToCharArray();

            Color org_back_color = e.Item.Selected ? Color.CornflowerBlue : (e.Item.Index == CurrentHover ? Color.Silver : Color.White);

            if (blackground)
                org_back_color = e.Item.Selected ? Color.CornflowerBlue : (e.Item.Index == CurrentHover ? Color.DimGray : Color.Black);

            Color fore_color = Color.Black;
            bool bold = false, italic = false, underline = false, back_color_required = false;
            int x = start_x;
            int color_finder;

            for (int i = 0; i < letters.Length; i++)
            {
                switch (letters[i])
                {
                    case '\x0006': // bold
                        bold = !bold;
                        break;

                    case '\x0007': // underline
                        underline = !underline;
                        break;

                    case '\x0009': // italic
                        italic = !italic;
                        break;

                    case '\x0003': // fore color
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

                    case '\x0005': // back color
                        if (letters.Length >= (i + 3))
                        {
                            if (int.TryParse(text.Substring(i + 1, 2), out color_finder))
                            {
                                Color back_color = this.GetColorFromCode(color_finder);
                                back_color_required = true;
                                i += 2;

                                using (SolidBrush brush = new SolidBrush(back_color))
                                    e.Graphics.FillRectangle(brush, new Rectangle(e.SubItem.Bounds.X + x + 2, e.SubItem.Bounds.Y + start_y, e.SubItem.Bounds.Width - x - 2, orgFont.Size == 7 ? 12 : 15));
                            }
                            else goto default;
                        }
                        else goto default;
                        break;

                    case ' ': // space
                        x += underline ? 1 : (bold ? 3 : 2);

                        if (x > (e.SubItem.Bounds.Width - 1))
                            break;

                        if (underline)
                            using (Font font = new Font(orgFont, this.CreateFont(bold, italic, underline)))
                            using (SolidBrush brush = new SolidBrush(fore_color))
                                e.Graphics.DrawString(" ", font, brush, new PointF(e.SubItem.Bounds.X + x, e.SubItem.Bounds.Y + start_y));
                        break;

                    case '+':
                    case '(':
                    case ':':
                    case ';': // emoticons
                        int emote_index = EmoticonFinder.GetRTFEmoticonFromKeyboardShortcut(text.Substring(i).ToUpper());

                        if (emote_index > -1)
                        {
                            if ((x + 15) > (e.SubItem.Bounds.Width - 1))
                            {
                                x += 15;
                                break;
                            }

                            e.Graphics.DrawImage(AresImages.TransparentEmoticons[emote_index], new RectangleF(e.SubItem.Bounds.X + x + 2,
                                e.SubItem.Bounds.Y + start_y, orgFont.Size == 7 ? 12 : 15, orgFont.Size == 7 ? 12 : 15));
                            x += orgFont.Size == 7 ? 12 : 15;
                            i += (EmoticonFinder.last_emote_length - 1);
                            break;
                        }
                        else goto default;

                    case '\x0001':
                        e.Graphics.DrawImage(AudioControllerImages.np_note, new RectangleF(e.SubItem.Bounds.X + x + 2, e.SubItem.Bounds.Y + start_y, 12, 12));
                        x += 14;
                        break;

                    default: // text
                        using (Font font = new Font(orgFont, this.CreateFont(bold, italic, underline)))
                        {
                            int width = (int)Math.Round((double)e.Graphics.MeasureString(letters[i].ToString(), font, 100, StringFormat.GenericTypographic).Width);

                            if ((x + width) > (e.SubItem.Bounds.Width - 1))
                            {
                                x += width;
                                break;
                            }

                            using (SolidBrush brush = new SolidBrush(fore_color))
                                e.Graphics.DrawString(letters[i].ToString(), font, brush, new PointF(e.SubItem.Bounds.X + x, e.SubItem.Bounds.Y + start_y));

                            x += width;
                        }
                        break;
                }

                if (x > (e.SubItem.Bounds.Width - 1)) // run out of space - stop drawing!!
                    return;
            }

            if (back_color_required) // trim excess background because the topic is shorter than the column width
                if ((x + 2) < e.SubItem.Bounds.Width)
                    using (SolidBrush brush = new SolidBrush(org_back_color))
                        e.Graphics.FillRectangle(brush, new Rectangle(e.SubItem.Bounds.X + x + 2, e.SubItem.Bounds.Y + start_y, e.SubItem.Bounds.Width - x - 2, orgFont.Size == 7 ? 12 : 15));
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
