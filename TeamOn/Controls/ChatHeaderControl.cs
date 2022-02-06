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

        public override void Draw(DrawingContext ctx)
        {
            Visible = ChatMessageAreaControl.CurrentChat != null;
            if (!Visible) return;
            var bound = Parent.GetRectangleOfChild(this).Value;
            ctx.Graphics.FillRectangle(Brushes.LightSeaGreen, bound);
            if (ChatMessageAreaControl.CurrentChat != null)
            {
                ctx.Graphics.DrawString(ChatMessageAreaControl.CurrentChat.Name, new Font("Arial", 18, FontStyle.Bold), Brushes.White, bound.X + 5, bound.Y + 5);

                if (!(ChatMessageAreaControl.CurrentChat is GroupChatItem))
                    connectButton.Draw(ctx);
            }
        }

        public override void Event(UIEvent ev)
        {

        }
    }
}
