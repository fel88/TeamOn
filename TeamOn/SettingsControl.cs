using System;
using System.Drawing;
using TeamOn.Controls;

namespace TeamOn
{
    public class SettingsControl : UIPanel
    {
        public SettingsControl()
        {
            AddElement(allowConnect);
            AddElement(nicknameLabel);
            AddElement(nickNameTextBox);
            AddElement(serverIpTextBox);
            AddElement(serverIpLabel);
            AddElement(serverPortTextBox);
            AddElement(serverPortLabel);
            AddElement(permanentChats);

            nicknameLabel.Text = "Nickname";
            serverIpLabel.Text = "Server IP";
            serverPortLabel.Text = "Server port";

            allowConnect.Text = "Allow remote connects";
            permanentChats.Text = "Permanent chats";
            nickNameTextBox.Text = Settings.Nickname;
            serverIpTextBox.Text = Settings.ServerIP;
            serverPortTextBox.Text = Settings.ServerPort.ToString();
            allowConnect.Checked = TeamScreen.TeamScreenServer.AllowConnects;
            permanentChats.Checked = Settings.PermanentChats;

            nickNameTextBox.TextChanged = (x) =>
            {
                Settings.Nickname = x.Text;
            };
            serverIpTextBox.TextChanged = (x) =>
            {
                Settings.ServerIP = x.Text;
            };
            serverPortTextBox.TextChanged = (x) =>
            {
                try
                {
                    
                    
                    Settings.ServerPort = int.Parse(x.Text);
                    serverPortTextBox.BackColor = Color.White;
                    serverPortTextBox.ForeColor = Color.Black;
                }
                catch (Exception ex)
                {
                    serverPortTextBox.BackColor = Color.Red;
                    serverPortTextBox. ForeColor= Color.White;
                }
            };

            allowConnect.CheckedChanged = (x) =>
            {
                TeamScreen.TeamScreenServer.AllowConnects = allowConnect.Checked;
                if (TeamScreen.TeamScreenServer.AllowConnects)
                {
                    TeamScreen.TeamScreenServer.event1.Set();
                }
            }; 
            
            permanentChats.CheckedChanged = (x) =>
            {
                Settings.PermanentChats = permanentChats.Checked;                
            };
        }

        UICheckBox allowConnect = new UICheckBox();
        UICheckBox permanentChats = new UICheckBox();
        UITextBox nickNameTextBox = new UITextBox();
        UILabel serverIpLabel = new UILabel();
        UITextBox serverIpTextBox = new UITextBox();
        UILabel serverPortLabel = new UILabel();
        UITextBox serverPortTextBox = new UITextBox();
        UILabel nicknameLabel = new UILabel();

        public override Rectangle? GetRectangleOfChild(UIElement elem)
        {
            var bound = Parent.GetRectangleOfChild(this).Value;
            if (elem == allowConnect)
            {
                return new Rectangle(bound.X + 20, bound.Y + 20, 20, 20);
            }
            if (elem == nickNameTextBox)
            {
                return new Rectangle(bound.X + 100, bound.Y + 60, 100, 20);
            }
            if (elem == nicknameLabel)
            {
                return new Rectangle(bound.X + 20, bound.Y + 60, 100, 20);
            }
            if (elem == serverIpTextBox)
            {
                return new Rectangle(bound.X + 100, bound.Y + 100, 100, 20);
            }
            if (elem == serverIpLabel)
            {
                return new Rectangle(bound.X + 20, bound.Y + 100, 100, 20);
            }
            if (elem == serverPortTextBox)
            {
                return new Rectangle(bound.X + 100, bound.Y + 140, 100, 20);
            }
            if (elem == serverPortLabel)
            {
                return new Rectangle(bound.X + 20, bound.Y + 140, 100, 20);
            }
            if (elem == permanentChats)
            {
                return new Rectangle(bound.X + 20, bound.Y + 180, 100, 20);
            }
            return base.GetRectangleOfChild(elem);
        }
    }

}
