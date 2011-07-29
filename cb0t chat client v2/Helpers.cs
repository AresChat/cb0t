using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.IO;
using System.Diagnostics;
using System.Threading;
using System.Net.Sockets;
using System.Text.RegularExpressions;

namespace cb0t_chat_client_v2
{
    class Helpers
    {
        private static Random rnd = new Random();

        public static bool IsIP(String str)
        {
            IPAddress ip;
            return IPAddress.TryParse(str, out ip);
        }

        public static bool IsUshort(String str)
        {
            ushort u;
            return ushort.TryParse(str, out u);
        }

        public static String StripColors(String input)
        {
            if (Regex.IsMatch(input, @"\x03|\x05", RegexOptions.IgnoreCase))
                input = Regex.Replace(input, @"(\x03|\x05)[0-9]{2}", "");

            input = input.Replace("\x06", "");
            input = input.Replace("\x07", "");
            input = input.Replace("\x09", "");
            input = input.Replace("\x02", "");
            input = input.Replace("­", "");

            return input;
        }

        public static String CountryCodeToString(byte country)
        {
            switch (country)
            {
                case 1:
                    return "Afghanistan";
                case 2:
                    return "Albania";
                case 3:
                    return "Algeria";
                case 4:
                    return "Andorra";
                case 5:
                    return "Angola";
                case 6:
                    return "Anguilla";
                case 7:
                    return "Antarctia";
                case 8:
                    return "Antigua and Barbuda";
                case 9:
                    return "Argentina";
                case 10:
                    return "Armenia";
                case 11:
                    return "Aruba";
                case 12:
                    return "Australia";
                case 13:
                    return "Austria";
                case 14:
                    return "Azerbaijan";
                case 15:
                    return "Bahamas";
                case 16:
                    return "Bahrain";
                case 17:
                    return "Bangladesh";
                case 18:
                    return "Barbados";
                case 19:
                    return "Belarus";
                case 20:
                    return "Belgium";
                case 21:
                    return "Belize";
                case 22:
                    return "Berin";
                case 23:
                    return "Bermuda";
                case 24:
                    return "Bhutan";
                case 25:
                    return "Bolivia";
                case 26:
                    return "Bosnia and Herzegovina";
                case 27:
                    return "Botswana";
                case 28:
                    return "Brazil";
                case 29:
                    return "Brunei";
                case 30:
                    return "Bulgaria";
                case 31:
                    return "Burkina Faso";
                case 32:
                    return "Burundi";
                case 33:
                    return "Cambodia";
                case 34:
                    return "Cameroon";
                case 35:
                    return "Canada";
                case 36:
                    return "Cape Verde";
                case 37:
                    return "Cayman Islands";
                case 38:
                    return "Central African Republic";
                case 39:
                    return "Chad";
                case 40:
                    return "Chile";
                case 41:
                    return "China";
                case 42:
                    return "Christmas Islands";
                case 43:
                    return "Cocos Islands";
                case 44:
                    return "Colombia";
                case 45:
                    return "Comoros";
                case 46:
                    return "Congo";
                case 47:
                    return "Congo";
                case 48:
                    return "Cook Islands";
                case 49:
                    return "Costa Rica";
                case 50:
                    return "Croatia";
                case 51:
                    return "Cuba";
                case 52:
                    return "Cyprus";
                case 53:
                    return "Czech Republic";
                case 54:
                    return "Denmark";
                case 55:
                    return "Fjibouti";
                case 56:
                    return "Dominica";
                case 57:
                    return "Dominican Republic";
                case 58:
                    return "Dutch antilles";
                case 59:
                    return "East Timor";
                case 60:
                    return "Ecuador";
                case 61:
                    return "Egypt";
                case 62:
                    return "El Salvador";
                case 63:
                    return "Equatorial Guinea";
                case 64:
                    return "Entea";
                case 65:
                    return "Estonia";
                case 66:
                    return "Ethiopia";
                case 67:
                    return "Falkland Islands";
                case 68:
                    return "Faroe Islands";
                case 69:
                    return "Fiji Islands";
                case 70:
                    return "Finland";
                case 71:
                    return "France";
                case 72:
                    return "French Polynesia";
                case 73:
                    return "Gabon";
                case 74:
                    return "Gambia";
                case 75:
                    return "Gaza";
                case 76:
                    return "Georgia";
                case 77:
                    return "Germany";
                case 78:
                    return "Ghana";
                case 79:
                    return "Gibraltar";
                case 80:
                    return "Greece";
                case 81:
                    return "Greenland";
                case 82:
                    return "Grenada";
                case 83:
                    return "Geuadaloupe";
                case 84:
                    return "Guatemala";
                case 85:
                    return "Guernsey";
                case 86:
                    return "Guinea";
                case 87:
                    return "Guinea-Bissau";
                case 88:
                    return "Guyana";
                case 89:
                    return "Guyana";
                case 90:
                    return "Haiti";
                case 91:
                    return "Honduras";
                case 92:
                    return "Hong Kong";
                case 93:
                    return "Hungary";
                case 94:
                    return "Iceland";
                case 95:
                    return "India";
                case 96:
                    return "Indonesia";
                case 97:
                    return "Iran";
                case 98:
                    return "Iraq";
                case 99:
                    return "Ireland";
                case 100:
                    return "Isle of Man";
                case 101:
                    return "Israel";
                case 102:
                    return "Italy";
                case 103:
                    return "Ivory Coast";
                case 104:
                    return "Jamaica";
                case 105:
                    return "Japan";
                case 106:
                    return "Jersey";
                case 107:
                    return "Jordan";
                case 108:
                    return "Kazakhstan";
                case 109:
                    return "Kenya";
                case 110:
                    return "Kiribati";
                case 111:
                    return "Kuwwait";
                case 112:
                    return "Kyrgyzstan";
                case 113:
                    return "Laos";
                case 114:
                    return "Latvia";
                case 115:
                    return "Lebanon";
                case 116:
                    return "Lesotho";
                case 117:
                    return "Liberia";
                case 118:
                    return "Libya";
                case 119:
                    return "Liechtenstein";
                case 120:
                    return "Lithuania";
                case 121:
                    return "Luxembourg";
                case 122:
                    return "Macao";
                case 123:
                    return "Macedonia";
                case 124:
                    return "Madagascar";
                case 125:
                    return "Malawi";
                case 126:
                    return "Malaysia";
                case 127:
                    return "Maldives";
                case 128:
                    return "Mali";
                case 129:
                    return "Malta";
                case 130:
                    return "Marshall Islands";
                case 131:
                    return "Martinique";
                case 132:
                    return "Mauritania";
                case 133:
                    return "Mauritius";
                case 134:
                    return "Mayotte";
                case 135:
                    return "Mexico";
                case 136:
                    return "Micronesia";
                case 137:
                    return "Moldova";
                case 138:
                    return "Monaco";
                case 139:
                    return "Mongolia";
                case 140:
                    return "Montserrat";
                case 141:
                    return "Morocco";
                case 142:
                    return "Mozambique";
                case 143:
                    return "Myanmar";
                case 144:
                    return "Namibia";
                case 145:
                    return "Nauru";
                case 146:
                    return "Nepal";
                case 147:
                    return "Netherlands";
                case 148:
                    return "New Caledonia";
                case 149:
                    return "New Zealand";
                case 150:
                    return "Nicaragua";
                case 151:
                    return "Niger";
                case 152:
                    return "Nigeria";
                case 153:
                    return "Niue";
                case 154:
                    return "Norfolk Island";
                case 155:
                    return "North Korea";
                case 156:
                    return "Norway";
                case 157:
                    return "Oman";
                case 158:
                    return "Pakistan";
                case 159:
                    return "Palau";
                case 160:
                    return "Panama";
                case 161:
                    return "Papua New Guinea";
                case 162:
                    return "Paraguay";
                case 163:
                    return "Peru";
                case 164:
                    return "Phillippines";
                case 165:
                    return "Pitcairn Island";
                case 166:
                    return "Poland";
                case 167:
                    return "Portugal";
                case 168:
                    return "Puerto Rico";
                case 169:
                    return "Qatar";
                case 170:
                    return "Reunion";
                case 171:
                    return "Romania";
                case 172:
                    return "Russia";
                case 173:
                    return "Rwanda";
                case 174:
                    return "Samoa";
                case 175:
                    return "San Marino";
                case 176:
                    return "Sao Tome and Principe";
                case 177:
                    return "Saudi Arabia";
                case 178:
                    return "Senegal";
                case 179:
                    return "Seychelles";
                case 180:
                    return "Sierra Leone";
                case 181:
                    return "Singapore";
                case 182:
                    return "Slovakia";
                case 183:
                    return "Slovenia";
                case 184:
                    return "Solomon Island";
                case 185:
                    return "Somalia";
                case 186:
                    return "South Africa";
                case 187:
                    return "South Georgia Island";
                case 188:
                    return "South Korea";
                case 189:
                    return "Spain";
                case 190:
                    return "Sri Lanka";
                case 191:
                    return "St Helens";
                case 192:
                    return "St Kitts and Nevis";
                case 193:
                    return "St Lucia";
                case 194:
                    return "St Pierre and Miquelon";
                case 195:
                    return "St Vicent";
                case 196:
                    return "Sudan";
                case 197:
                    return "Suriname";
                case 198:
                    return "Svalbard";
                case 199:
                    return "Swaziland";
                case 200:
                    return "Sweden";
                case 201:
                    return "Switzerland";
                case 202:
                    return "Syria";
                case 203:
                    return "Taiwan";
                case 204:
                    return "Tajikistan";
                case 205:
                    return "Tanzania";
                case 206:
                    return "Thailand";
                case 207:
                    return "Togo";
                case 208:
                    return "Tokelau";
                case 209:
                    return "Tonga";
                case 210:
                    return "Trinidad and Tobago";
                case 211:
                    return "Tunisia";
                case 212:
                    return "Turkey";
                case 213:
                    return "Turkmenistan";
                case 214:
                    return "Turks and Caicos Islands";
                case 215:
                    return "Tuvalu";
                case 216:
                    return "Uganda";
                case 217:
                    return "Ukraine";
                case 218:
                    return "United Arab Emirates";
                case 219:
                    return "United Kingdom";
                case 220:
                    return "United States";
                case 221:
                    return "Uruguay";
                case 222:
                    return "Uzbekistan";
                case 223:
                    return "Vanuatu";
                case 224:
                    return "Venezuela";
                case 225:
                    return "Vietnam";
                case 226:
                    return "Virgin Islands";
                case 227:
                    return "Wallis and Futuna";
                case 228:
                    return "West Bank";
                case 229:
                    return "Western Sahara";
                case 230:
                    return "Yemen";
                case 231:
                    return "Yugoslavia";
                case 232:
                    return "Zambia";
                case 233:
                    return "Zimbabwe";
            }

            return "?";
        }

