using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace cb0t
{
    class Emoji_Nature : UserControl
    {
        private ToolTip tip { get; set; }

        public void Populate(EventHandler callback)
        {
            this.tip = new ToolTip();

            EmojiMenuShortcutItem[] items = new EmojiMenuShortcutItem[116];
            items[0] = new EmojiMenuShortcutItem { SurrogateSequence = "55357 56374", Shortcut = "🐶", Description = "Dog Face" };
            items[1] = new EmojiMenuShortcutItem { SurrogateSequence = "55357 56378", Shortcut = "🐺", Description = "Wolf Face" };
            items[2] = new EmojiMenuShortcutItem { SurrogateSequence = "55357 56369", Shortcut = "🐱", Description = "Cat Face" };
            items[3] = new EmojiMenuShortcutItem { SurrogateSequence = "55357 56365", Shortcut = "🐭", Description = "Mouse Face" };
            items[4] = new EmojiMenuShortcutItem { SurrogateSequence = "55357 56377", Shortcut = "🐹", Description = "Hamster Face" };
            items[5] = new EmojiMenuShortcutItem { SurrogateSequence = "55357 56368", Shortcut = "🐰", Description = "Rabbit Face" };
            items[6] = new EmojiMenuShortcutItem { SurrogateSequence = "55357 56376", Shortcut = "🐸", Description = "Frog Face" };
            items[7] = new EmojiMenuShortcutItem { SurrogateSequence = "55357 56367", Shortcut = "🐯", Description = "Tiger Face" };
            items[8] = new EmojiMenuShortcutItem { SurrogateSequence = "55357 56360", Shortcut = "🐨", Description = "Koala" };
            items[9] = new EmojiMenuShortcutItem { SurrogateSequence = "55357 56379", Shortcut = "🐻", Description = "Bear Face" };
            items[10] = new EmojiMenuShortcutItem { SurrogateSequence = "55357 56375", Shortcut = "🐷", Description = "Pig Face" };
            items[11] = new EmojiMenuShortcutItem { SurrogateSequence = "55357 56381", Shortcut = "🐽", Description = "Pig Nose" };
            items[12] = new EmojiMenuShortcutItem { SurrogateSequence = "55357 56366", Shortcut = "🐮", Description = "Cow Face" };
            items[13] = new EmojiMenuShortcutItem { SurrogateSequence = "55357 56343", Shortcut = "🐗", Description = "Boar" };
            items[14] = new EmojiMenuShortcutItem { SurrogateSequence = "55357 56373", Shortcut = "🐵", Description = "Monkey Face" };
            items[15] = new EmojiMenuShortcutItem { SurrogateSequence = "55357 56338", Shortcut = "🐒", Description = "Monkey" };
            items[16] = new EmojiMenuShortcutItem { SurrogateSequence = "55357 56372", Shortcut = "🐴", Description = "Horse Face" };
            items[17] = new EmojiMenuShortcutItem { SurrogateSequence = "55357 56337", Shortcut = "🐑", Description = "Sheep" };
            items[18] = new EmojiMenuShortcutItem { SurrogateSequence = "55357 56344", Shortcut = "🐘", Description = "Elephant" };
            items[19] = new EmojiMenuShortcutItem { SurrogateSequence = "55357 56380", Shortcut = "🐼", Description = "Panda Face" };
            items[20] = new EmojiMenuShortcutItem { SurrogateSequence = "55357 56359", Shortcut = "🐧", Description = "Penguin" };
            items[21] = new EmojiMenuShortcutItem { SurrogateSequence = "55357 56358", Shortcut = "🐦", Description = "Bird" };
            items[22] = new EmojiMenuShortcutItem { SurrogateSequence = "55357 56356", Shortcut = "🐤", Description = "Baby Chick" };
            items[23] = new EmojiMenuShortcutItem { SurrogateSequence = "55357 56357", Shortcut = "🐥", Description = "Front-Facing Baby Chick" };
            items[24] = new EmojiMenuShortcutItem { SurrogateSequence = "55357 56355", Shortcut = "🐣", Description = "Hatching Chick" };
            items[25] = new EmojiMenuShortcutItem { SurrogateSequence = "55357 56340", Shortcut = "🐔", Description = "Chicken" };
            items[26] = new EmojiMenuShortcutItem { SurrogateSequence = "55357 56333", Shortcut = "🐍", Description = "Snake" };
            items[27] = new EmojiMenuShortcutItem { SurrogateSequence = "55357 56354", Shortcut = "🐢", Description = "Turtle" };
            items[28] = new EmojiMenuShortcutItem { SurrogateSequence = "55357 56347", Shortcut = "🐛", Description = "Bug" };
            items[29] = new EmojiMenuShortcutItem { SurrogateSequence = "55357 56349", Shortcut = "🐝", Description = "Honeybee" };
            items[30] = new EmojiMenuShortcutItem { SurrogateSequence = "55357 56348", Shortcut = "🐜", Description = "Ant" };
            items[31] = new EmojiMenuShortcutItem { SurrogateSequence = "55357 56350", Shortcut = "🐞", Description = "Lady Beetle" };
            items[32] = new EmojiMenuShortcutItem { SurrogateSequence = "55357 56332", Shortcut = "🐌", Description = "Snail" };
            items[33] = new EmojiMenuShortcutItem { SurrogateSequence = "55357 56345", Shortcut = "🐙", Description = "Octopus" };
            items[34] = new EmojiMenuShortcutItem { SurrogateSequence = "55357 56346", Shortcut = "🐚", Description = "Spiral Shell" };
            items[35] = new EmojiMenuShortcutItem { SurrogateSequence = "55357 56352", Shortcut = "🐠", Description = "Tropical Fish" };
            items[36] = new EmojiMenuShortcutItem { SurrogateSequence = "55357 56351", Shortcut = "🐟", Description = "Fish" };
            items[37] = new EmojiMenuShortcutItem { SurrogateSequence = "55357 56364", Shortcut = "🐬", Description = "Dolphin" };
            items[38] = new EmojiMenuShortcutItem { SurrogateSequence = "55357 56371", Shortcut = "🐳", Description = "Spouting Whale" };
            items[39] = new EmojiMenuShortcutItem { SurrogateSequence = "55357 56331", Shortcut = "🐋", Description = "Whale" };
            items[40] = new EmojiMenuShortcutItem { SurrogateSequence = "55357 56324", Shortcut = "🐄", Description = "Cow" };
            items[41] = new EmojiMenuShortcutItem { SurrogateSequence = "55357 56335", Shortcut = "🐏", Description = "Ram" };
            items[42] = new EmojiMenuShortcutItem { SurrogateSequence = "55357 56320", Shortcut = "🐀", Description = "Rat" };
            items[43] = new EmojiMenuShortcutItem { SurrogateSequence = "55357 56323", Shortcut = "🐃", Description = "Water Buffalo" };
            items[44] = new EmojiMenuShortcutItem { SurrogateSequence = "55357 56325", Shortcut = "🐅", Description = "Tiger" };
            items[45] = new EmojiMenuShortcutItem { SurrogateSequence = "55357 56327", Shortcut = "🐇", Description = "Rabbit" };
            items[46] = new EmojiMenuShortcutItem { SurrogateSequence = "55357 56329", Shortcut = "🐉", Description = "Dragon" };
            items[47] = new EmojiMenuShortcutItem { SurrogateSequence = "55357 56334", Shortcut = "🐎", Description = "Horse" };
            items[48] = new EmojiMenuShortcutItem { SurrogateSequence = "55357 56336", Shortcut = "🐐", Description = "Goat" };
            items[49] = new EmojiMenuShortcutItem { SurrogateSequence = "55357 56339", Shortcut = "🐓", Description = "Rooster" };
            items[50] = new EmojiMenuShortcutItem { SurrogateSequence = "55357 56341", Shortcut = "🐕", Description = "Dog" };
            items[51] = new EmojiMenuShortcutItem { SurrogateSequence = "55357 56342", Shortcut = "🐖", Description = "Pig" };
            items[52] = new EmojiMenuShortcutItem { SurrogateSequence = "55357 56321", Shortcut = "🐁", Description = "Mouse" };
            items[53] = new EmojiMenuShortcutItem { SurrogateSequence = "55357 56322", Shortcut = "🐂", Description = "Ox" };
            items[54] = new EmojiMenuShortcutItem { SurrogateSequence = "55357 56370", Shortcut = "🐲", Description = "Dragon Face" };
            items[55] = new EmojiMenuShortcutItem { SurrogateSequence = "55357 56353", Shortcut = "🐡", Description = "Blowfish" };
            items[56] = new EmojiMenuShortcutItem { SurrogateSequence = "55357 56330", Shortcut = "🐊", Description = "Crocodile" };
            items[57] = new EmojiMenuShortcutItem { SurrogateSequence = "55357 56363", Shortcut = "🐫", Description = "Bactrian Camel" };
            items[58] = new EmojiMenuShortcutItem { SurrogateSequence = "55357 56362", Shortcut = "🐪", Description = "Dromedary Camel" };
            items[59] = new EmojiMenuShortcutItem { SurrogateSequence = "55357 56326", Shortcut = "🐆", Description = "Leopard" };
            items[60] = new EmojiMenuShortcutItem { SurrogateSequence = "55357 56328", Shortcut = "🐈", Description = "Cat" };
            items[61] = new EmojiMenuShortcutItem { SurrogateSequence = "55357 56361", Shortcut = "🐩", Description = "Poodle" };
            items[62] = new EmojiMenuShortcutItem { SurrogateSequence = "55357 56382", Shortcut = "🐾", Description = "Paw Prints" };
            items[63] = new EmojiMenuShortcutItem { SurrogateSequence = "55357 56464", Shortcut = "💐", Description = "Bouquet" };
            items[64] = new EmojiMenuShortcutItem { SurrogateSequence = "55356 57144", Shortcut = "🌸", Description = "Cherry Blossom" };
            items[65] = new EmojiMenuShortcutItem { SurrogateSequence = "55356 57143", Shortcut = "🌷", Description = "Tulip" };
            items[66] = new EmojiMenuShortcutItem { SurrogateSequence = "55356 57152", Shortcut = "🍀", Description = "Four Leaf Clover" };
            items[67] = new EmojiMenuShortcutItem { SurrogateSequence = "55356 57145", Shortcut = "🌹", Description = "Rose" };
            items[68] = new EmojiMenuShortcutItem { SurrogateSequence = "55356 57147", Shortcut = "🌻", Description = "Sunflower" };
            items[69] = new EmojiMenuShortcutItem { SurrogateSequence = "55356 57146", Shortcut = "🌺", Description = "Hibiscus" };
            items[70] = new EmojiMenuShortcutItem { SurrogateSequence = "55356 57153", Shortcut = "🍁", Description = "Maple Leaf" };
            items[71] = new EmojiMenuShortcutItem { SurrogateSequence = "55356 57155", Shortcut = "🍃", Description = "Leaf Fluttering In Wind" };
            items[72] = new EmojiMenuShortcutItem { SurrogateSequence = "55356 57154", Shortcut = "🍂", Description = "Fallen Leaf" };
            items[73] = new EmojiMenuShortcutItem { SurrogateSequence = "55356 57151", Shortcut = "🌿", Description = "Herb" };
            items[74] = new EmojiMenuShortcutItem { SurrogateSequence = "55356 57150", Shortcut = "🌾", Description = "Ear Of Rice" };
            items[75] = new EmojiMenuShortcutItem { SurrogateSequence = "55356 57156", Shortcut = "🍄", Description = "Mushroom" };
            items[76] = new EmojiMenuShortcutItem { SurrogateSequence = "55356 57141", Shortcut = "🌵", Description = "Cactus" };
            items[77] = new EmojiMenuShortcutItem { SurrogateSequence = "55356 57140", Shortcut = "🌴", Description = "Palm Tree" };
            items[78] = new EmojiMenuShortcutItem { SurrogateSequence = "55356 57138", Shortcut = "🌲", Description = "Evergreen Tree" };
            items[79] = new EmojiMenuShortcutItem { SurrogateSequence = "55356 57139", Shortcut = "🌳", Description = "Deciduous Tree" };
            items[80] = new EmojiMenuShortcutItem { SurrogateSequence = "55356 57136", Shortcut = "🌰", Description = "Chestnut" };
            items[81] = new EmojiMenuShortcutItem { SurrogateSequence = "55356 57137", Shortcut = "🌱", Description = "Seedling" };
            items[82] = new EmojiMenuShortcutItem { SurrogateSequence = "55356 57148", Shortcut = "🌼", Description = "Blossom  " };
            items[83] = new EmojiMenuShortcutItem { SurrogateSequence = "55356 57104", Shortcut = "🌐", Description = "Globe With Meridians" };
            items[84] = new EmojiMenuShortcutItem { SurrogateSequence = "55356 57118", Shortcut = "🌞", Description = "Sun With Face" };
            items[85] = new EmojiMenuShortcutItem { SurrogateSequence = "55356 57117", Shortcut = "🌝", Description = "Full Moon With Face" };
            items[86] = new EmojiMenuShortcutItem { SurrogateSequence = "55356 57114", Shortcut = "🌚", Description = "New Moon With Face" };
            items[87] = new EmojiMenuShortcutItem { SurrogateSequence = "55356 57105", Shortcut = "🌑", Description = "New Moon Symbol" };
            items[88] = new EmojiMenuShortcutItem { SurrogateSequence = "55356 57106", Shortcut = "🌒", Description = "Waxing Crescent Moon Symbol" };
            items[89] = new EmojiMenuShortcutItem { SurrogateSequence = "55356 57107", Shortcut = "🌓", Description = "First Quarter Moon Symbol" };
            items[90] = new EmojiMenuShortcutItem { SurrogateSequence = "55356 57108", Shortcut = "🌔", Description = "Waxing Gibbous Moon Symbol" };
            items[91] = new EmojiMenuShortcutItem { SurrogateSequence = "55356 57109", Shortcut = "🌕", Description = "Full Moon Symbol" };
            items[92] = new EmojiMenuShortcutItem { SurrogateSequence = "55356 57110", Shortcut = "🌖", Description = "Waning Gibbous Moon Symbol" };
            items[93] = new EmojiMenuShortcutItem { SurrogateSequence = "55356 57111", Shortcut = "🌗", Description = "Last Quarter Moon Symbol" };
            items[94] = new EmojiMenuShortcutItem { SurrogateSequence = "55356 57112", Shortcut = "🌘", Description = "Waning Crescent Moon Symbol" };
            items[95] = new EmojiMenuShortcutItem { SurrogateSequence = "55356 57116", Shortcut = "🌜", Description = "Last Quarter Moon With Face" };
            items[96] = new EmojiMenuShortcutItem { SurrogateSequence = "55356 57115", Shortcut = "🌛", Description = "First Quarter Moon With Face" };
            items[97] = new EmojiMenuShortcutItem { SurrogateSequence = "55356 57113", Shortcut = "🌙", Description = "Crescent Moon" };
            items[98] = new EmojiMenuShortcutItem { SurrogateSequence = "55356 57101", Shortcut = "🌍", Description = "Earth Globe Europe-Africa" };
            items[99] = new EmojiMenuShortcutItem { SurrogateSequence = "55356 57102", Shortcut = "🌎", Description = "Earth Globe Americas" };
            items[100] = new EmojiMenuShortcutItem { SurrogateSequence = "55356 57103", Shortcut = "🌏", Description = "Earth Globe Asia-Australia" };
            items[101] = new EmojiMenuShortcutItem { SurrogateSequence = "55356 57099", Shortcut = "🌋", Description = "Volcano" };
            items[102] = new EmojiMenuShortcutItem { SurrogateSequence = "55356 57100", Shortcut = "🌌", Description = "Milky Way" };
            items[103] = new EmojiMenuShortcutItem { SurrogateSequence = "55356 57120", Shortcut = "🌠", Description = "Shooting Star" };
            items[104] = new EmojiMenuShortcutItem { SurrogateSequence = "11088", Shortcut = "⭐", Description = "White Medium Star" };
            items[105] = new EmojiMenuShortcutItem { SurrogateSequence = "9728", Shortcut = "☀", Description = "Black Sun With Rays" };
            items[106] = new EmojiMenuShortcutItem { SurrogateSequence = "9925", Shortcut = "⛅", Description = "Sun Behind Cloud" };
            items[107] = new EmojiMenuShortcutItem { SurrogateSequence = "9729", Shortcut = "☁", Description = "Cloud" };
            items[108] = new EmojiMenuShortcutItem { SurrogateSequence = "9889", Shortcut = "⚡", Description = "High Voltage Sign" };
            items[109] = new EmojiMenuShortcutItem { SurrogateSequence = "9748", Shortcut = "☔", Description = "Umbrella With Rain Drops" };
            items[110] = new EmojiMenuShortcutItem { SurrogateSequence = "10052", Shortcut = "❄", Description = "Snowflake" };
            items[111] = new EmojiMenuShortcutItem { SurrogateSequence = "9924", Shortcut = "⛄", Description = "Snowman Without Snow" };
            items[112] = new EmojiMenuShortcutItem { SurrogateSequence = "55356 57088", Shortcut = "🌀", Description = "Cyclone" };
            items[113] = new EmojiMenuShortcutItem { SurrogateSequence = "55356 57089", Shortcut = "🌁", Description = "Foggy" };
            items[114] = new EmojiMenuShortcutItem { SurrogateSequence = "55356 57096", Shortcut = "🌈", Description = "Rainbow" };
            items[115] = new EmojiMenuShortcutItem { SurrogateSequence = "55356 57098", Shortcut = "🌊", Description = "Water Wave" };

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
