using System.Drawing;
using System.Windows.Forms;
using TeamOn.Controls;

namespace TeamOn
{
    public class MinimizeButton : UIButton

    {
        public MinimizeButton(Form owner)
        {
            Click = (x) =>
            {

                owner.WindowState = FormWindowState.Minimized;

            };
        }

        public override void Draw(DrawingContext ctx)
        {
            var cursor = ctx.GetCursor();
            int gap = 3;
            if (Rect.Contains(cursor))
            {
                ctx.Graphics.FillRectangle(Brushes.AliceBlue, Rect);
                ctx.Graphics.DrawLine(new Pen(Color.Red, 3), Rect.X + gap, Rect.Bottom - gap, Rect.Right - gap, Rect.Bottom - gap);
            }
            else
            {
                ctx.Graphics.DrawLine(new Pen(Color.LightGray, 3), Rect.X + gap, Rect.Bottom - gap, Rect.Right - gap, Rect.Bottom - gap);
            }
            ctx.Graphics.DrawRectangle(Pens.Black, Rect);
        }
    }
   
}
