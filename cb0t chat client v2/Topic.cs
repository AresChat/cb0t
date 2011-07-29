using System;
using System.Drawing;
using System.Windows.Forms;

namespace cb0t_chat_client_v2
{
    public partial class Topic : UserControl
    {
        public Topic()
        {
            InitializeComponent();
        }

        private String _topic = String.Empty;

        public override String Text
        {
            get
            {
                return this._topic;
            }
            set
            {
                this._topic = value;
                this.Invalidate();
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            char[] letters = this._topic.ToCharArray();

            Color fore_color = Color.Black;
            bool bold = false, italic = false, underline = false, back_color_required = false;
            int x = 2;
            int y = 3;
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
                            if (int.TryParse(this.Text.Substring(i + 1, 2), out color_finder))
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
                            if (int.TryParse(this.Text.Substring(i + 1, 2), out color_finder))
                            {
                                Color back_color = this.GetColorFromCode(color_finder);
                                back_color_required = true;
                                i += 2;

                                using (SolidBrush brush = new SolidBrush(back_color))
                                    e.Graphics.FillRectangle(brush, new Rectangle(x + 2, y, e.ClipRectangle.Width - x - 2, 18));
                            }
                            else goto default;
                        }
                        else goto default;
                        break;

                    case ' ': // space
                        x += underline ? 2 : (bold ? 4 : 3);

                        if (x > (e.ClipRectangle.Width - 1))
                            break;

                        if (underline)
                            using (Font font = new Font(this.Font, this.CreateFont(bold, italic, underline)))
                            using (SolidBrush brush = new SolidBrush(fore_color))
                                e.Graphics.DrawString(" ", font, brush, new PointF(x, y));
                        break;

                    case '+':
                    case '(':
                    case ':':
                    case ';': // emoticons
                        int emote_index = EmoticonFinder.GetRTFEmoticonFromKeyboardShortcut(this._topic.Substring(i).ToUpper());

                        if (emote_index > -1)
                        {
                            if ((x + 15) > (e.ClipRectangle.Width - 1))
                            {
                                x += 15;
                                break;
                            }

                            e.Graphics.DrawImage(AresImages.TransparentEmoticons[emote_index], new RectangleF(x + 3, y, 16, 16));
                            x += 18;
                            i += (EmoticonFinder.last_emote_length - 1);
                            break;
                        }
                        else goto default;

                    default: // text
                        using (Font font = new Font(this.Font, this.CreateFont(bold, italic, underline)))
                        {
                            int width = (int)Math.Round((double)e.Graphics.MeasureString(letters[i].ToString(), font, 100, StringFormat.GenericTypographic).Width);

                            if ((x + width) > (e.ClipRectangle.Width - 1))
                            {
                                x += width;
                                break;
                            }

                            using (SolidBrush brush = new SolidBrush(fore_color))
                                e.Graphics.DrawString(letters[i].ToString(), font, brush, new PointF(x, y));

                            x += width;
                        }
                        break;
                }

                if (x > (e.ClipRectangle.Width - 1)) // run out of space - stop drawing!!
                    return;
            }

            if (back_color_required) // trim excess background because the topic is shorter than the column width
                if ((x + 2) < e.ClipRectangle.Width)
                    using (SolidBrush brush = new SolidBrush(SystemColors.Control))
                        e.Graphics.FillRectangle(brush, new Rectangle(x + 2, y, e.ClipRectangle.Width - x - 2, 18));
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
