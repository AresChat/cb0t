using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace cb0t
{
    class Emoji_Places : UserControl
    {
        private ToolTip tip { get; set; }

        public void Populate(EventHandler callback)
        {
            this.tip = new ToolTip();

            EmojiMenuShortcutItem[] items = new EmojiMenuShortcutItem[101];
            items[0] = new EmojiMenuShortcutItem { SurrogateSequence = "55356 57312", Shortcut = "🏠", Description = "House Building" };
            items[1] = new EmojiMenuShortcutItem { SurrogateSequence = "55356 57313", Shortcut = "🏡", Description = "House With Garden" };
            items[2] = new EmojiMenuShortcutItem { SurrogateSequence = "55356 57323", Shortcut = "🏫", Description = "School" };
            items[3] = new EmojiMenuShortcutItem { SurrogateSequence = "55356 57314", Shortcut = "🏢", Description = "Office Building" };
            items[4] = new EmojiMenuShortcutItem { SurrogateSequence = "55356 57315", Shortcut = "🏣", Description = "Japanese Post Office" };
            items[5] = new EmojiMenuShortcutItem { SurrogateSequence = "55356 57317", Shortcut = "🏥", Description = "Hospital" };
            items[6] = new EmojiMenuShortcutItem { SurrogateSequence = "55356 57318", Shortcut = "🏦", Description = "Bank" };
            items[7] = new EmojiMenuShortcutItem { SurrogateSequence = "55356 57322", Shortcut = "🏪", Description = "Convenience Store" };
            items[8] = new EmojiMenuShortcutItem { SurrogateSequence = "55356 57321", Shortcut = "🏩", Description = "Love Hotel" };
            items[9] = new EmojiMenuShortcutItem { SurrogateSequence = "55356 57320", Shortcut = "🏨", Description = "Hotel" };
            items[10] = new EmojiMenuShortcutItem { SurrogateSequence = "55357 56466", Shortcut = "💒", Description = "Wedding" };
            items[11] = new EmojiMenuShortcutItem { SurrogateSequence = "9962", Shortcut = "⛪", Description = "Church" };
            items[12] = new EmojiMenuShortcutItem { SurrogateSequence = "55356 57324", Shortcut = "🏬", Description = "Department Store" };
            items[13] = new EmojiMenuShortcutItem { SurrogateSequence = "55356 57316", Shortcut = "🏤", Description = "European Post Office" };
            items[14] = new EmojiMenuShortcutItem { SurrogateSequence = "55356 57095", Shortcut = "🌇", Description = "Sunset Over Buildings" };
            items[15] = new EmojiMenuShortcutItem { SurrogateSequence = "55356 57094", Shortcut = "🌆", Description = "Cityscape At Dusk" };
            items[16] = new EmojiMenuShortcutItem { SurrogateSequence = "55356 57327", Shortcut = "🏯", Description = "Japanese Castle" };
            items[17] = new EmojiMenuShortcutItem { SurrogateSequence = "55356 57328", Shortcut = "🏰", Description = "European Castle" };
            items[18] = new EmojiMenuShortcutItem { SurrogateSequence = "9978", Shortcut = "⛺", Description = "Tent" };
            items[19] = new EmojiMenuShortcutItem { SurrogateSequence = "55356 57325", Shortcut = "🏭", Description = "Factory" };
            items[20] = new EmojiMenuShortcutItem { SurrogateSequence = "55357 56828", Shortcut = "🗼", Description = "Tokyo Tower" };
            items[21] = new EmojiMenuShortcutItem { SurrogateSequence = "55357 56830", Shortcut = "🗾", Description = "Silhouette Of Japan" };
            items[22] = new EmojiMenuShortcutItem { SurrogateSequence = "55357 56827", Shortcut = "🗻", Description = "Mount Fuji" };
            items[23] = new EmojiMenuShortcutItem { SurrogateSequence = "55356 57092", Shortcut = "🌄", Description = "Sunrise Over Mountains" };
            items[24] = new EmojiMenuShortcutItem { SurrogateSequence = "55356 57093", Shortcut = "🌅", Description = "Sunrise" };
            items[25] = new EmojiMenuShortcutItem { SurrogateSequence = "55356 57091", Shortcut = "🌃", Description = "Night With Stars" };
            items[26] = new EmojiMenuShortcutItem { SurrogateSequence = "55357 56829", Shortcut = "🗽", Description = "Statue Of Liberty" };
            items[27] = new EmojiMenuShortcutItem { SurrogateSequence = "55356 57097", Shortcut = "🌉", Description = "Bridge At Night" };
            items[28] = new EmojiMenuShortcutItem { SurrogateSequence = "55356 57248", Shortcut = "🎠", Description = "Carousel Horse" };
            items[29] = new EmojiMenuShortcutItem { SurrogateSequence = "55356 57249", Shortcut = "🎡", Description = "Ferris Wheel" };
            items[30] = new EmojiMenuShortcutItem { SurrogateSequence = "9970", Shortcut = "⛲", Description = "Fountain" };
            items[31] = new EmojiMenuShortcutItem { SurrogateSequence = "55356 57250", Shortcut = "🎢", Description = "Roller Coaster" };
            items[32] = new EmojiMenuShortcutItem { SurrogateSequence = "55357 56994", Shortcut = "🚢", Description = "Ship" };
            items[33] = new EmojiMenuShortcutItem { SurrogateSequence = "9973", Shortcut = "⛵", Description = "Sailboat" };
            items[34] = new EmojiMenuShortcutItem { SurrogateSequence = "55357 56996", Shortcut = "🚤", Description = "Speedboat" };
            items[35] = new EmojiMenuShortcutItem { SurrogateSequence = "55357 56995", Shortcut = "🚣", Description = "Rowboat" };
            items[36] = new EmojiMenuShortcutItem { SurrogateSequence = "9875", Shortcut = "⚓", Description = "Anchor" };
            items[37] = new EmojiMenuShortcutItem { SurrogateSequence = "55357 56960", Shortcut = "🚀", Description = "Rocket" };
            items[38] = new EmojiMenuShortcutItem { SurrogateSequence = "9992", Shortcut = "✈", Description = "Airplane" };
            items[39] = new EmojiMenuShortcutItem { SurrogateSequence = "55357 56506", Shortcut = "💺", Description = "Seat" };
            items[40] = new EmojiMenuShortcutItem { SurrogateSequence = "55357 56961", Shortcut = "🚁", Description = "Helicopter" };
            items[41] = new EmojiMenuShortcutItem { SurrogateSequence = "55357 56962", Shortcut = "🚂", Description = "Steam Locomotive" };
            items[42] = new EmojiMenuShortcutItem { SurrogateSequence = "55357 56970", Shortcut = "🚊", Description = "Tram" };
            items[43] = new EmojiMenuShortcutItem { SurrogateSequence = "55357 56969", Shortcut = "🚉", Description = "Station" };
            items[44] = new EmojiMenuShortcutItem { SurrogateSequence = "55357 56990", Shortcut = "🚞", Description = "Mountain Railway" };
            items[45] = new EmojiMenuShortcutItem { SurrogateSequence = "55357 56966", Shortcut = "🚆", Description = "Train" };
            items[46] = new EmojiMenuShortcutItem { SurrogateSequence = "55357 56964", Shortcut = "🚄", Description = "High-Speed Train" };
            items[47] = new EmojiMenuShortcutItem { SurrogateSequence = "55357 56965", Shortcut = "🚅", Description = "High-Speed Train With Bullet Nose" };
            items[48] = new EmojiMenuShortcutItem { SurrogateSequence = "55357 56968", Shortcut = "🚈", Description = "Light Rail" };
            items[49] = new EmojiMenuShortcutItem { SurrogateSequence = "55357 56967", Shortcut = "🚇", Description = "Metro" };
            items[50] = new EmojiMenuShortcutItem { SurrogateSequence = "55357 56989", Shortcut = "🚝", Description = "Monorail" };
            items[51] = new EmojiMenuShortcutItem { SurrogateSequence = "55357 56971", Shortcut = "🚋", Description = "Tram Car" };
            items[52] = new EmojiMenuShortcutItem { SurrogateSequence = "55357 56963", Shortcut = "🚃", Description = "Railway Car" };
            items[53] = new EmojiMenuShortcutItem { SurrogateSequence = "55357 56974", Shortcut = "🚎", Description = "Trolleybus" };
            items[54] = new EmojiMenuShortcutItem { SurrogateSequence = "55357 56972", Shortcut = "🚌", Description = "Bus" };
            items[55] = new EmojiMenuShortcutItem { SurrogateSequence = "55357 56973", Shortcut = "🚍", Description = "Oncoming Bus" };
            items[56] = new EmojiMenuShortcutItem { SurrogateSequence = "55357 56985", Shortcut = "🚙", Description = "Recreational Vehicle" };
            items[57] = new EmojiMenuShortcutItem { SurrogateSequence = "55357 56984", Shortcut = "🚘", Description = "Oncoming Automobile" };
            items[58] = new EmojiMenuShortcutItem { SurrogateSequence = "55357 56983", Shortcut = "🚗", Description = "Automobile" };
            items[59] = new EmojiMenuShortcutItem { SurrogateSequence = "55357 56981", Shortcut = "🚕", Description = "Taxi" };
            items[60] = new EmojiMenuShortcutItem { SurrogateSequence = "55357 56982", Shortcut = "🚖", Description = "Oncoming Taxi" };
            items[61] = new EmojiMenuShortcutItem { SurrogateSequence = "55357 56987", Shortcut = "🚛", Description = "Articulated Lorry" };
            items[62] = new EmojiMenuShortcutItem { SurrogateSequence = "55357 56986", Shortcut = "🚚", Description = "Delivery Truck" };
            items[63] = new EmojiMenuShortcutItem { SurrogateSequence = "55357 57000", Shortcut = "🚨", Description = "Police Cars Revolving Light" };
            items[64] = new EmojiMenuShortcutItem { SurrogateSequence = "55357 56979", Shortcut = "🚓", Description = "Police Car" };
            items[65] = new EmojiMenuShortcutItem { SurrogateSequence = "55357 56980", Shortcut = "🚔", Description = "Oncoming Police Car" };
            items[66] = new EmojiMenuShortcutItem { SurrogateSequence = "55357 56978", Shortcut = "🚒", Description = "Fire Engine" };
            items[67] = new EmojiMenuShortcutItem { SurrogateSequence = "55357 56977", Shortcut = "🚑", Description = "Ambulance" };
            items[68] = new EmojiMenuShortcutItem { SurrogateSequence = "55357 56976", Shortcut = "🚐", Description = "Minibus" };
            items[69] = new EmojiMenuShortcutItem { SurrogateSequence = "55357 57010", Shortcut = "🚲", Description = "Bicycle  " };
            items[70] = new EmojiMenuShortcutItem { SurrogateSequence = "55357 56993", Shortcut = "🚡", Description = "Aerial Tramway" };
            items[71] = new EmojiMenuShortcutItem { SurrogateSequence = "55357 56991", Shortcut = "🚟", Description = "Suspension Railway" };
            items[72] = new EmojiMenuShortcutItem { SurrogateSequence = "55357 56992", Shortcut = "🚠", Description = "Mountain Cableway" };
            items[73] = new EmojiMenuShortcutItem { SurrogateSequence = "55357 56988", Shortcut = "🚜", Description = "Tractor" };
            items[74] = new EmojiMenuShortcutItem { SurrogateSequence = "55357 56456", Shortcut = "💈", Description = "Barber Pole" };
            items[75] = new EmojiMenuShortcutItem { SurrogateSequence = "55357 56975", Shortcut = "🚏", Description = "Bus Stop" };
            items[76] = new EmojiMenuShortcutItem { SurrogateSequence = "55356 57259", Shortcut = "🎫", Description = "Ticket" };
            items[77] = new EmojiMenuShortcutItem { SurrogateSequence = "55357 56998", Shortcut = "🚦", Description = "Vertical Traffic Light" };
            items[78] = new EmojiMenuShortcutItem { SurrogateSequence = "55357 56997", Shortcut = "🚥", Description = "Horizontal Traffic Light" };
            items[79] = new EmojiMenuShortcutItem { SurrogateSequence = "9888", Shortcut = "⚠", Description = "Warning Sign" };
            items[80] = new EmojiMenuShortcutItem { SurrogateSequence = "55357 56999", Shortcut = "🚧", Description = "Construction Sign" };
            items[81] = new EmojiMenuShortcutItem { SurrogateSequence = "55357 56624", Shortcut = "🔰", Description = "Japanese Symbol For Beginner" };
            items[82] = new EmojiMenuShortcutItem { SurrogateSequence = "9981", Shortcut = "⛽", Description = "Fuel Pump" };
            items[83] = new EmojiMenuShortcutItem { SurrogateSequence = "55356 57326", Shortcut = "🏮", Description = "Izakaya Lantern" };
            items[84] = new EmojiMenuShortcutItem { SurrogateSequence = "55356 57264", Shortcut = "🎰", Description = "Slot Machine" };
            items[85] = new EmojiMenuShortcutItem { SurrogateSequence = "9832", Shortcut = "♨", Description = "Hot Springs" };
            items[86] = new EmojiMenuShortcutItem { SurrogateSequence = "55357 56831", Shortcut = "🗿", Description = "Moyai" };
            items[87] = new EmojiMenuShortcutItem { SurrogateSequence = "55356 57258", Shortcut = "🎪", Description = "Circus Tent" };
            items[88] = new EmojiMenuShortcutItem { SurrogateSequence = "55356 57261", Shortcut = "🎭", Description = "Performing Arts" };
            items[89] = new EmojiMenuShortcutItem { SurrogateSequence = "55357 56525", Shortcut = "📍", Description = "Round Pushpin" };
            items[90] = new EmojiMenuShortcutItem { SurrogateSequence = "55357 57001", Shortcut = "🚩", Description = "Triangular Flag On Post" };
            items[91] = new EmojiMenuShortcutItem { SurrogateSequence = "55356 56815 55356 56821", Shortcut = "🇯🇵", Description = "Flag For Japan" };
            items[92] = new EmojiMenuShortcutItem { SurrogateSequence = "55356 56816 55356 56823", Shortcut = "🇰🇷", Description = "Flag For South Korea" };
            items[93] = new EmojiMenuShortcutItem { SurrogateSequence = "55356 56809 55356 56810", Shortcut = "🇩🇪", Description = "Flag For Germany" };
            items[94] = new EmojiMenuShortcutItem { SurrogateSequence = "55356 56808 55356 56819", Shortcut = "🇨🇳", Description = "Flag For China" };
            items[95] = new EmojiMenuShortcutItem { SurrogateSequence = "55356 56826 55356 56824", Shortcut = "🇺🇸", Description = "Flag For USA" };
            items[96] = new EmojiMenuShortcutItem { SurrogateSequence = "55356 56811 55356 56823", Shortcut = "🇫🇷", Description = "Flag For France" };
            items[97] = new EmojiMenuShortcutItem { SurrogateSequence = "55356 56810 55356 56824", Shortcut = "🇪🇸", Description = "Flag For Spain" };
            items[98] = new EmojiMenuShortcutItem { SurrogateSequence = "55356 56814 55356 56825", Shortcut = "🇮🇹", Description = "Flag For Italy" };
            items[99] = new EmojiMenuShortcutItem { SurrogateSequence = "55356 56823 55356 56826", Shortcut = "🇷🇺", Description = "Flag For Russia" };
            items[100] = new EmojiMenuShortcutItem { SurrogateSequence = "55356 56812 55356 56807", Shortcut = "🇬🇧", Description = "Flag For Great Britain" };

            for (int i = 0; i < items.Length; i++)
            {
                PictureBox pic = new PictureBox();
                pic.BackColor = Color.White;
                pic.Size = new Size(24, 24);
                int per_line = 8;
                pic.Location = new Point(1 + ((i % per_line) * 24) + (i % per_line), 1 + ((i / per_line) * 24) + (i / per_line));
                pic.Cursor = Cursors.Hand;
                pic.Tag = items[i];
                pic.MouseHover += this.pic_MouseHover;
                pic.Click += callback;
                EmojiItem item = Emoji.EmojiFromSurrogate(items[i].SurrogateSequence);
                pic.ImageLocation = Path.Combine(Settings.AppPath, "emoji", "at24", item.FileName);
                pic.SizeMode = PictureBoxSizeMode.CenterImage;
                this.Controls.Add(pic);
            }
        }

        private void pic_MouseHover(object sender, EventArgs e)
        {
            PictureBox pb = (PictureBox)sender;
            this.tip.SetToolTip(pb, ((EmojiMenuShortcutItem)pb.Tag).Description);
        }
    }
}
