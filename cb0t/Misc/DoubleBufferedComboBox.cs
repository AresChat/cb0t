using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace cb0t
{
    class DoubleBufferedComboBox : ComboBox
    {
        public DoubleBufferedComboBox()
        {
            this.DoubleBuffered = true;
        }
    }
}
