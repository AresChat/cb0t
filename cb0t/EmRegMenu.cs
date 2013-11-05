using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing.Drawing2D;

namespace cb0t
{
    public partial class EmRegMenu : UserControl
    {
        private Bitmap img { get; set; }
        public event EventHandler<EmoticonShortcutEventArgs> EmoticonClicked;

        private String[] s_cuts = new String[]
        {
            ":-)", ":-D", ";-)", ":-O", ":-P", "(H)", ":@",
            ":$", ":-S", ":-(", ":'(", ":-|", "(6)", "(A)",
            "(L)", "(U)", "(M)", "(@)", "(&)", "(S)", "(*)",
            "(~)", "(E)", "(8)", "(F)", "(W)", "(O)", "(K)",
            "(G)", "(^)", "(P)", "(I)", "(C)", "(T)", "({)",
            "(})", "(B)", "(D)", "(Z)", "(X)", "(Y)", "(N)",
            ":-[", "(1)", "(2)", "(3)", "(4)"
        };

        public EmRegMenu()
        {
            this.InitializeComponent();
            this.BackColor = Color.White;
            this.DoubleBuffered = true;
            this.Paint += this.PaintSurface;
            this.MouseMove += this.EmRegMenu_MouseMove;
            this.MouseLeave += this.EmRegMenu_MouseLeave;
            this.MouseClick += this.EmRegMenu_MouseClick;
            this.Leave += EmRegMenu_Leave;
            this.ResizeRedraw = true;
        }

        private void EmRegMenu_MouseClick(object sender, MouseEventArgs e)
        {
            if (this.last_y >= 48)
                if (this.last_x >= 80)
                    return;

            int x = this.last_x, y = this.last_y;

            x /= 16;
            y /= 16;

            int index = (y * 14);
            index += x;

            if (index >= 0 && index < this.s_cuts.Length)
                if (this.EmoticonClicked != null)
                    this.EmoticonClicked(this, new EmoticonShortcutEventArgs(this.s_cuts[index]));
        }

        private int last_x = -1;
        private int last_y = -1;
        private Pen pen = new Pen(new SolidBrush(Color.DimGray), 1);

        private void EmRegMenu_Leave(object sender, EventArgs e)
        {
            this.last_x = -1;
            this.last_y = -1;
            this.Invalidate();
        }

        private void EmRegMenu_MouseLeave(object sender, EventArgs e)
        {
            this.last_x = -1;
            this.last_y = -1;
            this.Invalidate();
        }

        private void EmRegMenu_MouseMove(object sender, MouseEventArgs e)
        {
            int x = e.X / 16;
            x *= 16;
            int y = e.Y / 16;
            y *= 16;

            if (x != this.last_x || y != this.last_y)
            {
                this.last_x = x;
                this.last_y = y;
                this.Invalidate();
            }
        }

        private void PaintSurface(object sender, PaintEventArgs e)
        {
            if (this.img == null)
            {
                this.img = new Bitmap(224, 64);

                using (Graphics g = Graphics.FromImage(this.img))
                {
                    g.CompositingQuality = CompositingQuality.HighQuality;
                    g.InterpolationMode = InterpolationMode.HighQualityBilinear;
                    g.SmoothingMode = SmoothingMode.HighQuality;
                    Rectangle r = new Rectangle(0, 0, 224, 64);

                    using (SolidBrush sb = new SolidBrush(Color.White))
                        g.FillRectangle(sb, r);

                    using (Bitmap org = (Bitmap)Properties.Resources.emotic.Clone())
                    {
                        org.MakeTransparent(Color.Magenta);

                        int org_x = 0, org_y = 0;
                        int pos_x = 0, pos_y = 0;

                        for (int i = 0; i < 47; i++)
                        {
                            Rectangle org_rec = new Rectangle((org_x * 16), (org_y * 16), 16, 16);
                            Rectangle pos_rec = new Rectangle((pos_x * 16), (pos_y * 16), 16, 16);

                            if (++org_x == 7)
                            {
                                org_x = 0;
                                org_y++;
                            }

                            if (++pos_x == 14)
                            {
                                pos_x = 0;
                                pos_y++;
                            }

                            g.DrawImage(org, pos_rec, org_rec, GraphicsUnit.Pixel);
                        }
                    }
                }
            }

            e.Graphics.DrawImage(this.img, new Point(0, 0));

            if (this.last_x >= 0 && this.last_y >= 0)
            {
                if (this.last_y >= 48)
                    if (this.last_x >= 80)
                        return;

                Rectangle tracker = new Rectangle(this.last_x, this.last_y, 15, 15);
                e.Graphics.DrawRectangle(this.pen, tracker);
            }
        }
    }

    public class EmoticonShortcutEventArgs : EventArgs
    {
        public String Shortcut { get; private set; }

        public EmoticonShortcutEventArgs(String str)
        {
            this.Shortcut = str;
        }
    }
}
