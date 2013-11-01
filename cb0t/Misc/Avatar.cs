using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;

namespace cb0t
{
    class Avatar
    {
        public static byte[] Data { get; set; }
        public static Bitmap Image { get; set; }

        public static void Load()
        {
            try
            {
                String path = Settings.DataPath + "avatar_big.dat";

                if (!File.Exists(path))
                    return;

                byte[] buf = File.ReadAllBytes(path);

                using (MemoryStream ms = new MemoryStream(buf))
                    Image = new Bitmap(ms);

                buf = null;
                path = Settings.DataPath + "avatar_small.dat";

                if (!File.Exists(path))
                {
                    Image.Dispose();
                    Image = null;
                    return;
                }

                Data = File.ReadAllBytes(path);
            }
            catch
            {
                if (Image != null)
                {
                    Image.Dispose();
                    Image = null;
                }

                Data = null;
            }
        }

        public static void Update(String path)
        {
            try
            {
                byte[] buf = File.ReadAllBytes(path);

                using (MemoryStream org_ms = new MemoryStream(buf))
                using (Bitmap org = new Bitmap(org_ms))
                {
                    using (Bitmap big_bmp = new Bitmap(70, 70))
                    using (Graphics big_gfx = Graphics.FromImage(big_bmp))
                    {
                        big_gfx.Clear(Color.White);
                        big_gfx.CompositingQuality = CompositingQuality.HighQuality;
                        big_gfx.InterpolationMode = InterpolationMode.HighQualityBilinear;
                        big_gfx.SmoothingMode = SmoothingMode.HighQuality;
                        big_gfx.DrawImage(org, new RectangleF(0, 0, 70, 70));

                        if (Image != null)
                        {
                            Image.Dispose();
                            Image = null;
                        }

                        Image = (Bitmap)big_bmp.Clone();

                        using (MemoryStream big_ms = new MemoryStream())
                        {
                            big_bmp.Save(big_ms, ImageFormat.Bmp);
                            File.WriteAllBytes(Settings.DataPath + "avatar_big.dat", big_ms.ToArray());
                        }
                    }

                    using (Bitmap small_bmp = new Bitmap(48, 48))
                    using (Graphics small_gfx = Graphics.FromImage(small_bmp))
                    {
                        small_gfx.Clear(Color.White);
                        small_gfx.CompositingQuality = CompositingQuality.HighQuality;
                        small_gfx.InterpolationMode = InterpolationMode.HighQualityBilinear;
                        small_gfx.SmoothingMode = SmoothingMode.HighQuality;
                        small_gfx.DrawImage(org, new RectangleF(0, 0, 48, 48));

                        using (MemoryStream small_ms = new MemoryStream())
                        {
                            ImageCodecInfo info = ImageCodecInfo.GetImageEncoders().FirstOrDefault(x => x.MimeType == "image/jpeg");

                            using (EncoderParameters encoding = new EncoderParameters())
                            {
                                encoding.Param[0] = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, 75L);
                                small_bmp.Save(small_ms, info, encoding);
                                Data = small_ms.ToArray();
                                File.WriteAllBytes(Settings.DataPath + "avatar_small.dat", Data);
                                encoding.Param[0].Dispose();
                            }

                            info = null;
                        }
                    }
                }

                buf = null;
            }
            catch
            {
                if (Image != null)
                {
                    Image.Dispose();
                    Image = null;
                }

                Data = null;
            }
        }

        public static void Clear()
        {
            try
            {
                if (Image != null)
                {
                    Image.Dispose();
                    Image = null;
                }

                Data = null;

                if (File.Exists(Settings.DataPath + "avatar_big.dat"))
                    File.Delete(Settings.DataPath + "avatar_big.dat");

                if (File.Exists(Settings.DataPath + "avatar_small.dat"))
                    File.Delete(Settings.DataPath + "avatar_small.dat");
            }
            catch { }
        }
    }
}
