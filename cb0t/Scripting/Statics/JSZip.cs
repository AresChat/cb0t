using Jurassic;
using Jurassic.Library;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace cb0t.Scripting.Statics
{
    [JSEmbed(Name = "Zip")]
    class JSZip : ObjectInstance
    {
        public JSZip(ScriptEngine engine)
            : base(engine)
        {
            this.PopulateFunctions();
        }

        protected override string InternalClassName
        {
            get { return "Zip"; }
        }

        [JSFunction(Name = "compress", IsWritable = false, IsEnumerable = true)]
        public static String Compress(object a)
        {
            String result = null;

            if (!(a is Undefined))
            {
                try
                {
                    String str = a.ToString();
                    byte[] buf = Encoding.UTF8.GetBytes(str);
                    buf = Zip.Compress(buf);
                    result = Encoding.Default.GetString(buf);
                }
                catch { }
            }

            return result;
        }

        [JSFunction(Name = "uncompress", IsWritable = false, IsEnumerable = true)]
        public static String Uncompress(object a)
        {
            String result = null;

            if (!(a is Undefined))
            {
                try
                {
                    String str = a.ToString();
                    byte[] buf = Encoding.Default.GetBytes(str);
                    buf = Zip.Decompress(buf);
                    result = Encoding.UTF8.GetString(buf);
                }
                catch { }
            }

            return result;
        }
    }
}
