using System;
using System.Collections.Generic;
using System.Text;
using System.Net;

namespace Ares.PacketHandlers
{
    /// <summary>
    /// Helpful Ares Packet Processor Object
    /// </summary>
    public class AresDataPacket
    {
        private int Position = 0;
        private List<byte> Data = new List<byte>();

        /// <summary>
        /// Creates an empty Ares Data Packet Object ready for Writing
        /// </summary>
        public AresDataPacket()
        {
            this.Data.Clear();
            this.Position = 0;
        }

        /// <summary>
        /// Creates a populated Ares Data Packet Object ready for Reading
        /// </summary>
        /// <param name="bytes">The Byte Array of the Received Ares Packet</param>
        public AresDataPacket(byte[] bytes)
        {
            this.Data.Clear();
            this.Position = 0;
            this.Data.AddRange(bytes);
        }

        /// <summary>
        /// Returns size of byte array
        /// </summary>
        /// <returns></returns>
        public int GetByteCount()
        {
            return this.Data.Count;
        }

        /// <summary>
        /// Examine current byte without incrementing the reader index
        /// </summary>
        /// <returns></returns>
        public byte PeekByte()
        {
            return this.Data[this.Position];
        }

        /// <summary>
        /// Return the reader index of the first instance of a byte
        /// </summary>
        /// <param name="b"></param>
        /// <returns></returns>
        public int IndexOf(byte b)
        {
            return this.Data.IndexOf(b, this.Position);
        }

        /// <summary>
        /// Return number of bytes unread in the packet reader
        /// </summary>
        /// <returns></returns>
        public int Remaining()
        {
            return this.Data.Count - this.Position;
        }

        /// <summary>
        /// Manually set the position of the Packet Reader
        /// </summary>
        /// <param name="position"></param>
        public void SetPosition(int position)
        {
            this.Position = position;
        }

        /// <summary>
        /// Force the Packet Reader to move forward one byte
        /// </summary>
        public void SkipByte()
        {
            this.Position++;
        }

        /// <summary>
        /// Force the Packet Reader to move forward a specified number of bytes
        /// </summary>
        /// <param name="count"></param>
        public void SkipBytes(int count)
        {
            this.Position += count;
        }

        /// <summary>
        /// Positions the Packet Reader ahead of the first 3 bytes which are reserved for Ares Protocol ID and Length
        /// </summary>
        public void PositionReaderAfterHeader()
        {
            this.Position = 3;
        }

        /// <summary>
        /// Packet Reader returns the next byte
        /// </summary>
        /// <returns></returns>
        public byte ReadByte()
        {
            byte tmp = this.Data[this.Position];
            this.Position++;
            return tmp;
        }

        /// <summary>
        /// Packet Reader returns the final byte
        /// </summary>
        /// <returns></returns>
        public byte LastByte()
        {
            return this.Data[this.Data.Count - 1];
        }

        /// <summary>
        /// Packet Reader returns the next specified number of bytes
        /// </summary>
        /// <param name="count"></param>
        /// <returns></returns>
        public byte[] ReadBytes(int count)
        {
            if ((this.Data.Count - this.Position) < count) return null;
            byte[] tmp = new byte[count];
            Array.Copy(this.Data.ToArray(), this.Position, tmp, 0, tmp.Length);
            this.Position += count;
            return tmp;
        }

        /// <summary>
        /// Packet Reader returns all remaining bytes
        /// </summary>
        /// <returns></returns>
        public byte[] ReadBytes()
        {
            byte[] tmp = new byte[this.Data.Count - this.Position];
            Array.Copy(this.Data.ToArray(), this.Position, tmp, 0, tmp.Length);
            this.Position += tmp.Length;
            return tmp;
        }

        /// <summary>
        /// Packet Reader returns the next 16 byte GUID
        /// </summary>
        /// <returns></returns>
        public Guid ReadGuid()
        {
            if ((this.Data.Count - this.Position) < 16) return new Guid();
            byte[] tmp = new byte[16];
            Array.Copy(this.Data.ToArray(), this.Position, tmp, 0, tmp.Length);
            this.Position += 16;
            return new Guid(tmp);
        }

