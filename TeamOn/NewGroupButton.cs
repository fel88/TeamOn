using System.Drawing;
using TeamOn.Controls;

namespace TeamOn
{
    public class NewGroupButton : UIButton
    {
        public NewGroupButton(Form1 owner)
        {
            Click = (x) =>
            {
                owner.SwitchLayoutGroupEdit();

            };
        }

        public override void Draw(DrawingContext ctx)
        {
            var icon = TeamOn.Properties.Resources.plus_octagon;

            var cursor = ctx.GetCursor();
            int gap = 3;
            if (Rect.Contains(cursor))
            {
                ctx.Graphics.FillRectangle(Brushes.AliceBlue, Rect);

            }
            else
            {

            }
            ctx.Graphics.DrawIcon(icon, Rect);

            //ctx.Graphics.DrawRectangle(Pens.Black, Rect);


        }
    }
}
