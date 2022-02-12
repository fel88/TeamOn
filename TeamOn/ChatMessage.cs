using System;

namespace TeamOn
{
    public abstract class ChatMessage
    {
        public ChatItem Parent;
        public UserInfo Owner;
        public DateTime DateTime;
    }
}
