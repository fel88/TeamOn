using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TeamOn
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            bmp = new Bitmap(Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height);
            ctx.Graphics = Graphics.FromImage(bmp);
            ctx.PictureBox = pictureBox1;
            Elements.Add(new CloseButton() { Rect = new Rectangle(Width - headerHeight - 1, 0, headerHeight, headerHeight - 1), Click = (x) => { Close(); } });


            mainPanel = new TwoColumnPanel() { Rect = new Rectangle(0, headerHeight, Width, Height) };
            mainPanel.FirstPanelWidth = 200;
            mainPanel.FirstPanelFixed = true;
            clc = new ChatsListControl() { Parent = mainPanel };
            clc.Chats.Add(new ChatItem() { Name = "chat1" });
            clc.Chats.Add(new ChatItem() { Name = "group1" });

            mainPanel.Elements.Add(clc);
            mainPanel.Elements.Add(new ChatControl() { Parent = mainPanel });

            pictureBox1.MouseDoubleClick += PictureBox1_MouseDoubleClick;

            Elements.Add(mainPanel);
            pictureBox1.MouseDown += PictureBox1_MouseDown;
            pictureBox1.MouseUp += PictureBox1_MouseUp;
            ResizeEnd += Form1_ResizeEnd;
            normalWidth = Width;
            normalHeight = Height;
            normalLeft = Left;
            normalTop = Top;

        }
        ChatsListControl clc;
        private void Form1_ResizeEnd(object sender, EventArgs e)
        {
            UIResize();
        }

        int normalWidth;
        int normalHeight;
        int normalLeft;
        int normalTop;
        private void PictureBox1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            var pos = ctx.GetCursor();
            //if (pos.Y < headerHeight)
            {
                if (Math.Abs(Width - Screen.PrimaryScreen.Bounds.Width) < 1)
                {
                    Width = normalWidth;
                    Height = normalHeight;
                    Left = normalLeft;
                    Top = normalTop;
                }
                else
                {
                    normalWidth = Width;
                    normalHeight = Height;
                    normalLeft = Left;
                    normalTop = Top;
                    Left = 0;
                    Top = 0;
                    Width = Screen.PrimaryScreen.Bounds.Width;
                    Height = Screen.PrimaryScreen.WorkingArea.Height;

                }

                UIResize();


            }
        }

        TwoColumnPanel mainPanel;

        private void PictureBox1_MouseUp(object sender, MouseEventArgs e)
        {
            var bd = new UIMouseButtonUp() { Position = ctx.GetCursor(), Button = e.Button };
            foreach (var item in Elements)
            {
                if (bd.Handled) continue;
                item.Event(bd);
            }
            captionCaptured = false;
            resizeCaptured = false;

        }

        bool captionCaptured = false;


        bool resizeCaptured = false;
        public enum ResizeDirectionEnum
        {
            Left, Right, Top, Bottom, TopLeft, TopTight, BottomLeft, BottomRight
        }
        ResizeDirectionEnum resizeDirection;
        int startWidth;
        int startHeight;


        private void PictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            var pos = ctx.GetCursor();
           // if (pos.Y > headerHeight)
            {
                if (pos.X < 10)
                {
                    captureCursorX = Cursor.Position.X;
                    startWindowPositionX = Left;
                    captureCursorY = Cursor.Position.Y;
                    startWidth = Width;
                    resizeDirection = ResizeDirectionEnum.Left;

                    resizeCaptured = true;
                }
                if (pos.Y < 5)
                {
                    captureCursorX = Cursor.Position.X;
                    startWindowPositionX = Left;
                    startWindowPositionY = Top;
                    captureCursorY = Cursor.Position.Y;
                    startHeight = Height;
                    resizeDirection = ResizeDirectionEnum.Top;

                    resizeCaptured = true;
                }
                if (pos.Y < Height && pos.Y>(Height-10))
                {
                    captureCursorX = Cursor.Position.X;
                    startWindowPositionX = Left;
                    startWindowPositionY = Top;
                    captureCursorY = Cursor.Position.Y;
                    startHeight = Height;
                    resizeDirection = ResizeDirectionEnum.Bottom;

                    resizeCaptured = true;
                }
                if (pos.X < Width && pos.X > (Width - 10))
                {
                    captureCursorX = Cursor.Position.X;
                    startWindowPositionX = Left;
                    captureCursorY = Cursor.Position.Y;
                    startWidth = Width;
                    resizeDirection = ResizeDirectionEnum.Right;

                    resizeCaptured = true;
                }
            }

            var bd = new UIMouseButtonDown() { Position = ctx.GetCursor(), Button = e.Button };
            foreach (var item in Elements)
            {
                if (bd.Handled) continue;
                item.Event(bd);
            }
            if (e.Y < headerHeight)
            {
                captionCaptured = true;
                startWindowPositionX = Left;
                startWindowPositionY = Top;
                captureCursorX = Cursor.Position.X;
                captureCursorY = Cursor.Position.Y;
            }
        }

        int startWindowPositionX;
        int startWindowPositionY;
        int captureCursorX;
        int captureCursorY;


        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        public List<UIElement> Elements = new List<UIElement>();
        DrawingContext ctx = new DrawingContext();
        Graphics gr => ctx.Graphics;
        Bitmap bmp;

        int headerHeight = 20;

        private void timer1_Tick(object sender, EventArgs e)
        {
            var pos2 = ctx.GetCursor();
            ctx.SetTempCursor(Cursors.Default);

            var pos = Cursor.Position;

            if (resizeCaptured)
            {
                if (resizeDirection == ResizeDirectionEnum.Left)
                {
                    //Width = startWidth - (captureCursorX - pos2.X);
                    Left = startWindowPositionX - (captureCursorX - pos.X);
                    Width = startWidth + (captureCursorX - pos.X);
                    UIResize();
                }
                if (resizeDirection == ResizeDirectionEnum.Top)
                {
                    //Width = startWidth - (captureCursorX - pos2.X);
                    Top = startWindowPositionY - (captureCursorY - pos.Y);
                    Height = startHeight + (captureCursorY - pos.Y);
                    UIResize();
                }
                if (resizeDirection == ResizeDirectionEnum.Right)
                {
                    Width = startWidth - (captureCursorX - pos.X);

                    UIResize();
                }
                if (resizeDirection == ResizeDirectionEnum.Bottom)
                {
                    Height= startHeight - (captureCursorY- pos.Y);

                    UIResize();
                }

            }
            //if (pos2.Y > headerHeight)
            {
                if ((resizeCaptured && (resizeDirection==ResizeDirectionEnum.Left || resizeDirection==ResizeDirectionEnum.Right) ) || (pos2.X < 10 || (pos2.X < Width && pos2.X > (Width - 10))))
                {
                    ctx.SetTempCursor(Cursors.SizeWE);
                }
                if ((pos2.Y < 5 || (pos2.Y < Height && pos2.Y > (Height - 5))))
                {
                    ctx.SetTempCursor(Cursors.SizeNS);
                }
            }

            if (captionCaptured && !resizeCaptured)
            {
                Left = startWindowPositionX - (captureCursorX - pos.X);
                Top = startWindowPositionY - (captureCursorY - pos.Y);
            }

            gr.Clear(Color.LightBlue);

            gr.FillRectangle((captionCaptured || (pos2.Y > 0 && pos2.Y < headerHeight)) ? Brushes.DarkGreen : Brushes.Navy, 0, 0, Width, headerHeight);
            var ms = gr.MeasureString("Team Online", SystemFonts.DefaultFont);
            gr.DrawString("Team Online", SystemFonts.DefaultFont, Brushes.Azure, Width / 2 - ms.Width / 2, 3);



            foreach (var item in Elements)
            {
                item.Draw(ctx);
            }

            //draw chats

            var newb = bmp.Clone() as Bitmap;
            var temp = pictureBox1.Image;
            if (temp != null)
            {
                temp.Dispose();
            }
            pictureBox1.Image = newb;
        }

        private void UIResize()
        {
            Elements[0].Rect = new Rectangle(Width - headerHeight - 1, 0, headerHeight, headerHeight - 1);
            mainPanel.Rect = new Rectangle(0, headerHeight, Width, Height);

        }
    }

    public class Panel : UIElement
    {
        public virtual Rectangle? GetRectangleOfChild(UIElement elem)
        {
            return null;
        }

        public List<UIElement> Elements = new List<UIElement>();
        public override void Draw(DrawingContext ctx)
        {
            foreach (var item in Elements)
            {
                item.Draw(ctx);
            }
        }

        public override void Event(UIEvent ev)
        {
            foreach (var item in Elements)
            {
                item.Event(ev);
            }
        }

    }
    public class TwoColumnPanel : Panel
    {
        public bool FirstPanelFixed;
        public int FirstPanelWidth = 250;
        public override Rectangle? GetRectangleOfChild(UIElement elem)
        {

            var idx = Elements.IndexOf(elem);



            if (idx == 0)
            {
                if (FirstPanelFixed)
                {
                    return new Rectangle(Rect.Left, Rect.Y, (int)FirstPanelWidth, Rect.Height);
                }
                return new Rectangle(Rect.Left, Rect.Y, Rect.Width / 2, Rect.Height);
            }
            if (FirstPanelFixed)
            {
                return new Rectangle(Rect.Left + FirstPanelWidth, Rect.Y, Rect.Width - FirstPanelWidth, Rect.Height);

            }
            return new Rectangle(Rect.Left + Rect.Width / 2, Rect.Y, Rect.Width - FirstPanelWidth, Rect.Height);

        }

    }

    public class ChatControl : UIElement
    {


        private Bitmap _back;
        public ChatControl()
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

            ctx.Graphics.FillRectangle(Brushes.White, bound.X, bound.Bottom - 50, bound.Width, 50);
            var pos = ctx.GetCursor();
            if (new Rectangle(bound.X, bound.Bottom - 50, bound.Width, 50).Contains(pos))
            {
                ctx.SetTempCursor(Cursors.IBeam);
            }
            ctx.Graphics.DrawString("Напишите сообщение", SystemFonts.DefaultFont, Brushes.Gray, bound.X + 10, bound.Bottom - 50 + 5);

            ctx.Graphics.ResetTransform();
            ctx.Graphics.ResetClip();
            ctx.ApplyCursor();

            //ctx.Graphics.FillRectangle(Brushes.DarkGoldenrod, bound.Value);
        }

        public override void Event(UIEvent ev)
        {

        }
    }
    public class ChatsListControl : UIElement
    {
        public List<ChatItem> Chats = new List<ChatItem>();
        public override void Draw(DrawingContext ctx)
        {
            var bound = Parent.GetRectangleOfChild(this).Value;
            ctx.Graphics.FillRectangle(Brushes.Khaki, bound);
            int yy = bound.Y;
            int chatHeight = 30;
            foreach (var item in Chats)
            {
                ctx.Graphics.FillRectangle(Brushes.White, bound.X, yy, bound.Width, chatHeight);
                ctx.Graphics.DrawRectangle(new Pen(Color.LightBlue, 1), bound.X, yy, bound.Width, chatHeight);
                ctx.Graphics.DrawString(item.Name, SystemFonts.DefaultFont, Brushes.Blue, 5, yy + 6);

                yy += chatHeight;
            }
        }

        public override void Event(UIEvent ev)
        {

        }
    }

    public class DrawingContext
    {
        public Graphics Graphics;
        public PictureBox PictureBox;
        public Point GetCursor()
        {
            return PictureBox.PointToClient(Cursor.Position);
        }
        Cursor tempCursor;
        internal void SetTempCursor(Cursor beam)
        {
            tempCursor = beam;
        }
        public void ApplyCursor()
        {
            PictureBox.Parent.Cursor = tempCursor;

        }

    }
    public abstract class UIElement
    {
        public Panel Parent;
        public Rectangle Rect;
        public abstract void Draw(DrawingContext ctx);

        public abstract void Event(UIEvent ev);
    }

    public class CloseButton : Button
    {
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
    public class Button : UIElement
    {
        public override void Draw(DrawingContext ctx)
        {
            var cursor = ctx.GetCursor();
            if (Rect.Contains(cursor))
            {
                ctx.Graphics.FillRectangle(Brushes.AliceBlue, Rect);
            }
            ctx.Graphics.DrawRectangle(Pens.Black, Rect);

        }
        public Action<Button> Click;
        public override void Event(UIEvent ev)
        {
            if (ev.Handled) return;
            if (ev is UIMouseButtonDown bd)
            {
                if (bd.Button == MouseButtons.Left)
                {
                    if (Rect.Contains(bd.Position))
                    {
                        Click?.Invoke(this);
                    }
                }
            }
        }
    }

    public class UIEvent
    {
        public bool Handled;

    }

    public class UIMouseButtonEvent : UIEvent
    {
        public Point Position;
        public MouseButtons Button;
    }
    public class UIMouseButtonDown : UIMouseButtonEvent
    {

    }
    public class UIMouseButtonUp : UIMouseButtonEvent
    {

    }

    public class ChatItem
    {
        public string Name;
    }

}
