using System;
using System.Collections.Generic;
using System.Text;

namespace cb0t_chat_client_v2
{
    class ReceivedBrowseItem
    {
        public byte Mime = 0;
        public ulong FileSize = 0;
        public ushort param1 = 0;
        public ushort param2 = 0;
        public ushort param3 = 0;
        public String Title = String.Empty;
        public String Artist = String.Empty;
        public String Album = String.Empty;
        public String Category = String.Empty;
        public String Year = String.Empty;
        public String Language = String.Empty;
        public String URL = String.Empty;
        public String Comment = String.Empty;
        public String Genre = String.Empty;
        public String Format = String.Empty;
        public String FileName = String.Empty;
        public String Path = String.Empty;
        public String FileSizeString = String.Empty;
        public byte[] SHA1Hash;
    }
}
