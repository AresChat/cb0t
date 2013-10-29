using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace cb0t
{
    public class CryptoService
    {
        public CryptoMode Mode { get; set; }

        private byte[] Key { get; set; }
        private byte[] IV { get; set; }

        public CryptoService()
        {
            this.Mode = CryptoMode.Unencrypted;
        }

        public byte[] Decrypt(byte[] data)
        {
            byte[] result;

            using (MemoryStream ms = new MemoryStream(data))
            using (AesCryptoServiceProvider aes = new AesCryptoServiceProvider())
            using (ICryptoTransform enc = aes.CreateDecryptor(this.Key, this.IV))
            using (CryptoStream cs = new CryptoStream(ms, enc, CryptoStreamMode.Read))
            {
                result = new byte[data.Length];
                int size = cs.Read(result, 0, result.Length);
                result = result.Take(size).ToArray();
            }

            return result;
        }

        public byte[] Encrypt(byte[] data)
        {
            byte[] result;

            using (MemoryStream ms = new MemoryStream())
            using (AesCryptoServiceProvider aes = new AesCryptoServiceProvider())
            using (ICryptoTransform enc = aes.CreateEncryptor(this.Key, this.IV))
            using (CryptoStream cs = new CryptoStream(ms, enc, CryptoStreamMode.Write))
            {
                cs.Write(data, 0, data.Length);
                cs.FlushFinalBlock();
                result = ms.ToArray();
            }

            return result;
        }

        public void SetCrypto(TCPPacketReader packet)
        {
            byte[] guid = Settings.Guid.ToByteArray();
            byte[] key = packet;

            using (MD5 md5 = MD5.Create())
                guid = md5.ComputeHash(guid);

            for (int i = (guid.Length - 2); i > -1; i -= 2)
                key = this.d67(key, BitConverter.ToUInt16(guid, i));

            List<byte> list = new List<byte>(key);
            this.Mode = CryptoMode.Encrypted;
            this.IV = list.GetRange(0, 16).ToArray();
            this.Key = list.GetRange(16, 32).ToArray();
        }

        private byte[] d67(byte[] data, int b)
        {
            byte[] buffer = new byte[data.Length];
            Array.Copy(data, buffer, data.Length);

            for (int i = 0; i < data.Length; i++)
            {
                buffer[i] = (byte)(data[i] ^ b >> 8 & 255);
                b = (b + data[i]) * 23219 + 36126 & 65535;
            }
            return buffer;
        }
    }

    public enum CryptoMode
    {
        Encrypted,
        Unencrypted
    }
}
