using System.Linq;

namespace TeamOn.TeamScreen
{
    public class FrameCommandProcessor : CommandProcessor
    {


        public override bool Process(CommandContext ctx)
        {
            var str = ctx.Command;
            if (str.StartsWith("FRAME"))
            {
                var ar1 = str.Split(new char[] { ';' }).ToArray();
                ChunkCommandProcessor.IsDelta = false;
                if (ar1[1] == "DELTA")
                {
                    ChunkCommandProcessor.IsDelta = true;
                }

                return true;
            }
            return false;
        }
    }
}
