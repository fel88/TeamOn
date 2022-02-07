using System;
using System.Drawing;
using System.Windows.Forms;

namespace TeamOn.Controls
{
    public class UIButton : UIElement
    {
        public override void Draw(DrawingContext ctx)
        {
            var bound = GetBound();
            var cursor = ctx.GetCursor();
            if (bound.Contains(cursor))
            {
                ctx.Graphics.FillRectangle(HoveredBrush, bound);
            }
            else
            {
                ctx.Graphics.FillRectangle(BackColor, bound);

            }
            ctx.Graphics.DrawRectangle(Pens.Black, bound);
            if(!string.IsNullOrEmpty(Text))
            {
                var ms = ctx.Graphics.MeasureString(Text, SystemFonts.DefaultFont);
                ctx.Graphics.DrawString(Text, SystemFonts.DefaultFont, Brushes.White, bound.X+bound.Width/2-ms.Width/2, bound.Y+bound.Height/2-ms.Height/2);
            }

        }
        public Brush HoveredBrush = Brushes.ForestGreen;
        public Brush BackColor = Brushes.Black;
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
