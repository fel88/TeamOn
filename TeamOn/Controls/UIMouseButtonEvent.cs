using System.Drawing;
using System.Windows.Forms;

namespace TeamOn.Controls
{
    public class UIMouseButtonEvent : UIEvent
    {
        public Point Position;
        public MouseButtons Button;
    }

    public class UIFocusChangedEvent : UIEvent
    {
        public IFocusContainer NewFocusOwner;
    }
}
