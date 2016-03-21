﻿using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace cb0t
{
    class Settings
    {
        public const String APP_NAME = "cb0t";
        public const String APP_VERSION = "3.36";

        public static bool CAN_WRITE_REG { get; set; }
        public static bool IsAway { get; set; }
        public static bool CanHTML { get; set; }

        public static AresFont MyFont { get; set; }
        public static String DataPath { get; set; }
        public static String AppPath { get; set; }
        public static String VoicePath { get; set; }
        public static String ScribblePath { get; set; }
        public static String ArtPath { get; set; }
        public static String ScriptPath { get; set; }
        public static String AniEmoticPath { get; set; }
        public static String CachePath { get; set; }

        public static ulong ScribbleIdent { get; set; }

        private static Stopwatch sw { get; set; }

        public static void Create()
        {
            ScribbleIdent = 0;
            DataPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\cb0tv3\\data\\";
            VoicePath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\cb0tv3\\data\\temp\\voice\\";
            ScribblePath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\cb0tv3\\data\\temp\\scribble\\";
            ArtPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\cb0tv3\\data\\temp\\art\\";
            CachePath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\cb0tv3\\data\\temp\\cache\\";
            ScriptPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\cb0tv3\\data\\scripts\\";
            AppPath = AppDomain.CurrentDomain.BaseDirectory;
            AniEmoticPath = (AppPath + "aniemotic\\").Replace("\\", "/");
            
            sw = new Stopwatch();
            sw.Start();

            if (!Directory.Exists(DataPath))
                Directory.CreateDirectory(DataPath);

            if (!Directory.Exists(ScriptPath))
                Directory.CreateDirectory(ScriptPath);

            SetupTemp();
        }

        private static void SetupTemp()
        {
            String path = Path.Combine(DataPath, "temp");

            if (!Directory.Exists(path))
                try { Directory.CreateDirectory(path); }
                catch { }

            path = Path.Combine(DataPath, "temp\\voice");

            if (!Directory.Exists(path))
                try { Directory.CreateDirectory(path); }
                catch { }

            FileInfo[] files = new DirectoryInfo(path).GetFiles();

            foreach (FileInfo f in files)
                try { File.Delete(f.FullName); }
                catch { }

            path = Path.Combine(DataPath, "temp\\art");

            if (!Directory.Exists(path))
                try { Directory.CreateDirectory(path); }
                catch { }

            path = Path.Combine(DataPath, "temp\\scribble");

            if (!Directory.Exists(path))
                try { Directory.CreateDirectory(path); }
                catch { }

            files = new DirectoryInfo(path).GetFiles();

            foreach (FileInfo f in files)
                try { File.Delete(f.FullName); }
                catch { }

            path = Path.Combine(DataPath, "temp\\cache");

            if (Directory.Exists(path))
                try { Directory.Delete(path, true); }
                catch { }

            if (!Directory.Exists(path))
                try { Directory.CreateDirectory(path); }
                catch { }
        }

        public static IPAddress LocalIP
        {
            get
            {
             //   foreach (IPAddress ip in Dns.GetHostEntry(Dns.GetHostName()).AddressList)
               //     if (ip.AddressFamily == AddressFamily.InterNetwork)
                 //       return ip;

                return IPAddress.Loopback;
            }
        }

        public static Guid Guid
        {
            get
            {
                byte[] buf = GetReg<byte[]>("account", null);

                if (buf == null)
                {
                    buf = Guid.NewGuid().ToByteArray();
                    SetReg("account", buf);
                }

                return new Guid(buf);
            }
        }

        public static ushort Port
        {
            get
            {
                int p = GetReg<int>("data_port", 0);

                if (p == 0)
                {
                    Random rnd = new Random((int)Helpers.UnixTime);
                    p = rnd.Next(1000, 60000);
                    SetReg("data_port", p);
                }

                return (ushort)p;
            }
        }

        public static uint Time
        {
            get
            {
                return (uint)Math.Floor((double)sw.ElapsedMilliseconds / 1000);
            }
        }

        public static ulong TimeLong
        {
            get
            {
                return (ulong)sw.ElapsedMilliseconds;
            }
        }

        public static void SetReg(String name, object value)
        {
            if (!CAN_WRITE_REG)
                return;

            RegistryKey key = Registry.CurrentUser.OpenSubKey("Software\\cb0tv3", true);

            if (key == null)
            {
                Registry.CurrentUser.CreateSubKey("Software\\cb0tv3");
                key = Registry.CurrentUser.OpenSubKey("Software\\cb0tv3", true);
            }

            if (value is uint || value is int || value is ushort || value is byte)
                key.SetValue(name, value, RegistryValueKind.DWord);
            else if (value is String)
                key.SetValue(name, Encoding.UTF8.GetBytes((String)value), RegistryValueKind.Binary);
            else if (value is byte[])
                key.SetValue(name, value, RegistryValueKind.Binary);
            else if (value is bool)
                key.SetValue(name, (int)((bool)value ? 1 : 0), RegistryValueKind.DWord);

            key.Close();
        }

        public static T GetReg<T>(String name, T fall_back)
        {
            RegistryKey key = Registry.CurrentUser.OpenSubKey("Software\\cb0tv3");

            if (key == null)
                return fall_back;

            object result = key.GetValue(name, null);
            key.Close();

            if (result == null)
                return fall_back;

            Type type = typeof(T);

            if (type.Equals(typeof(bool)))
                return (T)Convert.ChangeType((int)result == 1, type);
            else if (type.Equals(typeof(String)))
                return (T)Convert.ChangeType(Encoding.UTF8.GetString((byte[])result), type);
            else
                return (T)Convert.ChangeType(result, type);
        }
    }
}
