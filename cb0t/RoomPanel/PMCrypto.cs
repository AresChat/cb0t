using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace cb0t
{
    class PMCrypto
    {
        public static byte[] SoftEncrypt(String target, byte[] data)
        {
            byte[] result;

            using (DESCryptoServiceProvider des = new DESCryptoServiceProvider())
            {
                byte[] key = des.Key;

                using (SHA1 sha = SHA1.Create())
                    des.IV = sha.ComputeHash(Encoding.UTF8.GetBytes(target)).Take(8).ToArray();

                using (MemoryStream ms = new MemoryStream())
                using (ICryptoTransform enc = des.CreateEncryptor())
                using (CryptoStream cs = new CryptoStream(ms, enc, CryptoStreamMode.Write))
                {
                    cs.Write(data, 0, data.Length);
                    cs.FlushFinalBlock();
                    result = ms.ToArray();
                }

                result = key.Concat(result).ToArray();
            }

            return result;
        }

        public static byte[] SoftDecrypt(String name, byte[] data)
        {
            byte[] result;

            using (DESCryptoServiceProvider des = new DESCryptoServiceProvider())
            {
                des.Key = data.Take(8).ToArray();

                using (SHA1 sha = SHA1.Create())
                    des.IV = sha.ComputeHash(Encoding.UTF8.GetBytes(name)).Take(8).ToArray();

                using (MemoryStream ms = new MemoryStream())
                using (ICryptoTransform enc = des.CreateDecryptor())
                using (CryptoStream cs = new CryptoStream(ms, enc, CryptoStreamMode.Write))
                {
                    cs.Write(data, 8, data.Length - 8);
                    cs.FlushFinalBlock();
                    result = ms.ToArray();
                }
            }

            return result;
        }
    }
}