        public static String LanguageCodeToString(byte code)
        {
            switch (code)
            {
                case 11:
                    return "Arabic";
                case 12:
                    return "Chinese";
                case 14:
                    return "Czech";
                case 15:
                    return "Danish";
                case 16:
                    return "Dutch";
                case 10:
                    return "English";
                case 27:
                    return "Finnish";
                case 28:
                    return "French";
                case 29:
                    return "German";
                case 30:
                    return "Italian";
                case 17:
                    return "Japanese";
                case 19:
                    return "Kirghiz";
                case 20:
                    return "Polish";
                case 21:
                    return "Portuguese";
                case 31:
                    return "Russian";
                case 22:
                    return "Slovak";
                case 23:
                    return "Spanish";
                case 25:
                    return "Swedish";
                case 26:
                    return "Turkish";
                default:
                    return "English";
            }
        }

        public static System.Drawing.Color LevelToColor(byte level, bool black)
        {
            switch (level)
            {
                case 1:
                    return black ? System.Drawing.Color.Aqua : System.Drawing.Color.Blue;

                case 2:
                    return black ? System.Drawing.Color.Lime : System.Drawing.Color.Green;

                case 3:
                    return System.Drawing.Color.Red;

                default:
                    return black ? System.Drawing.Color.White : System.Drawing.Color.Black;
            }
        }

