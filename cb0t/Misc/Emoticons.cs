using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace cb0t
{
    class Emoticons
    {
        private static List<Emotic> shortcuts { get; set; }
        public static Bitmap[] emotic { get; set; }

        public static Emotic FindEmoticon(String sc)
        {
            foreach (Emotic e in shortcuts)
                if (sc.StartsWith(e.Shortcut))
                    return e;

            return null;
        }

        private static void Load_emotic()
        {
            emotic = new Bitmap[49];
            int count = 0;
            Rectangle r1 = new Rectangle(0, 0, 16, 16);
            Rectangle r2 = new Rectangle(0, 0, 16, 16);

            using (Bitmap raw = Properties.Resources.emotic)
            {
                for (int y = 0; y < 7; y++)
                {
                    for (int x = 0; x < 7; x++)
                    {
                        emotic[count] = new Bitmap(16, 16);
                        r2.X = (x * 16);
                        r2.Y = (y * 16);

                        using (Graphics g = Graphics.FromImage(emotic[count]))
                            g.DrawImage(raw, r1, r2, GraphicsUnit.Pixel);

                        emotic[count].MakeTransparent(Color.Magenta);
                        count++;
                    }
                }
            }
        }

        public static void Load()
        {
            Load_emotic();
            Load_shortcuts();
        }

        private static void Load_shortcuts()
        {
            shortcuts = new List<Emotic>();
            shortcuts.Add(new Emotic { Index = 0, Shortcut = ":)" });
            shortcuts.Add(new Emotic { Index = 0, Shortcut = ":-)" });
            shortcuts.Add(new Emotic { Index = 1, Shortcut = ":D" });
            shortcuts.Add(new Emotic { Index = 1, Shortcut = ":-D" });
            shortcuts.Add(new Emotic { Index = 2, Shortcut = ";)" });
            shortcuts.Add(new Emotic { Index = 2, Shortcut = ";-)" });
            shortcuts.Add(new Emotic { Index = 3, Shortcut = ":O" });
            shortcuts.Add(new Emotic { Index = 3, Shortcut = ":-O" });
            shortcuts.Add(new Emotic { Index = 4, Shortcut = ":P" });
            shortcuts.Add(new Emotic { Index = 4, Shortcut = ":-P" });
            shortcuts.Add(new Emotic { Index = 5, Shortcut = "(H)" });
            shortcuts.Add(new Emotic { Index = 6, Shortcut = ":@" });
            shortcuts.Add(new Emotic { Index = 7, Shortcut = ":$" });
            shortcuts.Add(new Emotic { Index = 7, Shortcut = ":-$" });
            shortcuts.Add(new Emotic { Index = 8, Shortcut = ":S" });
            shortcuts.Add(new Emotic { Index = 8, Shortcut = ":-S" });
            shortcuts.Add(new Emotic { Index = 9, Shortcut = ":(" });
            shortcuts.Add(new Emotic { Index = 9, Shortcut = ":-(" });
            shortcuts.Add(new Emotic { Index = 10, Shortcut = ":'(" });
            shortcuts.Add(new Emotic { Index = 11, Shortcut = ":|" });
            shortcuts.Add(new Emotic { Index = 11, Shortcut = ":-|" });
            shortcuts.Add(new Emotic { Index = 12, Shortcut = "(6)" });
            shortcuts.Add(new Emotic { Index = 13, Shortcut = "(A)" });
            shortcuts.Add(new Emotic { Index = 14, Shortcut = "(L)" });
            shortcuts.Add(new Emotic { Index = 15, Shortcut = "(U)" });
            shortcuts.Add(new Emotic { Index = 16, Shortcut = "(M)" });
            shortcuts.Add(new Emotic { Index = 17, Shortcut = "(@)" });
            shortcuts.Add(new Emotic { Index = 18, Shortcut = "(&)" });
            shortcuts.Add(new Emotic { Index = 19, Shortcut = "(S)" });
            shortcuts.Add(new Emotic { Index = 20, Shortcut = "(*)" });
            shortcuts.Add(new Emotic { Index = 21, Shortcut = "(~)" });
            shortcuts.Add(new Emotic { Index = 22, Shortcut = "(E)" });
            shortcuts.Add(new Emotic { Index = 23, Shortcut = "(8)" });
            shortcuts.Add(new Emotic { Index = 24, Shortcut = "(F)" });
            shortcuts.Add(new Emotic { Index = 25, Shortcut = "(W)" });
            shortcuts.Add(new Emotic { Index = 26, Shortcut = "(O)" });
            shortcuts.Add(new Emotic { Index = 27, Shortcut = "(K)" });
            shortcuts.Add(new Emotic { Index = 28, Shortcut = "(G)" });
            shortcuts.Add(new Emotic { Index = 29, Shortcut = "(^)" });
            shortcuts.Add(new Emotic { Index = 30, Shortcut = "(P)" });
            shortcuts.Add(new Emotic { Index = 31, Shortcut = "(I)" });
            shortcuts.Add(new Emotic { Index = 32, Shortcut = "(C)" });
            shortcuts.Add(new Emotic { Index = 33, Shortcut = "(T)" });
            shortcuts.Add(new Emotic { Index = 34, Shortcut = "({)" });
            shortcuts.Add(new Emotic { Index = 35, Shortcut = "(})" });
            shortcuts.Add(new Emotic { Index = 36, Shortcut = "(B)" });
            shortcuts.Add(new Emotic { Index = 37, Shortcut = "(D)" });
            shortcuts.Add(new Emotic { Index = 38, Shortcut = "(Z)" });
            shortcuts.Add(new Emotic { Index = 39, Shortcut = "(X)" });
            shortcuts.Add(new Emotic { Index = 40, Shortcut = "(Y)" });
            shortcuts.Add(new Emotic { Index = 41, Shortcut = "(N)" });
            shortcuts.Add(new Emotic { Index = 42, Shortcut = ":[" });
            shortcuts.Add(new Emotic { Index = 42, Shortcut = ":-[" });
            shortcuts.Add(new Emotic { Index = 43, Shortcut = "(1)" });
            shortcuts.Add(new Emotic { Index = 44, Shortcut = "(2)" });
            shortcuts.Add(new Emotic { Index = 45, Shortcut = "(3)" });
            shortcuts.Add(new Emotic { Index = 46, Shortcut = "(4)" });
        }
    }

    class Emotic
    {
        public String Shortcut { get; set; }
        public int Index { get; set; }
    }
}