        /// <summary>
        /// Packet Reader returns the next 2 byte Integer
        /// </summary>
        /// <returns></returns>
        public ushort ReadInt16()
        {
            if ((this.Data.Count - this.Position) < 2) return 0;
            ushort tmp = (ushort)BitConverter.ToInt16(this.Data.ToArray(), this.Position);
            this.Position += 2;
            return tmp;
        }

        /// <summary>
        /// Packet Reader returns the next 4 byte Integer
        /// </summary>
        /// <returns></returns>
        public uint ReadInt32()
        {
            if ((this.Data.Count - this.Position) < 4) return 0;
            uint tmp = (uint)BitConverter.ToInt32(this.Data.ToArray(), this.Position);
            this.Position += 4;
            return tmp;
        }

        /// <summary>
        /// Packet Reader returns the next 8 byte Integer
        /// </summary>
        /// <returns></returns>
        public ulong ReadInt64()
        {
            if ((this.Data.Count - this.Position) < 8) return 0;
            ulong tmp = (ulong)BitConverter.ToInt64(this.Data.ToArray(), this.Position);
            this.Position += 8;
            return tmp;
        }

        /// <summary>
        /// Packet Reader returns the next IPAddress Object
        /// </summary>
        /// <returns></returns>
        public IPAddress ReadIP()
        {
            if ((this.Data.Count - this.Position) < 4) return null;
            byte[] tmp = new byte[4];
            Array.Copy(this.Data.ToArray(), this.Position, tmp, 0, tmp.Length);
            this.Position += 4;
            return new IPAddress(tmp);
        }

        /// <summary>
        /// Packet Reader returns the next 4 byte Unix Timestamp DateTime Object
        /// </summary>
        /// <returns></returns>
        public DateTime ReadUnixTimeStamp()
        {
            if ((this.Data.Count - this.Position) < 4) return new DateTime();
            DateTime tmp = new System.DateTime(1970, 1, 1, 0, 0, 0, 0);
            tmp = tmp.AddSeconds(BitConverter.ToInt32(this.Data.ToArray(), this.Position));
            this.Position += 4;
            return tmp;
        }

        /// <summary>
        /// Packet Reader returns the next Null Terminated String
        /// </summary>
        /// <returns></returns>
        public String ReadString()
        {
            if (this.Position >= this.Data.Count) return String.Empty;
            int split = this.Data.IndexOf(0, this.Position);
            byte[] tmp = new byte[split > -1 ? (split - this.Position) : (this.Data.Count - this.Position)];
            Array.Copy(this.Data.ToArray(), this.Position, tmp, 0, tmp.Length);
            this.Position = split > -1 ? (split + 1) : this.Data.Count;
            return Encoding.UTF8.GetString(tmp);
        }

        /// <summary>
        /// Packet Writer adds a byte to the end of the packet
        /// </summary>
        /// <param name="b">The single Byte to be added</param>
        public void WriteByte(byte b)
        {
            this.Data.Add(b);
        }

        /// <summary>
        /// Packet Writer adds a byte array to the end of the packet
        /// </summary>
        /// <param name="b">The Byte Array to be added</param>
        public void WriteBytes(byte[] b)
        {
            this.Data.AddRange(b);
        }

        /// <summary>
        /// Packet Writer adds a 16 byte GUID to the end of the packet
        /// </summary>
        /// <param name="g">The GUID to be added</param>
        public void WriteGuid(Guid g)
        {
            this.Data.AddRange(g.ToByteArray());
        }

        /// <summary>
        /// Packet Writer adds a 2 byte Integer to the end of the packet
        /// </summary>
        /// <param name="i">The 16 bit Integer to be added</param>
        public void WriteInt16(ushort i)
        {
            this.Data.AddRange(BitConverter.GetBytes(i));
        }

        /// <summary>
        /// Packet Writer adds a 4 byte Integer to the end of the packet
        /// </summary>
        /// <param name="i">The 32 bit Integer to be added</param>
        public void WriteInt32(uint i)
        {
            this.Data.AddRange(BitConverter.GetBytes(i));
        }

