using System;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace MediaIPC.Foobar2000
{
    class FoobarListener
    {
        public String Song
        {
            get
            {
                Process[] ps = Process.GetProcessesByName("foobar2000");

                if (ps.Length > 0)
                {
                    String str = ps[0].MainWindowTitle;
                    int index = str.ToUpper().LastIndexOf("[FOOBAR");

                    if (index > -1)
                    {
                        str = str.Substring(0, index).Trim();

                        if (str.Contains("[") && str.Contains("]"))
                            str = Regex.Replace(str, @"\[(.*?)\]", "");

                        if (str.Contains("  "))
                            str = Regex.Replace(str, "  ", " ");

                        return str;
                    }
                }

                return String.Empty;
            }
        }
    }
}
