using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace cb0t
{
    class ScriptEvents
    {
        public static void OnDisconnected(Room room) { }

        public static void OnConnecting(Room room) { }

        public static void OnConnected(Room room) { }

        public static bool OnRedirecting(Room room, Redirect redirect) { return true; }

        public static bool OnTextReceiving(Room room, String name, String text) { return true; }

        public static void OnTextReceived(Room room, String name, String text) { }

        public static bool OnEmoteReceiving(Room room, String name, String text) { return true; }

        public static void OnEmoteReceived(Room room, String name, String text) { }

        public static bool OnAnnounceReceiving(Room room, String text) { return true; }

        public static void OnAnnounceReceived(Room room, String text) { }

        public static void OnUserlistReceived(Room room) { }

        public static bool OnUserJoining(Room room, User user) { return true; }

        public static void OnUserJoined(Room room, User user) { }

        public static bool OnUserParting(Room room, User user) { return true; }

        public static void OnUserParted(Room room, User user) { }

        public static void OnUserLevelChanged(Room room, User user) { }

        public static bool OnUserAvatarReceiving(Room room, User user) { return true; }

        public static bool OnUserMessageReceiving(Room room, User user, String text) { return true; }

        public static bool OnTopicReceiving(Room room, String text) { return true; }

        public static bool OnUrlReceiving(Room room, String text, String address) { return true; }

        public static bool OnUserFontChanging(Room room, User user) { return true; }

        public static void OnUserWritingStatusChanged(Room room, User user) { }

        public static void OnUserOnlineStatusChanged(Room room, User user) { }

        public static bool OnPmReceiving(Room room, User user, String text) { return true; }

        public static void OnPmReceived(Room room, User user, String text) { }
    }
}
