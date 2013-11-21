using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;

namespace cb0t
{
    class VoiceRecorder
    {
        [DllImport("winmm.dll")]
        private static extern Int32 mciSendString(String command, StringBuilder buffer, Int32 bufferSize, IntPtr hwndCallback);

        private const String RECORD_DEVICE_NAME = "cb_v3_record";

        public static bool Recording { get; private set; }

        public static void RecordStart(String target, bool pm)
        {
            mciSendString("open new type waveaudio alias " + RECORD_DEVICE_NAME, null, 0, IntPtr.Zero);
            mciSendString("set " + RECORD_DEVICE_NAME + " time format ms format tag pcm channels 1 samplespersec 16000 bytespersec 32000 alignment 2 bitspersample 16 input " + AudioHelpers.GetRecordIdent(), null, 0, IntPtr.Zero);
            mciSendString("record " + RECORD_DEVICE_NAME, null, 0, IntPtr.Zero);
            Recording = true;
        }

        public static void RecordCancel()
        {
            mciSendString("stop " + RECORD_DEVICE_NAME, null, 0, IntPtr.Zero);
            mciSendString("close " + RECORD_DEVICE_NAME, null, 0, IntPtr.Zero);
            Recording = false;
        }

        public static void RecordStop()
        {
            String path = Path.Combine(Settings.VoicePath, "record.wav");
            mciSendString("stop " + RECORD_DEVICE_NAME, null, 0, IntPtr.Zero);
            mciSendString("save " + RECORD_DEVICE_NAME + " \"" + path + "\"", null, 0, IntPtr.Zero);
            mciSendString("close " + RECORD_DEVICE_NAME, null, 0, IntPtr.Zero);
            Thread.Sleep(300);
            Recording = false;
        }

        public static byte[] RecordBytes
        {
            get
            {
                byte[] result = null;

                try
                {
                    String path = Path.Combine(Settings.VoicePath, "record.wav");

                    if (File.Exists(path))
                        result = File.ReadAllBytes(path);
                }
                catch { }

                return result;
            }
        }
    }
}
