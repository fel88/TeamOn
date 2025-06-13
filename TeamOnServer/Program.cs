using System.IO;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace TeamOnServer
{
    class Program
    {
        static void Main(string[] args)
        {
            int port = 8888;
            LoadConfig();
            var server = new ChatServer();
            server.Init(port);
            server.th.Join();
        }

        private static void LoadConfig()
        {
            if (!File.Exists("config.xml")) return;
            var doc = XDocument.Load("config.xml");
            foreach (var item in doc.Descendants("group"))
            {
                var gi = new GroupInfo();
                TcpRoutine.Groups.Add(gi);
                var nm = item.Attribute("name").Value;
                var owner = item.Attribute("owner").Value;
                foreach (var user in item.Elements("user"))
                {
                    var un = user.Attribute("name").Value;
                    gi.Users.Add(new UserInfo() { Name = un });
                }
            }
        }
    }
}
