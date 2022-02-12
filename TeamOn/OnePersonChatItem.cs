using System;

namespace TeamOn
{
    public class OnePersonChatItem : ChatItem
    {
        public override string Name { get => Person.Name; set { } }
        public UserInfo Person;
        public DateTime LastTyping;
    }
}
