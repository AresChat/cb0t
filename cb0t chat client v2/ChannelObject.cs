using System;
using System.Collections.Generic;
using System.Text;
using System.Net;

namespace cb0t_chat_client_v2
{
    class ChannelObject// : IComparable
    {
        public String name;
        public String topic;
        public IPAddress ip = IPAddress.Any;
        public ushort port;
        public bool auto_start = false;
        public String password = String.Empty;
        public uint cookie;
        public String server = String.Empty;
        public ushort users = 0;
        public String language = String.Empty;
        public String dyndns = String.Empty;

        public override bool Equals(object obj)
        {
            ChannelObject _obj = (ChannelObject)obj;

            if (this.name == _obj.name)
                return true;

            if (this.ip.Equals(_obj.ip))
                if (this.port == _obj.port)
                    return true;

            if (this.dyndns != String.Empty)
                if (this.dyndns == _obj.dyndns)
                    if (this.port == _obj.port)
                        return true;

            return false;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

     /*   public int CompareTo(object obj)
        {
            int result = 0;

            switch (ChannelList.SortOrdering)
            {
                case ChannelListSortOrder.Name:
                    result = String.Compare(this.name, ((ChannelObject)obj).name);
                    break;

                case ChannelListSortOrder.Server:
                    result = String.Compare(this.server, ((ChannelObject)obj).server);
                    break;

                case ChannelListSortOrder.Users:
                    result = ((ChannelObject)obj).users - this.users;
                    break;

                case ChannelListSortOrder.Language:
                    result = String.Compare(this.language, ((ChannelObject)obj).language);
                    break;

                case ChannelListSortOrder.Topic:
                    result = String.Compare(this.topic, ((ChannelObject)obj).topic);
                    break;
            }
            
            return result;
        } */
    }
}
