using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace cb0t
{
    public class BrowseItem
    {
        internal BrowseType Mime { get; private set; }
        public ulong FileSize = 0;
        public ushort param1 = 0;
        public ushort param2 = 0;
        public ushort param3 = 0;
        public String Title = String.Empty;
        public String Artist = String.Empty;
        public String Album = String.Empty;
        public String Category = String.Empty;
        public String Year = String.Empty;
        public String Language = String.Empty;
        public String URL = String.Empty;
        public String Comment = String.Empty;
        public String Genre = String.Empty;
        public String Format = String.Empty;
        public String FileName = String.Empty;
        public String Path = String.Empty;
        public String FileSizeString = String.Empty;
        public byte[] SHA1Hash;

        public BrowseItem(TCPPacketReader packet, CryptoService c)
        {
            byte b = packet;

            switch (b)
            {
                case 1:
                    this.Mime = BrowseType.Audio;
                    break;

                case 3:
                    this.Mime = BrowseType.Software;
                    break;

                case 5:
                    this.Mime = BrowseType.Video;
                    break;

                case 6:
                    this.Mime = BrowseType.Document;
                    break;

                case 7:
                    this.Mime = BrowseType.Image;
                    break;

                default:
                    this.Mime = BrowseType.Other;
                    break;
            }

            if (packet.Remaining >= 27)
            {
                this.FileSize = ((uint)packet);
                packet.SkipBytes(16); // ignore - pre 2005 file guid, now uses SHA1

                switch (this.Mime) // params
                {
                    case BrowseType.Audio: // audio
                        this.param1 = packet; // bit rate
                        this.param3 = packet; // duration
                        break;

                    case BrowseType.Video: // video
                        this.param1 = packet; // bit rate
                        this.param2 = packet; // sample rate
                        this.param3 = packet; // duration
                        break;

                    case BrowseType.Image: // image
                        this.param1 = packet; // width
                        this.param2 = packet; // height
                        this.param3 = ((byte)packet); // depth
                        break;
                }

                if (packet.Remaining >= 10)
                {
                    ushort data_size = packet;
                    ushort counter = 0;

                    while (packet.Remaining >= 2) // details
                    {
                        byte length = packet;
                        byte type = packet;

                        if (length > packet.Remaining)
                            break;

                        switch (type) // could make an object out of these !!
                        {
                            case 1: // title
                                this.Title = Encoding.UTF8.GetString(packet.ReadBytes(length));
                                break;

                            case 2: // artist
                                this.Artist = Encoding.UTF8.GetString(packet.ReadBytes(length));
                                break;

                            case 3: // album
                                this.Album = Encoding.UTF8.GetString(packet.ReadBytes(length));
                                break;

                            case 4: // category
                                this.Category = Encoding.UTF8.GetString(packet.ReadBytes(length));
                                break;

                            case 5: // year
                                this.Year = Encoding.UTF8.GetString(packet.ReadBytes(length));
                                break;

                            case 6: // language
                                this.Language = Encoding.UTF8.GetString(packet.ReadBytes(length));
                                break;

                            case 7: // url
                                this.URL = Encoding.UTF8.GetString(packet.ReadBytes(length));
                                break;

                            case 8: // comment
                                this.Comment = Encoding.UTF8.GetString(packet.ReadBytes(length));
                                break;

                            case 9: // genre
                                this.Genre = Encoding.UTF8.GetString(packet.ReadBytes(length));
                                break;

                            case 10: // format
                                this.Format = Encoding.UTF8.GetString(packet.ReadBytes(length));
                                break;

                            case 15: // filename
                                this.FileName = Encoding.UTF8.GetString(packet.ReadBytes(length));
                                break;

                            case 20: // SHA1 hash
                                this.SHA1Hash = packet.ReadBytes(20);
                                break;

                            case 23: // path
                                this.Path = Encoding.UTF8.GetString(packet.ReadBytes(length));
                                break;

                            case 24: // size64
                                this.FileSize = packet;
                                break;
                        }

                        counter += 2;
                        counter += length;

                        if (counter >= data_size)
                            break;
                    }
                }
            }

            if (this.Title.Length == 0)
                this.Title = this.FileName;

            this.FileSizeString = this.FileSize > 1024 ? (this.FileSize / 1024).ToString("#,##0") + " KB" : this.FileSize.ToString();
        }
    }

    enum BrowseType
    {
        Audio,
        Software,
        Video,
        Document,
        Image,
        Other
    }
}
