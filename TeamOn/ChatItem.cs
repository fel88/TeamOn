using System.Collections.Generic;
using TeamOn.Controls;

namespace TeamOn
{
    public abstract class ChatItem
    {
        public virtual string Name { get; set; }
        public List<ChatMessage> Messages = new List<ChatMessage>();
        public void AddMessage(ChatMessage msg)
        {
            Messages.Add(msg);
            if (msg.Owner.Name != ChatMessageAreaControl.CurrentUser.Name)
                NewMessagesCounter++;
        }
        public int NewMessagesCounter;
    }
}
