using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using ZLib;
using Ares.PacketHandlers;

namespace cb0t_chat_client_v2
{
    class VoiceRecorder
    {
        [DllImport("winmm.dll")]
        public static extern int mciSendString(string A_0, StringBuilder A_1, int A_2, int A_3);

        private byte[] _buffer;

        public byte[][] GetAresPackets(bool pm, String target_name, int length)
        {
            return pm ? this.PMPackets(target_name, length) : this.RoomPackets(length);
        }

        private byte[][] PMPackets(String target_name, int length)
        {
            List<byte[]> packets = new List<byte[]>();

            uint ident = Helpers.UnixTime();
            byte clip_length = (byte)(length + 2);
            byte compression_count = 0;
            uint uncompressed_size = (uint)this._buffer.Length;
            uint compressed_size = uncompressed_size;
            List<uint> compress_results = new List<uint>();
            byte[] buf = this._buffer;
            byte[] buf123;

            while (true)
            {
                byte[] tmp = Zlib.Compress(buf);

                if (tmp.Length < buf.Length)
                {
                    compression_count++;
                    compress_results.Add((uint)tmp.Length);
                    compressed_size = (uint)tmp.Length;
                    buf = tmp;
                }
                else break;
            }

            AresDataPacket first = new AresDataPacket();
            first.WriteString(target_name);
            first.WriteInt32(ident);
            first.WriteByte(clip_length);
            first.WriteByte(compression_count);
            first.WriteInt32(uncompressed_size);

            for (int i = 0; i < compress_results.Count; i++)
                first.WriteInt32(compress_results[i]);

            int first_remaining = (4000 - first.GetByteCount());

            if (compressed_size <= first_remaining)
            {
                first.WriteBytes(buf);
                buf123 = first.ToAresPacket(207);
                first = new AresDataPacket();
                first.WriteBytes(buf123);
                packets.Add(first.ToAresPacket(250));
                return packets.ToArray();
            }

            List<byte> voice_buf = new List<byte>(buf);
            first.WriteBytes(voice_buf.GetRange(0, first_remaining).ToArray());
            buf123 = first.ToAresPacket(207);
            first = new AresDataPacket();
            first.WriteBytes(buf123);
            packets.Add(first.ToAresPacket(250));
            voice_buf.RemoveRange(0, first_remaining);

            while (voice_buf.Count > 0)
            {
                AresDataPacket chunk = new AresDataPacket();
                chunk.WriteString(target_name);
                chunk.WriteInt32(ident);

                if (voice_buf.Count >= 4000)
                {
                    chunk.WriteBytes(voice_buf.GetRange(0, 4000).ToArray());
                    buf123 = chunk.ToAresPacket(209);
                    chunk = new AresDataPacket();
                    chunk.WriteBytes(buf123);
                    packets.Add(chunk.ToAresPacket(250));
                    voice_buf.RemoveRange(0, 4000);
                }
                else
                {
                    chunk.WriteBytes(voice_buf.ToArray());
                    buf123 = chunk.ToAresPacket(209);
                    chunk = new AresDataPacket();
                    chunk.WriteBytes(buf123);
                    packets.Add(chunk.ToAresPacket(250));
                    voice_buf.Clear();
                }
            }

            return packets.ToArray();
        }

        private byte[][] RoomPackets(int length)
        {
            List<byte[]> packets = new List<byte[]>();

            uint ident = Helpers.UnixTime();
            byte clip_length = (byte)(length + 2);
            byte compression_count = 0;
            uint uncompressed_size = (uint)this._buffer.Length;
            uint compressed_size = uncompressed_size;
            List<uint> compress_results = new List<uint>();
            byte[] buf = this._buffer;
            byte[] buf123;

            while (true)
            {
                byte[] tmp = Zlib.Compress(buf);

                if (tmp.Length < buf.Length)
                {
                    compression_count++;
                    compress_results.Add((uint)tmp.Length);
                    compressed_size = (uint)tmp.Length;
                    buf = tmp;
                }
                else break;
            }

            AresDataPacket first = new AresDataPacket();
            first.WriteInt32(ident);
            first.WriteByte(clip_length);
            first.WriteByte(compression_count);
            first.WriteInt32(uncompressed_size);

            for (int i = 0; i < compress_results.Count; i++)
                first.WriteInt32(compress_results[i]);

            int first_remaining = (4000 - first.GetByteCount());

            if (compressed_size <= first_remaining)
            {
                first.WriteBytes(buf);
                buf123 = first.ToAresPacket(206);
                first = new AresDataPacket();
                first.WriteBytes(buf123);
                packets.Add(first.ToAresPacket(250));
                return packets.ToArray();
            }

            List<byte> voice_buf = new List<byte>(buf);
            first.WriteBytes(voice_buf.GetRange(0, first_remaining).ToArray());
            buf123 = first.ToAresPacket(206);
            first = new AresDataPacket();
            first.WriteBytes(buf123);
            packets.Add(first.ToAresPacket(250));
            voice_buf.RemoveRange(0, first_remaining);

            while (voice_buf.Count > 0)
            {
                AresDataPacket chunk = new AresDataPacket();
                chunk.WriteInt32(ident);

                if (voice_buf.Count >= 4000)
                {
                    chunk.WriteBytes(voice_buf.GetRange(0, 4000).ToArray());
                    buf123 = chunk.ToAresPacket(208);
                    chunk = new AresDataPacket();
                    chunk.WriteBytes(buf123);
                    packets.Add(chunk.ToAresPacket(250));
                    voice_buf.RemoveRange(0, 4000);
                }
                else
                {
                    chunk.WriteBytes(voice_buf.ToArray());
                    buf123 = chunk.ToAresPacket(208);
                    chunk = new AresDataPacket();
                    chunk.WriteBytes(buf123);
                    packets.Add(chunk.ToAresPacket(250));
                    voice_buf.Clear();
                }
            }

            return packets.ToArray();
        }

        public void Start(bool high_quality)
        {
            this._buffer = null;
            mciSendString("open new type waveaudio alias mywav", null, 0, 0);

            if (high_quality)
                mciSendString("set mywav time format ms format tag pcm channels 1 samplespersec 16000 bytespersec 16000 alignment 1 bitspersample 8", null, 0, 0);
            else
                mciSendString("set mywav time format ms format tag pcm channels 1 samplespersec 8000 bytespersec 8000 alignment 1 bitspersample 8", null, 0, 0);

            mciSendString("record mywav", null, 0, 0);
        }

        public void Stop()
        {
            mciSendString("stop mywav", null, 0, 0);
            mciSendString("save mywav \"" + (Settings.folder_path + "cbot vc.wav") + "\"", null, 0, 0);
            mciSendString("close mywav", null, 0, 0);
            Thread.Sleep(250);
            this._buffer = File.ReadAllBytes(Settings.folder_path + "cbot vc.wav");
        }

        public void Cancel()
        {
            mciSendString("stop mywav", null, 0, 0);
            mciSendString("close mywav", null, 0, 0);
        }
    }
}
