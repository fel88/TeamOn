using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Threading;
using static TeamOn.TeamScreen.TeamScreenServer;

namespace TeamOn.TeamScreen
{
    public class ClientObject
    {
        public ClientObject(TcpClient client, ConnectInfo info)
        {
            Client = client;
            Info = info;
            info.ClientObject = this;
        }

        TcpClient Client;
        Thread th;
        public ConnectInfo Info;


        public List<CommandProcessor> Commands = new List<CommandProcessor>();

        public ImgEndCommandProcessor ImgProc;
        public void InitCommands()
        {
            Commands.Add(new PingCommandProcessor());
            Commands.Add(new FrameCommandProcessor());
            Commands.Add(new ChunkCommandProcessor());
            ImgProc = new ImgEndCommandProcessor();
            Commands.Add(ImgProc);
            ImgProc.ImageCaptured += ImgProc_ImageCaptured;

        }

        private void ImgProc_ImageCaptured(System.Drawing.Bitmap obj)
        {
            TeamScreenServer.OnImageCaptured(obj);
        }

        public StreamReader rdr;
        public StreamWriter wrt;
        public void Process()
        {
            InitCommands();

            th = new Thread(() =>
            {
                try
                {
                    var info = Info;
                    info.ConnectTimestamp = DateTime.Now;

                    var stream = Client.GetStream();
                    rdr = new StreamReader(stream);
                    wrt = new StreamWriter(stream);
                    CommandContext cctx = new CommandContext();
                    cctx.Info = info;
                    cctx.Writer = wrt;
                    cctx.Reader = rdr;
                    cctx.Ctx = this;

                    while (true)
                    {
                        var str = rdr.ReadLine();
                        if (str == null) break;
                        cctx.Command = str;

                        foreach (var item in Commands)
                        {
                            if (item.Process(cctx)) break;
                        }
                    }
                }
                catch (Exception ex)
                {
                    lock (TeamScreenServer.Infos)
                    {
                        TeamScreenServer.Infos.Remove(TeamScreenServer.Infos.First(z => z.Ip == Info.Ip));
                    }
                }
            });
            th.IsBackground = true;
            th.Start();
        }
    }
}
