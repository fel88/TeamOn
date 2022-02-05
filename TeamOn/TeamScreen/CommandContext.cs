using System.IO;
using static TeamOn.TeamScreen.TeamScreenServer;

namespace TeamOn.TeamScreen
{
    public class CommandContext
    {
        public StreamWriter Writer;
        public StreamReader Reader;
        public ClientObject Ctx;
        public ConnectInfo Info;
        public string Command;
    }
}
