using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace cb0t
{
    class Friends
    {
        private static List<String> friends { get; set; }

        public static void Load()
        {
            friends = new List<String>();

            try
            {
                if (File.Exists(Path.Combine(Settings.DataPath, "friends.dat")))
                {
                    String[] names = File.ReadAllText(Path.Combine(Settings.DataPath, "friends.dat")).Split(new String[] { "\0" }, StringSplitOptions.RemoveEmptyEntries);

                    foreach (String str in names)
                        if (!String.IsNullOrEmpty(str))
                            friends.Add(str);
                }
            }
            catch { }
        }

        private static void Update()
        {
            try
            {
                File.WriteAllText(Path.Combine(Settings.DataPath, "friends.dat"), String.Join("\0", friends.ToArray()));
            }
            catch { }
        }

        public static void FriendStatusChanged(String name)
        {
            if (friends.Find(x => x == name) != null)
            {
                friends.RemoveAll(x => x == name);
                RoomPool.Rooms.ForEach(x => x.FriendStatusChanged(name, false));
            }
            else
            {
                friends.Add(name);
                RoomPool.Rooms.ForEach(x => x.FriendStatusChanged(name, true));
            }

            Update();
        }

        public static bool IsFriend(String name)
        {
            return friends.Find(x => x == name) != null;
        }
    }
}
