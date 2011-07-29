using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace cb0t_chat_client_v2
{
    public partial class AudioController : UserControl
    {
        private Point mouse_pos = new Point(0, 0);
        private AudioControllerImages images = new AudioControllerImages();
        private ToolTip tip = new ToolTip();

        private bool stop_disabled = true;
        private bool shuffle_on = false;
        private bool repeat_on = false;
        private bool back_disabled = true;
        private bool forward_disabled = true;
        private bool sound_on = true;
        private bool playing = false;
        private bool playing_disabled = true;

        private int volume = 75;

        internal event AudioPlayerVolumeChangedHandler VolumeChanged;
        public event EventHandler Play;
        public event EventHandler Stop;
        public event EventHandler Pause;
        public event EventHandler Forward;
        public event EventHandler Back;

        public AudioController()
        {
            this.images.Load();

            InitializeComponent();

            this.DoubleBuffered = true;
        }

        public void SetToStopped()
        {
            if (!this.stop_disabled)
            {
                this.stop_disabled = true;
                this.playing = false;
                this.Invalidate();
            }
        }

        public void SetToPlay()
        {
            if (!this.playing)
            {
                this.stop_disabled = false;
                this.playing = true;
                this.Invalidate();
            }
        }

        public void SetRadioMode()
        {
            this.stop_disabled = false;
            this.playing = false;
            this.playing_disabled = true;
            this.forward_disabled = true;
            this.back_disabled = true;
            this.Invalidate();
        }

        public void PlaylistEmpty(bool empty)
        {
            if (AudioSettings.radio_mode)
                return;

            if (!empty)
            {
                if (this.playing_disabled)
                {
                    this.playing_disabled = false;
                    this.forward_disabled = false;
                    this.back_disabled = false;
                    this.Invalidate();
                }
            }
            else
            {
                this.playing_disabled = true;
                this.playing = false;
                this.forward_disabled = true;
                this.stop_disabled = true;
                this.back_disabled = true;
                this.Invalidate();
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            if (this.images.pause_in == null)
                return;

            e.Graphics.Clear(Color.Black);

            Color bg = Color.FromArgb(63, 65, 67);
            bool over;

            using (SolidBrush bg_sb = new SolidBrush(bg))
            {
                e.Graphics.FillPath(bg_sb, RoundedRectangle.Create(new Rectangle(1, 9, 386, 32), 12));

                // shuffle button
                over = (this.mouse_pos.X >= 22 && this.mouse_pos.X <= 42) && (this.mouse_pos.Y >= 13 && this.mouse_pos.Y <= 37);

                if (this.shuffle_on)
                {
                    if (over)
                        e.Graphics.DrawImage(this.images.shuffle_on_in, new Point(20, 14));
                    else
                        e.Graphics.DrawImage(this.images.shuffle_on_out, new Point(20, 14));
                }
                else
                {
                    if (over)
                        e.Graphics.DrawImage(this.images.shuffle_off_in, new Point(20, 14));
                    else
                        e.Graphics.DrawImage(this.images.shuffle_off_out, new Point(20, 14));
                }

                // repeat button
                over = (this.mouse_pos.X >= 52 && this.mouse_pos.X <= 72) && (this.mouse_pos.Y >= 13 && this.mouse_pos.Y <= 37);

                if (this.repeat_on)
                {
                    if (over)
                        e.Graphics.DrawImage(this.images.repeat_on_in, new Point(50, 14));
                    else
                        e.Graphics.DrawImage(this.images.repeat_on_out, new Point(50, 14));
                }
                else
                {
                    if (over)
                        e.Graphics.DrawImage(this.images.repeat_off_in, new Point(50, 14));
                    else
                        e.Graphics.DrawImage(this.images.repeat_off_out, new Point(50, 14));
                }

                // stop button
                over = (this.mouse_pos.X >= 94 && this.mouse_pos.X <= 114) && (this.mouse_pos.Y >= 13 && this.mouse_pos.Y <= 37);

                if (this.stop_disabled)
                    e.Graphics.DrawImage(this.images.stop_disabled, new Point(92, 14));
                else if (over)
                    e.Graphics.DrawImage(this.images.stop_in, new Point(92, 14));
                else
                    e.Graphics.DrawImage(this.images.stop_out, new Point(92, 14));

                // back button
                over = (this.mouse_pos.X >= 126 && this.mouse_pos.X <= 170) && (this.mouse_pos.Y >= 13 && this.mouse_pos.Y <= 37);

                if (this.back_disabled)
                    e.Graphics.DrawImage(this.images.back_disabled, new Point(124, 1));
                else if (over)
                    e.Graphics.DrawImage(this.images.back_in, new Point(124, 1));
                else
                    e.Graphics.DrawImage(this.images.back_out, new Point(124, 1));

                // play/pause button
                over = (this.mouse_pos.X >= 171 && this.mouse_pos.X <= 217) && (this.mouse_pos.Y >= 1 && this.mouse_pos.Y <= 49);

                if (this.playing_disabled)
                    e.Graphics.DrawImage(this.images.play_out, new Point(170, 1));
                else
                {
                    if (this.playing)
                    {
                        if (over)
                            e.Graphics.DrawImage(this.images.pause_in, new Point(170, 1));
                        else
                            e.Graphics.DrawImage(this.images.pause_out, new Point(170, 1));
                    }
                    else
                    {
                        if (over)
                            e.Graphics.DrawImage(this.images.play_in, new Point(170, 1));
                        else
                            e.Graphics.DrawImage(this.images.play_out, new Point(170, 1));
                    }
                }

                // forward button
                over = (this.mouse_pos.X >= 218 && this.mouse_pos.X <= 262) && (this.mouse_pos.Y >= 13 && this.mouse_pos.Y <= 37);

                if (this.forward_disabled)
                    e.Graphics.DrawImage(this.images.forward_disabled, new Point(216, 1));
                else if (over)
                    e.Graphics.DrawImage(this.images.forward_in, new Point(216, 1));
                else
                    e.Graphics.DrawImage(this.images.forward_out, new Point(216, 1));

                // sound button
                over = (this.mouse_pos.X >= 274 && this.mouse_pos.X <= 294) && (this.mouse_pos.Y >= 13 && this.mouse_pos.Y <= 37);

                if (this.sound_on)
                {
                    if (over)
                        e.Graphics.DrawImage(this.images.sound_on_in, new Point(272, 14));
                    else
                        e.Graphics.DrawImage(this.images.sound_on_out, new Point(272, 14));
                }
                else
                {
                    if (over)
                        e.Graphics.DrawImage(this.images.sound_off_in, new Point(272, 14));
                    else
                        e.Graphics.DrawImage(this.images.sound_off_out, new Point(272, 14));
                }

                //volume
                Color vol_rim = Color.FromArgb(102, 104, 106);
                Color vol_min = Color.FromArgb(30, 68, 123);
                Color vol_max = Color.FromArgb(4, 75, 176);
                Rectangle vol_rec = new Rectangle(310, 24, 60, 4);

                using (LinearGradientBrush lgb = new LinearGradientBrush(vol_rec, vol_min, vol_max, 45f))
                    e.Graphics.FillRectangle(lgb, vol_rec);

                int vol_pos = (int)Math.Round(0.6 * this.volume);
                e.Graphics.FillRectangle(bg_sb, new Rectangle((310 + vol_pos), 24, (60 - vol_pos), 4));

                using (SolidBrush sb = new SolidBrush(vol_rim))
                using (Pen pen = new Pen(sb, 1))
                    e.Graphics.DrawRectangle(pen, vol_rec);

                over = (this.mouse_pos.X >= (306 + vol_pos) && this.mouse_pos.X <= (316 + vol_pos)) && (this.mouse_pos.Y >= 13 && this.mouse_pos.Y <= 37);

                if (over)
                    e.Graphics.DrawImage(this.images.volume_in, new Point((304 + vol_pos), 20));
                else
                    e.Graphics.DrawImage(this.images.volume_out, new Point((304 + vol_pos), 20));
            }
        }

        private bool mouse_down = false;

        protected override void OnMouseMove(MouseEventArgs e)
        {
            this.mouse_pos = e.Location;

            if (this.mouse_down)
            {
                Rectangle vol_rec = new Rectangle(310, 24, 60, 4);

                if (e.X >= vol_rec.X && e.X <= (vol_rec.X + vol_rec.Width))
                {
                    int vol_pos = (int)Math.Round(0.6 * this.volume) + 310;

                    if (e.X != vol_pos)
                    {
                        this.volume = (int)Math.Round((e.X - 310) / 0.6);

                        if (this.volume < 0) this.volume = 0;
                        if (this.volume > 100) this.volume = 0;

                        this.tip.SetToolTip(this, this.volume.ToString());

                        if (this.sound_on)
                            if (this.VolumeChanged != null)
                                this.VolumeChanged(this, new AudioPlayerVolumeChanged(this.volume));
                    }
                }
            }
            else
            {
                String text = this.tip.GetToolTip(this);

                if ((e.X >= 171 && e.X <= 217) && (e.Y >= 1 && e.Y <= 49))
                {
                    if (text != (this.playing ? "Pause" : "Play"))
                        this.tip.SetToolTip(this, this.playing ? "Pause" : "Play");
                }
                else if ((e.X >= 126 && e.X <= 170) && (e.Y >= 13 && e.Y <= 37))
                {
                    if (text != "Previous song")
                        this.tip.SetToolTip(this, "Previous song");
                }
                else if ((e.X >= 218 && e.X <= 262) && (e.Y >= 13 && e.Y <= 37))
                {
                    if (text != "Next song")
                        this.tip.SetToolTip(this, "Next song");
                }
                else if ((e.X >= 274 && e.X <= 294) && (e.Y >= 13 && e.Y <= 37))
                {
                    if (text != (this.sound_on ? "Mute" : "Sound"))
                        this.tip.SetToolTip(this, this.sound_on ? "Mute" : "Sound");
                }
                else if ((e.X >= 22 && e.X <= 42) && (e.Y >= 13 && e.Y <= 37))
                {
                    if (text != (this.shuffle_on ? "Turn shuffle off" : "Turn shuffle on"))
                        this.tip.SetToolTip(this, this.shuffle_on ? "Turn shuffle off" : "Turn shuffle on");
                }
                else if ((e.X >= 52 && e.X <= 72) && (e.Y >= 13 && e.Y <= 37))
                {
                    if (text != (this.repeat_on ? "Turn repeat off" : "Turn repeat on"))
                        this.tip.SetToolTip(this, this.repeat_on ? "Turn repeat off" : "Turn repeat on");
                }
                else if ((e.X >= 312 && e.X <= 378) && (e.Y >= 13 && e.Y <= 37))
                {
                    if (text != "Volume")
                        this.tip.SetToolTip(this, "Volume");
                }
                else if ((e.X >= 94 && e.X <= 114) && (e.Y >= 13 && e.Y <= 37))
                {
                    if (text != "Stop")
                        this.tip.SetToolTip(this, "Stop");
                }
                else if (text != String.Empty)
                    this.tip.SetToolTip(this, String.Empty);
            }

            this.Invalidate();
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            this.mouse_pos = new Point(0, 0);
            this.Invalidate();
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            if (this.mouse_down)
            {
                this.mouse_down = false;
                Rectangle vol_rec = new Rectangle(310, 24, 60, 4);

                if ((e.X >= vol_rec.X && e.X <= (vol_rec.X + vol_rec.Width)) && (e.Y >= 13 && e.Y <= 37))
                {
                    int vol_pos = (int)Math.Round(0.6 * this.volume) + 310;

                    if (e.X != vol_pos)
                    {
                        this.volume = (int)Math.Round((e.X - 310) / 0.6);

                        if (this.volume < 0) this.volume = 0;
                        if (this.volume > 100) this.volume = 0;

                        if (this.sound_on)
                            if (this.VolumeChanged != null)
                                this.VolumeChanged(this, new AudioPlayerVolumeChanged(this.volume));
                    }
                }

                this.Invalidate();
            }
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                if (!this.mouse_down)
                {
                    Rectangle vol_rec = new Rectangle(310, 24, 60, 4);

                    if ((e.X >= (vol_rec.X - 7) && e.X <= (vol_rec.X + vol_rec.Width + 7)) && (e.Y >= 13 && e.Y <= 37))
                        this.mouse_down = true;
                }
            }
        }

        protected override void OnMouseClick(MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                if ((e.X >= 171 && e.X <= 217) && (e.Y >= 1 && e.Y <= 49))
                {
                    if (!this.playing_disabled)
                    {
                        this.playing = !this.playing;

                        if (this.stop_disabled)
                            this.stop_disabled = false;

                        if (!this.playing)
                        {
                            if (this.Pause != null)
                                this.Pause(this, new EventArgs());
                        }
                        else if (this.Play != null)
                            this.Play(this, new EventArgs());
                    }
                }
                else if ((e.X >= 126 && e.X <= 170) && (e.Y >= 13 && e.Y <= 37))
                {
                    if (!this.back_disabled)
                        if (this.Back != null)
                            this.Back(this, new EventArgs());
                }
                else if ((e.X >= 218 && e.X <= 262) && (e.Y >= 13 && e.Y <= 37))
                {
                    if (!this.forward_disabled)
                        if (this.Forward != null)
                            this.Forward(this, new EventArgs());
                }
                else if ((e.X >= 274 && e.X <= 294) && (e.Y >= 13 && e.Y <= 37))
                {
                    this.sound_on = !this.sound_on;

                    if (this.VolumeChanged != null)
                        this.VolumeChanged(this, new AudioPlayerVolumeChanged(this.sound_on ? this.volume : 0));

                    AudioSettings.mute = !this.sound_on;
                    AudioSettings.Save();
                }
                else if ((e.X >= 22 && e.X <= 42) && (e.Y >= 13 && e.Y <= 37))
                {
                    this.shuffle_on = !this.shuffle_on;
                    AudioSettings.shuffle = this.shuffle_on;
                    AudioSettings.Save();
                }
                else if ((e.X >= 52 && e.X <= 72) && (e.Y >= 13 && e.Y <= 37))
                {
                    this.repeat_on = !this.repeat_on;
                    AudioSettings.repeat = this.repeat_on;
                    AudioSettings.Save();
                }
                else if ((e.X >= 94 && e.X <= 114) && (e.Y >= 13 && e.Y <= 37))
                {
                    if (!this.stop_disabled)
                    {
                        this.stop_disabled = true;
                        this.playing = false;

                        if (this.Stop != null)
                            this.Stop(this, new EventArgs());
                    }
                }

                this.Invalidate();
            }
        }

    }

    internal delegate void AudioPlayerVolumeChangedHandler(object sender, AudioPlayerVolumeChanged e);
    class AudioPlayerVolumeChanged : EventArgs
    {
        public int volume;

        public AudioPlayerVolumeChanged(int volume)
        {
            this.volume = volume;
        }
    }


}
