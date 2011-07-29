using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.IO;
using System.Net;

namespace cb0t_chat_client_v2
{
    class PlsParser
    {
        internal event PlsParserEventHandler PlsParsed;

        private Thread thread;
        private bool busy = false;
        private String plsurl = String.Empty;

        public void Parse(String url)
        {
            this.plsurl = url;

            if (this.busy)
                return;

            this.thread = new Thread(new ParameterizedThreadStart(this.Worker));
            this.thread.Start(url);
        }

        private void Worker(object args)
        {
            this.busy = true;

            String result = null;
            String str = (String)args;

            try
            {
                WebRequest request = WebRequest.Create(str);
                WebResponse response = request.GetResponse();

                using (Stream stream = response.GetResponseStream())
                {
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        while (!reader.EndOfStream)
                        {
                            String line = reader.ReadLine();

                            if (line.ToUpper().StartsWith("FILE1="))
                            {
                                result = line.Substring(6).Trim();

                                if (result.Length == 0)
                                    result = null;

                                break;
                            }
                        }
                    }
                }
            }
            catch { }

            this.busy = false;

            if (str != this.plsurl)
            {
                str = this.plsurl;
                this.Worker(str);
            }
            else if (this.PlsParsed != null)
                this.PlsParsed(this, result == null ? new PlsParserEventArgs() : new PlsParserEventArgs(result));
        }
    }

    class PlsParserEventArgs : EventArgs
    {
        public bool success;
        public String url;

        public PlsParserEventArgs(String url)
        {
            this.success = true;
            this.url = url;
        }

        public PlsParserEventArgs()
        {
            this.success = false;
            this.url = null;
        }
    }

    internal delegate void PlsParserEventHandler(object sender, PlsParserEventArgs e);
}
