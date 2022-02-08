using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;

namespace TeamOn.Controls
{
    public class ChatMessageAreaControl : UIElement
    {    
        private Bitmap _back;

        public static ChatMessageAreaControl Instance;
        private ChatItem _currentChat;
        public static ChatItem CurrentChat => Instance._currentChat;
        public static UserInfo CurrentUser;
        public ChatMessageAreaControl()
        {
            Instance = this;

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

            foreach (var item in regions)
            {
                if (item.Rect.Contains(ctx.GetCursor()))
                {
                    ctx.SetTempCursor(Cursors.Hand);
                }
            }

            int gap = 5;
            int leftGap = 50;
            int currentY = bound.Bottom - gap * 2;
            if (ChatMessageAreaControl.CurrentChat != null)
            {
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
                            var rect = new RectangleF(bound.X + leftGap, currentY - ms.Height + scrollOffsetY, ms.Width, ms.Height);
                            var rect2 = new RectangleF(rect.X, rect.Y, rect.Width, rect.Height);
                            rect2.Inflate(5, 5);
                            ctx.Graphics.FillRectangle(Brushes.White, rect2);

                            CheckAndDrawTextBlock(msg, rect, ctx);

                            ctx.Graphics.DrawString(tcm.Owner.Name, SystemFonts.DefaultFont, Brushes.Black, bound.X + 2, currentY - ms.Height + scrollOffsetY);

                            currentY -= gap * 3 + (int)ms.Height;
                        }
                        else
                        {
                            var rect = new RectangleF(bound.Right - ms.Width - 10, currentY - ms.Height + scrollOffsetY, ms.Width, ms.Height);
                            var rect2 = new RectangleF(rect.X, rect.Y, rect.Width, rect.Height);
                            rect2.Inflate(5, 5);
                            ctx.Graphics.FillRectangle(Brushes.White, rect2);

                            CheckAndDrawTextBlock(msg, rect, ctx);
                            //ctx.Graphics.DrawString(tcm.Owner.Name, SystemFonts.DefaultFont, Brushes.Black, bound.Right - leftGap, currentY - ms.Height);

                            currentY -= gap * 3 + (int)ms.Height;
                        }
                    }
                    else if (msg is ImageLinkChatMessage ilcm)
                    {
                        ilcm.GenerateThumbnail();

                        var ms = new Rectangle(0, 0, 120, ilcm.Thumbnail.Height);
                        if (msg.Owner != CurrentUser)
                        {
                            var rect = new RectangleF(bound.X + leftGap, currentY - ms.Height + scrollOffsetY, ms.Width, ms.Height);
                            var rect2 = new RectangleF(rect.X, rect.Y, rect.Width, rect.Height);
                            rect2.Inflate(5, 5);

                            ctx.Graphics.DrawImage(ilcm.Thumbnail, rect2.X, rect2.Y);

                            //ctx.Graphics.FillRectangle(Brushes.White, rect2);

                            CheckAndDrawTextBlock(msg, rect, ctx);

                            ctx.Graphics.DrawString(ilcm.Owner.Name, SystemFonts.DefaultFont, Brushes.Black, bound.X + 2, currentY - ms.Height + scrollOffsetY);

                            currentY -= gap * 3 + (int)ms.Height;
                        }
                        else
                        {
                            var rect = new RectangleF(bound.Right - ms.Width - 10, currentY - ms.Height + scrollOffsetY, ms.Width, ms.Height);
                            var rect2 = new RectangleF(rect.X, rect.Y, rect.Width, rect.Height);
                            rect2.Inflate(5, 5);
                            //ctx.Graphics.FillRectangle(Brushes.White, rect2);
                            ctx.Graphics.DrawImage(ilcm.Thumbnail, rect2.X, rect2.Y);

                            CheckAndDrawTextBlock(msg, rect, ctx);
                            //ctx.Graphics.DrawString(tcm.Owner.Name, SystemFonts.DefaultFont, Brushes.Black, bound.Right - leftGap, currentY - ms.Height);

                            currentY -= gap * 3 + (int)ms.Height;
                        }
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

        internal void SwitchToChat(ChatItem item)
        {
            _currentChat = item;
            scrollOffsetY = 0;
        }

        private void CheckAndDrawTextBlock(ChatMessage c, RectangleF rect, DrawingContext ctx)
        {
            Uri outUri;
            if (c is ImageLinkChatMessage ilcm)
            {
                var fr = regions.FirstOrDefault(z => z.Owner == c);
                if (fr == null)
                {
                    regions.Add(new TextSpecificRegion() { Owner = c });
                    fr = regions.Last();
                }

                fr.Rect = rect;
            }
            else if (c is TextChatMessage tcm)
            {
                var text = tcm.Text;
                if (Uri.TryCreate(text, UriKind.Absolute, out outUri)
                   /*&& (outUri.Scheme == Uri.UriSchemeHttp || outUri.Scheme == Uri.UriSchemeHttps)*/)
                {
                    var fr = regions.FirstOrDefault(z => z.Owner == c);
                    if (fr == null)
                    {
                        regions.Add(new TextSpecificRegion() { Owner = c });
                        fr = regions.Last();
                    }

                    fr.Rect = rect;
                    ctx.Graphics.DrawString(text, SystemFonts.DefaultFont, Brushes.Blue, rect);
                    ctx.Graphics.DrawLine(Pens.Blue, rect.X, rect.Bottom - 1, rect.Right, rect.Bottom - 1);
                }
                else
                {
                    ctx.Graphics.DrawString(text, SystemFonts.DefaultFont, Brushes.Black, rect);
                }
            }
        }

        int scrollOffsetY = 0;


        public override void Event(UIEvent ev)
        {
            if (ev is UIMouseWheel w)
            {
                scrollOffsetY += w.Delta;
                if (scrollOffsetY < 0) { scrollOffsetY = 0; }
            }

            if (ev is UIMouseButtonDown md)
            {
                if (md.Button == MouseButtons.Left)
                {
                    foreach (var item in regions)
                    {
                        if (item.Rect.Contains(md.Position))
                        {
                            if (item.Owner is ImageLinkChatMessage ilcm)
                            {
                                try
                                {
                                    if (File.Exists(ilcm.Path))
                                    {
                                        Process.Start(ilcm.Path);
                                    }
                                    else
                                    {
                                        var uri = new Uri(ilcm.Path);
                                        if (File.Exists(uri.LocalPath))
                                        {
                                            Process.Start(uri.LocalPath);
                                        }
                                    }
                                }
                                catch (Exception ex)
                                {
                                    MessageBox.Show(ex.Message);
                                }
                            }
                            else
                            if (item.Owner is TextChatMessage tcm)
                            {
                                try
                                {
                                    var uri = new Uri(tcm.Text);
                                    if (File.Exists(uri.LocalPath))
                                    {
                                        Process.Start(uri.LocalPath);
                                    }
                                    else
                                    {
                                        Process.Start(tcm.Text);
                                    }
                                }
                                catch (Exception ex)
                                {
                                    MessageBox.Show(ex.Message);
                                }
                            }
                            break;
                        }
                    }
                }
            }

        }
        List<TextSpecificRegion> regions = new List<TextSpecificRegion>();
    }

    public class TextSpecificRegion
    {
        public ChatMessage Owner;
        public RectangleF Rect;
    }
}
