using System;
using System.Collections.Generic;
using System.Text;

namespace cb0t_chat_client_v2
{
    class AresCryptography
    {
        public static byte[] d67(byte[] data, int b)
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

        public static byte[] e67(byte[] data, int b)
        {
            byte[] buffer = new byte[data.Length];

            for (int i = 0; i < data.Length; i++)
            {
                buffer[i] = (byte)((data[i] ^ (b >> 8)) & 255);
                b = ((buffer[i] + b) * 23219 + 36126) & 65535;
            }

            return buffer;
        }
    }
}
