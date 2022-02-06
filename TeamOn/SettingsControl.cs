using System.Drawing;
using TeamOn.Controls;

namespace TeamOn
{
    public class SettingsControl : Controls.Panel
    {
        public SettingsControl()
        {
            AddElement(allowConnect);
            AddElement(nicknameLabel);
            AddElement(nickNameTextBox);
            AddElement(serverIpTextBox);
            AddElement(serverIpLabel);

            nicknameLabel.Text = "Nickname";
            serverIpLabel.Text = "Server IP";

            allowConnect.Text = "Allow remote connects";
            nickNameTextBox.Text = Settings.Nickname;
            serverIpTextBox.Text = Settings.ServerIP;
            nickNameTextBox.TextChanged = (x) =>
            {
                Settings.Nickname = x.Text;
            }; 
            serverIpTextBox.TextChanged = (x) =>
            {
                Settings.ServerIP = x.Text;
            };

            allowConnect.CheckedChanged = (x) =>
            {
                TeamScreen.TeamScreenServer.AllowConnects = allowConnect.Checked;
                if (TeamScreen.TeamScreenServer.AllowConnects)
                {
                    TeamScreen.TeamScreenServer.event1.Set();
                }
            };
        }
        UICheckBox allowConnect = new UICheckBox();
        UITextBox nickNameTextBox = new UITextBox();
        UILabel serverIpLabel = new UILabel();
        UITextBox serverIpTextBox = new UITextBox();
        UILabel nicknameLabel = new UILabel();

        
        
        public override Rectangle? GetRectangleOfChild(UIElement elem)
        {
            var bound = Parent.GetRectangleOfChild(this).Value;
            if (elem == allowConnect)
            {
                return new Rectangle(bound.X + 20, bound.Y + 20, 100, 30);
            }
            if (elem == nickNameTextBox)
            {
                return new Rectangle(bound.X + 100, bound.Y + 60, 100, 30);
            }
            if (elem == nicknameLabel)
            {
                return new Rectangle(bound.X + 20, bound.Y + 60, 100, 30);
            }
            if (elem == serverIpTextBox)
            {
                return new Rectangle(bound.X + 100, bound.Y + 100, 100, 30);
            }
            if (elem == serverIpLabel)
            {
                return new Rectangle(bound.X + 20, bound.Y + 100, 100, 30);
            }
            return base.GetRectangleOfChild(elem);
        }
    }

}
