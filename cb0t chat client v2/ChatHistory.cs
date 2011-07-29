using System;
using System.Collections.Generic;
using System.Text;

namespace cb0t_chat_client_v2
{
    class ChatHistory
    {
        private static List<String> text_buffer = new List<String>();

        private static int index = -1;

        public static void ResetIndex()
        {
            index = -1;
        }

        public static void AddLine(String text)
        {
            if (text_buffer.Count > 0)
                if (text_buffer[0] == text)
                    return;

            text_buffer.Insert(0, text);

            if (text_buffer.Count > 100)
                text_buffer.RemoveAt(text_buffer.Count - 1);
        }

        public static String GetLine()
        {
            if (text_buffer.Count == 0) return String.Empty;

            index++;

            if (index >= text_buffer.Count) index = 0;
            return text_buffer[index];

        }
    }
}
