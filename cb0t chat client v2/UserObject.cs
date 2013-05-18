using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using System.IO;

namespace cb0t_chat_client_v2
{
    class UserObject
    {
        public byte level;
        public bool browse;
        public ushort dcPort;
        public IPAddress externalIp;
        public ushort files;
        public bool me = false;
        public IPAddress localIp;
        public String name;
        public IPAddress nodeIp;
        public ushort nodePort;
        public uint speed;
        public bool ignored = false;
        public UserFont font = null;
        public bool can_vc_public = false;
        public bool can_vc_private = false;
        public bool pm_enc = false;

        // custom

        public List<CEmoteItem> custom_emotes = new List<CEmoteItem>();

        // new shit

        public Bitmap avatar;
        public byte[] avatar_data = null;
        public byte age;
        public byte sex;
        public byte country;
        public String countryname;
        public String region;
        public String personal_message;
        public bool is_song = false;
        public bool writing = false;
        public Settings.OnlineStatus status = Settings.OnlineStatus.online;

        public bool told_that_pm_is_disabled = false;
        public bool told_that_they_are_ignored = false;

        public UserObject() // set up new shit
        {
            this.avatar = null;
            this.age = 0;
            this.sex = 0;
            this.country = 0;
            this.region = String.Empty;
            this.personal_message = String.Empty;
            this.countryname = "?";
        }

        public String ToASLString()
        {
            String temp = String.Empty;
            temp += (this.age > 0 ? this.age.ToString() : "?") + "/";

            switch (this.sex)
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

            String _str = this.countryname;

            if (_str != "?")
                if (this.region.Length > 0)
                    _str = this.region + ", " + _str;

            if (_str == "?")
                if (this.region.Length > 0)
                    _str = this.region;

            temp += _str;

            return temp;
        }

        public void UpdateAvatar(byte[] data)
        {
            try
            {
                if (this.avatar != null)
                    this.avatar.Dispose();

                using (MemoryStream ms = new MemoryStream(data))
                {
                    using (Bitmap b = new Bitmap(ms))
                    {
                        this.avatar = new Bitmap(54, 54);
                        this.avatar_data = data;

                        using (Graphics g = Graphics.FromImage(this.avatar))
                        {
                            g.SmoothingMode = SmoothingMode.AntiAlias;
                            g.CompositingQuality = CompositingQuality.HighQuality;
                            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                            g.DrawImage(b, new RectangleF(0, 0, 54, 54));
                        }
                    }
                }
            }
            catch
            {
                if (this.avatar != null)
                    this.avatar.Dispose();

                this.avatar = null;
                this.avatar_data = null;
            }
        }

        public override bool Equals(object obj)
        {
            return this.name == ((UserObject)obj).name;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }

    class UserFont
    {
        public String name;
        public int size;
        public int name_col;
        public int text_col;

        public UserFont(String name, int size, int name_col, int text_col)
        {
            this.name = name;
            this.size = size;
            this.name_col = name_col;
            this.text_col = text_col;
        }
    }
}
