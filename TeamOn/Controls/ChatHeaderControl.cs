using System;
using System.Drawing;

namespace TeamOn.Controls
{
    public class ChatHeaderControl : Panel
    {
        public override Rectangle? GetRectangleOfChild(UIElement elem)
        {
            var bound = Parent.GetRectangleOfChild(this).Value;
            if (elem == connectButton)
            {
                return new Rectangle(bound.Right - 200, bound.Y + 10, 100, 20);
            }
            return null;
        }

        public ChatHeaderControl()
        {
            connectButton = new UIButton() { Text = "connect to screen", Parent = this, HoveredBrush = Brushes.ForestGreen };
        }

        UIButton connectButton;

        int animationDotsNum = 0;
        int animationCounter = 0;
        public override void Draw(DrawingContext ctx)
        {
            Visible = ChatMessageAreaControl.CurrentChat != null;
            if (!Visible) return;
            var bound = Parent.GetRectangleOfChild(this).Value;
            ctx.Graphics.FillRectangle(Brushes.LightSeaGreen, bound);
            if (ChatMessageAreaControl.CurrentChat != null)
            {
                ctx.Graphics.DrawString(ChatMessageAreaControl.CurrentChat.Name, new Font("Arial", 18, FontStyle.Bold), Brushes.White, bound.X + 5, bound.Y + 5);
                if (ChatMessageAreaControl.CurrentChat is OnePersonChatItem op)
                {
                    if (DateTime.Now.Subtract(op.LastTyping).TotalSeconds < 2)
                    {
                        ctx.Graphics.DrawString("typing text ", new Font("Arial",10), Brushes.White, bound.X, bound.Bottom - 20);
                        for (int i = 0; i < animationDotsNum; i++)
                        {
                            ctx.Graphics.FillEllipse(Brushes.White, bound.X + 100+i*20, bound.Bottom - 15, 10, 10);
                        }
                        animationCounter++;
                        if (animationCounter > 10)
                        {
                            animationDotsNum++;
                            animationDotsNum %= 4;
                            animationCounter = 0;
                        }
                    }
                }
                if (!(ChatMessageAreaControl.CurrentChat is GroupChatItem))
                    connectButton.Draw(ctx);
            }
        }

        public override void Event(UIEvent ev)
        {

        }
    }
}
