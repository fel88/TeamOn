using System;
using System.Linq;
using System.Threading;

namespace TeamOn.TeamScreen
{
    public class ChunkCommandProcessor : CommandProcessor
    {
        public static byte[] Data;
        public static bool IsDelta;
        public override bool Process(CommandContext ctx)
        {
            var str = ctx.Command;
            if (str.StartsWith("CHUNK"))
            {
                Interlocked.Add(ref TeamScreenServer.CommandsProcessed, 1);

                var ind1 = str.IndexOf('=');
                var sub = str.Substring(ind1 + 1);
                var ar = sub.Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries).ToArray();

                int did = int.Parse(ar[0]);

                int btsl = int.Parse(ar[1]);
                int arrl = int.Parse(ar[2]);
                int shift = int.Parse(ar[3]);
                string b64 = ar[4];

                var data = Convert.FromBase64String(b64);
                if (Data == null)
                {
                    Data = new byte[btsl];
                }
                for (int i = 0; i < arrl; i++)
                {
                    Data[i + shift] = data[i];
                }

                string md5 = ar[5];

                var _md5 = Stuff.CreateMD5(b64);
                if (md5 == _md5)
                {
                    //  ctx.Writer.WriteLine("OK");
                    ctx.Writer.Flush();
                }
                else
                {
                    //ctx.Writer.WriteLine("MD5FAIL");
                    ctx.Writer.Flush();
                }
                return true;
            }
            return false;
        }
    }
}
