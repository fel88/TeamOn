using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using TeamOn.Controls;

namespace TeamOn
{
    public class ChatTextBoxControl : UITextBox
    {
        public ChatTextBoxControl()
        {
            Instance = this;
            WatermarkText = "Enter message..";
            ImagePastedAllowed = true;
            TextChanged = (x) =>
            {
                var client = ChatClient.Instance;
                ChatMessageAreaControl.CurrentChat.NewMessagesCounter = 0;
                if (!(client == null || !client.Connected))
                {
                    if ((ChatMessageAreaControl.CurrentChat is OnePersonChatItem))
                    {
                        client.SendTyping((ChatMessageAreaControl.CurrentChat as OnePersonChatItem).Person.Name);
                    }
                }
            };
        }

        public string[] AttachedFiles;

        public static ChatTextBoxControl Instance;
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
                    if (BitmapContent != null)
                    {
                        Directory.CreateDirectory("Sended");
                        var dt = DateTime.Now;
                        var name = $"sended-image {dt.Year}-{dt.Month}-{dt.Day} {dt.Hour}-{dt.Minute}-{dt.Second}.jpg";
                        var path = Path.Combine("Sended", name);
                        BitmapContent.Save(path, System.Drawing.Imaging.ImageFormat.Jpeg);
                        if (ChatMessageAreaControl.CurrentChat is OnePersonChatItem op)
                        {
                            client.SendImage(BitmapContent, op.Person.Name, progress: (perc) =>
                            {

                            });                            
                        }
                        else if (ChatMessageAreaControl.CurrentChat is GroupChatItem gc)
                        {
                            client.SendGroupInfo(gc);
                            client.SendImage(BitmapContent, gc.Name, progress: (perc) =>
                            {

                            });                            
                        }
                       

                        ChatMessageAreaControl.CurrentChat.AddMessage(new ImageLinkChatMessage() { Owner = ChatMessageAreaControl.CurrentUser, Path = path, DateTime = DateTime.Now });


                        Text = string.Empty;
                        BitmapContent = null;
                        curretPosition = 0;
                    }
                    if (!string.IsNullOrEmpty(Text) && Text.Replace("\r", "").Replace("\n", "").Length != 0)
                    {

                        //var cc = FindParent<ChatControl>() as ChatControl;
                        //var ma = cc.Elements[1] as ChatMessageAreaControl;

                        if (ChatMessageAreaControl.CurrentChat is OnePersonChatItem op)
                        {
                            client.SendMsg(Text, op.Person.Name);
                        }
                        else if (ChatMessageAreaControl.CurrentChat is GroupChatItem gc)
                        {
                            client.SendGroupInfo(gc);
                            client.SendGroupMsg(gc.Name, Text);
                        }
                        ChatMessageAreaControl.CurrentChat.AddMessage(new TextChatMessage(DateTime.Now, Text) { Owner = ChatMessageAreaControl.CurrentUser });


                        Text = string.Empty;
                        BitmapContent = null;
                        curretPosition = 0;
                    }
                }
            }
        }
    }
}