using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace cb0t
{
    class UserListToolTip : ToolTip
    {
        public User CurrentUser { get; set; }
        private Font Font { get; set; }

        public UserListToolTip()
        {
            this.Font = new Font("Tahoma", 12f, FontStyle.Bold, GraphicsUnit.Pixel, 0);
            this.OwnerDraw = true;
            this.Popup += this.OnPopup;
            this.Draw += this.OnDraw;
        }

        public void Free()
        {
            this.Popup -= this.OnPopup;
            this.Draw -= this.OnDraw;
            this.CurrentUser = null;
            this.Font.Dispose();
            this.Font = null;
        }

        private void OnDraw(object sender, DrawToolTipEventArgs e)
        {
            if (this.CurrentUser != null)
            {
                using (LinearGradientBrush brush = new LinearGradientBrush(e.Bounds, Color.WhiteSmoke, Color.Silver, LinearGradientMode.Vertical))
                    e.Graphics.FillRectangle(brush, e.Bounds);

                if (this.CurrentUser.Avatar == null)
                    this.CurrentUser.SetAvatar();

                e.Graphics.DrawImage(this.CurrentUser.Avatar, new Rectangle(5, 5, 80, 80));
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
