using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

namespace cb0t
{
    class Emoji
    {
        private static List<EmojiItem> Items { get; set; }

        public static void Load()
        {
            Items = new List<EmojiItem>();

            DirectoryInfo at16 = new DirectoryInfo(Path.Combine(Settings.AppPath, "emoji", "at16"));
            DirectoryInfo at24 = new DirectoryInfo(Path.Combine(Settings.AppPath, "emoji", "at24"));

            EmojiItem ei;

            foreach (FileInfo file in at24.GetFiles())
            {
                ei = new EmojiItem();
                ei.FileName = file.Name;

                if (!ei.FileName.Contains("-"))
                    ei.SurrogateSequence = HexToSequence(Path.GetFileNameWithoutExtension(ei.FileName)).TrimEnd();
                else
                {
                    String hex = Path.GetFileNameWithoutExtension(ei.FileName);
                    String[] parts = hex.Split('-');
                    StringBuilder sb = new StringBuilder();

                    foreach (String p in parts)
                        sb.Append(HexToSequence(p));

                    ei.SurrogateSequence = sb.ToString().TrimEnd();
                }

                ei.Length = ei.SurrogateSequence.Split(' ').Length;

                using (MemoryStream ms = new MemoryStream(File.ReadAllBytes(Path.Combine(at16.FullName, ei.FileName))))
                using (Bitmap bmp = new Bitmap(ms))
                {
                    ei.Image = new Bitmap(16, 16);

                    using (Graphics g = Graphics.FromImage(ei.Image))
                        g.DrawImage(bmp, new Point(0, 0));
                }

                Items.Add(ei);
            }
        }

        private static String HexToSequence(String hex)
        {
            String str = Char.ConvertFromUtf32(int.Parse(hex, NumberStyles.HexNumber));
            char[] chrs = str.ToCharArray();
            StringBuilder sb = new StringBuilder();

            foreach (char c in chrs)
                sb.Append(((int)c) + " ");

            return sb.ToString();
        }

        public static EmojiItem GetEmoji24(char[] letters, int start_pos)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append((int)letters[start_pos]);

            for (int i = 0; i < 3; i++)
                if ((letters.Length - 1) >= (start_pos + i + 1))
                {
                    sb.Append(" ");
                    sb.Append((int)letters[start_pos + i + 1]);
                }

            String str = sb.ToString();
            return Items.FirstOrDefault(x => str.StartsWith(x.SurrogateSequence));
        }
    }

    class EmojiItem
    {
        public Bitmap Image { get; set; }
        public String FileName { get; set; }
        public String SurrogateSequence { get; set; }
        public int Length { get; set; } // max length = 4
    }
}
