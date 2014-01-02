using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace cb0t
{
    class InternalSounds
    {
        [DllImport("winmm.dll")]
        private static extern Int32 mciSendString(String command, StringBuilder buffer, Int32 bufferSize, IntPtr hwndCallback);

        public static void Nudge()
        {
            if (!Settings.GetReg<bool>("can_popup_sound", true))
                return;

            String path = Settings.AppPath + "nudge.wav";
            mciSendString("stop cb3_nudge", null, 0, IntPtr.Zero);
            mciSendString("close cb3_nudge", null, 0, IntPtr.Zero);
            mciSendString("stop cb3_notify", null, 0, IntPtr.Zero);
            mciSendString("close cb3_notify", null, 0, IntPtr.Zero);
            mciSendString("stop cb3_friend", null, 0, IntPtr.Zero);
            mciSendString("close cb3_friend", null, 0, IntPtr.Zero);
            mciSendString("stop cb3_script", null, 0, IntPtr.Zero);
            mciSendString("close cb3_script", null, 0, IntPtr.Zero);
            mciSendString("open \"" + path + "\" type mpegvideo alias cb3_nudge", null, 0, IntPtr.Zero);
            mciSendString("play cb3_nudge", null, 0, IntPtr.Zero);
        }

        public static void Notify()
        {
            if (!Settings.GetReg<bool>("can_popup_sound", true))
                return;

            String path = Settings.AppPath + "notify.wav";
            mciSendString("stop cb3_nudge", null, 0, IntPtr.Zero);
            mciSendString("close cb3_nudge", null, 0, IntPtr.Zero);
            mciSendString("stop cb3_notify", null, 0, IntPtr.Zero);
            mciSendString("close cb3_notify", null, 0, IntPtr.Zero);
            mciSendString("stop cb3_friend", null, 0, IntPtr.Zero);
            mciSendString("close cb3_friend", null, 0, IntPtr.Zero);
            mciSendString("stop cb3_script", null, 0, IntPtr.Zero);
            mciSendString("close cb3_script", null, 0, IntPtr.Zero);
            mciSendString("open \"" + path + "\" type mpegvideo alias cb3_notify", null, 0, IntPtr.Zero);
            mciSendString("play cb3_notify", null, 0, IntPtr.Zero);
        }

        public static void Friend()
        {
            if (!Settings.GetReg<bool>("can_popup_sound", true))
                return;

            String path = Settings.AppPath + "friend.wav";
            mciSendString("stop cb3_nudge", null, 0, IntPtr.Zero);
            mciSendString("close cb3_nudge", null, 0, IntPtr.Zero);
            mciSendString("stop cb3_notify", null, 0, IntPtr.Zero);
            mciSendString("close cb3_notify", null, 0, IntPtr.Zero);
            mciSendString("stop cb3_friend", null, 0, IntPtr.Zero);
            mciSendString("close cb3_friend", null, 0, IntPtr.Zero);
            mciSendString("stop cb3_script", null, 0, IntPtr.Zero);
            mciSendString("close cb3_script", null, 0, IntPtr.Zero);
            mciSendString("open \"" + path + "\" type mpegvideo alias cb3_friend", null, 0, IntPtr.Zero);
            mciSendString("play cb3_friend", null, 0, IntPtr.Zero);
        }

        public static void Script(String path)
        {
            mciSendString("stop cb3_nudge", null, 0, IntPtr.Zero);
            mciSendString("close cb3_nudge", null, 0, IntPtr.Zero);
            mciSendString("stop cb3_notify", null, 0, IntPtr.Zero);
            mciSendString("close cb3_notify", null, 0, IntPtr.Zero);
            mciSendString("stop cb3_friend", null, 0, IntPtr.Zero);
            mciSendString("close cb3_friend", null, 0, IntPtr.Zero);
            mciSendString("stop cb3_script", null, 0, IntPtr.Zero);
            mciSendString("close cb3_script", null, 0, IntPtr.Zero);
            mciSendString("open \"" + path + "\" type mpegvideo alias cb3_script", null, 0, IntPtr.Zero);
            mciSendString("play cb3_script", null, 0, IntPtr.Zero);
        }
    }
}
