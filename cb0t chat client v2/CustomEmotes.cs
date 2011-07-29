using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using Ares.PacketHandlers;

namespace cb0t_chat_client_v2
{
    class CustomEmotes
    {
        public static CEmoteItem[] Emotes = new CEmoteItem[16];

        public static void Load()
        {
            for (int i = 0; i < Emotes.Length; i++)
                Emotes[i] = new CEmoteItem();

            String path = Settings.folder_path + "customemotes.dat";

            if (File.Exists(path))
            {
                try
                {
                    AresDataPacket list = new AresDataPacket(File.ReadAllBytes(path));
                    int position = 0;

                    while (true)
                    {
                        CEmoteItem item = new CEmoteItem();
                        item.Shortcut = list.ReadString();
                        item.Size = list.ReadByte();
                        ushort size = list.ReadInt16();
                        item.Image = list.ReadBytes(size);
                        Emotes[position++] = item;
                        
                        if (position >= 16 || list.Remaining() == 0)
                            break;
                    }

                    list = null;
                }
                catch { }
            }
        }

        public static void Save()
        {
            String path = Settings.folder_path + "customemotes.dat";

            try
            {
                AresDataPacket list = new AresDataPacket();

                for (int i = 0; i < Emotes.Length; i++)
                {
                    if (Emotes[i].Image != null)
                    {
                        list.WriteString(Emotes[i].Shortcut);
                        list.WriteByte((byte)Emotes[i].Size);
                        list.WriteInt16((ushort)Emotes[i].Image.Length);
                        list.WriteBytes(Emotes[i].Image);
                    }
                }

                File.WriteAllBytes(path, list.ToByteArray());
                list = null;
            }
            catch {}
        }

        public static void AddItem(CEmoteItem item)
        {
            for (int i = 0; i < Emotes.Length; i++)
            {
                if (Emotes[i].Image == null)
                {
                    Emotes[i] = item;
                    break;
                }
            }
        }

        public static void RemoveItem(String shortcut)
        {
            for (int i = 0; i < Emotes.Length; i++)
            {
                if (Emotes[i].Image != null)
                {
                    if (Emotes[i].Shortcut == shortcut)
                    {
                        Emotes[i] = new CEmoteItem();
                        break;
                    }
                }
            }
        }

        public static void ImportEmoticon(ref byte[] data, out int size)
        {
            String path = null;

            using (OpenFileDialog fd = new OpenFileDialog())
            {
                fd.Multiselect = false;
                fd.Filter = "Supported file types (*.bmp;*.jpg;*.png)|*.bmp;*.jpg;*.png|Bitmap files (*.bmp)|*.bmp|Jpeg files (*.jpg)|*.jpg|PNG files (*.png)|*.png";

                if (fd.ShowDialog() == DialogResult.OK)
                    path = fd.FileName;
            }

            if (path != null)
            {
                using (CEmoticonSizeA ce = new CEmoticonSizeA())
                {
                    ce.ShowDialog();
                    size = ce.EmoticonSize;
                }

                try
                {
                    byte[] raw = File.ReadAllBytes(path);

                    using (MemoryStream raw_ms = new MemoryStream(raw))
                    {
                        using (Bitmap raw_bmp = new Bitmap(raw_ms))
                        {
                            using (Bitmap emote_bmp = new Bitmap(size, size))
                            {
                                using (Graphics g = Graphics.FromImage(emote_bmp))
                                {
                                    g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                                    g.CompositingQuality = CompositingQuality.HighQuality;
                                    g.SmoothingMode = SmoothingMode.HighQuality;

                                    int _w, _h, _sx, _sy;
                                    float _wp, _hp;

                                    if (raw_bmp.Width > raw_bmp.Height)
                                    {
                                        _w = size;
                                        _wp = (float)size / raw_bmp.Width;
                                        _h = Convert.ToInt32(raw_bmp.Height * _wp);
                                    }
                                    else
                                    {
                                        _h = size;
                                        _hp = (float)size / raw_bmp.Height;
                                        _w = Convert.ToInt32(raw_bmp.Width * _hp);
                                    }

                                    if (_w != _h)
                                    {
                                        if (_w > _h)
                                        {
                                            _sx = 0;
                                            _sy = (size - _h) - 1;
                                        }
                                        else
                                        {
                                            _sx = (size - _w) / 2;
                                            _sy = 0;
                                        }

                                        g.Clear(Color.White);
                                    }
                                    else
                                    {
                                        _sx = 0;
                                        _sy = 0;
                                    }

                                    g.DrawImage(raw_bmp, new Rectangle(_sx, _sy, _w, _h));

                                    ImageCodecInfo info = new List<ImageCodecInfo>(ImageCodecInfo.GetImageEncoders()).Find(
                                        delegate(ImageCodecInfo i)
                                        {
                                            return i.MimeType == "image/jpeg";
                                        });

                                    EncoderParameters encoding = new EncoderParameters();
                                    encoding.Param[0] = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, 100L);

                                    using (MemoryStream ms = new MemoryStream())
                                    {
                                        emote_bmp.Save(ms, info, encoding);
                                        data = ms.ToArray();
                                    }
                                }
                            }
                        }
                    }

                    return;
                }
                catch { }
            }

            size = -1;
        }

    }
}
