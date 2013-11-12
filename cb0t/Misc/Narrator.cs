using System;
using System.Collections.Generic;
using System.Linq;
using System.Speech.Synthesis;
using System.Text;
using System.Text.RegularExpressions;

namespace cb0t
{
    class Narrator
    {
        private static SpeechSynthesizer speech { get; set; }
        private static bool busy { get; set; }
        private static List<String> lines { get; set; }

        public static void Init()
        {
            lines = new List<String>();
            speech = new SpeechSynthesizer();
            speech.SpeakCompleted += SpeakCompleted;
        }

        public static void ClearList()
        {
            lock (lines)
                lines.Clear();
        }

        private static void SpeakCompleted(object sender, SpeakCompletedEventArgs e)
        {
            lock (lines)
                if (lines.Count > 0)
                {
                    String text = lines[0];
                    lines.RemoveAt(0);
                    speech.SpeakAsync(text);
                }
                else busy = false;
        }

        public static void Say(String text)
        {
            if (text.StartsWith("["))
                return;

            String str = text.Replace("..", "  ");
            List<String> words = new List<String>(str.Split(new String[] { " " }, StringSplitOptions.RemoveEmptyEntries));

            for (int i = (words.Count - 1); i > -1; i--)
                if (words[i].StartsWith("(") && words[i].EndsWith(")") ||
                    (words[i].StartsWith(":") && words[i].Length <= 3) ||
                    (words[i].StartsWith(";") && words[i].Length <= 3))
                    words.RemoveAt(i);
                else
                {
                    List<char> list = new List<char>(new char[] { (char)0, (char)0, (char)0 });
                    String s = String.Empty;

                    foreach (char c in words[i])
                    {
                        list.RemoveAt(0);
                        list.Add(c);

                        if (!list.TrueForAll(x => x == c))
                            s += c.ToString();
                    }

                    if (s.Length > 0)
                    {
                        foreach (String r in emotes)
                            s = Regex.Replace(s, Regex.Escape(r), String.Empty, RegexOptions.IgnoreCase);

                        if (s.ToUpper().Equals("LOL"))
                            s = "laugh out loud";
                        else if (s.ToUpper().Contains("LMAO"))
                            s = "laughing my ass off";
                        else if (s.ToUpper().Contains("PMSL"))
                            s = "pissing myself laugh";
                        else if (s.ToUpper().Contains("OMG"))
                            s = "oh my god";
                        else if (s.ToUpper().Contains("ROFL"))
                            s = "rolling on the floor laughing";
                        else if (s.ToUpper().Contains("LMFAO"))
                            s = "laughing my fucking ass off";
                        else if (s.ToUpper().Contains("OMFG"))
                            s = "oh my fucking god";
                        else if (s.ToUpper().Contains("WTF"))
                            s = "what the fuck?";
                        else if (s.ToUpper().Contains("WB"))
                            s = "welcome back";
                        else if (s.ToUpper().Contains("TY"))
                            s = "thank you";
                        else if (s.ToUpper().Contains("THX"))
                            s = "thank you";
                        else if (s.ToUpper().Contains("YW"))
                            s = "you're welcome";
                        else if (s.ToUpper().Contains("BRB"))
                            s = "be right back";
                        else if (s.ToUpper().Contains("BBL"))
                            s = "be back later";
                        else if (s.ToUpper().Contains("BBS"))
                            s = "be back soon";
                        else if (s.ToUpper().Contains("BBIAB"))
                            s = "be back in a bit";
                        else if (s.ToUpper().Contains("BIAB"))
                            s = "back in a bit";
                        else if (s.ToUpper().Contains("TYT"))
                            s = "take your time";
                        else if (s.ToUpper().Contains("TTYL"))
                            s = "talk to you later";

                        words[i] = s;

                        if (s.ToUpper().StartsWith("HTTP://") ||
                            s.ToUpper().StartsWith("HTTPS://") ||
                            s.ToUpper().StartsWith("WW.") ||
                            s.ToUpper().StartsWith("\\ARLNK") ||
                            s.ToUpper().StartsWith("ARLNK"))
                            words.RemoveAt(i);
                    }
                    else words.RemoveAt(i);
                }

            str = String.Join(" ", words.ToArray());
            str = str.Replace(":", String.Empty);
            str = str.Replace("*", String.Empty);
            str = str.Replace("#", String.Empty);
            str = str.Replace("&", String.Empty);
            str = str.Replace("^", String.Empty);
            str = str.Replace("/", String.Empty);
            str = str.Replace("\\", String.Empty);
            str = str.Replace("=", String.Empty);
            str = str.Replace("_", String.Empty);
            str = str.Replace(">", String.Empty);
            str = str.Replace("<", String.Empty);

            if (!String.IsNullOrEmpty(str))
            {
                if (busy)
                    lock (lines)
                        lines.Add(str);
                else
                    speech.SpeakAsync(str);
            }
        }

        public static void Suspend(bool suspend)
        {
            if (suspend)
                speech.Pause();
            else
                speech.Resume();
        }

        private static String[] emotes = new String[]
        {
            ":)", ":-)", ":D", ":-D", ";)", ";-)",
            ":O", ":-O", ":P", ":-P", "(H)", ":@",
            ":-@", ":$", ":-$", ":S", ":-S", ":(",
            ":-(", ":'(", ":|", ":-|", "(6)", "(A)",
            "(L)", "(U)", "(M)", "(@)", "(&)", "(S)",
            "(*)", "(~)", "(E)", "(8)", "(F)", "(W)",
            "(O)", "(K)", "(G)", "(^)", "(P)", "(I)",
            "(C)", "(T)", "({)", "(})", "(B)", "(D)",
            "(Z)", "(X)", "(Y)", "(N)", ":[", ":-[",
            "(1)", "(2)", "(3)", "(4)"
        };
    }
}
