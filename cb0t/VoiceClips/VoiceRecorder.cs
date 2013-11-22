using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;

namespace cb0t
{
    class VoiceRecorder
    {
        [DllImport("winmm.dll")]
        private static extern Int32 mciSendString(String command, StringBuilder buffer, Int32 bufferSize, IntPtr hwndCallback);

        private const String RECORD_DEVICE_NAME = "cb_v3_record";
        private const int MAX_CHUNK_SIZE = 6144;

        public static bool Recording { get; private set; }
        
        public static void RecordStart()
        {
            mciSendString("open new type waveaudio alias " + RECORD_DEVICE_NAME, null, 0, IntPtr.Zero);
            mciSendString("set " + RECORD_DEVICE_NAME + " time format ms format tag pcm channels 1 samplespersec 16000 bytespersec 32000 alignment 2 bitspersample 16 input " + AudioHelpers.GetRecordIdent(), null, 0, IntPtr.Zero);
            mciSendString("record " + RECORD_DEVICE_NAME, null, 0, IntPtr.Zero);
            Recording = true;
        }

        public static void RecordCancel()
        {
            mciSendString("stop " + RECORD_DEVICE_NAME, null, 0, IntPtr.Zero);
            mciSendString("close " + RECORD_DEVICE_NAME, null, 0, IntPtr.Zero);
            Recording = false;
        }

        public static void RecordStop()
        {
            String path = Path.Combine(Settings.VoicePath, "record.wav");
            mciSendString("stop " + RECORD_DEVICE_NAME, null, 0, IntPtr.Zero);
            mciSendString("save " + RECORD_DEVICE_NAME + " \"" + path + "\"", null, 0, IntPtr.Zero);
            mciSendString("close " + RECORD_DEVICE_NAME, null, 0, IntPtr.Zero);
            Thread.Sleep(300);
            Recording = false;
        }

        private static byte[] RecordBytes()
        {
            byte[] result = null;

            try
            {
                String path = Path.Combine(Settings.VoicePath, "record.wav");

                if (File.Exists(path))
                    result = File.ReadAllBytes(path);
            }
            catch { }

            return result;
        }

        public static byte[][] GetPackets(VoiceRecorderSendMethod m, byte len)
        {
            List<byte[]> packets = new List<byte[]>();
            byte[] org_data = RecordBytes();
            uint ident = Settings.Time;
            byte clip_len = (byte)(len + 2);
            uint uncompressed_size = (uint)org_data.Length;
            uint compressed_size = uncompressed_size;
            List<uint> compress_results = new List<uint>();

            if (m == VoiceRecorderSendMethod.Opus)
            {
                org_data = Opus.Encode(org_data);
                uncompressed_size = (uint)org_data.Length;
                List<byte> data_to_send = new List<byte>(org_data);

                if (data_to_send.Count < MAX_CHUNK_SIZE)
                {
                    packets.Add(TCPOutbound.VoiceFirst(ident, clip_len, 0, uncompressed_size, data_to_send.ToArray()));
                }
                else
                {
                    packets.Add(TCPOutbound.VoiceFirst(ident, clip_len, 0, uncompressed_size, data_to_send.ToArray()));
                    data_to_send.RemoveRange(0, MAX_CHUNK_SIZE);

                    while (data_to_send.Count >= MAX_CHUNK_SIZE)
                    {
                        packets.Add(TCPOutbound.VoiceChunk(ident, data_to_send.GetRange(0, MAX_CHUNK_SIZE).ToArray()));
                        data_to_send.RemoveRange(0, MAX_CHUNK_SIZE);
                    }

                    if (data_to_send.Count > 0)
                        packets.Add(TCPOutbound.VoiceChunk(ident, data_to_send.ToArray()));
                }
            }
            else
            {
                while (true)
                {
                    byte[] tmp = Zip.Compress(org_data);

                    if (tmp.Length < org_data.Length)
                    {
                        compress_results.Add((uint)tmp.Length);
                        compressed_size = (uint)tmp.Length;
                        org_data = tmp;
                    }
                    else break;
                }

                List<byte> data_to_send = new List<byte>(org_data);
                List<byte> compress_results_data = new List<byte>();

                for (int i = 0; i < compress_results.Count; i++)
                    compress_results_data.AddRange(BitConverter.GetBytes(compress_results[i]));

                if (data_to_send.Count < MAX_CHUNK_SIZE)
                {
                    packets.Add(TCPOutbound.VoiceFirst(ident, clip_len, (byte)compress_results.Count,
                        uncompressed_size, compress_results_data.ToArray().Concat(data_to_send).ToArray()));
                }
                else
                {
                    packets.Add(TCPOutbound.VoiceFirst(ident, clip_len, (byte)compress_results.Count,
                        uncompressed_size, compress_results_data.ToArray().Concat(data_to_send.GetRange(0, MAX_CHUNK_SIZE)).ToArray()));

                    data_to_send.RemoveRange(0, MAX_CHUNK_SIZE);

                    while (data_to_send.Count >= MAX_CHUNK_SIZE)
                    {
                        packets.Add(TCPOutbound.VoiceChunk(ident, data_to_send.GetRange(0, MAX_CHUNK_SIZE).ToArray()));
                        data_to_send.RemoveRange(0, MAX_CHUNK_SIZE);
                    }

                    if (data_to_send.Count > 0)
                        packets.Add(TCPOutbound.VoiceChunk(ident, data_to_send.ToArray()));
                }
            }

            return packets.ToArray();
        }