        public static IPAddress RandomIPAddress()
        {
            byte[] buf = new byte[4];

            for (int i = 0; i < 4; i++)
                buf[i] = (byte)rnd.Next(0, 255);

            return new IPAddress(buf);
        }

        public static void ExportHashlink(ChannelObject cObj)
        {
            try
            {
                String[] hash_info = new String[2];
                hash_info[0] = cObj.name;
                hash_info[1] = Hashlink.EncodeHashlink(cObj);

                if (hash_info[1] == null)
                    return;

                hash_info[1] = "arlnk://" + hash_info[1];
                File.WriteAllLines(Settings.folder_path + "hashlink.txt", hash_info, Encoding.UTF8);
                Process.Start("notepad.exe", Settings.folder_path + "hashlink.txt");
            }
            catch { }
        }

        public static String LineType_NumberToString(uint speed)
        {
            switch (speed)
            {
                case 1440:
                    return "14.4K";
                case 3330:
                    return "33.3K";
                case 5600:
                    return "56K";
                case 6400:
                    return "64K";
                case 12800:
                    return "128K";
                case 25000:
                    return "Cable";
                case 35000:
                    return "DSL";
                case 1000000:
                    return "T1";
                case 2500000:
                    return "T3";
                default:
                    return "N/A";
            }
        }

