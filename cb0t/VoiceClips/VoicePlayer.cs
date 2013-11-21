using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace cb0t
{
    class VoicePlayer
    {
        [DllImport("winmm.dll")]
        private static extern Int32 mciSendString(String command, StringBuilder buffer, Int32 bufferSize, IntPtr hwndCallback);

        private const String PLAYBACK_DEVICE_NAME = "cb_v3_playback";

        public static List<VoicePlayerInboundItem> Inbound { get; set; }
        public static List<VoicePlayerItem> Records { get; set; }
        
        private static List<VoicePlayerItem> PlaybackQueue { get; set; }
        private static VoicePlayerState State { get; set; }
        private static object padlock = new object();

        public static uint NEXT_SHORTCUT = 0;

        public static void Init()
        {
            Inbound = new List<VoicePlayerInboundItem>();
            PlaybackQueue = new List<VoicePlayerItem>();
            Records = new List<VoicePlayerItem>();
            State = VoicePlayerState.Available;
        }

        public static void QueueItem(VoicePlayerItem item)
        {
            lock (padlock)
                PlaybackQueue.Add(item);
        }

        public static void Tick(Form1 form)
        {
            if (State == VoicePlayerState.Available)
            {
                VoicePlayerItem item = null;

                lock (padlock)
                    if (PlaybackQueue.Count > 0)
                    {
                        item = PlaybackQueue[0];
                        PlaybackQueue.RemoveAt(0);
                    }

                if (item != null)
                {
                    String path = Path.Combine(Settings.VoicePath, item.FileName + ".wav");
                    int success = 0;
                    mciSendString("open \"" + path + "\" type mpegvideo alias " + PLAYBACK_DEVICE_NAME, null, 0, IntPtr.Zero);
                    success = mciSendString("set " + PLAYBACK_DEVICE_NAME + " output " + AudioHelpers.GetPlaybackIdent(), null, 0, IntPtr.Zero);
                    success = mciSendString("play " + PLAYBACK_DEVICE_NAME + " notify", null, 0, form.Handle);

                    if (success == 0)
                    {
                        State = VoicePlayerState.Busy;

                        if (item.Auto)
                        {
                            Room room = RoomPool.Rooms.Find(x => x.EndPoint.Equals(item.EndPoint));

                            if (room != null)
                                room.ShowAnnounceText("\x000314--- \\\\voice_clip_#" + item.ShortCut + " received from " + item.Sender);
                        }
                    }
                }
            }
        }

        public static void PlaybackCompleted()
        {
            mciSendString("stop " + PLAYBACK_DEVICE_NAME, null, 0, IntPtr.Zero);
            mciSendString("close " + PLAYBACK_DEVICE_NAME, null, 0, IntPtr.Zero);
            State = VoicePlayerState.Available;
        }
    }
}
