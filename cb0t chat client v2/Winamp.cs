using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace cb0t_chat_client_v2
{
    class Winamp
    {
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern IntPtr FindWindow([MarshalAs(UnmanagedType.LPTStr)] string lpClassName, [MarshalAs(UnmanagedType.LPTStr)] string lpWindowName);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern int GetWindowText(IntPtr hwnd, string lpString, int cch);

        public event EventHandler WinampSongChanged;

        public static String current_song = String.Empty;

        public void Tick()
        {
            IntPtr hwnd = FindWindow("Winamp v1.x", null);

            if (hwnd.Equals(IntPtr.Zero))
                return;

            String tmp = new String((char)0, 200);
            int strlen = GetWindowText(hwnd, tmp, 200);

            if (strlen <= 0)
                return;

            try
            {
                tmp = this.TidySongName(tmp.Substring(0, strlen));
            }
            catch { return; }

            if (String.IsNullOrEmpty(tmp))
                return;

            if (tmp != current_song)
            {
                current_song = tmp;
                this.WinampSongChanged(this, new EventArgs());
            }
        }

        private String TidySongName(String str)
        {
            if (str.IndexOf(".") > -1)
                str = str.Substring(str.IndexOf(".") + 2);

            if ((str.LastIndexOf("[") > 0) && (str.LastIndexOf("]") > -1))
                if (str.LastIndexOf("[") < str.LastIndexOf("]"))
                    str = str.Substring(0, (str.LastIndexOf("[") - 1));

            str = str.Replace(" - Winamp", String.Empty);

            if (str == "Winamp" ||
                str == "N/A" ||
                str == "Stopped" ||
                str == "Paused")
                return null;

            if (str.ToUpper().Contains("BUFFER"))
                return null;

            if (str.Length <= 1)
                return null;

            return str;
        }
    }
}
