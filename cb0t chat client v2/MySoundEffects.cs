using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Runtime.InteropServices;

namespace cb0t_chat_client_v2
{
    class MySoundEffects
    {
        [DllImport("winmm.dll")]
        private static extern long mciSendString(string strCommand, StringBuilder strReturn, int iReturnLength, int bla);

        private static String nudgesound;
        private static String notifysound;
        private static String pmsound;

        public static void LoadSounds()
        {
            nudgesound = AppDomain.CurrentDomain.BaseDirectory + "data/nudge.wav";
            notifysound = AppDomain.CurrentDomain.BaseDirectory + "data/notify.wav";
            pmsound = AppDomain.CurrentDomain.BaseDirectory + "data/pm.wav";
        }

        public static void PlayNudgeSound()
        {
            mciSendString("stop cbotnudgesound", null, 0, 0);
            mciSendString("close cbotnudgesound", null, 0, 0);
            mciSendString("open \"" + nudgesound + "\" type waveaudio alias cbotnudgesound", null, 0, 0);
            mciSendString("play cbotnudgesound", null, 0, 0);
        }

        public static void PlayNotifySound()
        {
            mciSendString("stop cbotnotifysound", null, 0, 0);
            mciSendString("close cbotnotifysound", null, 0, 0);
            mciSendString("open \"" + notifysound + "\" type waveaudio alias cbotnotifysound", null, 0, 0);
            mciSendString("play cbotnotifysound", null, 0, 0);
        }

        public static void PlayPMSound()
        {
            mciSendString("stop cbotpmsound", null, 0, 0);
            mciSendString("close cbotpmsound", null, 0, 0);
            mciSendString("open \"" + pmsound + "\" type waveaudio alias cbotpmsound", null, 0, 0);
            mciSendString("play cbotpmsound", null, 0, 0);
        }
    }
}
