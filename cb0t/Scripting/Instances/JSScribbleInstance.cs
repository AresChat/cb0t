using Jurassic;
using Jurassic.Library;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;

namespace cb0t.Scripting.Instances
{
class JSScribbleInstance : ObjectInstance
    {
        private bool busy = false;

        public JSScribbleInstance(ObjectInstance prototype)
            : base(prototype)
        {
            this.PopulateFunctions();
        }

        protected override string InternalClassName
        {
            get { return "Scribble"; }
        }

        [JSProperty(Name = "src")]
        public String Source { get; set; }

        [JSProperty(Name = "oncomplete")]
        public UserDefinedFunction Callback { get; set; }

        [JSFunction(Name = "download", IsWritable = false, IsEnumerable = true)]
        public bool Download(object a)
        {
            if (this.busy)
                return false;

            Thread thread = new Thread(new ThreadStart(() =>
            {
                this.busy = true;
                String arg = String.Empty;

                if (!(a is Undefined) && a != null)
                    arg = a.ToString();

                Objects.JSScribbleImage result = new Objects.JSScribbleImage(this.Engine.Object.InstancePrototype)
                {
                    Callback = this.Callback,
                    Data = null,
                    ScriptName = this.Engine.ScriptName,
                    Arg = arg,
                    URL = this.Source
                };

                try
                {
                    WebRequest request = WebRequest.Create(this.Source);
                    List<byte> bytes_in = new List<byte>();

                    using (WebResponse response = request.GetResponse())
                    {
                        int received = 0;
                        byte[] buf = new byte[1024];

                        using (Stream stream = response.GetResponseStream())
                            while ((received = stream.Read(buf, 0, 1024)) > 0)
                                bytes_in.AddRange(buf.Take(received));
                    }

                    using (Bitmap avatar_raw = new Bitmap(new MemoryStream(bytes_in.ToArray())))
                    {
                        int img_x = avatar_raw.Width;
                        int img_y = avatar_raw.Height;

                        if (img_x > 384)
                        {
                            img_x = 384;
                            img_y = avatar_raw.Height - (int)Math.Floor(Math.Floor((double)avatar_raw.Height / 100) * Math.Floor(((double)(avatar_raw.Width - 384) / avatar_raw.Width) * 100));
                        }

                        if (img_y > 384)
                        {
                            img_x -= (int)Math.Floor(Math.Floor((double)img_x / 100) * Math.Floor(((double)(img_y - 384) / img_y) * 100));
                            img_y = 384;
                        }

                        using (Bitmap avatar_sized = new Bitmap(img_x, img_y))
                        {
                            using (Graphics g = Graphics.FromImage(avatar_sized))
                            {
                                using (SolidBrush sb = new SolidBrush(Color.White))
                                    g.FillRectangle(sb, new Rectangle(0, 0, img_x, img_y));

                                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                                g.DrawImage(avatar_raw, new RectangleF(0, 0, img_x, img_y));

                                using (MemoryStream ms = new MemoryStream())
                                {
                                    avatar_sized.Save(ms, ImageFormat.Jpeg);
                                    result.Height = avatar_sized.Height;
                                    byte[] img_buffer = ms.ToArray();
                                    result.Data = Zip.Compress(img_buffer);
                                    bytes_in.Clear();
                                }
                            }
                        }
                    }
                }
                catch { }

                ScriptManager.PendingScriptingCallbacks.Enqueue(result);
                this.busy = false;
            }));

            thread.Start();

            return true;
        }

        [JSFunction(Name = "load", IsWritable = false, IsEnumerable = true)]
        public Objects.JSScribbleImage Load(object a)
        {
            Objects.JSScribbleImage scr = new Objects.JSScribbleImage(this.Engine.Object.InstancePrototype);

            if (a is String || a is ConcatenatedString)
            {
                String filename = a.ToString();

                if (filename.Length > 1)
                {
                    JSScript script = ScriptManager.Scripts.Find(x => x.ScriptName == this.Engine.ScriptName);
                    byte[] data = null;

                    if (script != null)
                    {
                        String path = a.ToString();
                        path = new String(path.Where(x => !Path.GetInvalidFileNameChars().Contains(x)).ToArray());
                        path = Path.Combine(script.DataPath, path);

                        if (new FileInfo(path).Directory.FullName != new DirectoryInfo(script.DataPath).FullName)
                            return scr;

                        try
                        {
                            data = File.ReadAllBytes(path);
                        }
                        catch { }
                    }

                    if (data != null)
                    {
                        try
                        {
                            using (Bitmap avatar_raw = new Bitmap(new MemoryStream(data)))
                            {
                                int img_x = avatar_raw.Width;
                                int img_y = avatar_raw.Height;

                                if (img_x > 384)
                                {
                                    img_x = 384;
                                    img_y = avatar_raw.Height - (int)Math.Floor(Math.Floor((double)avatar_raw.Height / 100) * Math.Floor(((double)(avatar_raw.Width - 384) / avatar_raw.Width) * 100));
                                }

                                if (img_y > 384)
                                {
                                    img_x -= (int)Math.Floor(Math.Floor((double)img_x / 100) * Math.Floor(((double)(img_y - 384) / img_y) * 100));
                                    img_y = 384;
                                }

                                using (Bitmap avatar_sized = new Bitmap(img_x, img_y))
                                {
                                    using (Graphics g = Graphics.FromImage(avatar_sized))
                                    {
                                        g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                                        g.DrawImage(avatar_raw, new RectangleF(0, 0, img_x, img_y));

                                        using (MemoryStream ms = new MemoryStream())
                                        {
                                            avatar_sized.Save(ms, ImageFormat.Jpeg);
                                            scr.Height = avatar_sized.Height;
                                            byte[] img_buffer = ms.ToArray();
                                            scr.Data = Zip.Compress(img_buffer);
                                        }
                                    }
                                }
                            }
                        }
                        catch { }
                    }
                }
            }

            return scr;
        }
    }
}
