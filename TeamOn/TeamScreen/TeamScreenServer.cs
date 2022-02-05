using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace TeamOn.TeamScreen
{
    public class TeamScreenServer
    {
        public static int port = 7701;
        public static TcpListener listener;

        public class ConnectInfo
        {
            public ClientObject ClientObject;
            public string Ip { get; set; }
            public DateTime ConnectTimestamp { get; set; }
        }
        public static List<ConnectInfo> Infos = new List<ConnectInfo>();
        public static int Connects = 0;
        public static int CommandsProcessed = 0;
        public static Thread MainThread;
        public static bool AllowConnects = true;
        public static AutoResetEvent event1 = new AutoResetEvent(false);
        public static void StartServer()
        {

            MainThread = new Thread(() =>
            {
                try
                {
                    listener = new TcpListener(IPAddress.Any, port);
                    listener.Start();

                    while (true)
                    {
                        if (!AllowConnects)
                        {
                            event1.WaitOne();
                        }
                        TcpClient client = listener.AcceptTcpClient();
                        var addr = (client.Client.RemoteEndPoint as IPEndPoint).Address;
                        var ip = addr.ToString();

                        lock (Infos)
                        {
                            Infos.Add(new ConnectInfo() { Ip = addr.ToString() });
                        }

                        var clientObject = new ClientObject(client, TeamScreenServer.Infos.Last());

                        Thread clientThread = new Thread(new ThreadStart(clientObject.Process));

                        clientThread.Start();
                        Connects++;

                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
                finally
                {
                    if (listener != null)
                        listener.Stop();
                }
            });

            MainThread.IsBackground = true;
            MainThread.Start();

        }

        public static Action<Bitmap> ImageCaptured;
        public static void OnImageCaptured(Bitmap bitmap)
        {
            if (ImageCaptured != null)
            {
                ImageCaptured(bitmap);
            }
        }
    }
}
