using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using TeamOn.Controls;

namespace TeamOn
{
    public class GroupAddMembersControl : RowsPanel
    {
        private readonly GroupChatItem _group;
        public GroupAddMembersControl(GroupChatItem group)
        {
            _group = group;

            AddElement(rowPanel);
            var panel1 = new RowsPanel() { BackColor = Color.Violet };
            panel1.Styles.Add(new RowColumnPanelStyle() { Size = 100, Type = SizeType.Percent });
            panel1.AddElement(new UIButton()
            {
                Text = "Add",
                Click = (x) =>
                {
                    group.Users.Clear();
                    var chats = rowPanel.Elements.OfType<MemberAddItem>().Where(z => z.IsSelected).Select(z => z.Chat);
                    group.Users.AddRange(chats.Select(z => z.Person));

                    (FindParent<RootElement>() as RootElement).BackToControl();
                }
            });
            AddElement(panel1);
            Styles.Add(new RowColumnPanelStyle() { Size = 100, Type = SizeType.Percent });
            Styles.Add(new RowColumnPanelStyle() { Size = 30, Type = SizeType.Absolute });
            foreach (var item in ChatsListControl.Chats.OfType<OnePersonChatItem>())
            {
                rowPanel.Styles.Add(new RowColumnPanelStyle() { Type = SizeType.Absolute, Size = 30 });
                rowPanel.AddElement(new MemberAddItem(item, group.Users.Any(z => z.Name == item.Person.Name)));
            }
        }

        RowsPanel rowPanel = new RowsPanel();
    }
}
