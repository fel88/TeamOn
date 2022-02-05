using System;
using System.Drawing;
using System.Windows.Forms;

namespace TeamOn.Controls
{
    public class Button : UIElement
    {
        public override void Draw(DrawingContext ctx)
        {
            var cursor = ctx.GetCursor();
            if (Rect.Contains(cursor))
            {
                ctx.Graphics.FillRectangle(Brushes.AliceBlue, Rect);
            }
            ctx.Graphics.DrawRectangle(Pens.Black, Rect);

        }
        public Action<Button> Click;
        public override void Event(UIEvent ev)
        {
            if (ev.Handled) return;
            if (ev is UIMouseButtonDown bd)
            {
                if (bd.Button == MouseButtons.Left)
                {
                    if (Rect.Contains(bd.Position))
                    {
                        Click?.Invoke(this);
                    }
                }
            }
        }
    }
}
