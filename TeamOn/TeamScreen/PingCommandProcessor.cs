namespace TeamOn.TeamScreen
{
    public class PingCommandProcessor : CommandProcessor
    {
        public override bool Process(CommandContext ctx)
        {
            var wrt = ctx.Writer;
            var str = ctx.Command;
            if (str.StartsWith("PING"))
            {
                wrt.WriteLine(true.ToString());
                wrt.Flush();
                return true;
            }
            return false;
        }
    }
}