        public static uint LineType_StringToNumber(String speed)
        {
            switch (speed)
            {
                case "14.4K":
                    return 1440;
                case "33.3K":
                    return 3330;
                case "56K":
                    return 5600;
                case "64K":
                    return 6400;
                case "128K":
                    return 12800;
                case "Cable":
                    return 25000;
                case "DSL":
                    return 35000;
                case "T1":
                    return 1000000;
                case "T3":
                    return 2500000;
                default:
                    return 0;
            }
        }

        public static String FormatAresColorCodes(String text)
        {
            String result = text;
            result = result.Replace("\x00023", "\x0003");
            result = result.Replace("\x00025", "\x0005");
            result = result.Replace("\x00026", "\x0006");
            result = result.Replace("\x00027", "\x0007");
            result = result.Replace("\x00029", "\x0009");
            return result;
        }

        public static int FilneItemByteSkip(byte filetype)
        {
            switch (filetype)
            {
                case 1:
                    return 28;
                case 2:
                    return 29;
                case 3:
                    return 24;
                case 5:
                    return 30;
                case 6:
                    return 24;
                case 7:
                    return 29;
            }

            return 24;
        }

        public static ReceivedBrowseItem ParseFileData(byte[] data, byte mime)
        {
            Ares.PacketHandlers.AresDataPacket packet = new Ares.PacketHandlers.AresDataPacket(data);

            if (packet.Remaining() < 27)
                return null;

            ReceivedBrowseItem item = new ReceivedBrowseItem();
            item.Mime = mime;
            item.FileSize = packet.ReadInt32();
            packet.SkipBytes(16); // ignore - pre 2005 file guid, now uses SHA1

            switch (mime) // params
            {
                case 1: // audio
                    item.param1 = packet.ReadInt16(); // bit rate
                    item.param3 = packet.ReadInt16(); // duration
                    break;

                case 5: // video
                    item.param1 = packet.ReadInt16(); // bit rate
                    item.param2 = packet.ReadInt16(); // sample rate
                    item.param3 = packet.ReadInt16(); // duration
                    break;

                case 7: // image
                    item.param1 = packet.ReadInt16(); // width
                    item.param2 = packet.ReadInt16(); // height
                    item.param3 = packet.ReadByte(); // depth
                    break;
            }

            if (packet.Remaining() < 10)
                return null;

            while (packet.Remaining() > 2) // details
            {
                byte length = packet.ReadByte();
                byte type = packet.ReadByte();

                if (length > packet.Remaining())
                    return null;

                switch (type) // could make an object out of these !!
                {
                    case 1: // title
                        item.Title = Encoding.UTF8.GetString(packet.ReadBytes(length));
                        break;

                    case 2: // artist
                        item.Artist = Encoding.UTF8.GetString(packet.ReadBytes(length));
                        break;

                    case 3: // album
                        item.Album = Encoding.UTF8.GetString(packet.ReadBytes(length));
                        break;

                    case 4: // category
                        item.Category = Encoding.UTF8.GetString(packet.ReadBytes(length));
                        break;

                    case 5: // year
                        item.Year = Encoding.UTF8.GetString(packet.ReadBytes(length));
                        break;

                    case 6: // language
                        item.Language = Encoding.UTF8.GetString(packet.ReadBytes(length));
                        break;

                    case 7: // url
                        item.URL = Encoding.UTF8.GetString(packet.ReadBytes(length));
                        break;

                    case 8: // comment
                        item.Comment = Encoding.UTF8.GetString(packet.ReadBytes(length));
                        break;

                    case 9: // genre
                        item.Genre = Encoding.UTF8.GetString(packet.ReadBytes(length));
                        break;

                    case 10: // format
                        item.Format = Encoding.UTF8.GetString(packet.ReadBytes(length));
                        break;

                    case 15: // filename
                        item.FileName = Encoding.UTF8.GetString(packet.ReadBytes(length));
                        break;

                    case 20: // SHA1 hash
                        item.SHA1Hash = packet.ReadBytes(20);
                        break;

                    case 23: // path
                        item.Path = Encoding.UTF8.GetString(packet.ReadBytes(length));
                        break;

                    case 24: // size64
                        item.FileSize = packet.ReadInt64();
                        break;
                }
            }

            if (item.FileName.Length == 0) // malicious or badly formed file data
                return null;

            if (item.Title.Length == 0)
                item.Title = item.FileName;

            item.FileSizeString = item.FileSize > 1024 ? (item.FileSize / 1024).ToString("#,##0") + " KB" : item.FileSize.ToString();
            return item;
        }

