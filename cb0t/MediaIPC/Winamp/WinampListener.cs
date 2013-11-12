using System;
using System.Diagnostics;

namespace MediaIPC.Winamp
{
    class WinampListener
    {
        private int i;

        public String Song
        {
            get
            {
                Process[] ps = Process.GetProcessesByName("winamp");

                if (ps.Length > 0)
                {
                    String str = ps[0].MainWindowTitle;
                    int index = str.LastIndexOf("-");

                    if (index > -1)
                    {
                        str = str.Substring(0, index).Trim();

                        if (str.ToUpper().Contains("BUFFER"))
                            return String.Empty;

                        String[] split = str.Split(new String[] { " " }, StringSplitOptions.RemoveEmptyEntries);

                        if (split[0].IndexOf(".") == split[0].Length - 1)
                        {
                            if (int.TryParse(split[0].Substring(0, split[0].Length - 1), out i))
                                split[0] = String.Empty;

                            str = String.Join(" ", split).Trim();
                        }

                        return str;
                    }
                }

                return String.Empty;
            }
        }
    }
}
