using System.Drawing;
using System.Windows.Forms;
using TeamOn.Controls;

namespace TeamOn
{
    public class ChatTextBoxControl : UITextBox
    {

        public override void Event(UIEvent ev)
        {
            base.Event(ev);
            var client = ChatClient.Instance;
            if (ev is UIKeyDown kd)
            {
                if (kd.Key.KeyCode == Keys.Enter)
                {
                    if (client == null || !client.Connected) return;
                    client.SendMsg(Text);
                    //var cc = FindParent<ChatControl>() as ChatControl;
                    //var ma = cc.Elements[1] as ChatMessageAreaControl;
                    ChatMessageAreaControl.CurrentChat.Messages.Add(new TextChatMessage() { Owner = ChatMessageAreaControl.CurrentUser, Text = Text });
                    Text = string.Empty;
                    curretPosition = 0;
                    
                }
            }
        }
    }
}
