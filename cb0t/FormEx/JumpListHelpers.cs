using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace FormEx.JumpListEx
{
    static class JumpListHelpers
    {
        public static String ApplicationPath { get; private set; }

        static JumpListHelpers()
        {
            ApplicationPath = Assembly.GetEntryAssembly().Location;
        }
    }
}
