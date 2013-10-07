using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace cb0t
{
    class RoomPool
    {
        static RoomPool()
        {
            Rooms = new List<Room>();
        }

        public static List<Room> Rooms { get; set; }
    }
}
