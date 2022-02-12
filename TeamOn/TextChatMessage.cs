using System;

namespace TeamOn
{
    public class TextChatMessage : ChatMessage
    {
        public TextChatMessage(DateTime time, string text)
        {
            Text = text;
            DateTime = time;
        }
        public string Text;
    }
}
