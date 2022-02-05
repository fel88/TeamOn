using System.Drawing;
using System.Windows.Forms;

namespace TeamOn.Controls
{
    public abstract class UIElement
    {
        public bool Visible = true;
        public Panel Parent;
        public Rectangle Rect;
        public abstract void Draw(DrawingContext ctx);

        public abstract void Event(UIEvent ev);
    }
}
