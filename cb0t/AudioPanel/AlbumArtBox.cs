using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.Xml;

namespace cb0t
{
    class AlbumArtBox : PictureBox
    {
        private Bitmap raw_bmp { get; set; }

        private bool busy = false;
        private bool pending = false;
        private String artist = String.Empty;
        private String album = String.Empty;
        private String song = String.Empty;
        private String last_img = String.Empty;
        private Thread thread;

        public AlbumArtBox()
        {
            this.SizeMode = PictureBoxSizeMode.CenterImage | PictureBoxSizeMode.StretchImage;
            this.Image = null;
        }

        public void UpdateArt(AudioPlayerItem item)
        {
            this.artist = item.Artist.Trim();
            this.album = item.Album.Trim();
            this.song = item.Title.Trim();
            String[] arr = new String[] { artist, album, song };

            if (this.busy)
            {
                this.pending = true;
                return;
            }

            if (!String.IsNullOrEmpty(this.artist) && (!String.IsNullOrEmpty(this.album) || !String.IsNullOrEmpty(this.song)))
            {
                this.thread = new Thread(new ParameterizedThreadStart(this.Worker));
                this.thread.Start(arr);
            }
        }

        private String GetFilename(String text)
        {
            byte[] buf = Encoding.UTF8.GetBytes(text);
            StringBuilder sb = new StringBuilder();

            foreach (byte b in buf)
                sb.AppendFormat("{0:X2}", b);

            return sb.ToString();
        }

        private void Worker(object args)
        {
            byte[] raw_bytes = null;
            String[] arr = (String[])args;

            if (this.LocalArt(arr))
            {
                if (this.pending)
                {
                    this.pending = false;
                    arr = new String[] { this.artist, this.album, this.song };
                    this.Worker(arr);
                }

                return;
            }

            String saver = null;
            this.busy = true;

            String url = "http://ws.audioscrobbler.com/2.0/?method=album.getinfo&api_key=4f17ae42e38a39194a0c6c2f82f8d16a&artist=" +
                    Uri.EscapeDataString(arr[0]) + "&album=" + Uri.EscapeDataString(arr[1]);

            try
            {
                WebRequest request = WebRequest.Create(url);

                using (WebResponse response = request.GetResponse())
                using (Stream stream = response.GetResponseStream())
                {
                    url = null;
                    XmlDocument xml = new XmlDocument();
                    xml.Load(stream);
                    XmlNodeList nodes = xml.GetElementsByTagName("image");

                    foreach (XmlNode n in nodes)
                    {
                        if (n.Attributes["size"].Value.ToUpper() == "LARGE")
                        {
                            url = n.InnerText;
                            saver = arr[0] + arr[1];
                            break;
                        }
                    }

                    xml = null;
                    nodes = null;
                }

                if (String.IsNullOrEmpty(url))
                    throw new Exception();
            }
            catch
            {
                url = "http://ws.audioscrobbler.com/2.0/?method=track.getinfo&api_key=4f17ae42e38a39194a0c6c2f82f8d16a&artist=" +
                    Uri.EscapeDataString(arr[0]) + "&track=" + Uri.EscapeDataString(arr[2]);

                try
                {
                    WebRequest request = WebRequest.Create(url);
                    WebResponse response = request.GetResponse();

                    using (Stream stream = response.GetResponseStream())
                    {
                        url = null;
                        XmlDocument xml = new XmlDocument();
                        xml.Load(stream);
                        XmlNodeList nodes = xml.GetElementsByTagName("image");

                        foreach (XmlNode n in nodes)
                        {
                            if (n.Attributes["size"].Value.ToUpper() == "LARGE")
                            {
                                url = n.InnerText;
                                saver = arr[0] + arr[2];
                                break;
                            }
                        }

                        xml = null;
                        nodes = null;
                    }

                    response.Close();
                }
                catch { url = null; }
            }

            try
            {
                if (!String.IsNullOrEmpty(url))
                {
                    if (url != this.last_img)
                    {
                        this.last_img = url;
                        WebRequest request = WebRequest.Create(url);
                        WebResponse response = request.GetResponse();

                        using (Stream stream = response.GetResponseStream())
                        {
                            List<byte> buf = new List<byte>();

                            while (true)
                            {
                                byte[] tmp1 = new byte[8192];
                                int size = stream.Read(tmp1, 0, 8192);

                                if (size > 0)
                                {
                                    byte[] tmp2 = new byte[size];
                                    Array.Copy(tmp1, tmp2, size);
                                    buf.AddRange(tmp2);
                                }
                                else break;
                            }

                            if (buf.Count > 32)
                                raw_bytes = buf.ToArray();

                            buf = null;
                        }

                        response.Close();
                    }
                }
                else this.last_img = String.Empty;
            }
            catch
            {
                this.last_img = String.Empty;
            }

            this.busy = false;

            if (this.pending)
            {
                this.pending = false;
                arr = new String[] { this.artist, this.album, this.song };
                this.Worker(arr);
            }
            else if (raw_bytes != null)
            {
                this.SetArt(raw_bytes);

                try
                {
                    if (!String.IsNullOrEmpty(saver))
                        File.WriteAllBytes(Path.Combine(Settings.ArtPath, this.GetFilename(saver)), raw_bytes);
                }
                catch { }
            }
        }

        private bool LocalArt(String[] arr)
        {
            String path = Path.Combine(Settings.ArtPath, this.GetFilename(arr[0] + arr[1]));
            byte[] raw_bytes = null;

            if (File.Exists(path))
                try { raw_bytes = File.ReadAllBytes(path); }
                catch { }

            if (raw_bytes == null)
            {
                path = Path.Combine(Settings.ArtPath, this.GetFilename(arr[0] + arr[2]));

                if (File.Exists(path))
                    try { raw_bytes = File.ReadAllBytes(path); }
                    catch { }
            }

            if (raw_bytes != null)
            {
                this.SetArt(raw_bytes);
                return true;
            }

            return false;
        }

        private void SetArt(object data)
        {
            if (this.InvokeRequired)
                this.BeginInvoke((Action)(() => this.SetArt(data)));
            else
            {
                using (MemoryStream ms = new MemoryStream((byte[])data))
                {
                    this.raw_bmp = new Bitmap(ms);
                    this.Image = this.raw_bmp;
                }
            }
        }

        public void ClearArt()
        {
            this.Image = null;

            if (this.raw_bmp != null)
            {
                this.raw_bmp.Dispose();
                this.raw_bmp = null;
            }

            this.artist = String.Empty;
            this.album = String.Empty;
            this.song = String.Empty;
            this.last_img = String.Empty;
            this.Invalidate();
        }
    }
}
