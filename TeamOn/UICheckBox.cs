using System;
using System.Drawing;
using TeamOn.Controls;

namespace TeamOn
{
    public class UICheckBox : UIElement
    {
        public bool Checked;
        public Action<UICheckBox> CheckedChanged;

        public string Text { get; internal set; }

        public override void Draw(DrawingContext ctx)
        {
            var bound = GetBound();
            int gap = 3;
            int boxHeight = 20;
            var rect = new Rectangle(bound.X, bound.Y, 20, boxHeight);
            var cursor = ctx.GetCursor();

            ctx.Graphics.FillRectangle(Brushes.AliceBlue, rect);
            if (Checked)
            {
                ctx.Graphics.DrawLine(new Pen(Color.Red, 3), rect.X + gap, rect.Y + gap, rect.Right - gap, rect.Bottom - gap);
                ctx.Graphics.DrawLine(new Pen(Color.Red, 3), rect.X + gap, rect.Bottom - gap, rect.Right - gap, rect.Top + gap);
            }

            ctx.Graphics.DrawRectangle(Pens.Black, rect);
            var ms = ctx.Graphics.MeasureString(Text, SystemFonts.DefaultFont);
            ctx.Graphics.DrawString(Text, SystemFonts.DefaultFont, Brushes.Blue, rect.Right + 10, rect.Y + boxHeight / 2 - ms.Height / 2);

        }

        public override void Event(UIEvent ev)
        {
            var bound = Parent.GetRectangleOfChild(this).Value;
            if (ev is UIMouseButtonDown bd)
            {
                if (bound.Contains(bd.Position))
                {
                    Checked = !Checked;
                    CheckedChanged?.Invoke(this);
                }
            }
        }
    }

}
