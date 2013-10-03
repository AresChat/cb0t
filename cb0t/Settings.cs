using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace cb0t
{
    class Settings
    {
        public static String DataPath { get; set; }
        public static String AppPath { get; set; }
        public static String VoicePath { get; set; }

        private static Stopwatch sw { get; set; }

        public static void Create()
        {
            DataPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\cb0t\\data\\";
            VoicePath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\cb0t\\data\\temp\\voice\\";
            AppPath = AppDomain.CurrentDomain.BaseDirectory;

            sw = new Stopwatch();
            sw.Start();

            if (!Directory.Exists(DataPath))
                Directory.CreateDirectory(DataPath);

            SetupTemp();
        }

        private static void SetupTemp()
        {
            String path = Path.Combine(DataPath, "temp");

            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            path = Path.Combine(DataPath, "temp\\voice");

            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            FileInfo[] files = new DirectoryInfo(path).GetFiles();

            foreach (FileInfo f in files)
                try { File.Delete(f.FullName); }
                catch { }

            path = Path.Combine(DataPath, "temp\\scribble");

            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            files = new DirectoryInfo(path).GetFiles();

            foreach (FileInfo f in files)
                try { File.Delete(f.FullName); }
                catch { }
        }

        public static uint Time
        {
            get
            {
                return (uint)Math.Floor((double)sw.ElapsedMilliseconds / 1000);
            }
        }
    }
}
