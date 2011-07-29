using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace cb0t_chat_client_v2
{
    class DCSendFileObject
    {
        private ulong sent_count = 0;
        private FileStream f;

        public String filename = String.Empty;
        public String displayname = String.Empty;
        public bool Authorised = false;
        public ulong filesize = 0;
        public ushort referal = 0;

        public byte[] current_chunk = null;

        public DCSendFileObject(String filename)
        {
            this.filename = filename;
            this.displayname = Helpers.ExtractDCFilename(filename);
        }

        public void Dispose()
        {
            try
            {
                this.f.Dispose();
                this.f.Close();
            }
            catch { }
        }

        public void Prepare()
        {
            this.f = new FileStream(this.filename, FileMode.Open, FileAccess.Read);
            this.filesize = (ulong)this.f.Length;
        }

        public void NextChunk()
        {
            if ((this.filesize - this.sent_count) >= 1024)
            {
                this.current_chunk = new byte[1024];
                this.sent_count += (ulong)this.f.Read(this.current_chunk, 0, 1024);
            }
            else
            {
                this.current_chunk = new byte[this.filesize - this.sent_count];
                this.sent_count += (ulong)this.f.Read(this.current_chunk, 0, this.current_chunk.Length);
            }

            if (this.current_chunk != null)
                if (this.current_chunk.Length == 0)
                    this.current_chunk = null;
        }

    }
}
