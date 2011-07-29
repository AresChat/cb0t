using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Runtime.InteropServices;
using ICSharpCode.SharpZipLib.Zip.Compression.Streams;

namespace ZLib
{
    public class Zlib
    {
        /*
      //  [DllImport("zlib1.dll", CallingConvention = CallingConvention.Cdecl)]
        [DllImport("zlib1.dll")]
        static extern int compress(byte[] destBuffer, ref uint destLen, byte[] sourceBuffer, uint sourceLen);

      //  [DllImport("zlib1.dll", CallingConvention = CallingConvention.Cdecl)]
        [DllImport("zlib1.dll")]
        static extern int uncompress(byte[] destBuffer, ref uint destLen, byte[] sourceBuffer, uint sourceLen);


        public static byte[] Compress(byte[] data)
        {
            uint _dLen = (uint)((data.Length * 1.1) + 12);
            byte[] _d = new byte[_dLen];
            compress(_d, ref _dLen, data, (uint)data.Length);
            byte[] result = new byte[_dLen];
            Array.Copy(_d, 0, result, 0, result.Length);
            return result;
        }

        public static byte[] Decompress(byte[] data, bool large)
        {
            return Decompress(data, (uint)(large ? 524288 : 8192));
        }

        public static byte[] Decompress(byte[] data, uint size)
        {
            uint _dLen = size;
            byte[] _d = new byte[_dLen];

            if (uncompress(_d, ref _dLen, data, (uint)data.Length) != 0)
                return null;

            byte[] result = new byte[_dLen];
            Array.Copy(_d, 0, result, 0, result.Length);
            return result;
        }
         */

        public static byte[] Compress(byte[] data)
        {
            byte[] result = null;

            using (MemoryStream ms = new MemoryStream())
            {
                using (Stream s = new DeflaterOutputStream(ms))
                {
                    s.Write(data, 0, data.Length);
                    s.Close();
                    result = ms.ToArray();
                }
            }

            return result;
        }

        public static byte[] Decompress(byte[] data, bool large)
        {
            byte[] r = null;

            using (Stream s = new InflaterInputStream(new MemoryStream(data)))
            {
                byte[] b = new byte[large ? 524288 : 8192];
                int count = s.Read(b, 0, b.Length);
                r = new byte[count];
                Array.Copy(b, 0, r, 0, r.Length);
            }

            return r;
        }

        public static byte[] Decompress(byte[] data, uint size)
        {
            byte[] r = null;

            using (Stream s = new InflaterInputStream(new MemoryStream(data)))
            {
                byte[] b = new byte[size];
                int count = s.Read(b, 0, b.Length);
                r = new byte[count];
                Array.Copy(b, 0, r, 0, r.Length);
            }

            return r;
        }
    }
}
