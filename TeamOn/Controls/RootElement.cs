using System.Collections.Generic;
using System.Drawing;

namespace TeamOn.Controls
{
    public class RootElement : Panel
    {
        public RootElement()
        {
            Instance = this;
        }


        public void CaptureFocus(IFocusContainer f)
        {

            foreach (var item in FocusContainers)
            {
                item.Focused = false;
                item.LostFocus();
            }
            f.Focused = true;
            CurrentFocusOwner = f;
        }
        public void RegisterFocusContainer(IFocusContainer c)
        {
            FocusContainers.Add(c);
        }

        public List<IFocusContainer> FocusContainers = new List<IFocusContainer>();
        public IFocusContainer CurrentFocusOwner;

        public static RootElement Instance;

        public override Rectangle? GetRectangleOfChild(UIElement elem)
        {
            return Rect;
        }
    }
}
