using System;
using System.IO;
using System.Text;
using System.Xml.Linq;

namespace TeamOn
{
    public static class Settings
    {
        public static string Nickname = Environment.UserName;
        public static string ServerIP = "127.0.0.1";
        public static int ServerPort = 8888;
        public static void LoadSettings()
        {
            if (!File.Exists("config.xml")) return;
            var doc = XDocument.Load("config.xml");

            foreach (var item in doc.Descendants("setting"))
            {
                var nm = item.Attribute("name").Value;
                var vl = item.Attribute("value").Value;
                switch (nm)
                {
                    case "serverIP":
                        ServerIP = vl;
                        break;
                    case "serverPort":
                        ServerPort = int.Parse(vl);
                        break;
                    case "nickname":
                        Nickname = vl;
                        break;
                    case "allowConnects":
                        TeamScreen.TeamScreenServer.AllowConnects = bool.Parse(vl);
                        break;
                    default:
                        break;
                }
            }

        }
        public static void SaveSettings()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("<?xml version=\"1.0\"?>");
            sb.AppendLine("<root>");
            sb.AppendLine($"<setting name=\"nickname\" value=\"{Nickname}\"/>");
            sb.AppendLine($"<setting name=\"allowConnects\" value=\"{TeamScreen.TeamScreenServer.AllowConnects}\"/>");
            sb.AppendLine($"<setting name=\"serverIP\" value=\"{ServerIP}\"/>");
            sb.AppendLine($"<setting name=\"serverPort\" value=\"{ServerPort}\"/>");

            sb.AppendLine("</root>");
            File.WriteAllText("config.xml", sb.ToString());
        }
    }
}
