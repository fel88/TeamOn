using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace TeamOn.Controls
{
    public class RowsPanel : UIPanel
    {
        public override Rectangle? GetRectangleOfChild(UIElement elem)
        {
            var bound = Parent.GetRectangleOfChild(this).Value;
            var totalAbs = Styles.Where(z => z.Type == SizeType.Absolute).Sum(z => z.Size);
            var last = bound.Height - totalAbs;
            int[] heights = new int[Styles.Count];
            for (int i = 0; i < Styles.Count; i++)
            {
                if (Styles[i].Type == SizeType.Absolute)
                {
                    heights[i] = Styles[i].Size;
                }
                else if (Styles[i].Type == SizeType.Percent)
                {
                    heights[i] = (int)(last * (Styles[i].Size / 100f));
                }
            }

            var idx = Elements.IndexOf(elem);
            int accumY = 0;
            for (int i = 0; i < idx; i++)
            {
                accumY += heights[i];
            }
            return new Rectangle(bound.X, bound.Y + accumY, bound.Width, heights[idx]);
        }


    }
}
