using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Drawing;

namespace cb0t_chat_client_v2
{
    class AudioControllerImages
    {
        public void Load()
        {
            try
            {
                this.LoadLarge();
                this.LoadMedium();
                this.LoadSmall();
                LoadNote();
            }
            catch { }
        }

        public Bitmap back_disabled;
        public Bitmap back_in;
        public Bitmap back_out;
        public Bitmap forward_in;
        public Bitmap forward_out;
        public Bitmap pause_in;
        public Bitmap pause_out;
        public Bitmap play_in;
        public Bitmap play_out;
        public Bitmap forward_disabled;

        private void LoadLarge()
        {
            using (MemoryStream ms = new MemoryStream(File.ReadAllBytes(AppDomain.CurrentDomain.BaseDirectory + "graphics\\mp3large.bmp")))
            {
                using (Bitmap raw = new Bitmap(ms))
                {
                    back_disabled = new Bitmap(48, 48);
                    using (Graphics g = Graphics.FromImage(back_disabled))
                        g.DrawImage(raw, new Rectangle(0, 0, 48, 48), new Rectangle(0, 0, 48, 48), GraphicsUnit.Pixel);
                    back_disabled.MakeTransparent(Color.Magenta);

                    back_in = new Bitmap(48, 48);
                    using (Graphics g = Graphics.FromImage(back_in))
                        g.DrawImage(raw, new Rectangle(0, 0, 48, 48), new Rectangle(48, 0, 48, 48), GraphicsUnit.Pixel);
                    back_in.MakeTransparent(Color.Magenta);

                    back_out = new Bitmap(48, 48);
                    using (Graphics g = Graphics.FromImage(back_out))
                        g.DrawImage(raw, new Rectangle(0, 0, 48, 48), new Rectangle(96, 0, 48, 48), GraphicsUnit.Pixel);
                    back_out.MakeTransparent(Color.Magenta);

                    forward_in = new Bitmap(48, 48);
                    using (Graphics g = Graphics.FromImage(forward_in))
                        g.DrawImage(raw, new Rectangle(0, 0, 48, 48), new Rectangle(0, 48, 48, 48), GraphicsUnit.Pixel);
                    forward_in.MakeTransparent(Color.Magenta);

                    forward_out = new Bitmap(48, 48);
                    using (Graphics g = Graphics.FromImage(forward_out))
                        g.DrawImage(raw, new Rectangle(0, 0, 48, 48), new Rectangle(48, 48, 48, 48), GraphicsUnit.Pixel);
                    forward_out.MakeTransparent(Color.Magenta);

                    pause_in = new Bitmap(48, 48);
                    using (Graphics g = Graphics.FromImage(pause_in))
                        g.DrawImage(raw, new Rectangle(0, 0, 48, 48), new Rectangle(96, 48, 48, 48), GraphicsUnit.Pixel);
                    pause_in.MakeTransparent(Color.Magenta);

                    pause_out = new Bitmap(48, 48);
                    using (Graphics g = Graphics.FromImage(pause_out))
                        g.DrawImage(raw, new Rectangle(0, 0, 48, 48), new Rectangle(0, 96, 48, 48), GraphicsUnit.Pixel);
                    pause_out.MakeTransparent(Color.Magenta);

                    play_in = new Bitmap(48, 48);
                    using (Graphics g = Graphics.FromImage(play_in))
                        g.DrawImage(raw, new Rectangle(0, 0, 48, 48), new Rectangle(48, 96, 48, 48), GraphicsUnit.Pixel);
                    play_in.MakeTransparent(Color.Magenta);

                    play_out = new Bitmap(48, 48);
                    using (Graphics g = Graphics.FromImage(play_out))
                        g.DrawImage(raw, new Rectangle(0, 0, 48, 48), new Rectangle(96, 96, 48, 48), GraphicsUnit.Pixel);
                    play_out.MakeTransparent(Color.Magenta);

                    forward_disabled = new Bitmap(48, 48);
                    using (Graphics g = Graphics.FromImage(forward_disabled))
                        g.DrawImage(raw, new Rectangle(0, 0, 48, 48), new Rectangle(0, 144, 48, 48), GraphicsUnit.Pixel);
                    forward_disabled.MakeTransparent(Color.Magenta);
                }
            }
        }

        public Bitmap repeat_off_in;
        public Bitmap repeat_off_out;
        public Bitmap repeat_on_in;
        public Bitmap repeat_on_out;
        public Bitmap shuffle_off_in;
        public Bitmap shuffle_off_out;
        public Bitmap shuffle_on_in;
        public Bitmap shuffle_on_out;
        public Bitmap sound_off_in;
        public Bitmap sound_off_out;
        public Bitmap sound_on_in;
        public Bitmap sound_on_out;
        public Bitmap stop_disabled;
        public Bitmap stop_in;
        public Bitmap stop_out;

