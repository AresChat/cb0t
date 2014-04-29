using iTunesLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace MediaIPC.iTunes
{
    class iTunesListener
    {
        private iTunesApp itunes { get; set; }
        private int reconnect_ticks = 0;

        [DllImport("user32.dll", SetLastError = true)]
        private static extern IntPtr FindWindow(IntPtr ptr, string lpWindowName);

        public String Song
        {
            get
            {
                if (this.itunes == null)
                {
                    if (this.reconnect_ticks++ < 5)
                        return String.Empty;

                    IntPtr ptr = FindWindow(IntPtr.Zero, "itunes");

                    if (!ptr.Equals(IntPtr.Zero))
                    {
                        this.itunes = new iTunesApp();
                        this.itunes.OnAboutToPromptUserToQuitEvent += this.DestroyCOMLink;
                    }
                }
                else
                {
                    try
                    {
                        IITTrack track = this.itunes.CurrentTrack;

                        if (track != null)
                        {
                            StringBuilder sb = new StringBuilder();

                            if (!String.IsNullOrEmpty(track.Artist))
                                sb.Append(track.Artist);
                            else if (!String.IsNullOrEmpty(track.Composer))
                                sb.Append(track.Composer);
                            else
                                sb.Append("Unknown Artist");

                            sb.Append(" - " + track.Name);
                            return sb.ToString();
                        }
                    }
                    catch { }
                }

                return String.Empty;
            }
        }

        private void DestroyCOMLink()
        {
            this.reconnect_ticks = 0;
            Marshal.ReleaseComObject(this.itunes);
            GC.Collect(GC.GetGeneration(this.itunes));
            this.itunes = null;
        }
    }
}
