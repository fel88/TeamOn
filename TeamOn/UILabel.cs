using System.Drawing;
using TeamOn.Controls;

namespace TeamOn
{
    public class UILabel : UIElement
    {
        public string Text;
        public Font TextFont=SystemFonts.DefaultFont;

        public Brush ForeColor = Brushes.Black;
        public override void Draw(DrawingContext ctx)
        {
            var bound = GetBound();
            ctx.Graphics.DrawString(Text, TextFont, ForeColor, bound);
        }

        public override void Event(UIEvent ev)
        {
            
        }
    }
}
