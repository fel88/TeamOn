using System.Drawing;
using System.Windows.Forms;

namespace TeamOn.Controls
{
    public class ChatControl : RowsPanel
    {
        public ChatControl()
        {
            Styles.Add(new RowColumnPanelStyle() { Type = SizeType.Absolute, Size = 60 });
            Styles.Add(new RowColumnPanelStyle() { Type = SizeType.Percent, Size = 100 });
            Styles.Add(new RowColumnPanelStyle() { Type = SizeType.Absolute, Size = 40 });
            Styles.Add(new RowColumnPanelStyle() { Type = SizeType.Absolute, Size = 3 });
            Elements.Add(new ChatHeaderControl() { Parent = this });
            Elements.Add(new ChatMessageAreaControl() { Parent = this, Visible = true });
            Elements.Add(new ChatTextBoxControl() { Parent = this, Visible = true });
            Elements.Add(new Panel() { Parent = this, BackColor = Color.LightBlue });

        }        
    }
}
