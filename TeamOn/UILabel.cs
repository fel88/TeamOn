using System.Drawing;
using TeamOn.Controls;

namespace TeamOn
{
    public class UILabel : UIElement
    {
        public string Text;
        public Font TextFont=SystemFonts.DefaultFont;
        public override void Draw(DrawingContext ctx)
        {
            var bound = GetBound();
            ctx.Graphics.DrawString(Text, TextFont, Brushes.Red, bound);
        }

        public override void Event(UIEvent ev)
        {
            
        }
    }
}
