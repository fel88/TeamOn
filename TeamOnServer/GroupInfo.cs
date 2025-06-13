using System.Collections.Generic;

namespace TeamOnServer
{
    public class GroupInfo
    {
        public List<UserInfo> Users = new List<UserInfo>();
        public string Name;
        public UserInfo Owner;
    }
}
