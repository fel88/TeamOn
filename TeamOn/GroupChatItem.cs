using System.Collections.Generic;

namespace TeamOn
{
    public class GroupChatItem : ChatItem
    {
        public List<UserInfo> Users = new List<UserInfo>();
        public UserInfo Owner;
    }
}
