using System.Drawing;
using System.Windows.Forms;

namespace TeamOn
{
    public class DrawingContext
    {
        public Graphics Graphics;
        public PictureBox PictureBox;
        public Point GetCursor()
        {
            return PictureBox.PointToClient(Cursor.Position);
        }
        Cursor tempCursor;
        int lastPriority = 0;
        internal void SetTempCursor(Cursor beam,int priority=1)
        {
            if (priority >= lastPriority)
            {
                lastPriority = priority;
                tempCursor = beam;
            }
        }
        public void ApplyCursor()
        {
            PictureBox.Parent.Cursor = tempCursor;
            lastPriority = 0;
        }
    }   
}
