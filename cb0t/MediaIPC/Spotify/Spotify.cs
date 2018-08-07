using System;
using System.Diagnostics;

namespace MediaIPC.Spotify
{
    class Spotify
    {
        public String Song
        {
            get
            {
                Process[] ps = Process.GetProcessesByName("spotify");

                if (ps.Length > 0)
                {
                    String str = ps[0].MainWindowTitle;
                    
                    if (str.ToUpper().Contains("SPOTIFY")) //Adverts ....
                        return String.Empty;
                    
                    return str;
                }

                return String.Empty;
            }
        }
    }
}
