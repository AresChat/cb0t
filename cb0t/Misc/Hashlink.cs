using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace cb0t
{
    class Hashlink
    {
        private static byte[] d67(byte[] data, int b)
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

        public static String EncodeHashlink(ChannelListItem room)
        {
            List<byte> list = new List<byte>();
            list.AddRange(new byte[20]);
            list.AddRange(Encoding.UTF8.GetBytes("CHATCHANNEL"));
            list.Add(0);
            list.AddRange(room.IP.GetAddressBytes());
            list.AddRange(BitConverter.GetBytes(room.Port));
            list.AddRange(room.IP.GetAddressBytes());
            list.AddRange(Encoding.UTF8.GetBytes(room.Name));
            list.Add(0);
            list.Add(0);

            byte[] buf = list.ToArray();
            buf = Zip.Compress(buf);
            buf = e67(buf, 28435);

            return Convert.ToBase64String(buf);
        }

        public static String EncodeHashlink(FavouritesListItem room)
        {
            List<byte> list = new List<byte>();
            list.AddRange(new byte[20]);
            list.AddRange(Encoding.UTF8.GetBytes("CHATCHANNEL"));
            list.Add(0);
            list.AddRange(room.IP.GetAddressBytes());
            list.AddRange(BitConverter.GetBytes(room.Port));
            list.AddRange(room.IP.GetAddressBytes());
            list.AddRange(Encoding.UTF8.GetBytes(room.Name));
            list.Add(0);
            list.Add(0);

            byte[] buf = list.ToArray();
            buf = Zip.Compress(buf);
            buf = e67(buf, 28435);

            return Convert.ToBase64String(buf);
        }

        public static String EncodeHashlink(Redirect room)
        {
            List<byte> list = new List<byte>();
            list.AddRange(new byte[20]);
            list.AddRange(Encoding.UTF8.GetBytes("CHATCHANNEL"));
            list.Add(0);
            list.AddRange(room.IP.GetAddressBytes());
            list.AddRange(BitConverter.GetBytes(room.Port));
            list.AddRange(room.IP.GetAddressBytes());
            list.AddRange(Encoding.UTF8.GetBytes(room.Name));
            list.Add(0);
            list.Add(0);

            byte[] buf = list.ToArray();
            buf = Zip.Compress(buf);
            buf = e67(buf, 28435);

            return Convert.ToBase64String(buf);
        }

        public static DecryptedHashlink DecodeHashlink(String hashlink)
        {
            DecryptedHashlink room = new DecryptedHashlink();

            try
            {
                if (hashlink.ToUpper().StartsWith("CHATROOM:")) // not encrypted
                {
                    hashlink = hashlink.Substring(9);
                    int split = hashlink.IndexOf(":");
                    room.IP = IPAddress.Parse(hashlink.Substring(0, split));
                    hashlink = hashlink.Substring(split + 1);
                    split = hashlink.IndexOf("|");
                    room.Port = ushort.Parse(hashlink.Substring(0, split));
                    room.Name = hashlink.Substring(split + 1);
                    return room;
                }
                else // encrypted
                {
                    byte[] buf = Convert.FromBase64String(hashlink);
                    buf = d67(buf, 28435);
                    buf = Zip.Decompress(buf);

                    HashlinkReader reader = new HashlinkReader(buf);
                    reader.SkipBytes(32);
                    room.IP = reader;
                    room.Port = reader;
                    reader.SkipBytes(4);
                    room.Name = reader;

                    return room;
                }
            }
            catch // badly formed hashlink
            {
                return null;
            }
        }
    }

    class DecryptedHashlink
    {
        public String Name { get; set; }
        public IPAddress IP { get; set; }
        public ushort Port { get; set; }
    }
}
