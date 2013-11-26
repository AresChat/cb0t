using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace cb0t
{
    class SpellChecker
    {
        public static byte[] AFF { get; private set; }
        public static byte[] DIC { get; private set; }

        public static void Load()
        {
            AllowedWords = new List<String>();

            try { AllowedWords.AddRange(File.ReadAllLines(Settings.DataPath + "sp_exceptions.txt", Encoding.UTF8)); }
            catch { }

            int id = Settings.GetReg<int>("spell_checker", 0);
            String path = Path.Combine(Settings.AppPath, "dictionary");

            switch (id)
            {
                case 0:
                    AFF = null;
                    DIC = null;
                    break;

                case 1:
                    AFF = File.ReadAllBytes(Path.Combine(path, "english\\dictionary.aff"));
                    DIC = File.ReadAllBytes(Path.Combine(path, "english\\dictionary.dic"));
                    break;

                case 2:
                    AFF = File.ReadAllBytes(Path.Combine(path, "dutch\\dictionary.aff"));
                    DIC = File.ReadAllBytes(Path.Combine(path, "dutch\\dictionary.dic"));
                    break;

                case 3:
                    AFF = File.ReadAllBytes(Path.Combine(path, "french\\dictionary.aff"));
                    DIC = File.ReadAllBytes(Path.Combine(path, "french\\dictionary.dic"));
                    break;

                case 4:
                    AFF = File.ReadAllBytes(Path.Combine(path, "italian\\dictionary.aff"));
                    DIC = File.ReadAllBytes(Path.Combine(path, "italian\\dictionary.dic"));
                    break;

                case 5:
                    AFF = File.ReadAllBytes(Path.Combine(path, "spanish\\dictionary.aff"));
                    DIC = File.ReadAllBytes(Path.Combine(path, "spanish\\dictionary.dic"));
                    break;

                case 6:
                    AFF = File.ReadAllBytes(Path.Combine(path, "spanish (mexico)\\dictionary.aff"));
                    DIC = File.ReadAllBytes(Path.Combine(path, "spanish (mexico)\\dictionary.dic"));
                    break;

                case 7:
                    AFF = File.ReadAllBytes(Path.Combine(path, "spanish (catalan)\\dictionary.aff"));
                    DIC = File.ReadAllBytes(Path.Combine(path, "spanish (catalan)\\dictionary.dic"));
                    break;
            }
        }

        public static List<String> AllowedWords { get; set; }

        public static void AddAllowedWord(String word)
        {
            AllowedWords.Add(word);

            try { File.WriteAllLines(Settings.DataPath + "sp_exceptions.txt", AllowedWords.ToArray(), Encoding.UTF8); }
            catch { }
        }
    }
}