        private void LoadMedium()
        {
            using (MemoryStream ms = new MemoryStream(File.ReadAllBytes(AppDomain.CurrentDomain.BaseDirectory + "graphics\\mp3medium.bmp")))
            {
                using (Bitmap raw = new Bitmap(ms))
                {
                    repeat_off_in = new Bitmap(24, 24);
                    using (Graphics g = Graphics.FromImage(repeat_off_in))
                        g.DrawImage(raw, new Rectangle(0, 0, 24, 24), new Rectangle(0, 0, 24, 24), GraphicsUnit.Pixel);
                    repeat_off_in.MakeTransparent(Color.Magenta);

                    repeat_off_out = new Bitmap(24, 24);
                    using (Graphics g = Graphics.FromImage(repeat_off_out))
                        g.DrawImage(raw, new Rectangle(0, 0, 24, 24), new Rectangle(24, 0, 24, 24), GraphicsUnit.Pixel);
                    repeat_off_out.MakeTransparent(Color.Magenta);

                    repeat_on_in = new Bitmap(24, 24);
                    using (Graphics g = Graphics.FromImage(repeat_on_in))
                        g.DrawImage(raw, new Rectangle(0, 0, 24, 24), new Rectangle(48, 0, 24, 24), GraphicsUnit.Pixel);
                    repeat_on_in.MakeTransparent(Color.Magenta);

                    repeat_on_out = new Bitmap(24, 24);
                    using (Graphics g = Graphics.FromImage(repeat_on_out))
                        g.DrawImage(raw, new Rectangle(0, 0, 24, 24), new Rectangle(72, 0, 24, 24), GraphicsUnit.Pixel);
                    repeat_on_out.MakeTransparent(Color.Magenta);

                    shuffle_off_in = new Bitmap(24, 24);
                    using (Graphics g = Graphics.FromImage(shuffle_off_in))
                        g.DrawImage(raw, new Rectangle(0, 0, 24, 24), new Rectangle(96, 0, 24, 24), GraphicsUnit.Pixel);
                    shuffle_off_in.MakeTransparent(Color.Magenta);

                    shuffle_off_out = new Bitmap(24, 24);
                    using (Graphics g = Graphics.FromImage(shuffle_off_out))
                        g.DrawImage(raw, new Rectangle(0, 0, 24, 24), new Rectangle(0, 24, 24, 24), GraphicsUnit.Pixel);
                    shuffle_off_out.MakeTransparent(Color.Magenta);

                    shuffle_on_in = new Bitmap(24, 24);
                    using (Graphics g = Graphics.FromImage(shuffle_on_in))
                        g.DrawImage(raw, new Rectangle(0, 0, 24, 24), new Rectangle(24, 24, 24, 24), GraphicsUnit.Pixel);
                    shuffle_on_in.MakeTransparent(Color.Magenta);

                    shuffle_on_out = new Bitmap(24, 24);
                    using (Graphics g = Graphics.FromImage(shuffle_on_out))
                        g.DrawImage(raw, new Rectangle(0, 0, 24, 24), new Rectangle(48, 24, 24, 24), GraphicsUnit.Pixel);
                    shuffle_on_out.MakeTransparent(Color.Magenta);

                    sound_off_in = new Bitmap(24, 24);
                    using (Graphics g = Graphics.FromImage(sound_off_in))
                        g.DrawImage(raw, new Rectangle(0, 0, 24, 24), new Rectangle(72, 24, 24, 24), GraphicsUnit.Pixel);
                    sound_off_in.MakeTransparent(Color.Magenta);

                    sound_off_out = new Bitmap(24, 24);
                    using (Graphics g = Graphics.FromImage(sound_off_out))
                        g.DrawImage(raw, new Rectangle(0, 0, 24, 24), new Rectangle(96, 24, 24, 24), GraphicsUnit.Pixel);
                    sound_off_out.MakeTransparent(Color.Magenta);

                    sound_on_in = new Bitmap(24, 24);
                    using (Graphics g = Graphics.FromImage(sound_on_in))
                        g.DrawImage(raw, new Rectangle(0, 0, 24, 24), new Rectangle(0, 48, 24, 24), GraphicsUnit.Pixel);
                    sound_on_in.MakeTransparent(Color.Magenta);

                    sound_on_out = new Bitmap(24, 24);
                    using (Graphics g = Graphics.FromImage(sound_on_out))
                        g.DrawImage(raw, new Rectangle(0, 0, 24, 24), new Rectangle(24, 48, 24, 24), GraphicsUnit.Pixel);
                    sound_on_out.MakeTransparent(Color.Magenta);

                    stop_disabled = new Bitmap(24, 24);
                    using (Graphics g = Graphics.FromImage(stop_disabled))
                        g.DrawImage(raw, new Rectangle(0, 0, 24, 24), new Rectangle(48, 48, 24, 24), GraphicsUnit.Pixel);
                    stop_disabled.MakeTransparent(Color.Magenta);

                    stop_in = new Bitmap(24, 24);
                    using (Graphics g = Graphics.FromImage(stop_in))
                        g.DrawImage(raw, new Rectangle(0, 0, 24, 24), new Rectangle(72, 48, 24, 24), GraphicsUnit.Pixel);
                    stop_in.MakeTransparent(Color.Magenta);

                    stop_out = new Bitmap(24, 24);
                    using (Graphics g = Graphics.FromImage(stop_out))
                        g.DrawImage(raw, new Rectangle(0, 0, 24, 24), new Rectangle(96, 48, 24, 24), GraphicsUnit.Pixel);
                    stop_out.MakeTransparent(Color.Magenta);
                }
            }
        }