        public static byte[][] GetPackets(String target, VoiceRecorderSendMethod m, byte len, CryptoService c)
        {
            List<byte[]> packets = new List<byte[]>();
            byte[] org_data = RecordBytes();
            uint ident = Settings.Time;
            byte clip_len = (byte)(len + 2);
            uint uncompressed_size = (uint)org_data.Length;
            uint compressed_size = uncompressed_size;
            List<uint> compress_results = new List<uint>();

            if (m == VoiceRecorderSendMethod.Opus)
            {
                org_data = Opus.Encode(org_data);
                uncompressed_size = (uint)org_data.Length;
                List<byte> data_to_send = new List<byte>(org_data);

                if (data_to_send.Count < MAX_CHUNK_SIZE)
                {
                    packets.Add(TCPOutbound.VoiceToFirst(target, ident, clip_len, 0, uncompressed_size, data_to_send.ToArray(), c));
                }
                else
                {
                    packets.Add(TCPOutbound.VoiceToFirst(target, ident, clip_len, 0, uncompressed_size, data_to_send.ToArray(), c));
                    data_to_send.RemoveRange(0, MAX_CHUNK_SIZE);

                    while (data_to_send.Count >= MAX_CHUNK_SIZE)
                    {
                        packets.Add(TCPOutbound.VoiceToChunk(target, ident, data_to_send.GetRange(0, MAX_CHUNK_SIZE).ToArray(), c));
                        data_to_send.RemoveRange(0, MAX_CHUNK_SIZE);
                    }

                    if (data_to_send.Count > 0)
                        packets.Add(TCPOutbound.VoiceToChunk(target, ident, data_to_send.ToArray(), c));
                }
            }
            else
            {
                while (true)
                {
                    byte[] tmp = Zip.Compress(org_data);

                    if (tmp.Length < org_data.Length)
                    {
                        compress_results.Add((uint)tmp.Length);
                        compressed_size = (uint)tmp.Length;
                        org_data = tmp;
                    }
                    else break;
                }

                List<byte> data_to_send = new List<byte>(org_data);
                List<byte> compress_results_data = new List<byte>();

                for (int i = 0; i < compress_results.Count; i++)
                    compress_results_data.AddRange(BitConverter.GetBytes(compress_results[i]));

                if (data_to_send.Count < MAX_CHUNK_SIZE)
                {
                    packets.Add(TCPOutbound.VoiceToFirst(target, ident, clip_len, (byte)compress_results.Count,
                        uncompressed_size, compress_results_data.ToArray().Concat(data_to_send).ToArray(), c));
                }
                else
                {
                    packets.Add(TCPOutbound.VoiceToFirst(target, ident, clip_len, (byte)compress_results.Count,
                        uncompressed_size, compress_results_data.ToArray().Concat(data_to_send.GetRange(0, MAX_CHUNK_SIZE)).ToArray(), c));

                    data_to_send.RemoveRange(0, MAX_CHUNK_SIZE);

                    while (data_to_send.Count >= MAX_CHUNK_SIZE)
                    {
                        packets.Add(TCPOutbound.VoiceToChunk(target, ident, data_to_send.GetRange(0, MAX_CHUNK_SIZE).ToArray(), c));
                        data_to_send.RemoveRange(0, MAX_CHUNK_SIZE);
                    }

                    if (data_to_send.Count > 0)
                        packets.Add(TCPOutbound.VoiceToChunk(target, ident, data_to_send.ToArray(), c));
                }
            }

            return packets.ToArray();
        }
    }

    enum VoiceRecorderSendMethod
    {
        Zip,
        Opus
    }
}
