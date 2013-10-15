using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace cb0t
{
    class ScriptEvents
    {
        public static void OnDisconnected(Room room)
        {

        }

        public static void OnConnecting(Room room)
        {

        }

        public static void OnConnected(Room room)
        {

        }

        public static bool OnRedirecting(Room room, Redirect redirect)
        {
            return true;
        }

        public static bool OnTextReceiving(Room room, String name, String text)
        {
            return true;
        }

        public static void OnTextReceived(Room room, String name, String text)
        {

        }

        public static bool OnEmoteReceiving(Room room, String name, String text)
        {
            return true;
        }

        public static void OnEmoteReceived(Room room, String name, String text)
        {

        }

        public static void OnUserlistReceived(Room room)
        {

        }

        public static bool OnUserJoining(Room room, User user)
        {
            return true;
        }

        public static void OnUserJoined(Room room, User user)
        {

        }

        public static bool OnUserParting(Room room, User user)
        {
            return true;
        }

        public static void OnUserParted(Room room, User user)
        {

        }

        public static void OnUserLevelChanged(Room room, User user)
        {

        }

        
    }
}
