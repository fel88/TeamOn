using System.Drawing;

namespace TeamOn.Controls
{
    public class TwoColumnPanel : UIPanel
    {
        public bool FirstPanelFixed;
        public int FirstPanelWidth = 250;
        public override Rectangle? GetRectangleOfChild(UIElement elem)
        {

            var idx = Elements.IndexOf(elem);
            
            if (idx == 0)
            {
                if (FirstPanelFixed)
                {
                    return new Rectangle(Rect.Left, Rect.Y, (int)FirstPanelWidth, Rect.Height);
                }
                return new Rectangle(Rect.Left, Rect.Y, Rect.Width / 2, Rect.Height);
            }
            if (FirstPanelFixed)
            {
                return new Rectangle(Rect.Left + FirstPanelWidth, Rect.Y, Rect.Width - FirstPanelWidth, Rect.Height);

            }
            return new Rectangle(Rect.Left + Rect.Width / 2, Rect.Y, Rect.Width - FirstPanelWidth, Rect.Height);

        }

    }
}
