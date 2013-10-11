using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing.Drawing2D;
using System.IO;
using System.Net;
using System.Threading;

namespace cb0t
{
    public partial class UserListHeader : UserControl
    {
        private Color column_bg1 = Color.Gainsboro;
        private Color column_bg2 = Color.Silver;
        private SolidBrush column_text_brush = new SolidBrush(Color.Black);
        private ToolTip tip = new ToolTip();

        public String HeaderText { get; set; }
        public String ServerVersion { get; set; }

        private bool IsEncrypted { get; set; }
        private byte[] server_icon { get; set; }
        private ulong lag { get; set; }

        private Bitmap strength1, strength2, strength3, crypto;

        public UserListHeader()
        {
            this.InitializeComponent();
            this.lag = 0;
            this.HeaderText = "Users (0)";
            this.strength1 = (Bitmap)Properties.Resources.strength1.Clone();
            this.strength2 = (Bitmap)Properties.Resources.strength2.Clone();
            this.strength3 = (Bitmap)Properties.Resources.strength3.Clone();
            this.crypto = (Bitmap)Properties.Resources.crypto.Clone();
            this.MouseHover += this.ShowTip;
        }

        public void Free()
        {
            this.MouseHover -= this.ShowTip;
            this.strength1.Dispose();
            this.strength1 = null;
            this.strength2.Dispose();
            this.strength2 = null;
            this.strength3.Dispose();
            this.strength3 = null;
            this.crypto.Dispose();
            this.crypto = null;
            this.server_icon = null;
            this.column_text_brush.Dispose();
            this.column_text_brush = null;
            this.tip.Dispose();
            this.tip = null;
        }

        public void SetCrypto(bool is_crypto)
        {
            this.IsEncrypted = is_crypto;
            this.Invalidate();
        }

        private void ShowTip(object sender, EventArgs e)
        {
            StringBuilder sb = new StringBuilder();

            if (!String.IsNullOrEmpty(this.HeaderText))
                sb.AppendLine(this.HeaderText);

            if (!String.IsNullOrEmpty(this.ServerVersion))
                sb.AppendLine("Server (" + this.ServerVersion + ")");

            if (this.lag > 0)
                sb.AppendLine("Lag (" + this.lag + " ms)");

            this.tip.SetToolTip(this, sb.ToString());
        }

        public void UpdateLag(ulong lag)
        {
            this.lag = lag;
            this.Invalidate();
        }

        private void UserListHeader_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.Clear(Color.White);

            Rectangle r = new Rectangle(1, 1, this.Width - 2, this.Height - 2);

            using (LinearGradientBrush lb = new LinearGradientBrush(r, this.column_bg1, this.column_bg2, 90f))
                e.Graphics.FillRectangle(lb, r);

            r = new Rectangle(1, 1, this.Width - 3, this.Height - 3);

            using (Pen column_outline = new Pen(new SolidBrush(Color.FromArgb(109, 115, 123)), 1))
                e.Graphics.DrawRectangle(column_outline, r);

            int icons_drawn = 0;

            if (this.server_icon != null)
            {
                using (MemoryStream ms = new MemoryStream(this.server_icon))
                using (Bitmap bmp = new Bitmap(ms))
                    e.Graphics.DrawImage(bmp, new Point(3 + (icons_drawn * 17), 3));

                icons_drawn++;
            }

            if (this.lag > 0)
            {
                if (this.lag < 250)
                    e.Graphics.DrawImage(this.strength1, new Rectangle(4 + (icons_drawn * 17), 4, 14, 14));
                else if (this.lag < 1000)
                    e.Graphics.DrawImage(this.strength2, new Rectangle(4 + (icons_drawn * 17), 4, 14, 14));
                else
                    e.Graphics.DrawImage(this.strength3, new Rectangle(4 + (icons_drawn * 17), 4, 14, 14));

                icons_drawn++;
            }

            if (this.IsEncrypted)
            {
                e.Graphics.DrawImage(this.crypto, new Point(3 + (icons_drawn * 17), 3));
                icons_drawn++;
            }

            e.Graphics.DrawString(this.HeaderText, this.Font, this.column_text_brush, new PointF(3 + (icons_drawn * 17), 4));
        }

        public void AcquireServerIcon(IPEndPoint ep)
        {
            Thread thread = new Thread(new ThreadStart(() =>
            {
                try
                {
                    WebRequest request = WebRequest.Create("http://" + ep.Address + ":" + ep.Port + "/favicon.ico");

                    using (WebResponse response = request.GetResponse())
                    using (Stream stream = response.GetResponseStream())
                    {
                        List<byte> list = new List<byte>();
                        byte[] buf = new byte[2048];
                        int size = 0;

                        while ((size = stream.Read(buf, 0, 2048)) > 0)
                            list.AddRange(buf.Take(size));

                        this.server_icon = list.ToArray();
                    }
                }
                catch { }

                this.BeginInvoke((Action)(() => this.Refresh()));
            }));

            thread.Start();
        }
    }
}
