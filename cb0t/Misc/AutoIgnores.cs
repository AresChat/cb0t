using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace cb0t
{
    class AutoIgnores
    {
        private static List<AutoIgnoreItem> Items { get; set; }

        public static void Load()
        {
            Items = new List<AutoIgnoreItem>();

            String path = Path.Combine(Settings.DataPath, "ignores.dat");
            List<byte> list = new List<byte>();
            byte[] buf;
            int count;

            try
            {
                buf = File.ReadAllBytes(path);
                list.AddRange(buf);
            }
            catch { }

            while (list.Count(x => x == 0) >= 3)
            {
                AutoIgnoreItem f = new AutoIgnoreItem();
                count = list.IndexOf(0);
                f.Name = Encoding.UTF8.GetString(list.ToArray(), 0, count);
                list.RemoveRange(0, (count + 1));
                count = list.IndexOf(0);
                f.Condition = (AutoIgnoreCondition)int.Parse(Encoding.UTF8.GetString(list.ToArray(), 0, count));
                list.RemoveRange(0, (count + 1));
                count = list.IndexOf(0);
                f.Action = (AutoIgnoreType)int.Parse(Encoding.UTF8.GetString(list.ToArray(), 0, count));
                list.RemoveRange(0, (count + 1));
                Items.Add(f);
            }
        }

        private static void Update()
        {
            String path = Path.Combine(Settings.DataPath, "ignores.dat");
            List<byte> list = new List<byte>();

            foreach (AutoIgnoreItem f in Items)
            {
                list.AddRange(Encoding.UTF8.GetBytes(f.Name));
                list.Add(0);
                list.AddRange(Encoding.UTF8.GetBytes(((int)f.Condition).ToString()));
                list.Add(0);
                list.AddRange(Encoding.UTF8.GetBytes(((int)f.Action).ToString()));
                list.Add(0);
            }

            try { File.WriteAllBytes(path, list.ToArray()); }
            catch { }

            list.Clear();
            list = null;
        }

        public static bool AddItem(String name, int condition, int action)
        {
            if (Items.Find(x => x.Name == name) != null)
                return false;

            Items.Add(new AutoIgnoreItem { Name = name, Action = (AutoIgnoreType)action, Condition = (AutoIgnoreCondition)condition });
            Update();
            return true;
        }

        public static void RemoveItem(int index)
        {
            if (index >= 0 && index < Items.Count)
            {
                Items.RemoveAt(index);
                Update();
            }
        }

        public static AutoIgnoreType IgnoreType(String n)
        {
            AutoIgnoreItem[] tmp = Items.ToArray();
            String space = " ";
            String empty = "";
            String name = n.ToUpper().Replace(space, empty);

            foreach (AutoIgnoreItem i in tmp)
                if (i.Condition == AutoIgnoreCondition.Includes)
                {
                    if (name.Contains(i.Name.ToUpper().Replace(space, empty)))
                        return i.Action;
                }
                else if (i.Condition == AutoIgnoreCondition.Ends)
                {
                    if (name.EndsWith(i.Name.ToUpper().Replace(space, empty)))
                        return i.Action;
                }
                else if (i.Condition == AutoIgnoreCondition.Equals)
                {
                    if (name.Equals(i.Name.ToUpper().Replace(space, empty)))
                        return i.Action;
                }
                else if (i.Condition == AutoIgnoreCondition.Starts)
                {
                    if (name.StartsWith(i.Name.ToUpper().Replace(space, empty)))
                        return i.Action;
                }

            return AutoIgnoreType.None;
        }

        public static int Count
        {
            get { return Items.Count; }
        }

        public static AutoIgnoreItem[] ToArray()
        {
            return Items.ToArray();
        }
    }

    public enum AutoIgnoreType
    {
        Text = 0,
        PM = 1,
        All = 2,
        None = 3
    }

    public enum AutoIgnoreCondition : int
    {
        Starts = 0,
        Ends = 1,
        Includes = 2,
        Equals = 3
    }

    public class AutoIgnoreItem
    {
        public String Name { get; set; }
        public AutoIgnoreCondition Condition { get; set; }
        public AutoIgnoreType Action { get; set; }
    }
}
