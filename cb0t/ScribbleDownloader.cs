using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.Net;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;

namespace cb0t
{
    public partial class ScribbleDownloader : Form
    {
        private const String USER_AGENT = "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/535.19 (KHTML, like Gecko) Chrome/18.0.1003.1 Safari/535.19 Awesomium/1.7.4.2";

        public ScribbleDownloader()
        {
            InitializeComponent();
        }

        private void ScribbleDownloader_Paint(object sender, PaintEventArgs e)
        {
            using (Pen pen = new Pen(Brushes.Black))
                e.Graphics.DrawRectangle(pen, new Rectangle(0, 0, e.ClipRectangle.Width - 1, e.ClipRectangle.Height - 1));
        }

        public void PrepareAnimation()
        {
            this.pictureBox1.ImageLocation = Path.Combine(Settings.AppPath, "loader.gif");
        }

        public void DownloadImage(Control owner, String url, Action<DownloadedImagedReceivedEventArgs> callback, bool save)
        {
            if (url.StartsWith("http://scribble.image/"))
            {
                String path = Path.Combine(Settings.ScribblePath, url.Substring(22));
                DownloadedImagedReceivedEventArgs args = new DownloadedImagedReceivedEventArgs();
                args.Save = save;
                args.ImageBytes = null;

                try { args.ImageBytes = File.ReadAllBytes(path); }
                catch { }

                callback(args);
            }
            else
            {
                new Thread(new ThreadStart(() =>
                {
                    DownloadedImagedReceivedEventArgs args = new DownloadedImagedReceivedEventArgs();
                    args.Save = save;
                    args.ImageBytes = null;

                    try
                    {
                        HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                        request.UserAgent = USER_AGENT;
                        
                        using (WebResponse response = request.GetResponse())
                        using (Stream stream = response.GetResponseStream())
                        {
                            List<byte> bytes = new List<byte>();
                            byte[] buf = new byte[1024];
                            int size = 0;

                            while ((size = stream.Read(buf, 0, 1024)) > 0)
                                bytes.AddRange(buf.Take(size));

                            args.ImageBytes = bytes.ToArray();
                            bytes = new List<byte>();
                        }
                    }
                    catch { }

                    if (args.Save && args.ImageBytes != null)
                        args.ImageBytes = this.ScaleImage(args.ImageBytes);

                    callback(args);
                    this.KillForm();
                })).Start();

                this.StartPosition = FormStartPosition.CenterParent;
                this.ShowDialog(owner);
            }
        }

        private byte[] ScaleImage(byte[] org_bytes)
        {
            byte[] result = new byte[] { };

            using (MemoryStream ms = new MemoryStream(org_bytes))
            using (Bitmap avatar_raw = new Bitmap(ms))
            {
                int img_x = avatar_raw.Width;
                int img_y = avatar_raw.Height;

                if (img_x > 396)
                {
                    img_x = 396;
                    img_y = avatar_raw.Height - (int)Math.Floor(Math.Floor((double)avatar_raw.Height / 100) * Math.Floor(((double)(avatar_raw.Width - 396) / avatar_raw.Width) * 100));
                }

                if (img_y > 396)
                {
                    img_x -= (int)Math.Floor(Math.Floor((double)img_x / 100) * Math.Floor(((double)(img_y - 396) / img_y) * 100));
                    img_y = 396;
                }

                using (Bitmap avatar_sized = new Bitmap(img_x, img_y))
                    using (Graphics g = Graphics.FromImage(avatar_sized))
                    {
                        g.Clear(Color.White);
                        g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                        g.DrawImage(avatar_raw, new RectangleF(0, 0, img_x, img_y));

                        using (MemoryStream stream = new MemoryStream())
                        {
                            avatar_sized.Save(stream, ImageFormat.Png);
                            result = stream.ToArray();
                        }
                    }
            }

            return result;
        }

        private void KillForm()
        {
            if (this.InvokeRequired)
                this.BeginInvoke(new Action(this.KillForm));
            else
            {
                this.DialogResult = DialogResult.OK;
            }
        }
    }

    public class DownloadedImagedReceivedEventArgs : EventArgs
    {
        public byte[] ImageBytes { get; set; }
        public bool Save { get; set; }
    }
}
