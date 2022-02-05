using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace TeamOnServer
{
    class Program
    {
        static void Main(string[] args)
        {
            int port = 8888;
            var server = new ChatServer();
            server.Init(port);
            server.th.Join();
        }
    }

    public class ChatServer : TcpRoutine
    {
        public void Init(int port)
        {
            InitTcp(IPAddress.Any, port, ThreadProcessor, () => new UserInfo());

        }
        public override void NewClient()
        {
            sendAllClientUpdates();
        }
        public void ThreadProcessor(NetworkStream stream, object obj)
        {
            var cinf = obj as ConnectionInfo;
            var uinfo = cinf.Tag as UserInfo;
            StreamReader reader = new StreamReader(stream);
            StreamWriter wrt2 = new StreamWriter(stream);

            while (true)
            {
                try
                {
                    var line = reader.ReadLine();

                    if (line.StartsWith("INIT"))
                    {
                        var ind = line.IndexOf("=");
                        var msg = line.Substring(ind + 1);
                        uinfo.Name = msg;
                        NewClient();

                    }
                    else if (line.StartsWith("MSG"))
                    {

                        var ind = line.IndexOf("=");
                        var msg = line.Substring(ind + 1);
                        var bs64 = Convert.FromBase64String(msg);

                        var str = Encoding.UTF8.GetString(bs64);


                        StringBuilder sb = new StringBuilder();
                        sb.AppendLine("<?xml version=\"1.0\"?>");
                        sb.AppendLine("<root>");
                        sb.AppendLine(string.Format("<message user=\"{0}\">", uinfo.Name));
                        sb.AppendFormat("<![CDATA[{0}]]>", str);
                        sb.AppendLine(string.Format("</message>", uinfo.Name, str));
                        sb.AppendLine("</root>");

                        var estr = sb.ToString();


                        var bt = Encoding.UTF8.GetBytes(estr);

                        var ree = Convert.ToBase64String(bt);

                        this.SendAll("MSG=" + ree);
                    }
                    else if (line.StartsWith("CLIENTS"))
                    {
                        sendClientsList(wrt2);
                    }
                    else if (line.StartsWith("ACK"))//file download ack
                    {
                        //1.parse xml
                        var ln = line.Substring("ACK".Length + 1);

                        var bs64 = Convert.FromBase64String(ln);

                        var str = Encoding.UTF8.GetString(bs64);
                        var doc = XDocument.Parse(str);
                        var w = doc.Descendants("ack").First();
                        var targ = w.Attribute("target").Value;
                        var cxstr = this.streams.First(z => (z.Tag as UserInfo).Name == targ);
                        var wr = new StreamWriter(cxstr.Stream);
                        //2. retranslate to specific client web request
                        wr.WriteLine(line);
                        wr.Flush();


                        //server.SendAll(line);
                    }
                    else if (line.StartsWith("FILE"))
                    {
                        //1.parse xml
                        var ln = line.Substring("FILE".Length + 1);

                        var bs64 = Convert.FromBase64String(ln);

                        var str = Encoding.UTF8.GetString(bs64);
                        var doc = XDocument.Parse(str);
                        var w = doc.Descendants("file").First();
                        var targ = w.Attribute("target").Value;
                        var cxstr = this.streams.First(z => (z.Tag as UserInfo).Name == targ);
                        var wr = new StreamWriter(cxstr.Stream);
                        //2. retranslate to specific client web request
                        wr.WriteLine(line);
                        wr.Flush();

                        //server.SendAll(line);
                    }


                }
                catch (IOException iex)
                {
                    lock (streams)
                        streams.Remove(cinf);
                    sendAllClientUpdates();
                    break;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    break;
                    //TcpRoutine.ErrorSend(stream);
                }
            }
        }

        void sendAllClientUpdates()
        {
            var estr = getClientsXml();
            var bt = Encoding.UTF8.GetBytes(estr);
            var ree = Convert.ToBase64String(bt);
            var ss = "CLIENTS=" + ree;

            SendAll(ss);
        }

        public string getClientsXml()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("<?xml version=\"1.0\"?>");
            sb.AppendLine("<root>");
            foreach (var connectionInfo in this.streams)
            {
                var uin = connectionInfo.Tag as UserInfo;
                sb.AppendLine(string.Format("<client name=\"{0}\" />", uin.Name));
            }
            sb.AppendLine("</root>");
            return sb.ToString();
        }
        private void sendClientsList(StreamWriter wrt2)
        {
            var estr = getClientsXml();
            var bt = Encoding.UTF8.GetBytes(estr);
            var ree = Convert.ToBase64String(bt);
            wrt2.WriteLine("CLIENTS=" + ree);
            wrt2.Flush();
        }
    }

    public class TcpRoutine
    {
        public static void ErrorSend(NetworkStream stream)
        {
            var writer = new StreamWriter(stream);
            writer.WriteLine("error");
            writer.Flush();
        }

        public static void SendAck(NetworkStream stream)
        {
            var writer = new StreamWriter(stream);

            writer.WriteLine("ack");
            writer.Flush();
        }

        public void SendAll(string ln)
        {
            List<ConnectionInfo> infos = new List<ConnectionInfo>();
            foreach (var connectionInfo in streams)
            {
                try
                {
                    StreamWriter wrt = new StreamWriter(connectionInfo.Stream);
                    wrt.WriteLine(ln);
                    wrt.Flush();
                }
                catch (Exception ex)
                {
                    infos.Add(connectionInfo);
                }
            }
            lock (streams)
            {
                foreach (var connectionInfo in infos)
                {
                    streams.Remove(connectionInfo);
                }
            }
        }


        public List<ConnectionInfo> streams = new List<ConnectionInfo>();

        public virtual void NewClient()
        {

        }

        public Thread th;
        public void InitTcp(IPAddress ip, int port, Action<NetworkStream, object> threadProcessor, Func<object> factory = null)
        {
            server1 = new TcpListener(ip, port);
            server1.Start();


            th = new Thread(() =>
            {
                while (true)
                {
                    var client = server1.AcceptTcpClient();
                    Console.WriteLine("client accepted");
                    lock (streams)
                    {
                        var stream = client.GetStream();
                        var addr = (client.Client.RemoteEndPoint as IPEndPoint).Address;
                        var _port = (client.Client.RemoteEndPoint as IPEndPoint).Port;
                        var obj = factory != null ? factory() : null;
                        var cinf = new ConnectionInfo() { Stream = stream, Client = client, Ip = addr, Port = _port, Tag = obj };

                        streams.Add(cinf);
                        Thread thp = new Thread(() => { threadProcessor(stream, cinf); });
                        thp.IsBackground = true;
                        thp.Start();
                    }

                }
            });
            th.IsBackground = true;
            th.Start();
        }

        private TcpListener server1;


    }

    public class ConnectionInfo
    {
        public NetworkStream Stream;
        public TcpClient Client;
        public IPAddress Ip;
        public int Port;
        public object Tag;

    }

    public class UserInfo
    {
        public string Name;
    }
}
