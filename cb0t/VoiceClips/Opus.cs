using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace cb0t
{
    class Opus
    {
        [DllImport("opus.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr opus_encoder_create(int Fs, int channels, int app, out IntPtr error);

        [DllImport("opus.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr opus_decoder_create(int Fs, int channels, out IntPtr error);

        [DllImport("opus.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr opus_encoder_init(IntPtr OpusEncoder, int Fs, int channels, int app);

        [DllImport("opus.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr opus_decoder_init(IntPtr OpusDecoder, int Fs, int channels);

        [DllImport("opus.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern int opus_encoder_ctl(IntPtr OpusEncoder, int request, int value);

        [DllImport("opus.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern int opus_encode(IntPtr OpusEncoder, byte[] PCMData, int numPCMsamples, byte[] outdata, int outDataSize);

        [DllImport("opus.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern int opus_decode(IntPtr OpusDecoder, byte[] OpusData, int lenData, byte[] PCMData, int frameSize, int interpolateMissing);

        [DllImport("opus.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern void opus_encoder_destroy(IntPtr OpusEncoder);

        [DllImport("opus.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern void opus_decoder_destroy(IntPtr OpusDecoder);

        public static byte[] Encode(byte[] decoded)
        {
            byte[] result = new byte[] { };

            try
            {
                IntPtr err = IntPtr.Zero;
                IntPtr enc = opus_encoder_create(16000, 1, 2048, out err);

                if (!err.Equals(IntPtr.Zero) || enc.Equals(IntPtr.Zero))
                    return result;

                err = opus_encoder_init(enc, 16000, 1, 2048);

                if (!err.Equals(IntPtr.Zero))
                    return result;

                opus_encoder_ctl(enc, 4002, 10000);
                opus_encoder_ctl(enc, 4016, 1);

                using (MemoryStream raw = new MemoryStream(decoded))
                using (MemoryStream ms = new MemoryStream())
                {
                    raw.Seek(44, SeekOrigin.Begin);
                    int sizePCMChunk = 1920;
                    byte[] PCMData = new byte[sizePCMChunk];
                    int sizeOpusBuffer = 1276;
                    byte[] opusBuffer = new byte[sizeOpusBuffer];
                    int PCMSamplesPerFrame = sizePCMChunk / 2;
                    int lenRead, numbytesEncoded;
                    ushort numW;

                    while (raw.Position < raw.Length)
                    {
                        lenRead = raw.Read(PCMData, 0, sizePCMChunk);

                        if (lenRead < sizePCMChunk)
                            break;

                        numbytesEncoded = opus_encode(enc, PCMData, PCMSamplesPerFrame, opusBuffer, sizeOpusBuffer);

                        if (numbytesEncoded > 0)
                        {
                            numW = (ushort)numbytesEncoded;
                            ms.Write(BitConverter.GetBytes(numW), 0, 2);
                            ms.Write(opusBuffer, 0, numbytesEncoded);
                        }
                    }

                    result = ms.ToArray();
                }

                opus_encoder_destroy(enc);
            }
            catch { }

            return result;
        }

        public static byte[] Decode(byte[] encoded)
        {
            byte[] result = new byte[] { };

            try
            {
                int inSize = encoded.Length;
                IntPtr err = IntPtr.Zero;
                IntPtr decoder = opus_decoder_create(16000, 1, out err);

                if (!err.Equals(IntPtr.Zero) || decoder.Equals(IntPtr.Zero))
                    return result;

                err = opus_decoder_init(decoder, 16000, 1);

                if (!err.Equals(IntPtr.Zero))
                    return result;

                using (MemoryStream ms = new MemoryStream())
                {
                    int sizePCMChunk = 1920;
                    int PCMSamplesPerFrame = sizePCMChunk / 2;
                    byte[] PCMBuffer = new byte[sizePCMChunk];
                    int numSamplesDecoded;
                    List<byte> list = new List<byte>(encoded);

                    while (inSize > 0)
                    {
                        ushort lenW = BitConverter.ToUInt16(list.GetRange(0, 2).ToArray(), 0);
                        list.RemoveRange(0, 2);
                        inSize -= 2;

                        if (inSize < lenW)
                            break;

                        byte[] opusBuffer = list.GetRange(0, lenW).ToArray();
                        list.RemoveRange(0, lenW);
                        inSize -= lenW;
                        numSamplesDecoded = opus_decode(decoder, opusBuffer, opusBuffer.Length, PCMBuffer, PCMSamplesPerFrame, 0);

                        if (numSamplesDecoded > 0)
                            ms.Write(PCMBuffer, 0, (numSamplesDecoded * 2));
                    }

                    opus_decoder_destroy(decoder);

                    int sizeTotal = ms.Length < 44 ? 36 : (int)(ms.Length - 8);
                    int sizeMinusHeader = (sizeTotal - 36);
                    List<byte> header = new List<byte>();
                    header.AddRange(Encoding.ASCII.GetBytes("RIFF"));
                    header.AddRange(BitConverter.GetBytes((uint)sizeTotal));
                    header.AddRange(Encoding.ASCII.GetBytes("WAVEfmt "));
                    header.AddRange(BitConverter.GetBytes((uint)16));
                    header.AddRange(BitConverter.GetBytes((ushort)1));
                    header.AddRange(BitConverter.GetBytes((ushort)1));
                    header.AddRange(BitConverter.GetBytes((uint)16000));
                    header.AddRange(BitConverter.GetBytes((uint)32000));
                    header.AddRange(BitConverter.GetBytes((ushort)2));
                    header.AddRange(BitConverter.GetBytes((ushort)16));
                    header.AddRange(Encoding.ASCII.GetBytes("data"));
                    header.AddRange(BitConverter.GetBytes((uint)sizeMinusHeader));
                    ms.Seek(0, SeekOrigin.Begin);
                    ms.Write(header.ToArray(), 0, header.Count);
                    header.Clear();
                    header = null;
                    result = ms.ToArray();
                }
            }
            catch { }

            return result;
        }
    }
}
