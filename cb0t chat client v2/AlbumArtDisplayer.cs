using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.IO;
using System.Threading;
using System.Net;
using System.Xml;

namespace cb0t_chat_client_v2
{
    class AlbumArtDisplayer : UserControl
    {
        private byte[] art = null;

        private bool busy = false;
        private bool pending = false;
        private String artist = String.Empty;
        private String album = String.Empty;
        private String song = String.Empty;
        private String last_img = String.Empty;
        private Thread thread;

        public AlbumArtDisplayer()
        {
            this.DoubleBuffered = true;
            this.Font = new Font("Verdana", 8.25f);
            this.Size = new Size(184, 184);
        }

        public void UpdateArt(String artist, String album, String song)
        {
            if (AudioSettings.show_album_art)
            {
                this.artist = artist.Trim();
                this.album = album.Trim();
                this.song = song.Trim();
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
            else this.UpdateArt();
        }

        public void UpdateArt()
        {
            this.art = null;
            this.artist = String.Empty;
            this.album = String.Empty;
            this.song = String.Empty;
            this.last_img = String.Empty;
            this.Invalidate();
        }

        private void Worker(object args)
        {
            
            String[] arr = (String[])args;
            this.busy = true;

            String url = "http://ws.audioscrobbler.com/2.0/?method=album.getinfo&api_key=4f17ae42e38a39194a0c6c2f82f8d16a&artist=" +
                    Uri.EscapeDataString(arr[0]) + "&album=" + Uri.EscapeDataString(arr[1]);

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
                        if (n.Attributes["size"].Value.ToUpper() == "EXTRALARGE")
                        {
                            url = n.InnerText;
                            break;
                        }
                    }
                    
                    xml = null;
                    nodes = null;
                }

                response.Close();

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
                            if (n.Attributes["size"].Value.ToUpper() == "EXTRALARGE")
                            {
                                url = n.InnerText;
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
                                this.art = buf.ToArray();

                            buf = null;
                        }

                        response.Close();
                    }
                }
                else
                {
                    this.art = null;
                    this.last_img = String.Empty;
                }
            }
            catch
            {
                this.art = null;
                this.last_img = String.Empty;
            }

            this.busy = false;

            if (this.pending)
            {
                this.pending = false;
                arr = new String[] { this.artist, this.album, this.song };
                this.Worker(arr);
            }
            else this.Invalidate();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            e.Graphics.Clear(Color.Black);

            if (this.art != null)
                using (MemoryStream ms = new MemoryStream(this.art))
                using (Bitmap bmp = new Bitmap(ms))
                {
                    int width = this.Width > this.Height ? this.Width : this.Height;

                    if (width > 300)
                        width = 300;

                    int start_x = (this.Width / 2) - 150;
                    int start_y = (this.Height / 2) - 150;

                    e.Graphics.DrawImage(bmp, new Rectangle(start_x, start_y, width, width));
                }
            else if (AudioControllerImages.note != null)
            {
                int width = this.Width > this.Height ? this.Width : this.Height;

                if (width > 256)
                    width = 256;

                int start_x = (this.Width / 2) - 128;
                int start_y = (this.Height / 2) - 128;

                e.Graphics.DrawImage(AudioSettings.radio_mode ? AudioControllerImages.radio : AudioControllerImages.note, new Rectangle(start_x, start_y, width, width));
            }
        }

        protected override void OnResize(EventArgs e)
        {
            this.Invalidate();
        }
    }
}
