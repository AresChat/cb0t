using Jurassic;
using Jurassic.Library;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace cb0t.Scripting.Objects
{
    class JSChannel : ObjectInstance
    {
        public ChannelListItem Item { get; set; }

        internal JSChannel(ScriptEngine eng)
            : base(eng)
        {
            this.PopulateFunctions();
        }

        public JSChannel(ObjectInstance prototype, ChannelListItem item)
            : base(prototype.Engine, ((ClrFunction)prototype.Engine.Global["Channel"]).InstancePrototype)
        {
            this.Item = item;
            this.PopulateFunctions();
        }

        protected override string InternalClassName
        {
            get { return "Channel"; }
        }

        [JSProperty(Name = "hashlink")]
        public String Hashlink
        {
            get
            {
                return "arlnk://" + cb0t.Hashlink.EncodeHashlink(this.Item);
            }
            set { }
        }

        [JSProperty(Name = "name")]
        public String Name
        {
            get { return this.Item.Name; }
            set { }
        }

        [JSProperty(Name = "topic")]
        public String Topic
        {
            get { return this.Item.Topic; }
            set { }
        }

        [JSProperty(Name = "userCount")]
        public int Users
        {
            get { return this.Item.Users; }
            set { }
        }

        [JSProperty(Name = "port")]
        public int Port
        {
            get { return this.Item.Port; }
            set { }
        }

        [JSProperty(Name = "externalIp")]
        public String IP
        {
            get { return this.Item.IP.ToString(); }
            set { }
        }
    }
}
