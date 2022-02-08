using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using TeamOn.Controls;

namespace TeamOn
{
    public class ChatsListControl : UIElement
    {
        public static List<ChatItem> Chats = new List<ChatItem>();
        public override void Draw(DrawingContext ctx)
        {
            var bound = Parent.GetRectangleOfChild(this).Value;
            ctx.Graphics.FillRectangle(Brushes.Khaki, bound);
            int yy = bound.Y;
            int chatHeight = 30;
            var pos = ctx.GetCursor();
            foreach (var item in Chats)
            {
                ctx.Graphics.FillRectangle(Brushes.White, bound.X, yy, bound.Width, chatHeight);
                if (item == ChatMessageAreaControl.CurrentChat)
                {
                    ctx.Graphics.FillRectangle(Brushes.LightCyan, bound.X, yy, bound.Width, chatHeight);
                }
                var rect = new Rectangle(bound.X, yy, bound.Width, chatHeight);
                if (rect.Contains(pos))
                {
                    ctx.SetTempCursor(Cursors.Hand);
                }
                ctx.Graphics.DrawRectangle(new Pen(Color.LightBlue, 1), bound.X, yy, bound.Width, chatHeight);
                ctx.Graphics.DrawString(item.Name, SystemFonts.DefaultFont, Brushes.Blue, 5, yy + 6);
                if (item is OnePersonChatItem op)
                {
                    ctx.Graphics.FillEllipse(op.Person.Online ? Brushes.LightGreen : Brushes.Orange, bound.Right - 20, yy + chatHeight/2-5, 10, 10);
                    ctx.Graphics.DrawEllipse(Pens.Black, bound.Right - 20, yy + chatHeight/2-5, 10, 10);
                }
                if (item is GroupChatItem gp)
                {
                    //ctx.Graphics.DrawIcon(TeamOn.Properties.Resources.pencil,bound.Right-20,yy+3);
                    
                }
                if (item.NewMessagesCounter > 0)
                {
                    ctx.Graphics.DrawString($"({item.NewMessagesCounter})", SystemFonts.DefaultFont, Brushes.Red, bound.Right - 50, yy + 3);
                }
                yy += chatHeight;
            }
        }

        public override void Event(UIEvent ev)
        {
            if (ev is UIMouseButtonDown mb)
            {
                var bound = Parent.GetRectangleOfChild(this).Value;

                int yy = bound.Y;
                int chatHeight = 30;

                foreach (var item in Chats)
                {
                    var rect = new Rectangle(bound.X, yy, bound.Width, chatHeight);
                    if (rect.Contains(mb.Position))
                    {
                        ChatMessageAreaControl.CurrentChat = item;
                        RootElement.Instance.SwitchToChat();
                        item.NewMessagesCounter = 0;
                        break;
                    }
                    yy += chatHeight;
                }


            }
        }
    }

}
