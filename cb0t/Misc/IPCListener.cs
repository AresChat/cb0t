using System;
using System.Collections.Generic;
using System.IO.Pipes;
using System.Linq;
using System.Text;

namespace cb0t
{
    class IPCListener
    {
        public event EventHandler<IPCListenerEventArgs> HashlinkReceived;
        public event EventHandler<IPCListenerEventArgs> CommandReceived;

        private NamedPipeServerStream listener;

        public void Start()
        {
            try
            {
                this.listener = new NamedPipeServerStream("cb0t_v3_pipe", PipeDirection.InOut, 10, PipeTransmissionMode.Byte, PipeOptions.Asynchronous);
                this.listener.BeginWaitForConnection(new AsyncCallback(this.ClientConnected), null);
            }
            catch { }
        }

        private void ClientConnected(IAsyncResult result)
        {
            this.listener.EndWaitForConnection(result);
            List<byte> list = new List<byte>();
            byte[] buf = new byte[1024];
            int size = 0;

            while ((size = listener.Read(buf, 0, 1024)) > 0)
                list.AddRange(buf.Take(size));

            String str = Encoding.UTF8.GetString(list.ToArray());

            if (!String.IsNullOrEmpty(str))
            {
                if (str.StartsWith("cb0t://"))
                {
                    str = str.Substring(7);

                    DecryptedHashlink dh = Hashlink.DecodeHashlink(str);

                    if (dh == null)
                        if (str.EndsWith("/"))
                            str = str.Substring(0, str.Length - 1);

                    dh = Hashlink.DecodeHashlink(str);

                    if (dh != null)
                        this.HashlinkReceived(null, new IPCListenerEventArgs { Hashlink = dh });
                }
                else if (str.StartsWith("cbjl_"))
                {
                    str = str.Substring(5);
                    this.CommandReceived(null, new IPCListenerEventArgs { Command = str });
                }
            }

            this.listener.Disconnect();
            this.listener.BeginWaitForConnection(new AsyncCallback(this.ClientConnected), null);
        }
    }

    class IPCListenerEventArgs : EventArgs
    {
        public DecryptedHashlink Hashlink { get; set; }
        public String Command { get; set; }
    }
}
