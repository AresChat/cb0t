using Jurassic;
using Jurassic.Library;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace cb0t.Scripting.Statics
{
    [JSEmbed(Name = "Hashlink")]
    class JSHashlink : ObjectInstance
    {
        public JSHashlink(ScriptEngine engine)
            : base(engine)
        {
            this.PopulateFunctions();
        }

        protected override string InternalClassName
        {
            get { return "Hashlink"; }
        }

        [JSFunction(Name = "encode", IsWritable = false, IsEnumerable = true)]
        public static String Encode(object a)
        {
            if (a is ObjectInstance)
            {
                try
                {
                    ObjectInstance obj = (ObjectInstance)a;

                    String str = Hashlink.EncodeHashlink(new ChannelListItem
                    {
                        Name = obj.GetPropertyValue("name").ToString(),
                        Topic = obj.GetPropertyValue("name").ToString(),
                        IP = IPAddress.Parse(obj.GetPropertyValue("ip").ToString()),
                        Port = ushort.Parse(obj.GetPropertyValue("port").ToString())
                    });

                    if (str != null)
                        return "\arlnk://" + str;
                }
                catch { }
            }

            return null;
        }

        [JSFunction(Name = "decode", Flags = JSFunctionFlags.HasEngineParameter, IsWritable = false, IsEnumerable = true)]
        public static Objects.JSHashlinkResult Decode(ScriptEngine eng, object a)
        {
            if (a is String || a is ConcatenatedString)
            {
                String str = a.ToString();

                try
                {
                    DecryptedHashlink hashlink = Hashlink.DecodeHashlink(a.ToString());

                    if (hashlink != null)
                        return new Objects.JSHashlinkResult(eng.Object.InstancePrototype, hashlink);
                }
                catch { }
            }

            return null;
        }
    }
}
