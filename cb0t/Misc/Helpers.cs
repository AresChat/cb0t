using System;
using System.Collections.Generic;
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
    }
}
