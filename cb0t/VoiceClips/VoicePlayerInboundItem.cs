using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

namespace cb0t
{
    class VoicePlayerInboundItem
    {
        public IPEndPoint EndPoint { get; private set; }
        public String Sender { get; private set; }
        public uint Ident { get; private set; }
        public String FileName { get; private set; }

        private byte compression_count = 0;
        private uint uncompressed_length = 0;
        private uint compressed_length = 0;
        private bool is_opus = false;
        private List<byte> data_in = new List<byte>();

        public VoicePlayerInboundItem(TCPPacketReader packet, CryptoService c, IPEndPoint ep)
        {
            this.EndPoint = ep;
            this.Sender = packet.ReadString(c);
            this.Ident = packet;
            packet.SkipByte();
            this.compression_count = packet;
            this.uncompressed_length = packet;

            for (int i = 0; i < this.compression_count; i++)
                this.compressed_length = packet;

            if (this.compressed_length == 0)
                this.compressed_length = this.uncompressed_length;

            this.is_opus = (this.compressed_length == this.uncompressed_length);
            this.data_in.AddRange(((byte[])packet));
        }

        public bool Received
        {
            get { return this.data_in.Count == this.compressed_length; }
        }

        public void AddChunk(byte[] chunk)
        {
            this.data_in.AddRange(chunk);
        }

        public void Save()
        {
            byte[] buf = this.data_in.ToArray();
            this.data_in.Clear();
            this.data_in = new List<byte>();

            if (this.is_opus)
                buf = Opus.Decode(buf);
            else
            {
                for (int i = 0; i < this.compression_count; i++)
                    buf = Zip.Decompress(buf);
            }

            if (buf.Length > 0)
            {
                StringBuilder sb = new StringBuilder();
                sb.Append(Helpers.EndPointToHexString(this.EndPoint));
                sb.Append("_");
                sb.Append(Helpers.UnixTimeMS);
                sb.Append("_");
                sb.Append(this.Ident);

                try
                {
                    File.WriteAllBytes(Path.Combine(Settings.VoicePath, sb.ToString() + ".wav"), buf);
                    this.FileName = sb.ToString();
                }
                catch { }

                sb = null;
            }
            
            buf = null;
        }

        public VoicePlayerItem ToVoicePlayerItem(uint sc, bool b)
        {
            VoicePlayerItem item = new VoicePlayerItem(sc, b);
            item.EndPoint = this.EndPoint;
            item.FileName = this.FileName;
            item.Ident = this.Ident;
            item.Sender = this.Sender;
            return item;
        }
    }
}
