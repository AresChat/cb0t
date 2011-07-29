using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace cb0t_chat_client_v2
{
    class DCReceiveFileObject
    {
        public ulong filesize = 0;
        public ulong received = 0;
        public ushort referal = 0;
        public String filename = String.Empty;
        public String displayname = String.Empty;

        private FileStream f;

        public DCReceiveFileObject(String filename, ushort referal, ulong filesize)
        {
            this.filename = filename;
            this.displayname = Helpers.ExtractDCFilename(this.filename);
            this.referal = referal;
            this.filesize = filesize;
        }

        public void Prepare()
        {
            String path = AppDomain.CurrentDomain.BaseDirectory + "/received files";

            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            if (File.Exists(path + "/CBOT_TRANSFER_" + this.referal + "_" + this.displayname))
                File.Delete(path + "/CBOT_TRANSFER_" + this.referal + "_" + this.displayname);

            this.f = new FileStream(path + "/CBOT_TRANSFER_" + this.referal + "_" + this.displayname,
                FileMode.Create, FileAccess.Write);
        }

        public void Dispose()
        {
            f.Dispose();
            f.Close();

            String path = AppDomain.CurrentDomain.BaseDirectory + "/received files";
            File.Copy(path + "/CBOT_TRANSFER_" + this.referal + "_" + this.displayname, path + "/" + this.displayname, true);
            File.Delete(path + "/CBOT_TRANSFER_" + this.referal + "_" + this.displayname);
        }

        public void Dispose(bool terminate)
        {
            try
            {
                f.Dispose();
                f.Close();
                String path = AppDomain.CurrentDomain.BaseDirectory + "/received files";
                File.Delete(path + "/CBOT_TRANSFER_" + this.referal + "_" + this.displayname);
            }
            catch { }
        }

        public void AddChunk(byte[] data)
        {
            this.f.Write(data, 0, data.Length);
            this.received += (ulong)data.Length;
        }

        public bool UploadComplete()
        {
            return this.filesize == this.received;
        }

    }
}
