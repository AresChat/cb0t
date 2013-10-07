using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace cb0t
{
    class FavouritesListItem
    {
        public String Name { get; set; }
        public String Topic { get; set; }
        public ushort Port { get; set; }
        public IPAddress IP { get; set; }
        public String Password { get; set; }
        public bool AutoJoin { get; set; }

        public FavouritesListItem()
        {
            this.Password = String.Empty;
            this.AutoJoin = false;
        }
    }
}
