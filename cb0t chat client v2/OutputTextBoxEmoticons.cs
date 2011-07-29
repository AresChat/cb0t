using System;
using System.Text;
using System.Runtime.InteropServices;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace cb0t_chat_client_v2
{
    class OutputTextBoxEmoticons
    {
   //     private static RichTextBox rtf_eval = new RichTextBox();

      /* public static String GetRTFEmoticon(int image_index, Color back_color, Graphics richtextbox)
        {
            String rtf = String.Empty;

            using (Bitmap image = new Bitmap(16, 16, PixelFormat.Format24bppRgb))
            {
                using (Graphics g = Graphics.FromImage(image))
                {
                    using (SolidBrush brush = new SolidBrush(back_color)) // setup emoticon background
                        g.FillRectangle(brush, new Rectangle(0, 0, 16, 16));

                    g.DrawImage(AresImages.TransparentEmoticons[image_index], new Point(0, 0)); // put the emoticon on top of the background

                    using (MemoryStream stream = new MemoryStream())
                    {
                        float xDpi = richtextbox.DpiX;
                        float yDpi = richtextbox.DpiY;

                        image.Save(stream, ImageFormat.Bmp);
                        stream.Position = 14;

                        StringBuilder sb = new StringBuilder();

                        while (stream.Position < stream.Length)
                            sb.Append(String.Format("{0:x2}", stream.ReadByte()));

                        string bits = sb.ToString();

                        if (!bits.Contains("c40e0000"))
                            return String.Empty;

                        string DIB = "010009000003B80100000000A20100000000050000000B0200000000050000000C02A701A701A2010000430F2000CC0000001000100000000000A701A70100000000280000001000000010000000010018000000000000030000".ToLower();
                        DIB += bits.Substring(bits.IndexOf("c40e0000")) + "030000000000";

                        sb.Length = 0;

                        sb.Append(@"{\pict\wmetafile8\picw");
                        sb.Append((int)Math.Round((image.Width / xDpi) * 2540));
                        sb.Append(@"\pich");
                        sb.Append((int)Math.Round((image.Height / yDpi) * 2540));
                        sb.Append(@"\picwgoal");
                        sb.Append((int)Math.Round((image.Width / xDpi) * 1440));
                        sb.Append(@"\pichgoal");
                        sb.Append((int)Math.Round((image.Height / yDpi) * 1440));
                        sb.Append(" ");
                        sb.Append(DIB);

                        rtf = sb.Append("}").ToString();
                    }
                }
            }

            return rtf;
        }*/

        [DllImport("gdiplus.dll")]
        private static extern uint GdipEmfToWmfBits(IntPtr hEmf, uint uBufferSize, byte[] bBuffer, int iMappingMode, uint flags);

        [DllImport("gdi32.dll")]
        private static extern bool DeleteEnhMetaFile(IntPtr hemf);

        public static String GetRTFEmoticon(int image_index, Color back_color, Graphics richtextbox)
        {
            StringBuilder result = new StringBuilder();

            using (Bitmap bmp = new Bitmap(16, 16))
            {
                using (Graphics g = Graphics.FromImage(bmp))
                {
                    g.Clear(back_color);
                    g.DrawImage(AresImages.TransparentEmoticons[image_index], new Point(0, 0));
                    
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

        public static String GetRTFScribble(Bitmap image, Graphics richtextbox)
        {
            StringBuilder result = new StringBuilder();
            result.Append(@"{\rtf");
            result.Append(@"{\pict\wmetafile8\picw");
            result.Append((int)Math.Round((image.Width / richtextbox.DpiX) * 2540));
            result.Append(@"\pich");
            result.Append((int)Math.Round((image.Height / richtextbox.DpiY) * 2540));
            result.Append(@"\picwgoal");
            result.Append((int)Math.Round((image.Width / richtextbox.DpiX) * 1440));
            result.Append(@"\pichgoal");
            result.Append((int)Math.Round((image.Height / richtextbox.DpiY) * 1440));
            result.Append(" ");

            using (MemoryStream ms = new MemoryStream())
            {
                using (Graphics g = Graphics.FromImage(image))
                {
                    IntPtr ptr = g.GetHdc();

                    using (Metafile meta = new Metafile(ms, ptr))
                    {
                        g.ReleaseHdc(ptr);

                        using (Graphics gfx = Graphics.FromImage(meta))
                            gfx.DrawImage(image, new Rectangle(0, 0, image.Width, image.Height));

                        ptr = meta.GetHenhmetafile();
                        uint size = GdipEmfToWmfBits(ptr, 0, null, 8, 0);
                        byte[] buffer = new byte[size];
                        GdipEmfToWmfBits(ptr, (uint)buffer.Length, buffer, 8, 0);
                        DeleteEnhMetaFile(ptr);

                        foreach (byte b in buffer)
                            result.Append(String.Format("{0:x2}", b));

                        result.Append("}}");
                        buffer = null;
                    }
                }
            }

            return result.ToString();
        }

        public static String GetRTFCustomEmote(CEmoteItem cemote, Graphics richtextbox)
        {
            StringBuilder result = new StringBuilder();
            result.Append(@"{\pict\wmetafile8\picw");
            result.Append((int)Math.Round((cemote.Size / richtextbox.DpiX) * 2540));
            result.Append(@"\pich");
            result.Append((int)Math.Round((cemote.Size / richtextbox.DpiY) * 2540));
            result.Append(@"\picwgoal");
            result.Append((int)Math.Round((cemote.Size / richtextbox.DpiX) * 1440));
            result.Append(@"\pichgoal");
            result.Append((int)Math.Round((cemote.Size / richtextbox.DpiY) * 1440));
            result.Append(" ");

            using (MemoryStream cm_ms = new MemoryStream(cemote.Image))
            {
                using (Bitmap cm_bmp = new Bitmap(cm_ms))
                {
                    using (MemoryStream ms = new MemoryStream())
                    {
                        using (Graphics g = Graphics.FromImage(cm_bmp))
                        {
                            IntPtr ptr = g.GetHdc();

                            using (Metafile meta = new Metafile(ms, ptr))
                            {
                                g.ReleaseHdc(ptr);

                                using (Graphics gfx = Graphics.FromImage(meta))
                                    gfx.DrawImage(cm_bmp, new Rectangle(0, 0, cemote.Size, cemote.Size));

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
            }

            return result.ToString();
        }

    }
}
