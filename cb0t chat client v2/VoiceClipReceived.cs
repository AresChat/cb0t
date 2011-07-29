using System;
using System.Collections.Generic;
using System.Text;
using Ares.PacketHandlers;
using ZLib;

namespace cb0t_chat_client_v2
{
    class VoiceClipReceived
    {
        public uint ident;
        public byte length;
        private byte compression_count;
        private uint uncompressed_size;
        public uint compressed_size;

        public String hash;

        public bool pm;
        public String from;

        private List<byte> bytes_in = new List<byte>();
        private List<uint> compressions = new List<uint>();

        private byte[] wav;

        public VoiceClipReceived(AresDataPacket first, bool pm)
        {
            this.pm = pm;
            this.from = first.ReadString();
            this.ident = first.ReadInt32();
            this.length = first.ReadByte();
            this.compression_count = first.ReadByte();
            this.uncompressed_size = first.ReadInt32();

            for (int i = 0; i < this.compression_count; i++)
                this.compressions.Add(first.ReadInt32());

            this.compressed_size = this.compressions.Count == 0 ? this.uncompressed_size : this.compressions[this.compressions.Count - 1];
            this.bytes_in.AddRange(first.ReadBytes());

            int z = 0;

            foreach (byte b in Encoding.UTF8.GetBytes(this.from))
                z += b;

            this.hash = Convert.ToBase64String(AresCryptography.e67(BitConverter.GetBytes(this.ident), z));
        }

        public void AddChunk(byte[] data)
        {
            this.bytes_in.AddRange(data);
        }

        public bool Received
        {
            get { return this.bytes_in.Count == this.compressed_size; }
        }

        public byte[] VoiceClip
        {
            get { return this.wav; }
        }

        public void Unpack()
        {
            this.wav = this.bytes_in.ToArray();

            if (this.compression_count > 0)
            {
                this.compressions.RemoveAt(this.compressions.Count - 1);

                while (this.compressions.Count > 0)
                {
                    this.wav = Zlib.Decompress(this.wav, this.compressions[this.compressions.Count - 1]);
                    this.compressions.RemoveAt(this.compressions.Count - 1);
                }

                this.wav = Zlib.Decompress(this.wav, this.uncompressed_size);
            }
        }

    }
}
