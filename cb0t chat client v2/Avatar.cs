using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using ZLib;

namespace cb0t_chat_client_v2
{
    class Avatar
    {
        public static Bitmap avatar_big;
        public static byte[] avatar_small;
        public static byte[] def_av;

        public static List<byte[]> avatar_dc;

        public static void LoadRegularAv()
        {
            try
            {
                def_av = File.ReadAllBytes(AppDomain.CurrentDomain.BaseDirectory + "graphics/def_av");
            }
            catch { }
        }

        public static void Load()
        {
            byte[] buf1;
            Graphics g;

            try
            {
                buf1 = File.ReadAllBytes(Settings.folder_path + "avdat1.dat");
                buf1 = Zlib.Decompress(buf1, false);
                avatar_big = new Bitmap(new MemoryStream(buf1));
            }
            catch
            {
                avatar_big = new Bitmap(94, 94);
                g = Graphics.FromImage(avatar_big);
                g.FillRectangle(Brushes.Silver, new Rectangle(0, 0, 94, 94));
                g.DrawRectangle(new Pen(Brushes.Black, 1), new Rectangle(0, 0, 93, 93));
            }

            try
            {
                avatar_small = File.ReadAllBytes(Settings.folder_path + "avdat2.dat");
                avatar_small = Zlib.Decompress(avatar_small, false);
            }
            catch
            {
                avatar_small = null;
            }

            UpdateDCAvatar();
        }

        public static void Clear()
        {
            try
            {
                File.Delete(Settings.folder_path + "avdat1.dat");
            }
            catch { }

            try
            {
                File.Delete(Settings.folder_path + "avdat2.dat");
            }
            catch { }

            avatar_big = new Bitmap(94, 94);
            Graphics g = Graphics.FromImage(avatar_big);
            g.FillRectangle(Brushes.Silver, new Rectangle(0, 0, 94, 94));
            g.DrawRectangle(new Pen(Brushes.Black, 1), new Rectangle(0, 0, 93, 93));
            avatar_small = null;
            UpdateDCAvatar();            
        }

        public static void Update(String path)
        {
            try
            {
                byte[] raw = File.ReadAllBytes(path);
                Bitmap avatar_raw = new Bitmap(new MemoryStream(raw));
                Bitmap avater_sized = new Bitmap(94, 94);
                Graphics g = Graphics.FromImage(avater_sized);
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                g.DrawImage(avatar_raw, new RectangleF(0, 0, 94, 94));
                g.DrawRectangle(new Pen(Brushes.Black, 1), new Rectangle(0, 0, 93, 93));
                MemoryStream stream = new MemoryStream();
                avater_sized.Save(stream, ImageFormat.Jpeg);
                byte[] buf1 = stream.ToArray();
                avatar_big = new Bitmap(new MemoryStream(buf1));
                buf1 = Zlib.Compress(buf1);
                File.WriteAllBytes(Settings.folder_path + "avdat1.dat", buf1);
                avater_sized = new Bitmap(48, 48);
                g = Graphics.FromImage(avater_sized);
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                g.DrawImage(avatar_raw, new RectangleF(0, 0, 48, 48));
                stream = new MemoryStream();
                avater_sized.Save(stream, ImageFormat.Jpeg);
                avatar_small = stream.ToArray();
                buf1 = Zlib.Compress(avatar_small);
                File.WriteAllBytes(Settings.folder_path + "avdat2.dat", buf1);
            }
            catch
            {
                avatar_big = new Bitmap(94, 94);
                Graphics g = Graphics.FromImage(avatar_big);
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                g.FillRectangle(Brushes.Silver, new Rectangle(0, 0, 94, 94));
                g.DrawRectangle(new Pen(Brushes.Black, 1), new Rectangle(0, 0, 93, 93));
                avatar_small = null;
            }

            UpdateDCAvatar();
        }

        private static void UpdateDCAvatar()
        {
            avatar_dc = new List<byte[]>();
            MemoryStream stream = new MemoryStream();
            avatar_big.Save(stream, ImageFormat.Jpeg);
            byte[] buf1 = stream.ToArray();
            List<byte> buf2 = new List<byte>(buf1);

            while (buf2.Count > 512)
            {
                buf1 = new byte[512];
                Array.Copy(buf2.ToArray(), 0, buf1, 0, 512);
                avatar_dc.Add(buf1);
                buf2.RemoveRange(0, 512);
            }

            if (buf2.Count > 0)
                avatar_dc.Add(buf2.ToArray());
        }
    }
}
