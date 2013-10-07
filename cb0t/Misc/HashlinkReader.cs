using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace cb0t
{
    class HashlinkReader
    {
        private int Position = 0;
        private List<byte> Data = new List<byte>();

        public HashlinkReader(byte[] bytes)
        {
            this.Data.Clear();
            this.Position = 0;
            this.Data.AddRange(bytes);
        }

        public void SkipBytes(int count)
        {
            this.Position += count;
        }

        public static implicit operator ushort(HashlinkReader p)
        {
            ushort tmp = BitConverter.ToUInt16(p.Data.ToArray(), p.Position);
            p.Position += 2;
            return tmp;
        }

        public static implicit operator IPAddress(HashlinkReader p)
        {
            byte[] tmp = new byte[4];
            Array.Copy(p.Data.ToArray(), p.Position, tmp, 0, tmp.Length);
            p.Position += 4;
            return new IPAddress(tmp);
        }

        public static implicit operator String(HashlinkReader p)
        {
            String str = String.Empty;
            int split = p.Data.IndexOf(0, p.Position);
            byte[] tmp = new byte[split > -1 ? (split - p.Position) : (p.Data.Count - p.Position)];
            Array.Copy(p.Data.ToArray(), p.Position, tmp, 0, tmp.Length);
            p.Position = split > -1 ? (split + 1) : p.Data.Count;
            return Encoding.UTF8.GetString(tmp).Replace("\r\n", "\n");
        }
    }
}
