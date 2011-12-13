using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace cb0t_chat_client_v2
{
    class iTunes
    {
        [DllImport("user32.dll", SetLastError = true)]
        static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        [DllImport("user32.dll", SetLastError = true)]
        static extern IntPtr FindWindowEx(IntPtr hwndParent, IntPtr hwndChildAfter, string lpszClass, string lpszWindow);

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        static extern int GetWindowTextLength(IntPtr hWnd);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

        public event EventHandler iTunesSongChanged;
        public static String current_song = String.Empty;

        public void Tick()
        {
            String tmp = this.GetItunesSong();

            if (String.IsNullOrEmpty(tmp))
                return;

            if (tmp != current_song)
            {
                current_song = tmp;
                this.iTunesSongChanged(this, new EventArgs());
            }
        }

        private String GetItunesSong()
        {
            IntPtr ptr = FindWindow(null, "iTunes");

            if (!ptr.Equals(IntPtr.Zero))
            {
                ptr = FindWindowEx(ptr, IntPtr.Zero, null, "LCD section");

                if (!ptr.Equals(IntPtr.Zero))
                {
                    IntPtr current = FindWindowEx(ptr, IntPtr.Zero, "Static", null);
                    List<String> items = new List<String>();

                    while (!current.Equals(IntPtr.Zero))
                    {
                        current = FindWindowEx(ptr, current, "Static", null);

                        if (!current.Equals(IntPtr.Zero))
                        {
                            int size = GetWindowTextLength(current);

                            if (size > 0)
                            {
                                StringBuilder sb = new StringBuilder(size + 1);
                                GetWindowText(current, sb, sb.Capacity);
                                String text = sb.ToString();

                                if (!String.IsNullOrEmpty(text))
                                    if (!text.StartsWith(" "))
                                        items.Add(text);
                            }
                        }
                    }

                    if (items.Count > 0)
                        return String.Join(" — ", items.ToArray()).Replace("—", "-");
                }
            }

            return String.Empty;
        }
    }
}
