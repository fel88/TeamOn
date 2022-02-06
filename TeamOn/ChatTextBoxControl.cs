using System;
using System.Drawing;
using System.Linq;
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
            TextChanged = (x) =>
            {
                var client = ChatClient.Instance;
                ChatMessageAreaControl.CurrentChat.NewMessagesCounter = 0;
                if (!(client == null || !client.Connected))
                    client.SendTyping((ChatMessageAreaControl.CurrentChat as OnePersonChatItem).Person.Name);
            };
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
                if (kd.Key.KeyCode == Keys.Up)
                {
                    var last = ChatMessageAreaControl.CurrentChat.Messages.Last();
                    if (last is TextChatMessage tcm)
                    {
                        Text = tcm.Text;
                    }
                }
                if (kd.Key.KeyCode == Keys.Enter)
                {
                    if (client == null || !client.Connected) return;
                    if (!string.IsNullOrEmpty(Text) && Text.Replace("\r", "").Replace("\n", "").Length != 0)
                    {
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
}