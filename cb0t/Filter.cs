using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace cb0t
{
    class Filter
    {
        public static List<FilterItem> Items { get; set; }

        public static void Load()
        {
            Items = new List<FilterItem>();
            String path = Path.Combine(Settings.DataPath, "filters.dat");
            List<byte> list = new List<byte>();
            byte[] buf;
            int count;

            try
            {
                buf = File.ReadAllBytes(path);
                list.AddRange(buf);
            }
            catch { }

            while (list.Count(x => x == 0) >= 7)
            {
                FilterItem f = new FilterItem();
                count = list.IndexOf(0);
                f.Argument = Encoding.UTF8.GetString(list.ToArray(), 0, count);
                list.RemoveRange(0, (count + 1));
                count = list.IndexOf(0);
                f.Condition = Encoding.UTF8.GetString(list.ToArray(), 0, count);
                list.RemoveRange(0, (count + 1));
                count = list.IndexOf(0);
                f.Event = Encoding.UTF8.GetString(list.ToArray(), 0, count);
                list.RemoveRange(0, (count + 1));
                count = list.IndexOf(0);
                f.Property = Encoding.UTF8.GetString(list.ToArray(), 0, count);
                list.RemoveRange(0, (count + 1));
                count = list.IndexOf(0);
                f.Room = Encoding.UTF8.GetString(list.ToArray(), 0, count);
                list.RemoveRange(0, (count + 1));
                count = list.IndexOf(0);
                f.Task = Encoding.UTF8.GetString(list.ToArray(), 0, count);
                list.RemoveRange(0, (count + 1));
                count = list.IndexOf(0);
                f.Text = Encoding.UTF8.GetString(list.ToArray(), 0, count);
                list.RemoveRange(0, (count + 1));
                Items.Add(f);
            }
        }

        public static void Update()
        {
            String path = Path.Combine(Settings.DataPath, "filters.dat");
            List<byte> list = new List<byte>();

            foreach (FilterItem f in Items)
            {
                list.AddRange(Encoding.UTF8.GetBytes(f.Argument));
                list.Add(0);
                list.AddRange(Encoding.UTF8.GetBytes(f.Condition));
                list.Add(0);
                list.AddRange(Encoding.UTF8.GetBytes(f.Event));
                list.Add(0);
                list.AddRange(Encoding.UTF8.GetBytes(f.Property));
                list.Add(0);
                list.AddRange(Encoding.UTF8.GetBytes(f.Room));
                list.Add(0);
                list.AddRange(Encoding.UTF8.GetBytes(f.Task));
                list.Add(0);
                list.AddRange(Encoding.UTF8.GetBytes(f.Text));
                list.Add(0);
            }

            try { File.WriteAllBytes(path, list.ToArray()); }
            catch { }

            list.Clear();
            list = null;
        }

        public static FilterResult[] GetFilters(String room, FilterEvent @event, String name, String text)
        {
            List<FilterResult> results = new List<FilterResult>();
            String _name = String.Empty;
            String _text = String.Empty;

            if (!String.IsNullOrEmpty(name))
                _name = name.ToUpper();

            if (!String.IsNullOrEmpty(text))
                _text = text.ToUpper();

            foreach (FilterItem item in Items)
                if (item.Room == room || String.IsNullOrEmpty(item.Room))
                    if (item.Event == @event.ToString())
                        if (item.Property == "Name")
                        {
                            String n = item.Argument.ToUpper();

                            if (item.Condition == "Starts" && _name.StartsWith(n))
                                results.Add(new FilterResult { Task = Raw2Task(item.Task), Text = item.Text });
                            else if (item.Condition == "Ends" && _name.EndsWith(n))
                                results.Add(new FilterResult { Task = Raw2Task(item.Task), Text = item.Text });
                            else if (item.Condition == "Includes" && _name.Contains(n))
                                results.Add(new FilterResult { Task = Raw2Task(item.Task), Text = item.Text });
                            else if (item.Condition == "Equals" && _name.Equals(n))
                                results.Add(new FilterResult { Task = Raw2Task(item.Task), Text = item.Text });
                        }
                        else if (item.Property == "Text")
                        {
                            String t = item.Argument.ToUpper();

                            if (item.Condition == "Starts" && _text.StartsWith(t))
                                results.Add(new FilterResult { Task = Raw2Task(item.Task), Text = item.Text });
                            else if (item.Condition == "Ends" && _text.EndsWith(t))
                                results.Add(new FilterResult { Task = Raw2Task(item.Task), Text = item.Text });
                            else if (item.Condition == "Includes" && _text.Contains(t))
                                results.Add(new FilterResult { Task = Raw2Task(item.Task), Text = item.Text });
                            else if (item.Condition == "Equals" && _text.Equals(t))
                                results.Add(new FilterResult { Task = Raw2Task(item.Task), Text = item.Text });
                        }

            return results.ToArray();
        }

        private static FilterTask Raw2Task(String raw)
        {
            switch (raw)
            {
                case "Block":
                    return FilterTask.Block;

                case "Send":
                    return FilterTask.Send;

                default:
                    return FilterTask.Notify;
            }
        }
    }

    public enum FilterEvent
    {
        Join,
        Part,
        Text,
        Emote,
        PM
    }

    public enum FilterTask
    {
        Block,
        Notify,
        Send
    }

    public class FilterResult
    {
        public FilterTask Task { get; set; }
        public String Text { get; set; }
    }
}
