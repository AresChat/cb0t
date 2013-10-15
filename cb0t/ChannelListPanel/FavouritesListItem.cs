using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace cb0t
{
    public class FavouritesListItem
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

        public FavouritesListItem Copy()
        {
            FavouritesListItem c = new FavouritesListItem();
            c.Name = this.Name;
            c.Topic = this.Topic;
            c.Port = this.Port;
            c.IP = this.IP;
            c.Password = this.Password;
            c.AutoJoin = this.AutoJoin;
            return c;
        }
    }
}
