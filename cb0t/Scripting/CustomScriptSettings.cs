using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace cb0t
{
    public partial class CustomScriptSettings : UserControl
    {
        public CustomScriptSettings(String script_name)
        {
            this.InitializeComponent();
            this.ScriptName = script_name;
            this.label1.Text = script_name;
        }

        public String ScriptName { get; set; }
    }
}
