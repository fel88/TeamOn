using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Xml.Linq;

namespace TeamOnServer
{
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
                        if (string.IsNullOrWhiteSpace(msg))
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine("[init] reject empty user name ");
                            Console.ForegroundColor = ConsoleColor.White;
                            //reject connection
                            stream.Close();
                            break;
                        }
                        uinfo.Name = msg;

                        if (streams.Where(z => z.Tag != uinfo).Select(z => z.Tag as UserInfo).Any(z => z.Name == msg))
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine("[init] reject duplicate user name = " + msg);
                            Console.ForegroundColor = ConsoleColor.White;
                            //reject connection
                            stream.Close();
                            break;
                        }
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine("user init = " + msg);
                        Console.ForegroundColor = ConsoleColor.White;
                        NewClient();

                    }
                    else if (line.StartsWith("MSG"))
                    {

                        var ind = line.IndexOf("=");
                        var msg = line.Substring(ind + 1);
                        var spl = msg.Split(';').ToArray();
                        var target = spl[1];

                        var bs64 = Convert.FromBase64String(spl[0]);
                        var str = Encoding.UTF8.GetString(bs64);


                        StringBuilder sb = new StringBuilder();
                        sb.AppendLine("<?xml version=\"1.0\"?>");
                        sb.AppendLine("<root>");
                        sb.AppendLine(string.Format("<message user=\"{0}\" target=\"{1}\">", uinfo.Name, target));
                        sb.AppendFormat("<![CDATA[{0}]]>", str);
                        sb.AppendLine(string.Format("</message>", uinfo.Name, str));
                        sb.AppendLine("</root>");

                        var estr = sb.ToString();


                        var bt = Encoding.UTF8.GetBytes(estr);

                        var ree = Convert.ToBase64String(bt);

                        this.SendTo("MSG=" + ree, target);
                    }
                    else if (line.StartsWith("GINFO"))
                    {

                        var ind = line.IndexOf("=");
                        var msg = line.Substring(ind + 1);

                        var bs64 = Convert.FromBase64String(msg);
                        var str = Encoding.UTF8.GetString(bs64);
                        var doc = XDocument.Parse(str);
                        var root = doc.Element("root");
                        var gnm = root.Attribute("name").Value;

                        if (!Groups.Any(z => z.Name == gnm))
                        {
                            var gi = new GroupInfo() { Name = gnm, Owner = uinfo };
                            Groups.Add(gi);

                            //skip
                        }

                        var cgrp = Groups.First(z => z.Name == gnm);
                        if (cgrp.Owner.Name == uinfo.Name)
                        {
                            List<string> all = new List<string>();
                            all.Add(uinfo.Name);
                            foreach (var item in root.Elements("user"))
                            {
                                var un = item.Attribute("name").Value;
                                all.Add(un);
                                if (!cgrp.Users.Any(z => z.Name == un))
                                {
                                    cgrp.Users.Add(new UserInfo() { Name = un });
                                }
                            }
                            if (!cgrp.Users.Any(z => z.Name == uinfo.Name))
                            {
                                cgrp.Users.Add(uinfo);
                            }
                            var toDel = cgrp.Users.Where(z => !all.Contains(z.Name)).ToArray();
                            var cnt = cgrp.Users.RemoveAll(z => toDel.Any(u => u.Name == z.Name));

                            StringBuilder sb = new StringBuilder();
                            sb.AppendLine("<?xml version=\"1.0\"?>");
                            sb.AppendLine("<root>");
                            sb.AppendLine($"<group owner=\"{uinfo.Name}\" name=\"{cgrp.Name}\" >");
                            foreach (var item in cgrp.Users)
                            {
                                sb.AppendLine($"<user name=\"{item.Name}\"/>");
                            }
                            sb.AppendLine(string.Format("</group>"));
                            sb.AppendLine("</root>");

                            var estr = sb.ToString();


                            var bt = Encoding.UTF8.GetBytes(estr);

                            var ree = Convert.ToBase64String(bt);

                            this.SendTo("GINFO=" + ree, cgrp.Users.Where(z => z.Name != uinfo.Name).Select(z => z.Name).ToArray());
                        }
                    }
                    else if (line.StartsWith("GMSG"))
                    {

                        var ind = line.IndexOf("=");
                        var msg = line.Substring(ind + 1);
                        var spl = msg.Split(';').ToArray();
                        var target = spl[1];

                        var bs64 = Convert.FromBase64String(spl[0]);
                        var str = Encoding.UTF8.GetString(bs64);

                        var cgrp = Groups.First(z => z.Name == target);
                        StringBuilder sb = new StringBuilder();
                        sb.AppendLine("<?xml version=\"1.0\"?>");
                        sb.AppendLine("<root>");
                        sb.AppendLine($"<message user=\"{uinfo.Name}\" group=\"{cgrp.Name}\">");
                        sb.AppendFormat("<![CDATA[{0}]]>", str);
                        sb.AppendLine(string.Format("</message>", uinfo.Name, str));
                        sb.AppendLine("</root>");

                        var estr = sb.ToString();


                        var bt = Encoding.UTF8.GetBytes(estr);
                        var ree = Convert.ToBase64String(bt);

                        this.SendTo("GMSG=" + ree, cgrp.Users.Where(z => z.Name != uinfo.Name).Select(z => z.Name).ToArray());

                    }
                    else if (line.StartsWith("TYPING"))
                    {

                        var ind = line.IndexOf("=");
                        var msg = line.Substring(ind + 1);
                        var spl = msg.Split(';').ToArray();
                        var target = spl[1];

                        StringBuilder sb = new StringBuilder();
                        sb.AppendLine("<?xml version=\"1.0\"?>");
                        sb.AppendLine("<root>");
                        sb.AppendLine(string.Format("<message user=\"{0}\" target=\"{1}\">", uinfo.Name, target));
                        sb.AppendLine("</message>");
                        sb.AppendLine("</root>");

                        var estr = sb.ToString();


                        var bt = Encoding.UTF8.GetBytes(estr);

                        var ree = Convert.ToBase64String(bt);

                        this.SendTo("TYPING=" + ree, target);
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
                        var grpf = Groups.FirstOrDefault(y => y.Name == targ);
                        if (grpf != null)
                        {                            
                            this.SendTo(line, grpf.Users.Where(z => z.Name != uinfo.Name).Select(z => z.Name).ToArray());                            
                        }
                        else
                        {
                            var cxstr = this.streams.First(z => (z.Tag as UserInfo).Name == targ);
                            var wr = new StreamWriter(cxstr.Stream);
                            //2. retranslate to specific client web request
                            wr.WriteLine(line);
                            wr.Flush();

                            //server.SendAll(line);
                        }
                    }
                }
                catch (IOException iex)
                {
                    lock (streams)
                    {
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.WriteLine((cinf.Tag as UserInfo).Name + " - removed");
                        Console.ForegroundColor = ConsoleColor.White;
                        streams.Remove(cinf);
                    }
                    sendAllClientUpdates();
                    break;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    lock (streams)
                    {
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.WriteLine((cinf.Tag as UserInfo).Name + " - removed");
                        Console.ForegroundColor = ConsoleColor.White;
                        streams.Remove(cinf);
                    }
                    sendAllClientUpdates();
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
}
