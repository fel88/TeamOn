using System.Collections.Generic;
using System.Drawing;

namespace TeamOn.Controls
{
    public class UIPanel : UIElement
    {
        public virtual Rectangle? GetRectangleOfChild(UIElement elem)
        {
            return null;
        }

        public void AddElement(UIElement el)
        {
            el.Parent = this;
            Elements.Add(el);
        }
        public List<RowColumnPanelStyle> Styles = new List<RowColumnPanelStyle>();

        public List<UIElement> Elements = new List<UIElement>();
        public Color BackColor = Color.Transparent;
        public override void Draw(DrawingContext ctx)
        {
            var bound = Parent.GetRectangleOfChild(this).Value;
            if (BackColor != Color.Transparent)
            {
                ctx.Graphics.FillRectangle(new SolidBrush(BackColor), bound);
            }
            foreach (var item in Elements)
            {
                item.Draw(ctx);
            }
        }

        public override void Event(UIEvent ev)
        {
            foreach (var item in Elements.ToArray())
            {
                if (ev.Handled) break;
                item.Event(ev);
            }
        }
    }
}
