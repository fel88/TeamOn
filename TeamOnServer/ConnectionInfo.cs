using System.Net;
using System.Net.Sockets;

namespace TeamOnServer
{
    public class ConnectionInfo
    {
        public NetworkStream Stream;
        public TcpClient Client;
        public IPAddress Ip;
        public int Port;
        public object Tag;

    }
}
