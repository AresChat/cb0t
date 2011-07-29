using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.IO;

namespace cb0t_chat_client_v2
{
    class EmoticonMenu : Form
    {
        private TextBox target = new TextBox();
        private Point MouseLocation = new Point(0, 0);
        private Bitmap empty;

        private String[,] emoticon_shortcuts = new String[,]
        {
            {":-)", ":-D", ";-)", ":-O", ":-P", "(H)", ":@", ":$", ":-S", ":-("},
            {":'(", ":-|", "(6)", "(A)", "(L)", "(U)", "(M)", "(@)", "(&)", "(S)"},
            {"(*)", "(~)", "(E)", "(8)", "(F)", "(W)", "(O)", "(K)", "(G)", "(^)"},
            {"(P)", "(I)", "(C)", "(T)", "({)", "(})", "(B)", "(D)", "(Z)", "(X)"},
            {"(Y)", "(N)", ":-[", "(1)", "(2)", "(3)", "(4)", "", "", ""}
        };

        public EmoticonMenu(TextBox target)
        {
            this.target = target;
            this.Hide();
            this.DoubleBuffered = true;
            this.ControlBox = false;
            this.Text = String.Empty;
            this.ShowInTaskbar = false;
            this.Visible = false;
            this.empty = (Bitmap)AresImages.DCBlock;
            this.empty.MakeTransparent(Color.Magenta);
            this.Opacity = 0.9;
        }
        
        protected override void OnPaintBackground(PaintEventArgs e)
        {
            using (SolidBrush sb = new SolidBrush(Color.WhiteSmoke))
                e.Graphics.FillRectangle(sb, e.ClipRectangle);

            using (SolidBrush blue_brush = new SolidBrush(Color.DarkBlue))
            {
                using (Pen blue_pen = new Pen(blue_brush, 1))
                {
                    e.Graphics.DrawString("Default Emoticons", this.Font, blue_brush, new PointF(4, 10));
                    e.Graphics.DrawLine(blue_pen, new Point(4, 26), new Point(192, 26));

                    int image = 0;

                    for (int i = 0; i < 5; i++)
                    {
                        for (int r = 0; r < 10; r++)
                        {
                            e.Graphics.DrawImage(AresImages.TransparentEmoticons[image++], new Point((r * 20) + 2, 42 + (i * 20)));

                            if (this.MouseLocation.X >= (r * 20) && this.MouseLocation.X <= ((r * 20) + 19))
                                if (this.MouseLocation.Y >= (40 + (i * 20)) && this.MouseLocation.Y <= (59 + (i * 20)))
                                    e.Graphics.DrawRectangle(blue_pen, new Rectangle((r * 20), 40 + (i * 20), 19, 19));

                            if (i == 4 && r == 6)
                                break;
                        }
                    }

                    e.Graphics.DrawString("Custom Emoticons", this.Font, blue_brush, new PointF(4, 150));
                    e.Graphics.DrawLine(blue_pen, new Point(4, 166), new Point(192, 166));

                    int c_index = 0;

                    using (SolidBrush white_brush = new SolidBrush(Color.White))
                    {
                        using (SolidBrush gray_brush = new SolidBrush(Color.Gray))
                        {
                            using (Pen gray_pen = new Pen(gray_brush))
                            {
                                for (int i = 0; i < 4; i++)
                                {
                                    for (int r = 0; r < 4; r++)
                                    {
                                        CEmoteItem citem = CustomEmotes.Emotes[c_index++];
                                        bool is_used = true;

                                        if (citem.Image == null)
                                        {
                                            e.Graphics.FillRectangle(white_brush, new Rectangle((r * 50) + 1, 181 + (i * 50), 48, 48));
                                            e.Graphics.DrawImage(this.empty, new Point((r * 50) + 17, 197 + (i * 50)));
                                            is_used = false;
                                        }
                                        else
                                        {
                                            using (MemoryStream ms = new MemoryStream(citem.Image))
                                            {
                                                using (Bitmap bmp = new Bitmap(ms))
                                                {
                                                    switch (citem.Size)
                                                    {
                                                        case 16:
                                                            e.Graphics.DrawImage(bmp, new Point((r * 50) + 17, 197 + (i * 50)));
                                                            break;

                                                        case 32:
                                                            e.Graphics.DrawImage(bmp, new Point((r * 50) + 9, 189 + (i * 50)));
                                                            break;

                                                        case 48:
                                                            e.Graphics.DrawImage(bmp, new Point((r * 50) + 1, 181 + (i * 50)));
                                                            break;
                                                    }
                                                }
                                            }
                                        }

                                        if (is_used)
                                        {
                                            if (this.MouseLocation.X >= (r * 50) && this.MouseLocation.X <= ((r * 50) + 49))
                                                if (this.MouseLocation.Y >= (180 + (i * 50)) && this.MouseLocation.Y <= (229 + (i * 50)))
                                                    e.Graphics.DrawRectangle(blue_pen, new Rectangle((r * 50), 180 + (i * 50), 49, 49));
                                        }
                                        else e.Graphics.DrawRectangle(gray_pen, new Rectangle((r * 50) + 1, 181 + (i * 50), 47, 47));
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        protected override void OnMouseClick(MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                // try defaults

                for (int i = 0; i < 5; i++)
                {
                    for (int r = 0; r < 10; r++)
                    {
                        if (e.X >= (r * 20) && e.X <= ((r * 20) + 19))
                        {
                            if (e.Y >= (40 + (i * 20)) && e.Y <= (59 + (i * 20)))
                            {
                                this.target.Text += emoticon_shortcuts[i, r];
                                this.target.SelectionStart = this.target.Text.Length;
                                this.Hide();
                            }
                        }

                        if (i == 4 && r == 6)
                            break;
                    }
                }

                // try customs

                int c_index = 0;

                for (int i = 0; i < 4; i++)
                {
                    for (int r = 0; r < 4; r++)
                    {
                        if (e.X >= (r * 50) && e.X <= ((r * 50) + 49))
                        {
                            if (e.Y >= (180 + (i * 50)) && e.Y <= (229 + (i * 50)))
                            {
                                String shortcut = CustomEmotes.Emotes[c_index].Shortcut;

                                if (!String.IsNullOrEmpty(shortcut))
                                {
                                    this.target.Text += shortcut;
                                    this.target.SelectionStart = this.target.Text.Length;
                                    this.Hide();
                                }
                            }
                        }

                        c_index++;
                    }
                }
            }
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            this.MouseLocation = e.Location;
            this.Invalidate();
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            this.MouseLocation = new Point(0, 0);
            this.Invalidate();
        }

        public new void Show()
        {
            this.ClientSize = new Size(200, 390);
            this.MaximumSize = this.Size;
            this.MinimumSize = this.Size;
            this.Visible = true;
            this.Location = new Point(MousePosition.X - (this.Width / 2), MousePosition.Y - (this.Height + 10));
        }

        protected override void OnLostFocus(EventArgs e)
        {
            base.OnLostFocus(e);
            this.Hide();
        }
    }
}
