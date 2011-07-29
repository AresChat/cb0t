using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace cb0t_chat_client_v2
{
    class DCListViewItem : ListViewItem
    {
        public ushort referal;
        public String state = "connecting";
        public bool receiving;
        public bool connected;
        public ulong bytes_full;
        public ulong bytes_so_far;
        public String filename;
        public String path;
    }
}
