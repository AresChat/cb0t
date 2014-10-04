using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace cb0t
{
    class Emoticons
    {
        [DllImport("gdiplus.dll")]
        private static extern uint GdipEmfToWmfBits(IntPtr hEmf, uint uBufferSize, byte[] bBuffer, int iMappingMode, uint flags);
        [DllImport("gdi32.dll")]
        private static extern bool DeleteEnhMetaFile(IntPtr hemf);

        private static List<Emotic> shortcuts { get; set; }

        public static Bitmap[] emotic { get; set; }
        public static ExtendedEmoticon[] ex_emotic { get; set; }

        public static String GetRTFScribble(byte[] data, Graphics richtextbox)
        {
            StringBuilder result = new StringBuilder();

            try
            {
                using (MemoryStream org_ms = new MemoryStream(data))
                using (Bitmap org_bmp = new Bitmap(org_ms))
                using (Bitmap bmp = new Bitmap(org_bmp.Width, org_bmp.Height))
                using (Graphics g = Graphics.FromImage(bmp))
                {
                    g.CompositingQuality = CompositingQuality.HighQuality;
                    g.InterpolationMode = InterpolationMode.HighQualityBilinear;
                    g.SmoothingMode = SmoothingMode.HighQuality;
                    g.Clear(Color.White);
                    g.DrawImage(org_bmp, new Point(0, 0));

                    result.Append(@"{\pict\wmetafile8\picw");
                    result.Append((int)Math.Round((bmp.Width / richtextbox.DpiX) * 2540));
                    result.Append(@"\pich");
                    result.Append((int)Math.Round((bmp.Height / richtextbox.DpiY) * 2540));
                    result.Append(@"\picwgoal");
                    result.Append((int)Math.Round((bmp.Width / richtextbox.DpiX) * 1440));
                    result.Append(@"\pichgoal");
                    result.Append((int)Math.Round((bmp.Height / richtextbox.DpiY) * 1440));
                    result.Append(" ");

                    using (MemoryStream ms = new MemoryStream())
                    {
                        IntPtr ptr = g.GetHdc();

                        using (Metafile meta = new Metafile(ms, ptr))
                        {
                            g.ReleaseHdc(ptr);

                            using (Graphics gfx = Graphics.FromImage(meta))
                                gfx.DrawImage(bmp, new Rectangle(0, 0, bmp.Width, bmp.Height));

                            ptr = meta.GetHenhmetafile();
                            uint size = GdipEmfToWmfBits(ptr, 0, null, 8, 0);
                            byte[] buffer = new byte[size];
                            GdipEmfToWmfBits(ptr, (uint)buffer.Length, buffer, 8, 0);
                            DeleteEnhMetaFile(ptr);

                            foreach (byte b in buffer)
                                result.Append(String.Format("{0:x2}", b));

                            result.Append("}");
                            buffer = null;
                        }
                    }
                }
            }
            catch
            {
                return String.Empty;
            }

            return result.ToString();
        }

        public static String GetRTFEmoticon(int image_index, Color back_color, Graphics richtextbox)
        {
            StringBuilder result = new StringBuilder();

            using (Bitmap bmp = new Bitmap(16, 16))
            {
                using (Graphics g = Graphics.FromImage(bmp))
                {
                    g.Clear(back_color);
                    g.DrawImage(emotic[image_index], new Point(0, 0));

                    result.Append(@"{\pict\wmetafile8\picw");
                    result.Append((int)Math.Round((16 / richtextbox.DpiX) * 2540));
                    result.Append(@"\pich");
                    result.Append((int)Math.Round((16 / richtextbox.DpiY) * 2540));
                    result.Append(@"\picwgoal");
                    result.Append((int)Math.Round((16 / richtextbox.DpiX) * 1440));
                    result.Append(@"\pichgoal");
                    result.Append((int)Math.Round((16 / richtextbox.DpiY) * 1440));
                    result.Append(" ");

                    using (MemoryStream ms = new MemoryStream())
                    {
                        IntPtr ptr = g.GetHdc();

                        using (Metafile meta = new Metafile(ms, ptr))
                        {
                            g.ReleaseHdc(ptr);

                            using (Graphics gfx = Graphics.FromImage(meta))
                                gfx.DrawImage(bmp, new Rectangle(0, 0, 16, 16));

                            ptr = meta.GetHenhmetafile();
                            uint size = GdipEmfToWmfBits(ptr, 0, null, 8, 0);
                            byte[] buffer = new byte[size];
                            GdipEmfToWmfBits(ptr, (uint)buffer.Length, buffer, 8, 0);
                            DeleteEnhMetaFile(ptr);

                            foreach (byte b in buffer)
                                result.Append(String.Format("{0:x2}", b));

                            result.Append("}");
                            buffer = null;
                        }
                    }
                }
            }

            return result.ToString();
        }

        public static int GetExEmoticonHeight(int image_index)
        {
            return ex_emotic[image_index].Height;
        }

        public static String GetRTFExEmoticon(int image_index, Color back_color, Graphics richtextbox)
        {
            StringBuilder result = new StringBuilder();
            int width = ex_emotic[image_index].Img.Width;
            int height = ex_emotic[image_index].Img.Height;

            using (Bitmap bmp = new Bitmap(width, height))
            {
                using (Graphics g = Graphics.FromImage(bmp))
                {
                    g.Clear(back_color);
                    g.DrawImage(ex_emotic[image_index].Img, new Point(0, 0));

                    result.Append(@"{\pict\wmetafile8\picw");
                    result.Append((int)Math.Round((width / richtextbox.DpiX) * 2540));
                    result.Append(@"\pich");
                    result.Append((int)Math.Round((height / richtextbox.DpiY) * 2540));
                    result.Append(@"\picwgoal");
                    result.Append((int)Math.Round((width / richtextbox.DpiX) * 1440));
                    result.Append(@"\pichgoal");
                    result.Append((int)Math.Round((height / richtextbox.DpiY) * 1440));
                    result.Append(" ");

                    using (MemoryStream ms = new MemoryStream())
                    {
                        IntPtr ptr = g.GetHdc();

                        using (Metafile meta = new Metafile(ms, ptr))
                        {
                            g.ReleaseHdc(ptr);

                            using (Graphics gfx = Graphics.FromImage(meta))
                                gfx.DrawImage(bmp, new Rectangle(0, 0, width, height));

                            ptr = meta.GetHenhmetafile();
                            uint size = GdipEmfToWmfBits(ptr, 0, null, 8, 0);
                            byte[] buffer = new byte[size];
                            GdipEmfToWmfBits(ptr, (uint)buffer.Length, buffer, 8, 0);
                            DeleteEnhMetaFile(ptr);

                            foreach (byte b in buffer)
                                result.Append(String.Format("{0:x2}", b));

                            result.Append("}");
                            buffer = null;
                        }
                    }
                }
            }

            return result.ToString();
        }

        public static Emotic FindEmoticon(String sc)
        {
            foreach (Emotic e in shortcuts)
                if (sc.StartsWith(e.Shortcut))
                    return e;

            return null;
        }

        private static void Load_emotic()
        {
            DirectoryInfo di = new DirectoryInfo(Settings.AppPath + "emoticons");
            FileInfo[] files = di.GetFiles();
            ex_emotic = new ExtendedEmoticon[files.Length];

            for (int i = 0; i < files.Length; i++)
            {
                using (MemoryStream ms = new MemoryStream(File.ReadAllBytes(files[i].FullName)))
                using (Bitmap raw = new Bitmap(ms))
                {
                    ExtendedEmoticon ex = new ExtendedEmoticon();
                    ex.Img = new Bitmap(raw.Width, raw.Height);

                    using (Graphics g = Graphics.FromImage(ex.Img))
                        g.DrawImage(raw, new Point(0, 0));

                    ex.ShortcutText = Path.GetFileNameWithoutExtension(files[i].Name).ToUpper();
                    ex.Height = ex.Img.Height;
                    ex_emotic[i] = ex;
                }
            }

            emotic = new Bitmap[49];
            int count = 0;
            Rectangle r1 = new Rectangle(0, 0, 16, 16);
            Rectangle r2 = new Rectangle(0, 0, 16, 16);

            using (Bitmap raw = Properties.Resources.emotic)
            {
                for (int y = 0; y < 7; y++)
                {
                    for (int x = 0; x < 7; x++)
                    {
                        emotic[count] = new Bitmap(16, 16);
                        r2.X = (x * 16);
                        r2.Y = (y * 16);

                        using (Graphics g = Graphics.FromImage(emotic[count]))
                            g.DrawImage(raw, r1, r2, GraphicsUnit.Pixel);

                        emotic[count].MakeTransparent(Color.Magenta);
                        count++;
                    }
                }
            }

            Emoji.Load();
        }

        public static void Load()
        {
            Load_emotic();
            Load_shortcuts();
        }

        private static void Load_shortcuts()
        {
            shortcuts = new List<Emotic>();
            shortcuts.Add(new Emotic { Index = 0, Shortcut = ":)" });
            shortcuts.Add(new Emotic { Index = 0, Shortcut = ":-)" });
            shortcuts.Add(new Emotic { Index = 1, Shortcut = ":D" });
            shortcuts.Add(new Emotic { Index = 1, Shortcut = ":-D" });
            shortcuts.Add(new Emotic { Index = 2, Shortcut = ";)" });
            shortcuts.Add(new Emotic { Index = 2, Shortcut = ";-)" });
            shortcuts.Add(new Emotic { Index = 3, Shortcut = ":O" });
            shortcuts.Add(new Emotic { Index = 3, Shortcut = ":-O" });
            shortcuts.Add(new Emotic { Index = 4, Shortcut = ":P" });
            shortcuts.Add(new Emotic { Index = 4, Shortcut = ":-P" });
            shortcuts.Add(new Emotic { Index = 5, Shortcut = "(H)" });
            shortcuts.Add(new Emotic { Index = 6, Shortcut = ":@" });
            shortcuts.Add(new Emotic { Index = 7, Shortcut = ":$" });
            shortcuts.Add(new Emotic { Index = 7, Shortcut = ":-$" });
            shortcuts.Add(new Emotic { Index = 8, Shortcut = ":S" });
            shortcuts.Add(new Emotic { Index = 8, Shortcut = ":-S" });
            shortcuts.Add(new Emotic { Index = 9, Shortcut = ":(" });
            shortcuts.Add(new Emotic { Index = 9, Shortcut = ":-(" });
            shortcuts.Add(new Emotic { Index = 10, Shortcut = ":'(" });
            shortcuts.Add(new Emotic { Index = 11, Shortcut = ":|" });
            shortcuts.Add(new Emotic { Index = 11, Shortcut = ":-|" });
            shortcuts.Add(new Emotic { Index = 12, Shortcut = "(6)" });
            shortcuts.Add(new Emotic { Index = 13, Shortcut = "(A)" });
            shortcuts.Add(new Emotic { Index = 14, Shortcut = "(L)" });
            shortcuts.Add(new Emotic { Index = 15, Shortcut = "(U)" });
            shortcuts.Add(new Emotic { Index = 16, Shortcut = "(M)" });
            shortcuts.Add(new Emotic { Index = 17, Shortcut = "(@)" });
            shortcuts.Add(new Emotic { Index = 18, Shortcut = "(&)" });
            shortcuts.Add(new Emotic { Index = 19, Shortcut = "(S)" });
            shortcuts.Add(new Emotic { Index = 20, Shortcut = "(*)" });
            shortcuts.Add(new Emotic { Index = 21, Shortcut = "(~)" });
            shortcuts.Add(new Emotic { Index = 22, Shortcut = "(E)" });
            shortcuts.Add(new Emotic { Index = 23, Shortcut = "(8)" });
            shortcuts.Add(new Emotic { Index = 24, Shortcut = "(F)" });
            shortcuts.Add(new Emotic { Index = 25, Shortcut = "(W)" });
            shortcuts.Add(new Emotic { Index = 26, Shortcut = "(O)" });
            shortcuts.Add(new Emotic { Index = 27, Shortcut = "(K)" });
            shortcuts.Add(new Emotic { Index = 28, Shortcut = "(G)" });
            shortcuts.Add(new Emotic { Index = 29, Shortcut = "(^)" });
            shortcuts.Add(new Emotic { Index = 30, Shortcut = "(P)" });
            shortcuts.Add(new Emotic { Index = 31, Shortcut = "(I)" });
            shortcuts.Add(new Emotic { Index = 32, Shortcut = "(C)" });
            shortcuts.Add(new Emotic { Index = 33, Shortcut = "(T)" });
            shortcuts.Add(new Emotic { Index = 34, Shortcut = "({)" });
            shortcuts.Add(new Emotic { Index = 35, Shortcut = "(})" });
            shortcuts.Add(new Emotic { Index = 36, Shortcut = "(B)" });
            shortcuts.Add(new Emotic { Index = 37, Shortcut = "(D)" });
            shortcuts.Add(new Emotic { Index = 38, Shortcut = "(Z)" });
            shortcuts.Add(new Emotic { Index = 39, Shortcut = "(X)" });
            shortcuts.Add(new Emotic { Index = 40, Shortcut = "(Y)" });
            shortcuts.Add(new Emotic { Index = 41, Shortcut = "(N)" });
            shortcuts.Add(new Emotic { Index = 42, Shortcut = ":[" });
            shortcuts.Add(new Emotic { Index = 42, Shortcut = ":-[" });
            shortcuts.Add(new Emotic { Index = 43, Shortcut = "(1)" });
            shortcuts.Add(new Emotic { Index = 44, Shortcut = "(2)" });
            shortcuts.Add(new Emotic { Index = 45, Shortcut = "(3)" });
            shortcuts.Add(new Emotic { Index = 46, Shortcut = "(4)" });
        }
    }

    class Emotic
    {
        public String Shortcut { get; set; }
        public int Index { get; set; }
    }
}
