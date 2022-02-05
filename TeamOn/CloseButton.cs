using System.Drawing;
using System.Windows.Forms;
using Button = TeamOn.Controls.Button;

namespace TeamOn
{
    public class CloseButton : Button
    {
        public CloseButton(Form owner)
        {
            Click = (x) =>
            {
                if (MessageBox.Show("Close?", owner.Text, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    owner.Close();
                }
            };
        }

        public override void Draw(DrawingContext ctx)
        {
            var cursor = ctx.GetCursor();
            int gap = 3;
            if (Rect.Contains(cursor))
            {
                ctx.Graphics.FillRectangle(Brushes.AliceBlue, Rect);

                ctx.Graphics.DrawLine(new Pen(Color.Red, 3), Rect.X + gap, Rect.Y + gap, Rect.Right - gap, Rect.Bottom - gap);
                ctx.Graphics.DrawLine(new Pen(Color.Red, 3), Rect.X + gap, Rect.Bottom - gap, Rect.Right - gap, Rect.Top + gap);
            }
            else
            {
                ctx.Graphics.DrawLine(new Pen(Color.LightGray, 3), Rect.X + gap, Rect.Y + gap, Rect.Right - gap, Rect.Bottom - gap);
                ctx.Graphics.DrawLine(new Pen(Color.LightGray, 3), Rect.X + gap, Rect.Bottom - gap, Rect.Right - gap, Rect.Top + gap);
            }
            ctx.Graphics.DrawRectangle(Pens.Black, Rect);


        }
    }
   
}