        /// <summary>
        /// Packet Writer adds a 8 byte Integer to the end of the packet
        /// </summary>
        /// <param name="i">The 64 bit Integer to be added</param>
        public void WriteInt64(ulong i)
        {
            this.Data.AddRange(BitConverter.GetBytes(i));
        }

        /// <summary>
        /// Packet Writer converts a dotted IP Address to bytes and adds to the end of the packet
        /// </summary>
        /// <param name="ip_string">The Dotted IP Address to be added</param>
        public void WriteIP(String ip_string)
        {
            this.Data.AddRange(IPAddress.Parse(ip_string).GetAddressBytes());
        }

        /// <summary>
        /// Packet Writer converts a numeric IP Address to bytes and adds to the end of the packet
        /// </summary>
        /// <param name="ip_numeric">The Numeric IP Address to be added</param>
        public void WriteIP(long ip_numeric)
        {
            this.Data.AddRange(new IPAddress(ip_numeric).GetAddressBytes());
        }

        /// <summary>
        /// Packet Writer converts a 4 element byte array into Network Order and adds to the end of the packet
        /// </summary>
        /// <param name="ip_bytes">The 4 element Byte Array to be added</param>
        public void WriteIP(byte[] ip_bytes)
        {
            if (ip_bytes.Length != 4) return;
            this.Data.AddRange(new IPAddress(ip_bytes).GetAddressBytes());
        }

        /// <summary>
        /// Packet Writer converts an IPAddress Object into bytes and adds to the end of the packet
        /// </summary>
        /// <param name="ip_object">The IPAddress Object to be added</param>
        public void WriteIP(IPAddress ip_object)
        {
            this.Data.AddRange(ip_object.GetAddressBytes());
        }

        /// <summary>
        /// Packet Writer adds a 4 byte Integer representing the current Unix Time to the end of the packet
        /// </summary>
        public void WriteUnixTimeStamp()
        {
            TimeSpan ts = (DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0));
            this.Data.AddRange(BitConverter.GetBytes((uint)ts.TotalSeconds));
        }

        /// <summary>
        /// Packet Writer adds a Null Terminated String to the end of the packet
        /// </summary>
        /// <param name="text">The String to be added</param>
        public void WriteString(String text)
        {
            this.Data.AddRange(Encoding.UTF8.GetBytes(text));
            this.Data.Add(0);
        }

        /// <summary>
        /// Packet Writer adds a String to the end of the packet
        /// </summary>
        /// <param name="text">The String to be added to the packet</param>
        /// <param name="null_terminated">Set to False if the string does not require Null Termination</param>
        public void WriteString(String text, bool null_terminated)
        {
            this.Data.AddRange(Encoding.UTF8.GetBytes(text));
            if (null_terminated) this.Data.Add(0);
        }

        /// <summary>
        /// Convert AresDataPacket Object into a Byte Array
        /// </summary>
        /// <returns></returns>
        public byte[] ToByteArray()
        {
            return this.Data.ToArray();
        }

        /// <summary>
        /// Convert AresDataPacket Object into a correctly formatted Ares Packet Byte Array ready for sending
        /// </summary>
        /// <param name="packet_id">The Ares Protocol Packet Identification Number</param>
        /// <returns></returns>
        public byte[] ToAresPacket(byte packet_id)
        {
            List<byte> tmp = new List<byte>(this.Data.ToArray());
            tmp.Insert(0, packet_id);
            tmp.InsertRange(0, BitConverter.GetBytes((ushort)(tmp.Count - 1)));
            return tmp.ToArray();
        }

        /// <summary>
        /// Convert AresDataPacket Object into a corrently formatted Ares DC Packet
        /// </summary>
        /// <param name="packet_id">The Ares Protocol Packet Identification Number</param>
        /// <returns></returns>
        public byte[] ToAresDCPacket(byte packet_id)
        {
            List<byte> tmp = new List<byte>(this.Data.ToArray());
            tmp.Insert(0, packet_id);
            tmp.InsertRange(0, BitConverter.GetBytes((ushort)(tmp.Count - 1)));
            tmp.Insert(0, 0);
            return tmp.ToArray();
        }
    }
}
