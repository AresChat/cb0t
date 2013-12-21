using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Drawing.Imaging;

namespace cb0t
{
    public class User
    {
        public String Name { get; set; }
        public IPAddress ExternalIP { get; set; }
        public IPAddress LocalIP { get; set; }
        public ushort Port { get; set; }
        public String PersonalMessage { get; set; }
        public byte Level { get; set; }
        public byte Age { get; set; }
        public byte Gender { get; set; }
        public String Region { get; set; }
        public String Country { get; set; }
        public bool HasFiles { get; set; }
        public bool IsFriend { get; set; }
        public uint Ident { get; set; }
        public bool Writing { get; set; }
        public bool Ignored { get; set; }
        public bool IsAway { get; set; }
        public List<byte> ScribbleBuffer { get; set; }
        public AresFont Font { get; set; }
        public bool SupportsPMEnc { get; set; }
        public bool SupportsVC { get; set; }
        public bool SupportsOpusVC { get; set; }
        public byte[] AvatarBytes { get; set; }

        public User()
        {
            this.PersonalMessage = String.Empty;
            this.Ident = Helpers.UserIdent++;
            this.ScribbleBuffer = new List<byte>();
            this.AvatarBytes = new byte[] { };
        }

        public void Dispose()
        {
            this.ScribbleBuffer.Clear();
            this.ScribbleBuffer = new List<byte>();
            this.AvatarBytes = new byte[] { };
        }

        public void SetAvatar(byte[] data)
        {
            try
            {
                using (MemoryStream ms = new MemoryStream(data))
                using (Bitmap org = new Bitmap(ms))
                using (Bitmap sized = new Bitmap(53, 53))
                using (Graphics sized_g = Graphics.FromImage(sized))
                {
                    sized_g.DrawImage(org, new Rectangle(0, 0, 53, 53), new Rectangle(0, 0, 48, 48), GraphicsUnit.Pixel);

                    using (Bitmap Av = new Bitmap(53, 53))
                    using (Graphics av_g = Graphics.FromImage(Av))
                    using (GraphicsPath path = new Rectangle(0, 0, 52, 52).Rounded(8))
                    using (TextureBrush brush = new TextureBrush(sized))
                    {
                        av_g.SmoothingMode = SmoothingMode.HighQuality;
                        av_g.CompositingQuality = CompositingQuality.HighQuality;

                        using (SolidBrush sb = new SolidBrush(Color.White))
                            av_g.FillPath(sb, path);

                        av_g.FillPath(brush, path);

                        using (Pen pen = new Pen(Color.Gainsboro, 1))
                            av_g.DrawPath(pen, path);

                        using (MemoryStream save_stream = new MemoryStream())
                        {
                            Av.Save(save_stream, ImageFormat.Png);
                            this.AvatarBytes = save_stream.ToArray();
                        }
                    }
                }
            }
            catch { this.AvatarBytes = new byte[] { }; }
        }

        public void ClearAvatar()
        {
            this.AvatarBytes = new byte[] { };
        }

        public String ToASLString()
        {
            String temp = String.Empty;
            temp += (this.Age > 0 ? this.Age.ToString() : "?") + "/";

            switch (this.Gender)
            {
                case 0:
                    temp += "?";
                    break;

                case 1:
                    temp += "m";
                    break;

                case 2:
                    temp += "f";
                    break;
            }

            temp += "/";

            String _str = this.Country;

            if (_str != "?")
                if (!String.IsNullOrEmpty(this.Region))
                    _str = this.Region + ", " + _str;

            if (_str == "?")
                if (!String.IsNullOrEmpty(this.Region))
                    _str = this.Region;

            temp += _str;

            return temp;
        }
    }
}
