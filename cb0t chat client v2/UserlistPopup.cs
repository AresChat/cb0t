using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Windows.Forms;

namespace cb0t_chat_client_v2
{
    class UserlistPopup : ToolTip
    {
        private UserObject user;
        private Font f;

        private int name_width = 0;
        private int pm_width = 0;
        private int asl_width = 0;
        private int ip_width = 0;

        public UserObject User
        {
            get { return this.user; }
            set { this.user = value; }
        }

        public bool IsUser(UserObject userobj)
        {
            if (this.user == null)
                return false;

            if (this.user.name == userobj.name)
                return true;
            
            return false;
        }

        public void Reset()
        {
            this.user = null;
            this.name_width = 0;
            this.pm_width = 0;
            this.asl_width = 0;
            this.ip_width = 0;
        }

        public UserlistPopup()
        {
            this.f = new Font("Verdana", 8.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            this.Popup += new PopupEventHandler(this.OnMeasure);
            this.Draw += new DrawToolTipEventHandler(this.OnPaint);
            this.OwnerDraw = true;
        }

        private void OnPaint(object sender, DrawToolTipEventArgs e)
        {
            // text is 15 pixels high

            using (SolidBrush sb = new SolidBrush(Color.Silver))
                e.Graphics.FillRectangle(sb, e.Bounds);

            for (int i = 0; i < 8; i++)
            {
                using (SolidBrush sb = new SolidBrush(Color.FromArgb((i+3) * 20, (i+3) * 20, (i+3) * 20)))
                using (Pen pen = new Pen(sb, 1))
                    e.Graphics.DrawRectangle(pen, new Rectangle(e.Bounds.X + i, e.Bounds.Y + i, e.Bounds.Width - ((i * 2) + 1), e.Bounds.Height - ((i * 2) + 1)));
            }

            // start at 10 by 10

            if (this.user.avatar != null)
            {
                using (Bitmap b = new Bitmap(80, 80))
                {
                    using (Graphics g = Graphics.FromImage(b))
                    {
                        g.SmoothingMode = SmoothingMode.AntiAlias;
                        g.CompositingQuality = CompositingQuality.HighQuality;
                        g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                        g.DrawImage(this.user.avatar, new RectangleF(0, 0, 80, 80));
                        e.Graphics.DrawImage(b, new Point(e.Bounds.X + 10, e.Bounds.Y + 10));

                        using (Pen pen = new Pen(Brushes.Black, 1))
                            e.Graphics.DrawRectangle(pen, new Rectangle(e.Bounds.X + 10, e.Bounds.Y + 10, 79, 79));
                    }
                }
            }
            else
            {
                using (Pen pen = new Pen(Brushes.Black, 1))
                    e.Graphics.DrawRectangle(pen, new Rectangle(e.Bounds.X + 10, e.Bounds.Y + 10, 79, 79));
            }

            int y_pos = 15;

            if (this.name_width > 0)
            {
                String make_name = String.Empty;

                switch (this.user.level)
                {
                    case 0:
                        make_name += "\x000301";
                        break;

                    case 1:
                        make_name += "\x000312";
                        break;

                    case 2:
                        make_name += "\x000303";
                        break;

                    case 3:
                        make_name += "\x000304";
                        break;
                }

                this.PaintText("\x0006" + make_name + this.user.name, 95, y_pos, this.name_width, e.Graphics);
                y_pos += 20;
            }

            if (this.asl_width > 0)
            {
                this.PaintText(this.user.ToASLString(), 95, y_pos, this.asl_width, e.Graphics);
                y_pos += 16;
            }

            if (this.ip_width > 0)
            {
                switch (this.user.status)
                {
                    case Settings.OnlineStatus.be_back_later:
                        e.Graphics.DrawImage(AresImages.mini_away, new Point(97, y_pos + 3));
                        this.PaintText("Away", 108, y_pos, this.ip_width, e.Graphics);
                        break;

                    case Settings.OnlineStatus.do_not_disturb:
                        e.Graphics.DrawImage(AresImages.mini_busy, new Point(97, y_pos + 3));
                        this.PaintText("Busy", 108, y_pos, this.ip_width, e.Graphics);
                        break;

                    case Settings.OnlineStatus.sleeping:
                        e.Graphics.DrawImage(AresImages.mini_sleep, new Point(97, y_pos + 3));
                        this.PaintText("Sleeping", 108, y_pos, this.ip_width, e.Graphics);
                        break;

                    default:
                        e.Graphics.DrawImage(AresImages.mini_ignore, new Point(97, y_pos + 3));
                        this.PaintText("Voice enabled", 108, y_pos, this.ip_width, e.Graphics);
                        break;
                }

                y_pos += 16;
            }

            if (this.pm_width > 0)
            {
                if (this.user.is_song)
                    this.PaintText("\x0007\x000312" + Helpers.StripColors(this.user.personal_message), 95, y_pos, this.pm_width, e.Graphics);
                else
                    this.PaintText(this.user.personal_message, 95, y_pos, this.pm_width, e.Graphics);
            }
        }

        private void OnMeasure(object sender, PopupEventArgs e)
        {
            if (this.user == null)
            {
                e.Cancel = true;
                return;
            }

            int largest_width = 0;

            this.name_width = this.MeasureText("\x0006" + this.user.name);

            if (this.name_width > largest_width)
                largest_width = this.name_width;

            if (this.user.is_song)
                this.pm_width = this.MeasureText("\x0007" + Helpers.StripColors(this.user.personal_message));
            else
                this.pm_width = this.MeasureText(this.user.personal_message);

            if (this.pm_width > largest_width)
                largest_width = this.pm_width;

            this.asl_width = this.MeasureText(this.user.ToASLString());

            if (this.asl_width > largest_width)
                largest_width = this.asl_width;

            switch (this.user.status)
            {
                case Settings.OnlineStatus.be_back_later:
                    this.ip_width = this.MeasureText("Away") + 12;
                    break;

                case Settings.OnlineStatus.do_not_disturb:
                    this.ip_width = this.MeasureText("Busy") + 12;
                    break;

                case Settings.OnlineStatus.sleeping:
                    this.ip_width = this.MeasureText("Sleeping") + 12;
                    break;

                case Settings.OnlineStatus.online:
                    if (!this.user.can_vc_public)
                        this.ip_width = 0;
                    else
                        this.ip_width = this.MeasureText("Voice enabled") + 12;
                    break;
            }

            if (this.ip_width > largest_width)
                largest_width = this.ip_width;

            if (this.name_width > 0)
                this.name_width += 2;

            if (this.pm_width > 0)
                this.pm_width += 2;

            if (this.asl_width > 0)
                this.asl_width += 2;

            if (this.ip_width > 0)
                this.ip_width += 2;

            e.ToolTipSize = new Size(110 + largest_width, 100);
        }

        private int MeasureText(String text)
        {
            if (String.IsNullOrEmpty(text))
                return 0;
            else
            {
                int color_finder;
                int width = 0;
                char[] letters = text.ToCharArray();
                Color org_back_color = Color.Silver;
                Color fore_color = Color.Black;
                bool bold = false, italic = false, underline = false;

                using (Bitmap b = new Bitmap(400, 15))
                {
                    using (Graphics g = Graphics.FromImage(b))
                    {
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
                                            i += 2;
                                        }
                                        else goto default;
                                    }
                                    else goto default;
                                    break;

                                case ' ': // space
                                    width += underline ? 1 : (bold ? 3 : 2);
                                    break;

                                case '+':
                                case '(':
                                case ':':
                                case ';': // emoticons
                                    int emote_index = EmoticonFinder.GetRTFEmoticonFromKeyboardShortcut(text.Substring(i).ToUpper());

                                    if (emote_index > -1)
                                    {
                                        width += 15;
                                        i += (EmoticonFinder.last_emote_length - 1);
                                        break;
                                    }
                                    else goto default;

                                default: // text
                                    using (Font font = new Font(this.f, this.CreateFont(bold, italic, underline)))
                                        width += (int)Math.Round((double)g.MeasureString(letters[i].ToString(), font, 100, StringFormat.GenericTypographic).Width);
                                    break;
                            }
                        }
                    }
                }

