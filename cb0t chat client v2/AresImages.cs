using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace cb0t_chat_client_v2
{
    class AresImages
    {
        public static Bitmap ChatTabIcon;
        public static Bitmap ControlPanelTabIcon;
        public static Bitmap AudioPlayerIcon;
        public static Bitmap IRCIcon;

        public static Bitmap ChannelList_Refresh;
        public static Bitmap ChannelList_Search;

        public static Bitmap PM_Unread;
        public static Bitmap Chat_Read;
        public static Bitmap Chat_Unread;
        public static Bitmap PM_Read;
        public static Bitmap ChannelList;
        public static Bitmap Chat;
        public static Bitmap RedStar_NoFiles;
        public static Bitmap RedStar_Files;
        public static Bitmap BlueStar_NoFiles;
        public static Bitmap BlueStar_Files;
        public static Bitmap GoldStar_NoFiles;
        public static Bitmap GoldStar_Files;
        public static Bitmap Mime_Other;
        public static Bitmap Mime_Audio;
        public static Bitmap Mime_Video;
        public static Bitmap Mime_Picture;
        public static Bitmap Mime_Software;
        public static Bitmap Mime_Document;
        public static Bitmap Files_Closed;
        public static Bitmap GreenStar_NoFiles;
        public static Bitmap GreenStar_Files;
        public static Bitmap Files;
        public static Bitmap GrayStar_NoFiles;
        public static Bitmap GrayStar_Files;

        public static Bitmap Upload;
        public static Bitmap Download;
        public static Bitmap DCBlock;

        public static Bitmap Writing;

        public static Bitmap mini_busy;
        public static Bitmap mini_away;
        public static Bitmap mini_sleep;
        public static Bitmap mini_ignore;

        public static Bitmap[] TransparentEmoticons = new Bitmap[49];

        public static Bitmap tab_close;
        public static Bitmap tab_close_hidden;

        public static void LoadLargeImages()
        {
            String path = AppDomain.CurrentDomain.BaseDirectory + "graphics";

            if (!Directory.Exists(path))
                return;

            try
            {
                byte[] buf1 = File.ReadAllBytes(path + "/tabsbig.bmp");

                using (MemoryStream ms = new MemoryStream(buf1))
                {
                    using (Bitmap b = new Bitmap(ms))
                    {
                        ChatTabIcon = new Bitmap(32, 32);
                        using (Graphics g = Graphics.FromImage(ChatTabIcon))
                            g.DrawImage(b, new Rectangle(0, 0, 32, 32), new Rectangle(0, 0, 32, 32), GraphicsUnit.Pixel);

                        ControlPanelTabIcon = new Bitmap(32, 32);
                        using (Graphics g = Graphics.FromImage(ControlPanelTabIcon))
                            g.DrawImage(b, new Rectangle(0, 0, 32, 32), new Rectangle(32, 0, 32, 32), GraphicsUnit.Pixel);

                        AudioPlayerIcon = new Bitmap(32, 32);
                        using (Graphics g = Graphics.FromImage(AudioPlayerIcon))
                            g.DrawImage(b, new Rectangle(0, 0, 32, 32), new Rectangle(64, 0, 32, 32), GraphicsUnit.Pixel);

                        IRCIcon = new Bitmap(32, 32);
                        using (Graphics g = Graphics.FromImage(IRCIcon))
                            g.DrawImage(b, new Rectangle(0, 0, 32, 32), new Rectangle(96, 0, 32, 32), GraphicsUnit.Pixel);
                    }
                }
            }
            catch { }
        }

        public static void LoadMiniImages()
        {
            String path = AppDomain.CurrentDomain.BaseDirectory + "graphics";

            if (!Directory.Exists(path))
                return;

            try
            {
                byte[] buf1 = File.ReadAllBytes(path + "/minis.bmp");

                using (MemoryStream ms = new MemoryStream(buf1))
                {
                    using (Bitmap b = new Bitmap(ms))
                    {
                        mini_sleep = new Bitmap(10, 10);
                        using (Graphics g = Graphics.FromImage(mini_sleep))
                        {
                            g.DrawImage(b, new Rectangle(0, 0, 10, 10), new Rectangle(0, 0, 10, 10), GraphicsUnit.Pixel);
                            mini_sleep.MakeTransparent(Color.Magenta);
                        }

                        mini_away = new Bitmap(10, 10);
                        using (Graphics g = Graphics.FromImage(mini_away))
                        {
                            g.DrawImage(b, new Rectangle(0, 0, 10, 10), new Rectangle(10, 0, 10, 10), GraphicsUnit.Pixel);
                            mini_away.MakeTransparent(Color.Magenta);
                        }

                        mini_busy = new Bitmap(10, 10);
                        using (Graphics g = Graphics.FromImage(mini_busy))
                        {
                            g.DrawImage(b, new Rectangle(0, 0, 10, 10), new Rectangle(20, 0, 10, 10), GraphicsUnit.Pixel);
                            mini_busy.MakeTransparent(Color.Magenta);
                        }

                        mini_ignore = new Bitmap(10, 10);
                        using (Graphics g = Graphics.FromImage(mini_ignore))
                        {
                            g.DrawImage(b, new Rectangle(0, 0, 10, 10), new Rectangle(30, 0, 10, 10), GraphicsUnit.Pixel);
                            mini_ignore.MakeTransparent(Color.Magenta);
                        }
                    }
                }
            }
            catch { }
        }

        public static void LoadChannelListImages()
        {
            String path = AppDomain.CurrentDomain.BaseDirectory + "graphics";

            if (!Directory.Exists(path))
                return;

            try
            {
                byte[] buf1 = File.ReadAllBytes(path + "/tabssmall.bmp");

                using (MemoryStream ms = new MemoryStream(buf1))
                {
                    using (Bitmap b = new Bitmap(ms))
                    {
                        ChannelList_Refresh = new Bitmap(20, 20);
                        using (Graphics g = Graphics.FromImage(ChannelList_Refresh))
                            g.DrawImage(b, new Rectangle(0, 0, 20, 20), new Rectangle(0, 0, 20, 20), GraphicsUnit.Pixel);

                        ChannelList_Search = new Bitmap(20, 20);
                        using (Graphics g = Graphics.FromImage(ChannelList_Search))
                            g.DrawImage(b, new Rectangle(0, 0, 20, 20), new Rectangle(20, 0, 20, 20), GraphicsUnit.Pixel);
                    }
                }
            }
            catch { }
        }

        public static void LoadChatImages()
        {
            String path = AppDomain.CurrentDomain.BaseDirectory + "graphics";

            if (!Directory.Exists(path))
                return;

            try
            {
                byte[] buf1 = File.ReadAllBytes(path + "/chat.bmp");

                using (MemoryStream ms = new MemoryStream(buf1))
                {
                    using (Bitmap b = new Bitmap(ms))
                    {
                        PM_Unread = new Bitmap(16, 16);
                        using (Graphics g = Graphics.FromImage(PM_Unread))
                            g.DrawImage(b, new Rectangle(0, 0, 16, 16), new Rectangle(0, 0, 16, 16), GraphicsUnit.Pixel);
                        Chat_Read = new Bitmap(16, 16);
                        using (Graphics g = Graphics.FromImage(Chat_Read))
                        {
                            g.DrawImage(b, new Rectangle(0, 0, 16, 16), new Rectangle(16, 0, 16, 16), GraphicsUnit.Pixel);
                            Chat_Read.MakeTransparent(Color.Magenta);
                            Chat_Unread = new Bitmap(16, 16);
                        }
                        using (Graphics g = Graphics.FromImage(Chat_Unread))
                        {
                            g.DrawImage(b, new Rectangle(0, 0, 16, 16), new Rectangle(32, 0, 16, 16), GraphicsUnit.Pixel);
                            Chat_Unread.MakeTransparent(Color.Magenta);
                        }
                        PM_Read = new Bitmap(16, 16);
                        using (Graphics g = Graphics.FromImage(PM_Read))
                            g.DrawImage(b, new Rectangle(0, 0, 16, 16), new Rectangle(48, 0, 16, 16), GraphicsUnit.Pixel);
                        ChannelList = new Bitmap(16, 16);
                        using (Graphics g = Graphics.FromImage(ChannelList))
                        {
                            g.DrawImage(b, new Rectangle(0, 0, 16, 16), new Rectangle(64, 0, 16, 16), GraphicsUnit.Pixel);
                            ChannelList.MakeTransparent(Color.Magenta);
                        }
                        Chat = new Bitmap(16, 16);
                        using (Graphics g = Graphics.FromImage(Chat))
                        {
                            g.DrawImage(b, new Rectangle(0, 0, 16, 16), new Rectangle(80, 0, 16, 16), GraphicsUnit.Pixel);
                            Chat.MakeTransparent(Color.Magenta);
                        }
                        RedStar_NoFiles = new Bitmap(16, 16);
                        using (Graphics g = Graphics.FromImage(RedStar_NoFiles))
                        {
                            g.DrawImage(b, new Rectangle(0, 0, 16, 16), new Rectangle(0, 16, 16, 16), GraphicsUnit.Pixel);
                            RedStar_NoFiles.MakeTransparent(Color.Magenta);
                        }
                        RedStar_Files = new Bitmap(16, 16);
                        using (Graphics g = Graphics.FromImage(RedStar_Files))
                        {
                            g.DrawImage(b, new Rectangle(0, 0, 16, 16), new Rectangle(16, 16, 16, 16), GraphicsUnit.Pixel);
                            RedStar_Files.MakeTransparent(Color.Magenta);
                        }
                        BlueStar_NoFiles = new Bitmap(16, 16);
                        using (Graphics g = Graphics.FromImage(BlueStar_NoFiles))
                        {
                            g.DrawImage(b, new Rectangle(0, 0, 16, 16), new Rectangle(32, 16, 16, 16), GraphicsUnit.Pixel);
                            BlueStar_NoFiles.MakeTransparent(Color.Magenta);
                        }
                        BlueStar_Files = new Bitmap(16, 16);
                        using (Graphics g = Graphics.FromImage(BlueStar_Files))
                        {
                            g.DrawImage(b, new Rectangle(0, 0, 16, 16), new Rectangle(48, 16, 16, 16), GraphicsUnit.Pixel);
                            BlueStar_Files.MakeTransparent(Color.Magenta);
                        }
                        GoldStar_NoFiles = new Bitmap(16, 16);
                        using (Graphics g = Graphics.FromImage(GoldStar_NoFiles))
                            g.DrawImage(b, new Rectangle(0, 0, 16, 16), new Rectangle(64, 16, 16, 16), GraphicsUnit.Pixel);
                        GoldStar_Files = new Bitmap(16, 16);
                        using (Graphics g = Graphics.FromImage(GoldStar_Files))
                            g.DrawImage(b, new Rectangle(0, 0, 16, 16), new Rectangle(80, 16, 16, 16), GraphicsUnit.Pixel);
                        Mime_Other = new Bitmap(16, 16);
                        using (Graphics g = Graphics.FromImage(Mime_Other))
                            g.DrawImage(b, new Rectangle(0, 0, 16, 16), new Rectangle(0, 32, 16, 16), GraphicsUnit.Pixel);
                        Mime_Audio = new Bitmap(16, 16);
                        using (Graphics g = Graphics.FromImage(Mime_Audio))
                            g.DrawImage(b, new Rectangle(0, 0, 16, 16), new Rectangle(16, 32, 16, 16), GraphicsUnit.Pixel);
                        Mime_Video = new Bitmap(16, 16);
                        using (Graphics g = Graphics.FromImage(Mime_Video))
                            g.DrawImage(b, new Rectangle(0, 0, 16, 16), new Rectangle(32, 32, 16, 16), GraphicsUnit.Pixel);
                        Mime_Picture = new Bitmap(16, 16);
                        using (Graphics g = Graphics.FromImage(Mime_Picture))
                            g.DrawImage(b, new Rectangle(0, 0, 16, 16), new Rectangle(48, 32, 16, 16), GraphicsUnit.Pixel);
                        Mime_Software = new Bitmap(16, 16);
                        using (Graphics g = Graphics.FromImage(Mime_Software))
                            g.DrawImage(b, new Rectangle(0, 0, 16, 16), new Rectangle(64, 32, 16, 16), GraphicsUnit.Pixel);
                        Mime_Document = new Bitmap(16, 16);
                        using (Graphics g = Graphics.FromImage(Mime_Document))
                            g.DrawImage(b, new Rectangle(0, 0, 16, 16), new Rectangle(80, 32, 16, 16), GraphicsUnit.Pixel);
                        Files_Closed = new Bitmap(16, 16);
                        using (Graphics g = Graphics.FromImage(Files_Closed))
                        {
                            g.DrawImage(b, new Rectangle(0, 0, 16, 16), new Rectangle(0, 48, 16, 16), GraphicsUnit.Pixel);
                            Files_Closed.MakeTransparent(Color.Magenta);
                        }
                        GreenStar_NoFiles = new Bitmap(16, 16);
                        using (Graphics g = Graphics.FromImage(GreenStar_NoFiles))
                        {
                            g.DrawImage(b, new Rectangle(0, 0, 16, 16), new Rectangle(16, 48, 16, 16), GraphicsUnit.Pixel);
                            GreenStar_NoFiles.MakeTransparent(Color.Magenta);
                        }
                        GreenStar_Files = new Bitmap(16, 16);
                        using (Graphics g = Graphics.FromImage(GreenStar_Files))
                        {
                            g.DrawImage(b, new Rectangle(0, 0, 16, 16), new Rectangle(32, 48, 16, 16), GraphicsUnit.Pixel);
                            GreenStar_Files.MakeTransparent(Color.Magenta);
                        }

                        Files = new Bitmap(16, 16);
                        using (Graphics g = Graphics.FromImage(Files))
                            g.DrawImage(b, new Rectangle(0, 0, 16, 16), new Rectangle(48, 48, 16, 16), GraphicsUnit.Pixel);
                        GrayStar_NoFiles = new Bitmap(16, 16);
                        using (Graphics g = Graphics.FromImage(GrayStar_NoFiles))
                        {
                            g.DrawImage(b, new Rectangle(0, 0, 16, 16), new Rectangle(64, 48, 16, 16), GraphicsUnit.Pixel);
                            GrayStar_NoFiles.MakeTransparent(Color.Magenta);
                        }
                        GrayStar_Files = new Bitmap(16, 16);
                        using (Graphics g = Graphics.FromImage(GrayStar_Files))
                        {
                            g.DrawImage(b, new Rectangle(0, 0, 16, 16), new Rectangle(80, 48, 16, 16), GraphicsUnit.Pixel);
                            GrayStar_Files.MakeTransparent(Color.Magenta);
                        }
                        Upload = new Bitmap(16, 16);
                        using (Graphics g = Graphics.FromImage(Upload))
                            g.DrawImage(b, new Rectangle(0, 0, 16, 16), new Rectangle(0, 64, 16, 16), GraphicsUnit.Pixel);
                        Download = new Bitmap(16, 16);
                        using (Graphics g = Graphics.FromImage(Download))
                            g.DrawImage(b, new Rectangle(0, 0, 16, 16), new Rectangle(16, 64, 16, 16), GraphicsUnit.Pixel);
                        DCBlock = new Bitmap(16, 16);
                        using (Graphics g = Graphics.FromImage(DCBlock))
                            g.DrawImage(b, new Rectangle(0, 0, 16, 16), new Rectangle(32, 64, 16, 16), GraphicsUnit.Pixel);
                        Writing = new Bitmap(16, 16);
                        using (Graphics g = Graphics.FromImage(Writing))
                        {
                            g.DrawImage(b, new Rectangle(0, 0, 16, 16), new Rectangle(48, 64, 16, 16), GraphicsUnit.Pixel);
                            Writing.MakeTransparent(Color.Magenta);
                        }
                        tab_close = new Bitmap(16, 16);
                        using (Graphics g = Graphics.FromImage(tab_close))
                        {
                            g.DrawImage(b, new Rectangle(0, 0, 16, 16), new Rectangle(64, 64, 16, 16), GraphicsUnit.Pixel);
                            tab_close.MakeTransparent(Color.Magenta);
                        }
                        tab_close_hidden = new Bitmap(16, 16);
                        using (Graphics g = Graphics.FromImage(tab_close_hidden))
                        {
                            g.DrawImage(b, new Rectangle(0, 0, 16, 16), new Rectangle(80, 64, 16, 16), GraphicsUnit.Pixel);
                            tab_close_hidden.MakeTransparent(Color.Magenta);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                System.Windows.Forms.MessageBox.Show(e.Message, "error loading large images");
            }
        }

        public static void LoadEmoticons()
        {
            String path = AppDomain.CurrentDomain.BaseDirectory + "graphics";

            if (!Directory.Exists(path))
                return;

            try
            {
                byte[] buf1 = File.ReadAllBytes(path + "/emotic.bmp");

                using (MemoryStream ms = new MemoryStream(buf1))
                {
                    using (Bitmap emoticons = new Bitmap(ms))
                    {
                        for (int i = 0; i < TransparentEmoticons.Length; i++)
                        {
                            TransparentEmoticons[i] = new Bitmap(16, 16);
                            Graphics g2 = Graphics.FromImage(TransparentEmoticons[i]);
                            g2.DrawImage(emoticons, new Rectangle(0, 0, 16, 16), EmoticonRectangle(i), GraphicsUnit.Pixel);
                            TransparentEmoticons[i].MakeTransparent(Color.Magenta);
                        }
                    }
                }
            }
            catch { }
        }

        private static Rectangle EmoticonRectangle(int array_position)
        {
            int x, y, z;

            z = array_position + 1;
            y = (int)Math.Floor((double)z / 7);
            x = z - (y * 7);
            x--;
            x *= 16;
            y *= 16;

            if (x < 0)
            {
                x += 112;
                y -= 16;
            }

            return new Rectangle(x, y, 16, 16);
        }

    }
}
