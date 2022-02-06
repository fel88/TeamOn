using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;

namespace TeamOn.Controls
{
    public class ChatMessageAreaControl : UIElement
    {
        private Bitmap _back;

        public static ChatItem CurrentChat;
        public static UserInfo CurrentUser;
        public ChatMessageAreaControl()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var resourceName = "pattern1";

            using (Stream stream = assembly.GetManifestResourceStream(assembly.GetManifestResourceNames().First(z => z.Contains(resourceName))))
            {
                _back = Bitmap.FromStream(stream) as Bitmap;
            }

            for (int i = 0; i < _back.Width; i++)
            {
                for (int j = 0; j < _back.Height; j++)
                {
                    var px = _back.GetPixel(i, j);
                    _back.SetPixel(i, j, Color.FromArgb(64, px));
                }
            }
        }
        public override void Draw(DrawingContext ctx)
        {
            if (!Visible) return;
            var bound = Parent.GetRectangleOfChild(this).Value;
            int nX = bound.Width / _back.Width + 1;
            int nY = bound.Height / _back.Height + 1;
            ctx.Graphics.SetClip(bound);
            //ctx.Graphics.RotateTransform(45);
            //    ctx.Graphics.TranslateTransform(0, -500);
            for (int i = 0; i < nX; i++)
            {
                for (int j = 0; j < nY; j++)
                {
                    ctx.Graphics.DrawImage(_back, bound.X + i * _back.Width, bound.Y + j * _back.Height);
                }
            }

            int gap = 5;
            int leftGap = 50;
            int currentY = bound.Bottom - gap * 2;
            if (ChatMessageAreaControl.CurrentChat != null)
                for (int i = CurrentChat.Messages.Count - 1; i >= 0; i--)
                {
                    var msg = CurrentChat.Messages[i];

                    if (msg is TextChatMessage tcm)
                    {
                        var ms = ctx.Graphics.MeasureString(tcm.Text, SystemFonts.DefaultFont);
                        if (ms.Width > bound.Width / 2)
                        {
                            ms = ctx.Graphics.MeasureString(tcm.Text, SystemFonts.DefaultFont, bound.Width / 2);
                        }

                        if (msg.Owner != CurrentUser)
                        {
                            var rect = new RectangleF(bound.X + leftGap, currentY - ms.Height, ms.Width, ms.Height);
                            var rect2 = new RectangleF(rect.X, rect.Y, rect.Width, rect.Height);
                            rect2.Inflate(5, 5);
                            ctx.Graphics.FillRectangle(Brushes.White, rect2);
                            ctx.Graphics.DrawString(tcm.Text, SystemFonts.DefaultFont, Brushes.Black, rect);
                            ctx.Graphics.DrawString(tcm.Owner.Name, SystemFonts.DefaultFont, Brushes.Black, bound.X + 2, currentY - ms.Height);

                            currentY -= gap * 3 + (int)ms.Height;
                        }
                        else
                        {
                            var rect = new RectangleF(bound.Right - ms.Width - leftGap, currentY - ms.Height, ms.Width, ms.Height);
                            var rect2 = new RectangleF(rect.X, rect.Y, rect.Width, rect.Height);
                            rect2.Inflate(5, 5);
                            ctx.Graphics.FillRectangle(Brushes.White, rect2);
                            ctx.Graphics.DrawString(tcm.Text, SystemFonts.DefaultFont, Brushes.Black, rect);
                            ctx.Graphics.DrawString(tcm.Owner.Name, SystemFonts.DefaultFont, Brushes.Black, bound.Right - leftGap, currentY - ms.Height);

                            currentY -= gap * 3 + (int)ms.Height;
                        }


                    }
                }


            //ctx.Graphics.FillRectangle(Brushes.White, bound.X, bound.Bottom - 50, bound.Width, 50);
            var pos = ctx.GetCursor();
            /*if (new Rectangle(bound.X, bound.Bottom - 50, bound.Width, 50).Contains(pos))
            {
                ctx.SetTempCursor(Cursors.IBeam);
            }
            ctx.Graphics.DrawString("Напишите сообщение", SystemFonts.DefaultFont, Brushes.Gray, bound.X + 10, bound.Bottom - 50 + 5);
            */
            ctx.Graphics.ResetTransform();
            ctx.Graphics.ResetClip();

            //ctx.Graphics.FillRectangle(Brushes.DarkGoldenrod, bound.Value);
        }

        public override void Event(UIEvent ev)
        {

        }
    }
}
