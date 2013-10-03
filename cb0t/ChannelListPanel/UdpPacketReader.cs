using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace cb0t
{
    class UdpPacketReader
    {
        private int Position = 0;
        private List<byte> Data = new List<byte>();

        public UdpPacketReader(byte[] bytes)
        {
            this.Data.Clear();
            this.Position = 0;
            this.Data.AddRange(bytes);
        }

        public int Remaining()
        {
            return this.Data.Count - this.Position;
        }

        public byte ReadByte()
        {
            byte tmp = this.Data[this.Position];
            this.Position++;
            return tmp;
        }

        public byte[] ReadBytes(int count)
        {
            byte[] tmp = new byte[count];
            Array.Copy(this.Data.ToArray(), this.Position, tmp, 0, tmp.Length);
            this.Position += count;
            return tmp;
        }

        public byte[] ReadBytes()
        {
            byte[] tmp = new byte[this.Data.Count - this.Position];
            Array.Copy(this.Data.ToArray(), this.Position, tmp, 0, tmp.Length);
            this.Position += tmp.Length;
            return tmp;
        }

        public ushort ReadUInt16()
        {
            ushort tmp = BitConverter.ToUInt16(this.Data.ToArray(), this.Position);
            this.Position += 2;
            return tmp;
        }

        public uint ReadUInt32()
        {
            uint tmp = BitConverter.ToUInt32(this.Data.ToArray(), this.Position);
            this.Position += 4;
            return tmp;
        }

        public ulong ReadUInt64()
        {
            ulong tmp = BitConverter.ToUInt64(this.Data.ToArray(), this.Position);
            this.Position += 8;
            return tmp;
        }

        public IPAddress ReadIP()
        {
            byte[] tmp = new byte[4];
            Array.Copy(this.Data.ToArray(), this.Position, tmp, 0, tmp.Length);
            this.Position += 4;
            return new IPAddress(tmp);
        }

        public String ReadString()
        {
            ushort len = BitConverter.ToUInt16(this.Data.ToArray(), this.Position);
            this.Position += 2;
            String str = Encoding.UTF8.GetString(this.Data.ToArray(), this.Position, len);
            this.Position += len;
            return str;
        }
    }
}
