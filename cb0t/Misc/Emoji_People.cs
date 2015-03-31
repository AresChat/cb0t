﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace cb0t
{
    class Emoji_People : UserControl
    {

        private ToolTip tip { get; set; }

        public void Populate(EventHandler callback)
        {
            this.tip = new ToolTip();

            EmojiMenuShortcutItem[] items = new EmojiMenuShortcutItem[189];
            items[0] = new EmojiMenuShortcutItem { SurrogateSequence = "55357 56836", Shortcut = "😄", Description = "Smiling Face With Open Mouth And Smiling Eyes" };
            items[1] = new EmojiMenuShortcutItem { SurrogateSequence = "55357 56835", Shortcut = "😃", Description = "Smiling Face With Open Mouth" };
            items[2] = new EmojiMenuShortcutItem { SurrogateSequence = "55357 56832", Shortcut = "😀", Description = "Grinning Face" };
            items[3] = new EmojiMenuShortcutItem { SurrogateSequence = "55357 56842", Shortcut = "😊", Description = "Smiling Face With Smiling Eyes" };
            items[4] = new EmojiMenuShortcutItem { SurrogateSequence = "9786", Shortcut = "☺", Description = "White Smiling Face" };
            items[5] = new EmojiMenuShortcutItem { SurrogateSequence = "55357 56841", Shortcut = "😉", Description = "Winking Face" };
            items[6] = new EmojiMenuShortcutItem { SurrogateSequence = "55357 56845", Shortcut = "😍", Description = "Smiling Face With Heart-Shaped Eyes" };
            items[7] = new EmojiMenuShortcutItem { SurrogateSequence = "55357 56856", Shortcut = "😘", Description = "Face Throwing A Kiss" };
            items[8] = new EmojiMenuShortcutItem { SurrogateSequence = "55357 56858", Shortcut = "😚", Description = "Kissing Face With Closed Eyes" };
            items[9] = new EmojiMenuShortcutItem { SurrogateSequence = "55357 56855", Shortcut = "😗", Description = "Kissing Face" };
            items[10] = new EmojiMenuShortcutItem { SurrogateSequence = "55357 56857", Shortcut = "😙", Description = "Kissing Face With Smiling Eyes" };
            items[11] = new EmojiMenuShortcutItem { SurrogateSequence = "55357 56860", Shortcut = "😜", Description = "Face With Stuck-Out Tongue And Winking Eye" };
            items[12] = new EmojiMenuShortcutItem { SurrogateSequence = "55357 56861", Shortcut = "😝", Description = "Face With Stuck-Out Tongue And Tightly-Closed Eyes" };
            items[13] = new EmojiMenuShortcutItem { SurrogateSequence = "55357 56859", Shortcut = "😛", Description = "Face With Stuck-Out Tongue" };
            items[14] = new EmojiMenuShortcutItem { SurrogateSequence = "55357 56883", Shortcut = "😳", Description = "Flushed Face" };
            items[15] = new EmojiMenuShortcutItem { SurrogateSequence = "55357 56833", Shortcut = "😁", Description = "Grinning Face With Smiling Eyes" };
            items[16] = new EmojiMenuShortcutItem { SurrogateSequence = "55357 56852", Shortcut = "😔", Description = "Pensive Face" };
            items[17] = new EmojiMenuShortcutItem { SurrogateSequence = "55357 56844", Shortcut = "😌", Description = "Relieved Face" };
            items[18] = new EmojiMenuShortcutItem { SurrogateSequence = "55357 56850", Shortcut = "😒", Description = "Unamused Face" };
            items[19] = new EmojiMenuShortcutItem { SurrogateSequence = "55357 56862", Shortcut = "😞", Description = "Disappointed Face" };
            items[20] = new EmojiMenuShortcutItem { SurrogateSequence = "55357 56867", Shortcut = "😣", Description = "Persevering Face" };
            items[21] = new EmojiMenuShortcutItem { SurrogateSequence = "55357 56866", Shortcut = "😢", Description = "Crying Face" };
            items[22] = new EmojiMenuShortcutItem { SurrogateSequence = "55357 56834", Shortcut = "😂", Description = "Face With Tears Of Joy" };
            items[23] = new EmojiMenuShortcutItem { SurrogateSequence = "55357 56877", Shortcut = "😭", Description = "Loudly Crying Face" };
            items[24] = new EmojiMenuShortcutItem { SurrogateSequence = "55357 56874", Shortcut = "😪", Description = "Sleepy Face" };
            items[25] = new EmojiMenuShortcutItem { SurrogateSequence = "55357 56869", Shortcut = "😥", Description = "Disappointed But Relieved Face" };
            items[26] = new EmojiMenuShortcutItem { SurrogateSequence = "55357 56880", Shortcut = "😰", Description = "Face With Open Mouth And Cold Sweat" };
            items[27] = new EmojiMenuShortcutItem { SurrogateSequence = "55357 56837", Shortcut = "😅", Description = "Smiling Face With Open Mouth And Cold Sweat" };
            items[28] = new EmojiMenuShortcutItem { SurrogateSequence = "55357 56851", Shortcut = "😓", Description = "Face With Cold Sweat" };
            items[29] = new EmojiMenuShortcutItem { SurrogateSequence = "55357 56873", Shortcut = "😩", Description = "Weary Face" };
            items[30] = new EmojiMenuShortcutItem { SurrogateSequence = "55357 56875", Shortcut = "😫", Description = "Tired Face" };
            items[31] = new EmojiMenuShortcutItem { SurrogateSequence = "55357 56872", Shortcut = "😨", Description = "Fearful Face" };
            items[32] = new EmojiMenuShortcutItem { SurrogateSequence = "55357 56881", Shortcut = "😱", Description = "Face Screaming In Fear" };
            items[33] = new EmojiMenuShortcutItem { SurrogateSequence = "55357 56864", Shortcut = "😠", Description = "Angry Face" };
            items[34] = new EmojiMenuShortcutItem { SurrogateSequence = "55357 56865", Shortcut = "😡", Description = "Pouting Face" };
            items[35] = new EmojiMenuShortcutItem { SurrogateSequence = "55357 56868", Shortcut = "😤", Description = "Face With Look Of Triumph" };
            items[36] = new EmojiMenuShortcutItem { SurrogateSequence = "55357 56854", Shortcut = "😖", Description = "Confounded Face" };
            items[37] = new EmojiMenuShortcutItem { SurrogateSequence = "55357 56838", Shortcut = "😆", Description = "Smiling Face With Open Mouth And Tightly-Closed Eyes" };
            items[38] = new EmojiMenuShortcutItem { SurrogateSequence = "55357 56843", Shortcut = "😋", Description = "Face Savouring Delicious Food" };
            items[39] = new EmojiMenuShortcutItem { SurrogateSequence = "55357 56887", Shortcut = "😷", Description = "Face With Medical Mask" };
            items[40] = new EmojiMenuShortcutItem { SurrogateSequence = "55357 56846", Shortcut = "😎", Description = "Smiling Face With Sunglasses" };
            items[41] = new EmojiMenuShortcutItem { SurrogateSequence = "55357 56884", Shortcut = "😴", Description = "Sleeping Face" };
            items[42] = new EmojiMenuShortcutItem { SurrogateSequence = "55357 56885", Shortcut = "😵", Description = "Dizzy Face" };
            items[43] = new EmojiMenuShortcutItem { SurrogateSequence = "55357 56882", Shortcut = "😲", Description = "Astonished Face" };
            items[44] = new EmojiMenuShortcutItem { SurrogateSequence = "55357 56863", Shortcut = "😟", Description = "Worried Face" };
            items[45] = new EmojiMenuShortcutItem { SurrogateSequence = "55357 56870", Shortcut = "😦", Description = "Frowning Face With Open Mouth" };
            items[46] = new EmojiMenuShortcutItem { SurrogateSequence = "55357 56871", Shortcut = "😧", Description = "Anguished Face" };
            items[47] = new EmojiMenuShortcutItem { SurrogateSequence = "55357 56840", Shortcut = "😈", Description = "Smiling Face With Horns" };
            items[48] = new EmojiMenuShortcutItem { SurrogateSequence = "55357 56447", Shortcut = "👿", Description = "Imp" };
            items[49] = new EmojiMenuShortcutItem { SurrogateSequence = "55357 56878", Shortcut = "😮", Description = "Face With Open Mouth" };
            items[50] = new EmojiMenuShortcutItem { SurrogateSequence = "55357 56876", Shortcut = "😬", Description = "Grimacing Face" };
            items[51] = new EmojiMenuShortcutItem { SurrogateSequence = "55357 56848", Shortcut = "😐", Description = "Neutral Face" };
            items[52] = new EmojiMenuShortcutItem { SurrogateSequence = "55357 56853", Shortcut = "😕", Description = "Confused Face" };
            items[53] = new EmojiMenuShortcutItem { SurrogateSequence = "55357 56879", Shortcut = "😯", Description = "Hushed Face" };
            items[54] = new EmojiMenuShortcutItem { SurrogateSequence = "55357 56886", Shortcut = "😶", Description = "Face Without Mouth" };
            items[55] = new EmojiMenuShortcutItem { SurrogateSequence = "55357 56839", Shortcut = "😇", Description = "Smiling Face With Halo" };
            items[56] = new EmojiMenuShortcutItem { SurrogateSequence = "55357 56847", Shortcut = "😏", Description = "Smirking Face" };
            items[57] = new EmojiMenuShortcutItem { SurrogateSequence = "55357 56849", Shortcut = "😑", Description = "Expressionless Face" };
            items[58] = new EmojiMenuShortcutItem { SurrogateSequence = "55357 56434", Shortcut = "👲", Description = "Man With Gua Pi Mao" };
            items[59] = new EmojiMenuShortcutItem { SurrogateSequence = "55357 56435", Shortcut = "👳", Description = "Man With Turban" };
            items[60] = new EmojiMenuShortcutItem { SurrogateSequence = "55357 56430", Shortcut = "👮", Description = "Police Officer" };
            items[61] = new EmojiMenuShortcutItem { SurrogateSequence = "55357 56439", Shortcut = "👷", Description = "Construction Worker" };
            items[62] = new EmojiMenuShortcutItem { SurrogateSequence = "55357 56450", Shortcut = "💂", Description = "Guardsman" };
            items[63] = new EmojiMenuShortcutItem { SurrogateSequence = "55357 56438", Shortcut = "👶", Description = "Baby" };
            items[64] = new EmojiMenuShortcutItem { SurrogateSequence = "55357 56422", Shortcut = "👦", Description = "Boy" };
            items[65] = new EmojiMenuShortcutItem { SurrogateSequence = "55357 56423", Shortcut = "👧", Description = "Girl" };
            items[66] = new EmojiMenuShortcutItem { SurrogateSequence = "55357 56424", Shortcut = "👨", Description = "Man" };
            items[67] = new EmojiMenuShortcutItem { SurrogateSequence = "55357 56425", Shortcut = "👩", Description = "Woman" };
            items[68] = new EmojiMenuShortcutItem { SurrogateSequence = "55357 56436", Shortcut = "👴", Description = "Older Man" };
            items[69] = new EmojiMenuShortcutItem { SurrogateSequence = "55357 56437", Shortcut = "👵", Description = "Older Woman" };
            items[70] = new EmojiMenuShortcutItem { SurrogateSequence = "55357 56433", Shortcut = "👱", Description = "Person With Blond Hair" };
            items[71] = new EmojiMenuShortcutItem { SurrogateSequence = "55357 56444", Shortcut = "👼", Description = "Baby Angel" };
            items[72] = new EmojiMenuShortcutItem { SurrogateSequence = "55357 56440", Shortcut = "👸", Description = "Princess" };
            items[73] = new EmojiMenuShortcutItem { SurrogateSequence = "55357 56890", Shortcut = "😺", Description = "Smiling Cat Face With Open Mouth" };
            items[74] = new EmojiMenuShortcutItem { SurrogateSequence = "55357 56888", Shortcut = "😸", Description = "Grinning Cat Face With Smiling Eyes" };
            items[75] = new EmojiMenuShortcutItem { SurrogateSequence = "55357 56891", Shortcut = "😻", Description = "Smiling Cat Face With Heart-Shaped Eyes" };
            items[76] = new EmojiMenuShortcutItem { SurrogateSequence = "55357 56893", Shortcut = "😽", Description = "Kissing Cat Face With Closed Eyes" };
            items[77] = new EmojiMenuShortcutItem { SurrogateSequence = "55357 56892", Shortcut = "😼", Description = "Cat Face With Wry Smile" };
            items[78] = new EmojiMenuShortcutItem { SurrogateSequence = "55357 56896", Shortcut = "🙀", Description = "Weary Cat Face" };
            items[79] = new EmojiMenuShortcutItem { SurrogateSequence = "55357 56895", Shortcut = "😿", Description = "Crying Cat Face" };
            items[80] = new EmojiMenuShortcutItem { SurrogateSequence = "55357 56889", Shortcut = "😹", Description = "Cat Face With Tears Of Joy" };
            items[81] = new EmojiMenuShortcutItem { SurrogateSequence = "55357 56894", Shortcut = "😾", Description = "Pouting Cat Face" };
            items[82] = new EmojiMenuShortcutItem { SurrogateSequence = "55357 56441", Shortcut = "👹", Description = "Japanese Ogre" };
            items[83] = new EmojiMenuShortcutItem { SurrogateSequence = "55357 56442", Shortcut = "👺", Description = "Japanese Goblin" };
            items[84] = new EmojiMenuShortcutItem { SurrogateSequence = "55357 56904", Shortcut = "🙈", Description = "See-No-Evil Monkey" };
            items[85] = new EmojiMenuShortcutItem { SurrogateSequence = "55357 56905", Shortcut = "🙉", Description = "Hear-No-Evil Monkey" };
            items[86] = new EmojiMenuShortcutItem { SurrogateSequence = "55357 56906", Shortcut = "🙊", Description = "Speak-No-Evil Monkey" };
            items[87] = new EmojiMenuShortcutItem { SurrogateSequence = "55357 56448", Shortcut = "💀", Description = "Skull" };
            items[88] = new EmojiMenuShortcutItem { SurrogateSequence = "55357 56445", Shortcut = "👽", Description = "Extraterrestrial Alien" };
            items[89] = new EmojiMenuShortcutItem { SurrogateSequence = "55357 56489", Shortcut = "💩", Description = "Pile Of Poo" };
            items[90] = new EmojiMenuShortcutItem { SurrogateSequence = "55357 56613", Shortcut = "🔥", Description = "Fire" };
            items[91] = new EmojiMenuShortcutItem { SurrogateSequence = "10024", Shortcut = "✨", Description = "Sparkles" };
            items[92] = new EmojiMenuShortcutItem { SurrogateSequence = "55356 57119", Shortcut = "🌟", Description = "Glowing Star" };
            items[93] = new EmojiMenuShortcutItem { SurrogateSequence = "55357 56491", Shortcut = "💫", Description = "Dizzy Symbol" };
            items[94] = new EmojiMenuShortcutItem { SurrogateSequence = "55357 56485", Shortcut = "💥", Description = "Collision Symbol" };
            items[95] = new EmojiMenuShortcutItem { SurrogateSequence = "55357 56482", Shortcut = "💢", Description = "Anger Symbol" };
            items[96] = new EmojiMenuShortcutItem { SurrogateSequence = "55357 56486", Shortcut = "💦", Description = "Splashing Sweat Symbol" };
            items[97] = new EmojiMenuShortcutItem { SurrogateSequence = "55357 56487", Shortcut = "💧", Description = "Droplet" };
            items[98] = new EmojiMenuShortcutItem { SurrogateSequence = "55357 56484", Shortcut = "💤", Description = "Sleeping Symbol" };
            items[99] = new EmojiMenuShortcutItem { SurrogateSequence = "55357 56488", Shortcut = "💨", Description = "Dash Symbol  " };
            items[100] = new EmojiMenuShortcutItem { SurrogateSequence = "55357 56386", Shortcut = "👂", Description = "Ear" };
            items[101] = new EmojiMenuShortcutItem { SurrogateSequence = "55357 56384", Shortcut = "👀", Description = "Eyes" };
            items[102] = new EmojiMenuShortcutItem { SurrogateSequence = "55357 56387", Shortcut = "👃", Description = "Nose" };
            items[103] = new EmojiMenuShortcutItem { SurrogateSequence = "55357 56389", Shortcut = "👅", Description = "Tongue" };
            items[104] = new EmojiMenuShortcutItem { SurrogateSequence = "55357 56388", Shortcut = "👄", Description = "Mouth" };
            items[105] = new EmojiMenuShortcutItem { SurrogateSequence = "55357 56397", Shortcut = "👍", Description = "Thumbs Up Sign" };
            items[106] = new EmojiMenuShortcutItem { SurrogateSequence = "55357 56398", Shortcut = "👎", Description = "Thumbs Down Sign" };
            items[107] = new EmojiMenuShortcutItem { SurrogateSequence = "55357 56396", Shortcut = "👌", Description = "Ok Hand Sign" };
            items[108] = new EmojiMenuShortcutItem { SurrogateSequence = "55357 56394", Shortcut = "👊", Description = "Fisted Hand Sign" };
            items[109] = new EmojiMenuShortcutItem { SurrogateSequence = "9994", Shortcut = "✊", Description = "Raised Fist" };
            items[110] = new EmojiMenuShortcutItem { SurrogateSequence = "9996", Shortcut = "✌", Description = "Victory Hand" };
            items[111] = new EmojiMenuShortcutItem { SurrogateSequence = "55357 56395", Shortcut = "👋", Description = "Waving Hand Sign" };
            items[112] = new EmojiMenuShortcutItem { SurrogateSequence = "9995", Shortcut = "✋", Description = "Raised Hand" };
            items[113] = new EmojiMenuShortcutItem { SurrogateSequence = "55357 56400", Shortcut = "👐", Description = "Open Hands Sign" };
            items[114] = new EmojiMenuShortcutItem { SurrogateSequence = "55357 56390", Shortcut = "👆", Description = "White Up Pointing Backhand Index" };
            items[115] = new EmojiMenuShortcutItem { SurrogateSequence = "55357 56391", Shortcut = "👇", Description = "White Down Pointing Backhand Index" };
            items[116] = new EmojiMenuShortcutItem { SurrogateSequence = "55357 56393", Shortcut = "👉", Description = "White Right Pointing Backhand Index" };
            items[117] = new EmojiMenuShortcutItem { SurrogateSequence = "55357 56392", Shortcut = "👈", Description = "White Left Pointing Backhand Index" };
            items[118] = new EmojiMenuShortcutItem { SurrogateSequence = "55357 56908", Shortcut = "🙌", Description = "Person Raising Both Hands In Celebration" };
            items[119] = new EmojiMenuShortcutItem { SurrogateSequence = "55357 56911", Shortcut = "🙏", Description = "Person With Folded Hands" };
            items[120] = new EmojiMenuShortcutItem { SurrogateSequence = "9757", Shortcut = "☝", Description = "White Up Pointing Index" };
            items[121] = new EmojiMenuShortcutItem { SurrogateSequence = "55357 56399", Shortcut = "👏", Description = "Clapping Hands Sign" };
            items[122] = new EmojiMenuShortcutItem { SurrogateSequence = "55357 56490", Shortcut = "💪", Description = "Flexed Biceps" };
            items[123] = new EmojiMenuShortcutItem { SurrogateSequence = "55357 57014", Shortcut = "🚶", Description = "Pedestrian" };
            items[124] = new EmojiMenuShortcutItem { SurrogateSequence = "55356 57283", Shortcut = "🏃", Description = "Runner" };
            items[125] = new EmojiMenuShortcutItem { SurrogateSequence = "55357 56451", Shortcut = "💃", Description = "Dancer" };
            items[126] = new EmojiMenuShortcutItem { SurrogateSequence = "55357 56427", Shortcut = "👫", Description = "Man And Woman Holding Hands" };
            items[127] = new EmojiMenuShortcutItem { SurrogateSequence = "55357 56426", Shortcut = "👪", Description = "Family" };
            items[128] = new EmojiMenuShortcutItem { SurrogateSequence = "55357 56428", Shortcut = "👬", Description = "Two Men Holding Hands" };
            items[129] = new EmojiMenuShortcutItem { SurrogateSequence = "55357 56429", Shortcut = "👭", Description = "Two Women Holding Hands" };
            items[130] = new EmojiMenuShortcutItem { SurrogateSequence = "55357 56463", Shortcut = "💏", Description = "Kiss" };
            items[131] = new EmojiMenuShortcutItem { SurrogateSequence = "55357 56465", Shortcut = "💑", Description = "Couple With Heart" };
            items[132] = new EmojiMenuShortcutItem { SurrogateSequence = "55357 56431", Shortcut = "👯", Description = "Woman With Bunny Ears" };
            items[133] = new EmojiMenuShortcutItem { SurrogateSequence = "55357 56902", Shortcut = "🙆", Description = "Face With Ok Gesture" };
            items[134] = new EmojiMenuShortcutItem { SurrogateSequence = "55357 56901", Shortcut = "🙅", Description = "Face With No Good Gesture" };
            items[135] = new EmojiMenuShortcutItem { SurrogateSequence = "55357 56449", Shortcut = "💁", Description = "Information Desk Person" };
            items[136] = new EmojiMenuShortcutItem { SurrogateSequence = "55357 56907", Shortcut = "🙋", Description = "Happy Person Raising One Hand" };
            items[137] = new EmojiMenuShortcutItem { SurrogateSequence = "55357 56454", Shortcut = "💆", Description = "Face Massage" };
            items[138] = new EmojiMenuShortcutItem { SurrogateSequence = "55357 56455", Shortcut = "💇", Description = "Haircut" };
            items[139] = new EmojiMenuShortcutItem { SurrogateSequence = "55357 56453", Shortcut = "💅", Description = "Nail Polish" };
            items[140] = new EmojiMenuShortcutItem { SurrogateSequence = "55357 56432", Shortcut = "👰", Description = "Bride With Veil" };
            items[141] = new EmojiMenuShortcutItem { SurrogateSequence = "55357 56910", Shortcut = "🙎", Description = "Person With Pouting Face" };
            items[142] = new EmojiMenuShortcutItem { SurrogateSequence = "55357 56909", Shortcut = "🙍", Description = "Person Frowning" };
            items[143] = new EmojiMenuShortcutItem { SurrogateSequence = "55357 56903", Shortcut = "🙇", Description = "Person Bowing Deeply" };
            items[144] = new EmojiMenuShortcutItem { SurrogateSequence = "55356 57257", Shortcut = "🎩", Description = "Top Hat" };
            items[145] = new EmojiMenuShortcutItem { SurrogateSequence = "55357 56401", Shortcut = "👑", Description = "Crown" };
            items[146] = new EmojiMenuShortcutItem { SurrogateSequence = "55357 56402", Shortcut = "👒", Description = "Womans Hat" };
            items[147] = new EmojiMenuShortcutItem { SurrogateSequence = "55357 56415", Shortcut = "👟", Description = "Athletic Shoe" };
            items[148] = new EmojiMenuShortcutItem { SurrogateSequence = "55357 56414", Shortcut = "👞", Description = "Mans Shoe" };
            items[149] = new EmojiMenuShortcutItem { SurrogateSequence = "55357 56417", Shortcut = "👡", Description = "Womans Sandal" };
            items[150] = new EmojiMenuShortcutItem { SurrogateSequence = "55357 56416", Shortcut = "👠", Description = "High-Heeled Shoe" };
            items[151] = new EmojiMenuShortcutItem { SurrogateSequence = "55357 56418", Shortcut = "👢", Description = "Womans Boots" };
            items[152] = new EmojiMenuShortcutItem { SurrogateSequence = "55357 56405", Shortcut = "👕", Description = "T-Shirt" };
            items[153] = new EmojiMenuShortcutItem { SurrogateSequence = "55357 56404", Shortcut = "👔", Description = "Necktie" };
            items[154] = new EmojiMenuShortcutItem { SurrogateSequence = "55357 56410", Shortcut = "👚", Description = "Womans Clothes" };
            items[155] = new EmojiMenuShortcutItem { SurrogateSequence = "55357 56407", Shortcut = "👗", Description = "Dress" };
            items[156] = new EmojiMenuShortcutItem { SurrogateSequence = "55356 57277", Shortcut = "🎽", Description = "Running Shirt With Sash" };
            items[157] = new EmojiMenuShortcutItem { SurrogateSequence = "55357 56406", Shortcut = "👖", Description = "Jeans" };
            items[158] = new EmojiMenuShortcutItem { SurrogateSequence = "55357 56408", Shortcut = "👘", Description = "Kimono" };
            items[159] = new EmojiMenuShortcutItem { SurrogateSequence = "55357 56409", Shortcut = "👙", Description = "Bikini" };
            items[160] = new EmojiMenuShortcutItem { SurrogateSequence = "55357 56508", Shortcut = "💼", Description = "Briefcase" };
            items[161] = new EmojiMenuShortcutItem { SurrogateSequence = "55357 56412", Shortcut = "👜", Description = "Handbag" };
            items[162] = new EmojiMenuShortcutItem { SurrogateSequence = "55357 56413", Shortcut = "👝", Description = "Pouch" };
            items[163] = new EmojiMenuShortcutItem { SurrogateSequence = "55357 56411", Shortcut = "👛", Description = "Purse" };
            items[164] = new EmojiMenuShortcutItem { SurrogateSequence = "55357 56403", Shortcut = "👓", Description = "Eyeglasses" };
            items[165] = new EmojiMenuShortcutItem { SurrogateSequence = "55356 57216", Shortcut = "🎀", Description = "Ribbon" };
            items[166] = new EmojiMenuShortcutItem { SurrogateSequence = "55356 57090", Shortcut = "🌂", Description = "Closed Umbrella" };
            items[167] = new EmojiMenuShortcutItem { SurrogateSequence = "55357 56452", Shortcut = "💄", Description = "Lipstick" };
            items[168] = new EmojiMenuShortcutItem { SurrogateSequence = "55357 56475", Shortcut = "💛", Description = "Yellow Heart" };
            items[169] = new EmojiMenuShortcutItem { SurrogateSequence = "55357 56473", Shortcut = "💙", Description = "Blue Heart" };
            items[170] = new EmojiMenuShortcutItem { SurrogateSequence = "55357 56476", Shortcut = "💜", Description = "Purple Heart" };
            items[171] = new EmojiMenuShortcutItem { SurrogateSequence = "55357 56474", Shortcut = "💚", Description = "Green Heart" };
            items[172] = new EmojiMenuShortcutItem { SurrogateSequence = "10084", Shortcut = "❤", Description = "Heavy Black Heart" };
            items[173] = new EmojiMenuShortcutItem { SurrogateSequence = "55357 56468", Shortcut = "💔", Description = "Broken Heart" };
            items[174] = new EmojiMenuShortcutItem { SurrogateSequence = "55357 56471", Shortcut = "💗", Description = "Growing Heart" };
            items[175] = new EmojiMenuShortcutItem { SurrogateSequence = "55357 56467", Shortcut = "💓", Description = "Beating Heart" };
            items[176] = new EmojiMenuShortcutItem { SurrogateSequence = "55357 56469", Shortcut = "💕", Description = "Two Hearts" };
            items[177] = new EmojiMenuShortcutItem { SurrogateSequence = "55357 56470", Shortcut = "💖", Description = "Sparkling Heart" };
            items[178] = new EmojiMenuShortcutItem { SurrogateSequence = "55357 56478", Shortcut = "💞", Description = "Revolving Hearts" };
            items[179] = new EmojiMenuShortcutItem { SurrogateSequence = "55357 56472", Shortcut = "💘", Description = "Heart With Arrow" };
            items[180] = new EmojiMenuShortcutItem { SurrogateSequence = "55357 56460", Shortcut = "💌", Description = "Love Letter" };
            items[181] = new EmojiMenuShortcutItem { SurrogateSequence = "55357 56459", Shortcut = "💋", Description = "Kiss Mark" };
            items[182] = new EmojiMenuShortcutItem { SurrogateSequence = "55357 56461", Shortcut = "💍", Description = "Ring" };
            items[183] = new EmojiMenuShortcutItem { SurrogateSequence = "55357 56462", Shortcut = "💎", Description = "Gem Stone" };
            items[184] = new EmojiMenuShortcutItem { SurrogateSequence = "55357 56420", Shortcut = "👤", Description = "Bust In Silhouette" };
            items[185] = new EmojiMenuShortcutItem { SurrogateSequence = "55357 56421", Shortcut = "👥", Description = "Busts In Silhouette" };
            items[186] = new EmojiMenuShortcutItem { SurrogateSequence = "55357 56492", Shortcut = "💬", Description = "Speech Balloon" };
            items[187] = new EmojiMenuShortcutItem { SurrogateSequence = "55357 56419", Shortcut = "👣", Description = "Footprints" };
            items[188] = new EmojiMenuShortcutItem { SurrogateSequence = "55357 56493", Shortcut = "💭", Description = "Thought Balloon" };

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