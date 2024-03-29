﻿using System.Drawing;
using System.Windows.Forms;
using TeamOn.Controls;

namespace TeamOn
{
    public class SettingsButton : UIButton
    {
        public SettingsButton(Form1 owner)
        {
            Click = (x) =>
            {
                owner.SwitchLayoutSettings();
            };
        }

        public override void Draw(DrawingContext ctx)
        {
            var icon = TeamOn.Properties.Resources.gear;

            var cursor = ctx.GetCursor();
            int gap = 3;
            if (Rect.Contains(cursor))
            {
                ctx.Graphics.FillRectangle(Brushes.AliceBlue, Rect);
                
            }
            else
            {
                
            }
            ctx.Graphics.DrawIcon(icon, Rect);

            //ctx.Graphics.DrawRectangle(Pens.Black, Rect);


        }
    }
}
