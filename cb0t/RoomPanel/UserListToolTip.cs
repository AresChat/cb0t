using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace cb0t
{
    class UserListToolTip : ToolTip
    {
        public User CurrentUser { get; set; }
        private Font Font { get; set; }
        private Bitmap def_av = null;

        public UserListToolTip()
        {
            if (Avatar.DefaultAvatar != null)
                this.createdefav();

            this.Font = new Font("Tahoma", 12f, FontStyle.Bold, GraphicsUnit.Pixel, 0);
            this.OwnerDraw = true;
            this.Popup += this.OnPopup;
            this.Draw += this.OnDraw;
        }

        private void createdefav()
        {
            using (MemoryStream ms = new MemoryStream(Avatar.DefaultAvatar))
            using (Bitmap dav = new Bitmap(ms))
            using (Bitmap sized = new Bitmap(53, 53))
            using (Graphics sized_g = Graphics.FromImage(sized))
            {
                sized_g.SmoothingMode = SmoothingMode.HighQuality;
                sized_g.CompositingQuality = CompositingQuality.HighQuality;
                sized_g.DrawImage(dav, new Rectangle(0, 0, 53, 53), new Rectangle(0, 0, 48, 48), GraphicsUnit.Pixel);
                this.def_av = new Bitmap(53, 53);

                using (Graphics av_g = Graphics.FromImage(this.def_av))
                using (GraphicsPath path = new Rectangle(0, 0, 52, 52).Rounded(8))
                using (TextureBrush brush = new TextureBrush(sized))
                {
                    av_g.SmoothingMode = SmoothingMode.HighQuality;
                    av_g.CompositingQuality = CompositingQuality.HighQuality;

                    using (SolidBrush sb = new SolidBrush(Color.White))
                        av_g.FillPath(sb, path);

                    av_g.FillPath(brush, path);

                    using (Pen pen = new Pen(Color.Gainsboro, 1))
                        av_g.DrawPath(pen, path);
                }
            }
        }

        public void Free()
        {
            this.Popup -= this.OnPopup;
            this.Draw -= this.OnDraw;
            this.CurrentUser = null;
            this.Font.Dispose();
            this.Font = null;

            if (this.def_av != null)
            {
                this.def_av.Dispose();
                this.def_av = null;
            }
        }

        private void OnDraw(object sender, DrawToolTipEventArgs e)
        {
            if (this.CurrentUser != null)
            {
                using (LinearGradientBrush brush = new LinearGradientBrush(e.Bounds, Color.WhiteSmoke, Color.Silver, LinearGradientMode.Vertical))
                    e.Graphics.FillRectangle(brush, e.Bounds);

                if (this.CurrentUser.Avatar != null)
                    e.Graphics.DrawImage(this.CurrentUser.Avatar, new Rectangle(5, 5, 80, 80));
                else
                    e.Graphics.DrawImage(this.def_av, new Rectangle(5, 5, 80, 80));

                this.DrawString(this.CurrentUser.Name, e.Graphics);
            }
        }

        private void OnPopup(object sender, PopupEventArgs e)
        {
            if (this.CurrentUser == null)
                e.Cancel = true;
            else
            {
                int x, y;
                x = 140;
                y = 90;

                using (Bitmap bmp = new Bitmap(10, 10))
                using (Graphics g = Graphics.FromImage(bmp))
                    x += this.StringLength(this.CurrentUser.Name, g);

                e.ToolTipSize = new Size(x, y);
            }
        }

        private void DrawString(String str, Graphics g)
        {
            byte l = this.CurrentUser.Level;
            Color col = l == 3 ? Color.Red : l == 2 ? Color.Green : l == 1 ? Color.Blue : Color.Black;

            using (SolidBrush brush = new SolidBrush(col))
            {
                char[] letters = str.ToCharArray();
                int x = 0;

                foreach (char c in letters)
                    if (c == ' ')
                        x += 3;
                    else
                    {
                        g.DrawString(c.ToString(), this.Font, brush, new PointF(100 + x, 38));
                        x += (int)Math.Round((double)g.MeasureString(c.ToString(), this.Font, 100, StringFormat.GenericTypographic).Width);
                    }
            }
        }

        private int StringLength(String str, Graphics g)
        {
            char[] letters = str.ToCharArray();
            int x = 0;

            foreach (char c in letters)
                if (c == ' ')
                    x += 3;
                else
                    x += (int)Math.Round((double)g.MeasureString(c.ToString(), this.Font, 100, StringFormat.GenericTypographic).Width);

            return x;
        }
    }
}
