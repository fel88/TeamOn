using System;
using System.Drawing;
using System.Windows.Forms;

namespace TeamOn.Controls
{
    public class UIButton : UIElement
    {
        public override void Draw(DrawingContext ctx)
        {
            var bound = Parent.GetRectangleOfChild(this).Value;
            var cursor = ctx.GetCursor();
            if (bound.Contains(cursor))
            {
                ctx.Graphics.FillRectangle(HoveredBrush, bound);
            }
            ctx.Graphics.DrawRectangle(Pens.Black, bound);
            if(!string.IsNullOrEmpty(Text))
            {
                ctx.Graphics.DrawString(Text, SystemFonts.DefaultFont, Brushes.White, bound.X, bound.Y);
            }

        }
        public Brush HoveredBrush = Brushes.AliceBlue;
        public string Text;
        public Action<UIButton> Click;
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
