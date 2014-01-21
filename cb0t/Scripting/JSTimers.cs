using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace cb0t.Scripting
{
    class JSTimers
    {
        public static int NextIdent = 0;

        private static List<JSTimerInstance> Items = new List<JSTimerInstance>();

        public static void ClearTimers(String script)
        {
            Items.RemoveAll(x => x.ScriptName == script);
        }

        public static void Add(JSTimerInstance item)
        {
            Items.Add(item);
        }

        public static bool Remove(int i)
        {
            bool success = Items.Find(x => x.Ident == i) != null;
            Items.RemoveAll(x => x.Ident == i);
            return success;
        }

        public static void ServiceTimers(ulong time)
        {
            for (int i = (Items.Count - 1); i > -1; i--)
                if (time >= Items[i].Time)
                {
                    if (Items[i].Callback != null)
                        try { Items[i].Callback.Call(Items[i].Callback.Engine.Global); }
                        catch { }

                    if (Items[i].Loop)
                        Items[i].Time = (time + (ulong)Items[i].Interval);
                    else
                        Items.RemoveAt(i);
                }
        }
    }
}
