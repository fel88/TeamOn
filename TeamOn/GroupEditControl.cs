using System.Drawing;
using System.Linq;
using TeamOn.Controls;

namespace TeamOn
{
    public class GroupEditControl : UIPanel
    {
        public GroupEditControl()
        {
            AddElement(nameLabel);
            AddElement(nameTextBox);
            AddElement(acceptButton);

            acceptButton.Click = (x) =>
            {
                bool valid = true;
                if (string.IsNullOrWhiteSpace(nameTextBox.Text))
                {
                    valid = false;
                }

                if (valid)
                {
                    if (_group == null)
                    {
                        if (!ChatsListControl.Chats.OfType<GroupChatItem>().Any(z => z.Name == nameTextBox.Text))
                        {
                            ChatsListControl.Chats.Add(new GroupChatItem() { Name = nameTextBox.Text, Owner = ChatMessageAreaControl.CurrentUser });
                        }
                        else
                        {
                            valid = false;
                        }

                    }
                    else
                    {
                        _group.Name = nameTextBox.Text;
                    }

                    nameTextBox.BackColor = Color.White;
                    (x.FindParent<RootElement>() as RootElement).SwitchToChat();                    
                }
                else
                {
                    nameTextBox.BackColor = Color.Red;
                }
            };
        }

        UILabel nameLabel = new UILabel() { Text = "Name" };
        UITextBox nameTextBox = new UITextBox();
        UIButton acceptButton = new UIButton() { Text = "Accept" };

        public override Rectangle? GetRectangleOfChild(UIElement elem)
        {
            var bound = GetBound();
            int topGap = 20;
            if (elem == nameLabel)
            {
                return new Rectangle(bound.X, bound.Y + topGap, bound.Width, 25);
            }
            if (elem == nameTextBox)
            {
                return new Rectangle(bound.X + 100, bound.Y + topGap, 150, 25);
            }
            if (elem == acceptButton)
            {
                return new Rectangle(bound.X + 100, bound.Bottom - 30, 150, 25);
            }
            return base.GetRectangleOfChild(elem);
        }


        GroupChatItem _group;
        public void Init(GroupChatItem group)
        {
            _group = group;
            if (group != null)
                nameTextBox.SetText(group.Name);
            else
            {
                nameTextBox.SetText(string.Empty);
            }
        }
    }
}