        public static String FileItemFormatName(byte[] data)
        {
            List<byte> buf1 = new List<byte>(data);
            int i = buf1.IndexOf(1);

            if (i == -1)
                return null;

            byte[] buf2 = buf1.GetRange(0, i - 1).ToArray();
            return Encoding.UTF8.GetString(buf2);
        }

        public static String FileItemFormatAudioName(byte[] data)
        {
            List<byte> buf1 = new List<byte>(data);
            int i = buf1.IndexOf(1);

            if (i == -1)
                return null;

            byte[] buf4 = buf1.GetRange(0, i - 1).ToArray();
            String filename = Encoding.UTF8.GetString(buf4);

            String result = String.Empty;
            buf1.RemoveRange(0, i + 1);
            i = buf1.IndexOf(2);

            if (i == -1)
                return filename;

            byte[] buf2 = buf1.GetRange(0, i - 1).ToArray();
            buf1.RemoveRange(0, i + 1);
            i = buf1.IndexOf(3);

            if (i == -1)
                return filename;

            int i2 = buf1.IndexOf(4);

            if (i2 > -1 && i2 < i)
                i = i2;

            i2 = buf1.IndexOf(20);

            if (i2 > -1 && i2 < i)
                i = i2;

            byte[] buf3 = buf1.GetRange(0, i - 1).ToArray();

            result += Encoding.UTF8.GetString(buf2);
            result += " - ";
            result += Encoding.UTF8.GetString(buf3);

            return result;
        }

        public static String SecToMin(int i)
        {
            int m = (int)Math.Floor((double)i / 60);
            int s = i - (m * 60);

            String divider = ":";

            if (s < 10)
                divider += "0";

            return m + divider + s;
        }

        public static String ExtractDCFilename(String orgname)
        {
            String fullname = orgname;

            int i = fullname.LastIndexOf("/");

            if (i == -1)
                i = fullname.LastIndexOf("\\");

            if (i == -1)
                return fullname;

            return fullname.Substring(i + 1);
        }

        public static uint UnixTime()
        {
            TimeSpan ts = (DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0));
            return (uint)ts.TotalSeconds;
        }

        public static String DCPresentBytes(ulong so_far, ulong total)
        {
            ulong temp1 = so_far;

            if (temp1 >= 1024)
                temp1 = (ulong)Math.Round((double)temp1 / 1024);

            ulong temp2 = total;

            if (temp2 >= 1024)
                temp2 = (ulong)Math.Round((double)temp2 / 1024);

            return temp1 + "KB/" + temp2 + "KB";
        }

    }
}
