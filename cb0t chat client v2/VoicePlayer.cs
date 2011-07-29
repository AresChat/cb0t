using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Runtime.InteropServices;

namespace cb0t_chat_client_v2
{
    class VoicePlayer
    {
        public delegate void SPNHandler(VoiceClipReceived vcr);
        public event SPNHandler OnPlayingNow;
        private List<VoiceClipReceived> queue = new List<VoiceClipReceived>();
        private bool busy = false;
        private bool is_paused = false;

        [DllImport("winmm.dll")]
        private static extern Int32 mciSendString(String command, StringBuilder buffer, Int32 bufferSize, IntPtr hwndCallback);

        [DllImport("winmm.dll")]
        private static extern long mciSendString(string strCommand, StringBuilder strReturn, int iReturnLength, int bla);

        public event EventHandler VoiceClipPlaying;
        public event EventHandler VoiceClipStopped;

        public void Pause(bool pause)
        {
            if (pause)
            {
                mciSendString("stop cbotpback", null, 0, 0);
                mciSendString("close cbotpback", null, 0, 0);
                this.busy = false;
                this.is_paused = true;
            }
            else this.is_paused = false;
        }

        public void Play(VoiceClipReceived vcr, bool queue_if_busy, IntPtr ptr)
        {
            if (this.busy || this.is_paused)
            {
                if (queue_if_busy)
                    this.queue.Add(vcr);

                return;
            }

            if (queue_if_busy)
                this.OnPlayingNow(vcr);

            this.PlayClip(vcr, ptr);
        }

        private void PlayClip(VoiceClipReceived vcr, IntPtr ptr)
        {
            if (!this.busy)
                this.VoiceClipPlaying(this, new EventArgs());

            this.busy = true;

            mciSendString("stop cbotpback", null, 0, 0);
            mciSendString("close cbotpback", null, 0, 0);

            String path = null;

            for (int i = 0; i < 100; i++)
            {
                try
                {
                    path = Settings.folder_path + ("cbpb" + i);
                    File.WriteAllBytes(path, vcr.VoiceClip);
                    break;
                }
                catch { path = null; }
            }

            if (path != null)
            {
                mciSendString("open \"" + path + "\" type waveaudio alias cbotpback", null, 0, 0);
                mciSendString("play cbotpback notify", null, 0, ptr);
            }
            else this.busy = false;
        }

        public void Tick(IntPtr ptr)
        {
            if (this.is_paused)
                return;

            if (!this.busy && this.queue.Count > 0)
            {
                VoiceClipReceived vcr = this.queue[0];

                if (this.queue.Count > 0)
                    this.queue.RemoveAt(0);

                this.OnPlayingNow(vcr);
                this.PlayClip(vcr, ptr);
            }
        }

        public void PlaybackComplete(IntPtr ptr)
        {
            if (this.queue.Count > 0)
            {
                VoiceClipReceived vcr = this.queue[0];

                if (this.queue.Count > 0)
                    this.queue.RemoveAt(0);

                this.OnPlayingNow(vcr);
                this.PlayClip(vcr, ptr);
            }
            else
            {
                this.busy = false;
                this.VoiceClipStopped(this, new EventArgs());
            }
        }
    }
}
