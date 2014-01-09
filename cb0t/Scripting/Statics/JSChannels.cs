using Jurassic;
using Jurassic.Library;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace cb0t.Scripting.Statics
{
    [JSEmbed(Name = "Channels")]
    class JSChannels : ObjectInstance
    {
        public JSChannels(ScriptEngine engine)
            : base(engine)
        {
            this.PopulateFunctions();
        }

        protected override string InternalClassName
        {
            get { return "Channels"; }
        }

        [JSFunction(Name = "search", Flags = JSFunctionFlags.HasEngineParameter, IsWritable = false, IsEnumerable = true)]
        public static Objects.JSChannelCollection Search(ScriptEngine eng, Object a)
        {
            List<Objects.JSChannel> results = new List<Objects.JSChannel>();

            if (!(a is Undefined))
            {
                String str = a.ToString().ToUpper();
                List<ChannelListItem> matches = new List<ChannelListItem>();

                ChannelListPanel.full_channel_list.ForEach(x =>
                {
                    if (x.Name.ToUpper().Contains(str))
                        if (x.Users < 200)
                            matches.Add(x);
                });

                matches.Sort((y, x) => x.Users.CompareTo(y.Users));

                if (matches.Count > 10)
                    matches = matches.GetRange(0, 10);

                foreach (ChannelListItem m in matches)
                    results.Add(new Objects.JSChannel(eng.Object.InstancePrototype, m));
            }

            return new Objects.JSChannelCollection(eng.Object.InstancePrototype, results.ToArray(), eng.ScriptName);
        }
    }
}
