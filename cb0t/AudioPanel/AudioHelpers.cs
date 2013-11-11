using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using WMPLib;

namespace cb0t
{
    class AudioHelpers
    {
        [DllImport("winmm.dll")]
        private static extern int waveInGetNumDevs();
        [DllImport("winmm.dll")]
        private static extern int waveOutGetNumDevs();
        [DllImport("winmm.dll", EntryPoint = "waveInGetDevCaps")]
        private static extern int waveInGetDevCaps(int uDeviceID, ref WaveCaps lpCaps, int uSize);
        [DllImport("winmm.dll", EntryPoint = "waveOutGetDevCaps")]
        private static extern uint waveOutGetDevCaps(int uDeviceID, ref WaveCaps lpCaps, int uSize);

        [StructLayout(LayoutKind.Sequential, Pack = 4)]
        private struct WaveCaps
        {
            public short wMid;
            public short wPid;
            public int vDriverVersion;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
            public string szPname;
            public int dwFormats;
            public short wChannels;
            public short wReserved1;
            public int dwSupport;
        }

        public static int GetRecordIdent()
        {
            String q = Settings.GetReg<String>("vc_rec_device", String.Empty);

            for (int i = 0; i < waveInGetNumDevs(); i++)
            {
                WaveCaps w = new WaveCaps();
                waveInGetDevCaps(i, ref w, Marshal.SizeOf(typeof(WaveCaps)));

                String str = w.szPname;

                if (str.IndexOf("\0") > -1)
                    str = str.Substring(0, str.IndexOf("\0"));

                if (str.Length > 0)
                    if (str == q)
                        return i;
            }

            return 0;
        }

        public static int GetPlaybackIdent()
        {
            String q = Settings.GetReg<String>("vc_play_device", String.Empty);

            for (int i = 0; i < waveOutGetNumDevs(); i++)
            {
                WaveCaps w = new WaveCaps();
                waveOutGetDevCaps(i, ref w, Marshal.SizeOf(typeof(WaveCaps)));

                String str = w.szPname;

                if (str.IndexOf("\0") > -1)
                    str = str.Substring(0, str.IndexOf("\0"));

                if (str.Length > 0)
                    if (str == q)
                        return i;
            }

            return 0;
        }

        public static String[] GetRecordDevices()
        {
            List<String> list = new List<String>();

            for (int i = 0; i < waveInGetNumDevs(); i++)
            {
                WaveCaps w = new WaveCaps();
                waveInGetDevCaps(i, ref w, Marshal.SizeOf(typeof(WaveCaps)));

                String str = w.szPname;

                if (str.IndexOf("\0") > -1)
                    str = str.Substring(0, str.IndexOf("\0"));

                if (str.Length > 0)
                    list.Add(str);
            }

            return list.ToArray();
        }

        public static String[] GetPlaybackDevices()
        {
            List<String> list = new List<String>();

            for (int i = 0; i < waveOutGetNumDevs(); i++)
            {
                WaveCaps w = new WaveCaps();
                waveOutGetDevCaps(i, ref w, Marshal.SizeOf(typeof(WaveCaps)));

                String str = w.szPname;

                if (str.IndexOf("\0") > -1)
                    str = str.Substring(0, str.IndexOf("\0"));

                if (str.Length > 0)
                    list.Add(str);
            }

            return list.ToArray();
        }

        public static AudioPlayerItem CreateAudioPlayerItem(WindowsMediaPlayer wmp, String path)
        {
            AudioPlayerItem item = new AudioPlayerItem();
            
            try
            {
                IWMPMedia media = wmp.newMedia(path);

                if (media.duration > 0)
                {
                    item.Duration = (int)media.duration;
                    item.SetDurationText(item.Duration);
                    item.Title = media.name.Trim();
                    item.Path = path;

                    for (int i = 0; i < media.attributeCount; i++)
                    {
                        switch (media.getAttributeName(i).ToUpper())
                        {
                            case "ALBUMIDALBUMARTIST":
                                String[] strs = media.getItemInfo(media.getAttributeName(i)).Split(new String[] { "*;*" }, StringSplitOptions.None);

                                if (strs.Length > 0)
                                    item.Album = strs[0].Trim();
                                if (strs.Length > 1)
                                    item.Artist = strs[1].Trim();
                                break;

                            case "AUTHOR":
                                item.Author = media.getItemInfo(media.getAttributeName(i)).Trim();
                                break;

                            case "DISPLAYARTIST":
                                if (item.Artist.Length == 0)
                                    item.Artist = media.getItemInfo(media.getAttributeName(i)).Trim();
                                break;

                            case "TITLE":
                                if (item.Title.Length == 0)
                                    item.Title = media.getItemInfo(media.getAttributeName(i)).Trim();
                                break;

                            case "WM/ALBUMARTIST":
                                if (item.Artist.Length == 0)
                                    item.Artist = media.getItemInfo(media.getAttributeName(i)).Trim();
                                break;

                            case "WM/ALBUMTITLE":
                                if (item.Album.Length == 0)
                                    item.Album = media.getItemInfo(media.getAttributeName(i)).Trim();
                                break;
                        }
                    }

                    if (String.IsNullOrEmpty(item.Artist))
                        item.Artist = item.Author;
                }
            }
            catch { return null; }

            return item;
        }
    }
}
