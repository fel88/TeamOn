using System.Drawing;
using TeamOn.Controls;

namespace TeamOn
{
    public class MemberAddItem : UIPanel
    {
        public MemberAddItem(OnePersonChatItem chat, bool selected)
        {
            _chat = chat;
            checkBox = new UICheckBox() { AbsoluteRectPosition = true, Text = _chat.Name ,Checked=selected};
            AddElement(checkBox);
        }

        public OnePersonChatItem Chat => _chat;
        public bool IsSelected => checkBox.Checked;
            
        UICheckBox checkBox;
        private readonly OnePersonChatItem _chat;

        public override Rectangle? GetRectangleOfChild(UIElement elem)
        {
            if (elem is UICheckBox)
            {
                return GetBound();
            }
            return base.GetRectangleOfChild(elem);
        }
        public override void Draw(DrawingContext ctx)
        {
            var bound = GetBound();
            if (bound.Contains(ctx.GetCursor()))
            {
                ctx.Graphics.FillRectangle(Brushes.LightGreen, bound);
            }
            ctx.Graphics.DrawRectangle(Pens.Black, bound);

            foreach (var item in Elements)
            {
                item.Draw(ctx);
            }
            //ctx.Graphics.DrawString(_chat.Name, SystemFonts.DefaultFont, Brushes.Blue, bound.X, bound.Y);

        }
    }
}
