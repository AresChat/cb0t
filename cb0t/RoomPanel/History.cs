using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace cb0t
{
    class History
    {
        private static List<String> lines = new List<String>();
        private static int position = -1;

        public static void AddText(String text)
        {
            position = -1;

            if (lines.Count > 1)
                if (lines[0] == text)
                    return;

            lines.Insert(0, text);

            if (lines.Count > 150)
                lines = lines.Take(75).ToList();
        }

        public static String GetText()
        {
            if (lines.Count == 0)
                return String.Empty;

            if (++position >= lines.Count)
                position = 0;

            return lines[position];
        }

        public static void Reset()
        {
            position = -1;
        }
    }
}
