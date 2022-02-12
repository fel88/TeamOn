using System;
using System.Drawing;
using System.Text;

namespace TeamOn.Controls
{
    public class ChatHeaderControl : UIPanel
    {
        public override Rectangle? GetRectangleOfChild(UIElement elem)
        {
            var bound = Parent.GetRectangleOfChild(this).Value;
            if (elem == connectButton)
            {
                return new Rectangle(bound.Right - 200, bound.Y + 10, 100, 20);
            }
            if (elem == addMembers)
            {
                return new Rectangle(bound.Right - 200, bound.Y + 10, 50, 20);
            }
            if (elem == leaveGroup)
            {
                return new Rectangle(bound.Right - 250, bound.Y + 10, 50, 20);
            }
            if (elem == editGroup)
            {
                return new Rectangle(bound.Right - 300, bound.Y + 10, 50, 20);
            }

            return null;
        }

        public ChatHeaderControl()
        {
            connectButton = new UIButton() { Text = "connect to screen", Parent = this, HoveredBrush = Brushes.ForestGreen };
            leaveGroup = new UIButton() { Text = "leave", Parent = this, HoveredBrush = Brushes.ForestGreen };
            addMembers = new UIButton() { Text = "add", Parent = this, HoveredBrush = Brushes.ForestGreen };
            editGroup = new UIButton() { Text = "edit", Parent = this, HoveredBrush = Brushes.ForestGreen };

            editGroup.Click = (x) =>
            {
                (this.FindParent<RootElement>() as RootElement).SwitchToGroupEdit(ChatMessageAreaControl.CurrentChat as GroupChatItem);
            };
            addMembers.Click = (x) =>
            {
                (this.FindParent<RootElement>() as RootElement).SwitchToControl(new GroupAddMembersControl(ChatMessageAreaControl.CurrentChat as GroupChatItem));
            };

            AddElement(connectButton);
            AddElement(addMembers);
            AddElement(leaveGroup);
            AddElement(editGroup);
        }

        UIButton connectButton;

        UIButton leaveGroup;
        UIButton addMembers;
        UIButton editGroup;

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
                StringBuilder sb = new StringBuilder();
                sb.Append(ChatMessageAreaControl.CurrentChat.Name);
                if (ChatMessageAreaControl.CurrentChat is GroupChatItem gg)
                {
                    if (gg.Owner.Name == ChatMessageAreaControl.CurrentUser.Name)
                        sb.Append($" (owner)");
                }
                ctx.Graphics.DrawString(sb.ToString(), new Font("Arial", 18, FontStyle.Bold), Brushes.White, bound.X + 5, bound.Y + 5);
                if (ChatMessageAreaControl.CurrentChat is OnePersonChatItem op)
                {
                    if (DateTime.Now.Subtract(op.LastTyping).TotalSeconds < 2)
                    {
                        ctx.Graphics.DrawString("typing text ", new Font("Arial", 10), Brushes.White, bound.X, bound.Bottom - 20);
                        for (int i = 0; i < animationDotsNum; i++)
                        {
                            ctx.Graphics.FillEllipse(Brushes.White, bound.X + 100 + i * 20, bound.Bottom - 15, 10, 10);
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
                if ((ChatMessageAreaControl.CurrentChat is GroupChatItem))
                {
                    editGroup.Draw(ctx);
                    addMembers.Draw(ctx);
                    leaveGroup.Draw(ctx);
                }
            }
        }

        public override void Event(UIEvent ev)
        {
            if (!(ChatMessageAreaControl.CurrentChat is GroupChatItem))
                connectButton.Event(ev);
            if ((ChatMessageAreaControl.CurrentChat is GroupChatItem))
            {
                editGroup.Event(ev);
                addMembers.Event(ev);
                leaveGroup.Event(ev);
            }
        }
    }
}
