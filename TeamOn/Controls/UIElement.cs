using System.Drawing;
using System.Windows.Forms;

namespace TeamOn.Controls
{
    public abstract class UIElement
    {
        public bool Visible = true;
        public Panel Parent;
        public Rectangle Rect;

        public Rectangle GetBound()
        {
            return Parent.GetRectangleOfChild(this).Value;
        }
        public abstract void Draw(DrawingContext ctx);

        public abstract void Event(UIEvent ev);

        
        public UIElement FindParent<T>() 
        {
            var parent = Parent;
            while (parent != null)
            {
                if (parent is T) return parent;
                parent = parent.Parent;
            }
            return null;
        }
    }
}
