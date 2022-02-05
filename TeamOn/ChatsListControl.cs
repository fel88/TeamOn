using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using TeamOn.Controls;

namespace TeamOn
{
    public class ChatsListControl : UIElement
    {
        public List<ChatItem> Chats = new List<ChatItem>();
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

                yy += chatHeight;
            }
        }

        public override void Event(UIEvent ev)
        {
            if(ev is UIMouseButtonDown mb)
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
                        break;
                    }
                    yy += chatHeight;
                }

                
            }
        }
    }
   
}
