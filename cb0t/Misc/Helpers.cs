using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace cb0t
{
    class Helpers
    {
        public static uint UserIdent { get; set; }

        static Helpers()
        {
            UserIdent = 0;
        }

        public static String StripColors(String input)
        {
            if (Regex.IsMatch(input, @"\x03|\x05", RegexOptions.IgnoreCase))
                input = Regex.Replace(input, @"(\x03|\x05)[0-9]{2}", "");

            input = input.Replace("\x06", "");
            input = input.Replace("\x07", "");
            input = input.Replace("\x09", "");
            input = input.Replace("\x02", "");
            input = input.Replace("­", "");

            return input;
        }

        public static bool IsHexCode(String hex)
        {
            if (hex.Substring(0, 1) != "#")
                return false;

            String str = hex.Substring(1);
            String r_str = str.Substring(0, 2);
            String g_str = str.Substring(2, 2);
            String b_str = str.Substring(4, 2);

            byte r, g, b;

            if (byte.TryParse(r_str, NumberStyles.HexNumber, null, out r))
                if (byte.TryParse(g_str, NumberStyles.HexNumber, null, out g))
                    if (byte.TryParse(b_str, NumberStyles.HexNumber, null, out b))
                        return true;

            return false;
        }

        public static String Timestamp
        {
            get
            {
                StringBuilder sb = new StringBuilder();
                DateTime dt = DateTime.Now;
                sb.Append(dt.Hour < 10 ? ("0" + dt.Hour) : dt.Hour.ToString());
                sb.Append(":");
                sb.Append(dt.Minute < 10 ? ("0" + dt.Minute) : dt.Minute.ToString());
                sb.Append(" ");
                return sb.ToString();
            }
        }
    }
}