                return width;
            }
        }

        private void PaintText(String text, int start_x, int start_y, int max_width, Graphics g)
        {
            int color_finder;
            int width = 0;
            char[] letters = text.ToCharArray();
            Color org_back_color = Color.Silver;
            Color fore_color = Color.Black;
            bool bold = false, italic = false, underline = false;

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
                                i += 2;

                                using (SolidBrush brush = new SolidBrush(back_color))
                                    g.FillRectangle(brush, new Rectangle(start_x + width + 2, start_y, max_width - width - 2, 15));
                            }
                            else goto default;
                        }
                        else goto default;
                        break;

                    case ' ': // space
                        width += underline ? 1 : (bold ? 3 : 2);

                        if (underline)
                            using (Font font = new Font(this.f, this.CreateFont(bold, italic, underline)))
                            using (SolidBrush brush = new SolidBrush(fore_color))
                                g.DrawString(" ", font, brush, new PointF(start_x + width, start_y));
                        break;

                    case '+':
                    case '(':
                    case ':':
                    case ';': // emoticons
                        int emote_index = EmoticonFinder.GetRTFEmoticonFromKeyboardShortcut(text.Substring(i).ToUpper());

                        if (emote_index > -1)
                        {
                            g.DrawImage(AresImages.TransparentEmoticons[emote_index], new RectangleF(start_x + width + 2, start_y, 15, 15));
                            width += 15;
                            i += (EmoticonFinder.last_emote_length - 1);
                            break;
                        }
                        else goto default;

                    default: // text
                        using (Font font = new Font(this.f, this.CreateFont(bold, italic, underline)))
                        {
                            int char_width = (int)Math.Round((double)g.MeasureString(letters[i].ToString(), font, 100, StringFormat.GenericTypographic).Width);

                            using (SolidBrush brush = new SolidBrush(fore_color))
                                g.DrawString(letters[i].ToString(), font, brush, new PointF(start_x + width, start_y));

                            width += char_width;
                        }
                        break;
                }
            }
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

        private FontStyle CreateFont(bool bold, bool italic, bool underline)
        {
            FontStyle f = bold ? FontStyle.Bold : FontStyle.Regular;

            if (italic)
                f |= FontStyle.Italic;

            if (underline)
                f |= FontStyle.Underline;

            return f;
        }
    }
}
