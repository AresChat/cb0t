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
    public partial class UserListContainer : UserControl
    {
        public UserListContainer()
        {
            InitializeComponent();
        }

        public void Free()
        {
            while (this.Controls.Count > 0)
                this.Controls.RemoveAt(0);

            this.userListHeader1.Free();
            this.userListHeader1.Dispose();
            this.userListHeader1 = null;
            this.userListBox1.Free();
            this.userListBox1.Dispose();
            this.userListBox1 = null;
        }
    }
}
