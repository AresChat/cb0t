using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace cb0t_chat_client_v2
{
    class EmoticonFinder
    {
        public static int last_emote_length = 0;

        public static Bitmap GetEmoticonFromKeyboardShortcut(String code)
        {
            if (code.StartsWith(":)"))
            {
                last_emote_length = 2;
                return AresImages.TransparentEmoticons[0];
            }

            if (code.StartsWith(":-)"))
            {
                last_emote_length = 3;
                return AresImages.TransparentEmoticons[0];
            }

            if (code.StartsWith(":D"))
            {
                last_emote_length = 2;
                return AresImages.TransparentEmoticons[1];
            }

            if (code.StartsWith(":-D"))
            {
                last_emote_length = 3;
                return AresImages.TransparentEmoticons[1];
            }

            if (code.StartsWith(";)"))
            {
                last_emote_length = 2;
                return AresImages.TransparentEmoticons[2];
            }

            if (code.StartsWith(";-)"))
            {
                last_emote_length = 3;
                return AresImages.TransparentEmoticons[2];
            }

            if (code.StartsWith(":O"))
            {
                last_emote_length = 2;
                return AresImages.TransparentEmoticons[3];
            }

            if (code.StartsWith(":-O"))
            {
                last_emote_length = 3;
                return AresImages.TransparentEmoticons[3];
            }

            if (code.StartsWith(":P"))
            {
                last_emote_length = 2;
                return AresImages.TransparentEmoticons[4];
            }

            if (code.StartsWith(":-P"))
            {
                last_emote_length = 3;
                return AresImages.TransparentEmoticons[4];
            }

            if (code.StartsWith("(H)"))
            {
                last_emote_length = 3;
                return AresImages.TransparentEmoticons[5];
            }

            if (code.StartsWith(":@"))
            {
                last_emote_length = 2;
                return AresImages.TransparentEmoticons[6];
            }

            if (code.StartsWith(":$"))
            {
                last_emote_length = 2;
                return AresImages.TransparentEmoticons[7];
            }

            if (code.StartsWith(":-$"))
            {
                last_emote_length = 3;
                return AresImages.TransparentEmoticons[7];
            }

            if (code.StartsWith(":S"))
            {
                last_emote_length = 2;
                return AresImages.TransparentEmoticons[8];
            }

            if (code.StartsWith(":-S"))
            {
                last_emote_length = 3;
                return AresImages.TransparentEmoticons[8];
            }

            if (code.StartsWith(":("))
            {
                last_emote_length = 2;
                return AresImages.TransparentEmoticons[9];
            }

            if (code.StartsWith(":-("))
            {
                last_emote_length = 3;
                return AresImages.TransparentEmoticons[9];
            }

            if (code.StartsWith(":'("))
            {
                last_emote_length = 3;
                return AresImages.TransparentEmoticons[10];
            }

            if (code.StartsWith(":|"))
            {
                last_emote_length = 2;
                return AresImages.TransparentEmoticons[11];
            }

            if (code.StartsWith(":-|"))
            {
                last_emote_length = 3;
                return AresImages.TransparentEmoticons[11];
            }

            if (code.StartsWith("(6)"))
            {
                last_emote_length = 3;
                return AresImages.TransparentEmoticons[12];
            }

            if (code.StartsWith("(A)"))
            {
                last_emote_length = 3;
                return AresImages.TransparentEmoticons[13];
            }

            if (code.StartsWith("(L)"))
            {
                last_emote_length = 3;
                return AresImages.TransparentEmoticons[14];
            }

            if (code.StartsWith("(U)"))
            {
                last_emote_length = 3;
                return AresImages.TransparentEmoticons[15];
            }

            if (code.StartsWith("(M)"))
            {
                last_emote_length = 3;
                return AresImages.TransparentEmoticons[16];
            }

            if (code.StartsWith("(@)"))
            {
                last_emote_length = 3;
                return AresImages.TransparentEmoticons[17];
            }

            if (code.StartsWith("(&)"))
            {
                last_emote_length = 3;
                return AresImages.TransparentEmoticons[18];
            }

            if (code.StartsWith("(S)"))
            {
                last_emote_length = 3;
                return AresImages.TransparentEmoticons[19];
            }

            if (code.StartsWith("(*)"))
            {
                last_emote_length = 3;
                return AresImages.TransparentEmoticons[20];
            }

            if (code.StartsWith("(~)"))
            {
                last_emote_length = 3;
                return AresImages.TransparentEmoticons[21];
            }

            if (code.StartsWith("(E)"))
            {
                last_emote_length = 3;
                return AresImages.TransparentEmoticons[22];
            }

            if (code.StartsWith("(8)"))
            {
                last_emote_length = 3;
                return AresImages.TransparentEmoticons[23];
            }

            if (code.StartsWith("(F)"))
            {
                last_emote_length = 3;
                return AresImages.TransparentEmoticons[24];
            }

            if (code.StartsWith("(W)"))
            {
                last_emote_length = 3;
                return AresImages.TransparentEmoticons[25];
            }

            if (code.StartsWith("(O)"))
            {
                last_emote_length = 3;
                return AresImages.TransparentEmoticons[26];
            }

            if (code.StartsWith("(K)"))
            {
                last_emote_length = 3;
                return AresImages.TransparentEmoticons[27];
            }

            if (code.StartsWith("(G)"))
            {
                last_emote_length = 3;
                return AresImages.TransparentEmoticons[28];
            }

            if (code.StartsWith("(^)"))
            {
                last_emote_length = 3;
                return AresImages.TransparentEmoticons[29];
            }

            if (code.StartsWith("(P)"))
            {
                last_emote_length = 3;
                return AresImages.TransparentEmoticons[30];
            }

            if (code.StartsWith("(I)"))
            {
                last_emote_length = 3;
                return AresImages.TransparentEmoticons[31];
            }

            if (code.StartsWith("(C)"))
            {
                last_emote_length = 3;
                return AresImages.TransparentEmoticons[32];
            }

            if (code.StartsWith("(T)"))
            {
                last_emote_length = 3;
                return AresImages.TransparentEmoticons[33];
            }

            if (code.StartsWith("({)"))
            {
                last_emote_length = 3;
                return AresImages.TransparentEmoticons[34];
            }

            if (code.StartsWith("(})"))
            {
                last_emote_length = 3;
                return AresImages.TransparentEmoticons[35];
            }

            if (code.StartsWith("(B)"))
            {
                last_emote_length = 3;
                return AresImages.TransparentEmoticons[36];
            }

            if (code.StartsWith("(D)"))
            {
                last_emote_length = 3;
                return AresImages.TransparentEmoticons[37];
            }

            if (code.StartsWith("(Z)"))
            {
                last_emote_length = 3;
                return AresImages.TransparentEmoticons[38];
            }

            if (code.StartsWith("(X)"))
            {
                last_emote_length = 3;
                return AresImages.TransparentEmoticons[39];
            }

            if (code.StartsWith("(Y)"))
            {
                last_emote_length = 3;
                return AresImages.TransparentEmoticons[40];
            }

            if (code.StartsWith("(N)"))
            {
                last_emote_length = 3;
                return AresImages.TransparentEmoticons[41];
            }

            if (code.StartsWith(":["))
            {
                last_emote_length = 2;
                return AresImages.TransparentEmoticons[42];
            }

            if (code.StartsWith(":-["))
            {
                last_emote_length = 3;
                return AresImages.TransparentEmoticons[42];
            }

            if (code.StartsWith("(1)"))
            {
                last_emote_length = 3;
                return AresImages.TransparentEmoticons[43];
            }

            if (code.StartsWith("(2)"))
            {
                last_emote_length = 3;
                return AresImages.TransparentEmoticons[44];
            }

            if (code.StartsWith("(3)"))
            {
                last_emote_length = 3;
                return AresImages.TransparentEmoticons[45];
            }

            if (code.StartsWith("(4)"))
            {
                last_emote_length = 3;
                return AresImages.TransparentEmoticons[46];
            }

            last_emote_length = 0;
            return null;
        }

        public static int GetRTFEmoticonFromKeyboardShortcut(StringBuilder sb, int index)
        {
            int len = sb.Length - index;

            if (len < 2)
                return -1;

            len = len > 4 ? 4 : len;
            char[] chrs = new char[len];

            for (int i = 0; i < chrs.Length; i++)
                chrs[i] = sb[index + i];

            return GetRTFEmoticonFromKeyboardShortcut(new String(chrs).ToUpper());
        }
        
        public static int GetRTFEmoticonFromKeyboardShortcut(String code)
        {
            if (code.StartsWith(":)"))
            {
                last_emote_length = 2;
                return 0;
            }

            if (code.StartsWith(":-)"))
            {
                last_emote_length = 3;
                return 0;
            }

            if (code.StartsWith(":D"))
            {
                last_emote_length = 2;
                return 1;
            }

            if (code.StartsWith(":-D"))
            {
                last_emote_length = 3;
                return 1;
            }

            if (code.StartsWith(";)"))
            {
                last_emote_length = 2;
                return 2;
            }

            if (code.StartsWith(";-)"))
            {
                last_emote_length = 3;
                return 2;
            }

            if (code.StartsWith(":O"))
            {
                last_emote_length = 2;
                return 3;
            }

            if (code.StartsWith(":-O"))
            {
                last_emote_length = 3;
                return 3;
            }

            if (code.StartsWith(":P"))
            {
                last_emote_length = 2;
                return 4;
            }

            if (code.StartsWith(":-P"))
            {
                last_emote_length = 3;
                return 4;
            }

            if (code.StartsWith("(H)"))
            {
                last_emote_length = 3;
                return 5;
            }

            if (code.StartsWith(":@"))
            {
                last_emote_length = 2;
                return 6;
            }

            if (code.StartsWith(":$"))
            {
                last_emote_length = 2;
                return 7;
            }

            if (code.StartsWith(":-$"))
            {
                last_emote_length = 3;
                return 7;
            }

            if (code.StartsWith(":S"))
            {
                last_emote_length = 2;
                return 8;
            }

            if (code.StartsWith(":-S"))
            {
                last_emote_length = 3;
                return 8;
            }

            if (code.StartsWith(":("))
            {
                last_emote_length = 2;
                return 9;
            }

            if (code.StartsWith(":-("))
            {
                last_emote_length = 3;
                return 9;
            }

            if (code.StartsWith(":'("))
            {
                last_emote_length = 3;
                return 10;
            }

            if (code.StartsWith(":|"))
            {
                last_emote_length = 2;
                return 11;
            }

            if (code.StartsWith(":-|"))
            {
                last_emote_length = 3;
                return 11;
            }

            if (code.StartsWith("(6)"))
            {
                last_emote_length = 3;
                return 12;
            }

            if (code.StartsWith("(A)"))
            {
                last_emote_length = 3;
                return 13;
            }

            if (code.StartsWith("(L)"))
            {
                last_emote_length = 3;
                return 14;
            }

            if (code.StartsWith("(U)"))
            {
                last_emote_length = 3;
                return 15;
            }

            if (code.StartsWith("(M)"))
            {
                last_emote_length = 3;
                return 16;
            }

            if (code.StartsWith("(@)"))
            {
                last_emote_length = 3;
                return 17;
            }

            if (code.StartsWith("(&)"))
            {
                last_emote_length = 3;
                return 18;
            }

            if (code.StartsWith("(S)"))
            {
                last_emote_length = 3;
                return 19;
            }

            if (code.StartsWith("(*)"))
            {
                last_emote_length = 3;
                return 20;
            }

            if (code.StartsWith("(~)"))
            {
                last_emote_length = 3;
                return 21;
            }

            if (code.StartsWith("(E)"))
            {
                last_emote_length = 3;
                return 22;
            }

            if (code.StartsWith("(8)"))
            {
                last_emote_length = 3;
                return 23;
            }

            if (code.StartsWith("(F)"))
            {
                last_emote_length = 3;
                return 24;
            }

            if (code.StartsWith("(W)"))
            {
                last_emote_length = 3;
                return 25;
            }

            if (code.StartsWith("(O)"))
            {
                last_emote_length = 3;
                return 26;
            }

            if (code.StartsWith("(K)"))
            {
                last_emote_length = 3;
                return 27;
            }

            if (code.StartsWith("(G)"))
            {
                last_emote_length = 3;
                return 28;
            }

            if (code.StartsWith("(^)"))
            {
                last_emote_length = 3;
                return 29;
            }

            if (code.StartsWith("(P)"))
            {
                last_emote_length = 3;
                return 30;
            }

            if (code.StartsWith("(I)"))
            {
                last_emote_length = 3;
                return 31;
            }

            if (code.StartsWith("(C)"))
            {
                last_emote_length = 3;
                return 32;
            }

            if (code.StartsWith("(T)"))
            {
                last_emote_length = 3;
                return 33;
            }

            if (code.StartsWith("({)"))
            {
                last_emote_length = 3;
                return 34;
            }

            if (code.StartsWith("(})"))
            {
                last_emote_length = 3;
                return 35;
            }

            if (code.StartsWith("(B)"))
            {
                last_emote_length = 3;
                return 36;
            }

            if (code.StartsWith("(D)"))
            {
                last_emote_length = 3;
                return 37;
            }

            if (code.StartsWith("(Z)"))
            {
                last_emote_length = 3;
                return 38;
            }

            if (code.StartsWith("(X)"))
            {
                last_emote_length = 3;
                return 39;
            }

            if (code.StartsWith("(Y)"))
            {
                last_emote_length = 3;
                return 40;
            }

            if (code.StartsWith("(N)"))
            {
                last_emote_length = 3;
                return 41;
            }

            if (code.StartsWith(":["))
            {
                last_emote_length = 2;
                return 42;
            }

            if (code.StartsWith(":-["))
            {
                last_emote_length = 3;
                return 42;
            }

            if (code.StartsWith("(1)"))
            {
                last_emote_length = 3;
                return 43;
            }

            if (code.StartsWith("(2)"))
            {
                last_emote_length = 3;
                return 44;
            }

            if (code.StartsWith("(3)"))
            {
                last_emote_length = 3;
                return 45;
            }

            if (code.StartsWith("(4)"))
            {
                last_emote_length = 3;
                return 46;
            }

            last_emote_length = 0;
            return -1;
        }
    }
}