        public Bitmap volume_in;
        public Bitmap volume_out;
        public static Bitmap arrow_up;
        public static Bitmap arrow_down;
        public static Bitmap np_note;

        private void LoadSmall()
        {
            using (MemoryStream ms = new MemoryStream(File.ReadAllBytes(AppDomain.CurrentDomain.BaseDirectory + "graphics\\mp3small.bmp")))
            {
                using (Bitmap raw = new Bitmap(ms))
                {
                    volume_in = new Bitmap(14, 14);
                    using (Graphics g = Graphics.FromImage(volume_in))
                        g.DrawImage(raw, new Rectangle(0, 0, 14, 14), new Rectangle(0, 0, 14, 14), GraphicsUnit.Pixel);
                    volume_in.MakeTransparent(Color.Magenta);

                    volume_out = new Bitmap(14, 14);
                    using (Graphics g = Graphics.FromImage(volume_out))
                        g.DrawImage(raw, new Rectangle(0, 0, 14, 14), new Rectangle(14, 0, 14, 14), GraphicsUnit.Pixel);
                    volume_out.MakeTransparent(Color.Magenta);

                    arrow_up = new Bitmap(14, 14);
                    using (Graphics g = Graphics.FromImage(arrow_up))
                        g.DrawImage(raw, new Rectangle(0, 0, 14, 14), new Rectangle(28, 0, 14, 14), GraphicsUnit.Pixel);
                    arrow_up.MakeTransparent(Color.Magenta);

                    arrow_down = new Bitmap(14, 14);
                    using (Graphics g = Graphics.FromImage(arrow_down))
                        g.DrawImage(raw, new Rectangle(0, 0, 14, 14), new Rectangle(42, 0, 14, 14), GraphicsUnit.Pixel);
                    arrow_down.MakeTransparent(Color.Magenta);

                    np_note = new Bitmap(14, 14);
                    using (Graphics g = Graphics.FromImage(np_note))
                        g.DrawImage(raw, new Rectangle(0, 0, 14, 14), new Rectangle(56, 0, 14, 14), GraphicsUnit.Pixel);
                    np_note.MakeTransparent(Color.Magenta);
                }
            }
        }

        public static Bitmap note;
        public static Bitmap radio;

        private static void LoadNote()
        {
            using (MemoryStream ms = new MemoryStream(File.ReadAllBytes(AppDomain.CurrentDomain.BaseDirectory + "graphics\\mp3note.bmp")))
            {
                using (Bitmap raw = new Bitmap(ms))
                {
                    note = new Bitmap(256, 256);
                    using (Graphics g = Graphics.FromImage(note))
                        g.DrawImage(raw, new Rectangle(0, 0, 256, 256), new Rectangle(0, 0, 256, 256), GraphicsUnit.Pixel);
                    note.MakeTransparent(Color.Magenta);

                    radio = new Bitmap(256, 256);
                    using (Graphics g = Graphics.FromImage(radio))
                        g.DrawImage(raw, new Rectangle(0, 0, 256, 256), new Rectangle(256, 0, 256, 256), GraphicsUnit.Pixel);
                    radio.MakeTransparent(Color.Magenta);
                }
            }
        }
    }
}
