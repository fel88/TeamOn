using System.Drawing;
using TeamOn.Controls;

namespace TeamOn
{
    public class SettingsControl : Controls.Panel
    {
        public SettingsControl()
        {
            allowConnect.Parent = this;
            allowConnect.Text = "Allow remote connects";
            allowConnect.CheckedChanged = (x) =>
            {
                TeamScreen.TeamScreenServer.AllowConnects = allowConnect.Checked;
                if (TeamScreen.TeamScreenServer.AllowConnects)
                {
                    TeamScreen.TeamScreenServer.event1.Set();
                }
            };
        }
        UICheckBox allowConnect = new UICheckBox();

        public override void Draw(DrawingContext ctx)
        {
            allowConnect.Draw(ctx);
            base.Draw(ctx);
        }
        public override void Event(UIEvent ev)
        {
            allowConnect.Event(ev);

        }
        public override Rectangle? GetRectangleOfChild(UIElement elem)
        {
            var bound = Parent.GetRectangleOfChild(this).Value;
            if (elem == allowConnect)
            {
                return new Rectangle(bound.X+20, bound.Y+20, 100, 30);
            }
            return base.GetRectangleOfChild(elem);
        }
    }

}
