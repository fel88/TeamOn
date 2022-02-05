using System.Drawing;

namespace TeamOn.Controls
{
    public class RootElement : Controls.Panel
    {
        public override Rectangle? GetRectangleOfChild(UIElement elem)
        {
            return Rect;
        }
    }
}
