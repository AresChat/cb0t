using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace cb0t
{
    class Menus
    {
        public static List<CustomMenuOption> UserList { get; set; }
        public static List<CustomMenuOption> Room { get; set; }

        public static void Load()
        {
            UserList = new List<CustomMenuOption>();
            Room = new List<CustomMenuOption>();

            String path = Path.Combine(Settings.DataPath, "ulmenu.dat");
            List<byte> list = new List<byte>();
            byte[] buf;
            int count;

            try
            {
                buf = File.ReadAllBytes(path);
                list.AddRange(buf);
            }
            catch { }

            while (list.Count(x => x == 0) >= 2)
            {
                CustomMenuOption f = new CustomMenuOption();
                count = list.IndexOf(0);
                f.Name = Encoding.UTF8.GetString(list.ToArray(), 0, count);
                list.RemoveRange(0, (count + 1));
                count = list.IndexOf(0);
                f.Text = Encoding.UTF8.GetString(list.ToArray(), 0, count);
                list.RemoveRange(0, (count + 1));
                UserList.Add(f);
            }

            path = Path.Combine(Settings.DataPath, "rmenu.dat");
            list = new List<byte>();

            try
            {
                buf = File.ReadAllBytes(path);
                list.AddRange(buf);
            }
            catch { }

            while (list.Count(x => x == 0) >= 2)
            {
                CustomMenuOption f = new CustomMenuOption();
                count = list.IndexOf(0);
                f.Name = Encoding.UTF8.GetString(list.ToArray(), 0, count);
                list.RemoveRange(0, (count + 1));
                count = list.IndexOf(0);
                f.Text = Encoding.UTF8.GetString(list.ToArray(), 0, count);
                list.RemoveRange(0, (count + 1));
                Room.Add(f);
            }
        }

        public static void UpdateUL()
        {
            String path = Path.Combine(Settings.DataPath, "ulmenu.dat");
            List<byte> list = new List<byte>();

            foreach (CustomMenuOption f in UserList)
            {
                list.AddRange(Encoding.UTF8.GetBytes(f.Name));
                list.Add(0);
                list.AddRange(Encoding.UTF8.GetBytes(f.Text));
                list.Add(0);
            }

            try { File.WriteAllBytes(path, list.ToArray()); }
            catch { }

            list.Clear();
            list = null;
        }

        public static void UpdateR()
        {
            String path = Path.Combine(Settings.DataPath, "rmenu.dat");
            List<byte> list = new List<byte>();

            foreach (CustomMenuOption f in Room)
            {
                list.AddRange(Encoding.UTF8.GetBytes(f.Name));
                list.Add(0);
                list.AddRange(Encoding.UTF8.GetBytes(f.Text));
                list.Add(0);
            }

            try { File.WriteAllBytes(path, list.ToArray()); }
            catch { }

            list.Clear();
            list = null;
        }
    }

    public class CustomMenuOption
    {
        public String Name { get; set; }
        public String Text { get; set; }
    }
}
