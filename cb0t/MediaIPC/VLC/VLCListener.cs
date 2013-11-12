using System;
using System.Diagnostics;

namespace MediaIPC.VLC
{
    class VLCListener
    {
        public String Song
        {
            get
            {
                Process[] ps = Process.GetProcessesByName("vlc");

                if (ps.Length > 0)
                {
                    String str = ps[0].MainWindowTitle;
                    int index = str.LastIndexOf("-");

                    if (index > -1)
                        return str.Substring(0, index).Trim();
                }

                return String.Empty;
            }
        }
    }
}
