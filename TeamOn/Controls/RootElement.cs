using System;
using System.Collections.Generic;
using System.Drawing;

namespace TeamOn.Controls
{
    public class RootElement : UIPanel
    {
        public RootElement(Form1 form)
        {
            AbsoluteRectPosition = true;
            Instance = this;
            ownerForm = form;
        }

        Form1 ownerForm;

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

        internal void SwitchToChat()
        {
            ownerForm.SwitchToChat();
        }
        internal void BackToControl()
        {
            ownerForm.BackToControl();
        }
        internal void SwitchToGroupEdit(GroupChatItem group)
        {
            ownerForm.SwitchLayoutGroupEdit(group);
        }
        internal void SwitchToControl(UIPanel panel)
        {
            ownerForm.SwitchLayoutToControl(panel);
        }
    }
}
