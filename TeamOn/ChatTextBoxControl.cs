using System;
using System.Drawing;
using System.Windows.Forms;
using TeamOn.Controls;

namespace TeamOn
{
    public class ChatTextBoxControl : UITextBox
    {
        public ChatTextBoxControl()
        {
            WatermarkText = "Enter message..";
            ImagePastedAllowed = true;
        }

        public override void Draw(DrawingContext ctx)
        {
            Visible = ChatMessageAreaControl.CurrentChat != null;
            base.Draw(ctx);
        }

        public override void Event(UIEvent ev)
        {
            base.Event(ev);
            var client = ChatClient.Instance;
            if (ev is UIKeyDown kd)
            {
                if (kd.Key.KeyCode == Keys.Enter)
                {
                    if (client == null || !client.Connected) return;
                    client.SendMsg(Text, (ChatMessageAreaControl.CurrentChat as OnePersonChatItem).Person.Name);
                    //var cc = FindParent<ChatControl>() as ChatControl;
                    //var ma = cc.Elements[1] as ChatMessageAreaControl;
                    if (BitmapContent != null)
                    {
                        ChatMessageAreaControl.CurrentChat.AddMessage(new ImageChatMessage() { Owner = ChatMessageAreaControl.CurrentUser, Thumbnail = BitmapContent, DateTime = DateTime.Now });
                    }
                    else
                    {
                        ChatMessageAreaControl.CurrentChat.AddMessage(new TextChatMessage(DateTime.Now, Text) { Owner = ChatMessageAreaControl.CurrentUser });
                    }

                    Text = string.Empty;
                    BitmapContent = null;
                    curretPosition = 0;

                }
            }
        }
    }
}
