using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;
using TeamOn.Controls;
using Button = TeamOn.Controls.Button;

namespace TeamOn
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            bmp = new Bitmap(Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height);
            ctx.Graphics = Graphics.FromImage(bmp);
            ctx.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            ctx.PictureBox = pictureBox1;
            Elements.Add(new CloseButton(this) { Rect = new Rectangle(Width - headerHeight - 1, 0, headerHeight, headerHeight - 1) });
            Elements.Add(new MinimizeButton(this) { Rect = new Rectangle(Width - headerHeight*2 - 1, 0, headerHeight, headerHeight - 1) });

            TopMost = true;

            notifyIcon1.BalloonTipIcon = ToolTipIcon.Info;
            notifyIcon1.ContextMenuStrip = contextMenuStrip1;
            notifyIcon1.Icon = TeamOn.Properties.Resources.smiley_mr_green;
            Icon= TeamOn.Properties.Resources.smiley_mr_green; ;

            root = new RootElement();
            root.Rect = new Rectangle(0, headerHeight, Width, Height - headerHeight);

            mainPanel = new TwoColumnPanel() { Rect = new Rectangle(0, headerHeight, Width, Height - headerHeight), Parent = root };
            mainPanel.FirstPanelWidth = 200;
            mainPanel.FirstPanelFixed = true;
            clc = new ChatsListControl() { Parent = mainPanel };
            clc.Chats.Add(new ChatItem() { Name = "chat1" });
            clc.Chats.Add(new ChatItem() { Name = "group1" });
            clc.Chats.Add(new ChatItem() { Name = "group2" });
            ChatMessageAreaControl.CurrentChat = clc.Chats.First();
            var chat1 = clc.Chats.First();
            UserInfo currentUser = new UserInfo() { Name = Environment.UserName };
            chat1.Messages.Add(new TextChatMessage() { Text = "Hello!  123 :)" , Owner = currentUser });
            chat1.Messages.Add(new TextChatMessage() { Text = "Hi" ,Owner=currentUser });
            chat1.Messages.Add(new TextChatMessage() { Text = "how are you?" ,Owner=new UserInfo() { Name="user2"} });


            mainPanel.Elements.Add(clc);
            mainPanel.Elements.Add(new ChatControl() { Parent = mainPanel });

            pictureBox1.MouseDoubleClick += PictureBox1_MouseDoubleClick;

            Elements.Add(mainPanel);
            pictureBox1.MouseDown += PictureBox1_MouseDown;
            pictureBox1.MouseUp += PictureBox1_MouseUp;
            ResizeEnd += Form1_ResizeEnd;
            Resize += Form1_Resize;
            SizeChanged += Form1_SizeChanged;
            normalWidth = Width;
            normalHeight = Height;
            normalLeft = Left;
            normalTop = Top;

            loadConfig();
            connectStart();

        }

        private void Form1_SizeChanged(object sender, EventArgs e)
        {
            if (Left < 0)
            {

            }
        }

        RootElement root;


        void loadConfig()
        {
            if (!File.Exists("config.xml")) return;
            var doc = XDocument.Load("config.xml");

            foreach (var item in doc.Descendants("setting"))
            {
                var nm = item.Attribute("name").Value;
                var vl = item.Attribute("value").Value;
                switch (nm)
                {
                    case "serverIP":
                        serverIP = vl;
                        break;
                    case "serverPort":
                        serverPort = int.Parse(vl);
                        break;
                    default:
                        break;
                }
            }

        }

        string serverIP = "127.0.0.1";
        int serverPort = 8888;

        private void connectStart()
        {
            Thread th = new Thread(() =>
            {
                while (true)
                {
                    try
                    {
                        connect(serverIP, serverPort);
                        if (client.Connected) connectEvent.WaitOne();

                    }
                    catch (Exception)
                    {

                    }
                }
            });
            th.IsBackground = true;
            th.Start();
        }

        AutoResetEvent connectEvent = new AutoResetEvent(false);

        public static void Invoke(Control ctrl, Action act)
        {
            if (ctrl.InvokeRequired)
                ctrl.Invoke(act);
            else
                act();
        }

        public void UpdateClientsList()
        {
            /*Invoke(listView2, () =>
            {
                listView2.Items.Clear();
                foreach (var userInfo in client.Users)
                {
                    listView2.Items.Add(new ListViewItem(new string[] { userInfo.Name }) { Tag = userInfo });
                }
            });*/
        }
        ChatClient client;
        void connect(string ipAddr, int port)
        {
            client = new ChatClient();
            //client.Nickname = textBox3.Text;
            client.OnClientsListUpdate = UpdateClientsList;
            client.OnError = (msg) =>
            {
                /*richTextBox1.Invoke((Action)(() =>
                {
                    richTextBox1.Text += $"{DateTime.Now.ToLongTimeString()}: {msg}";

                }));*/
            };
            client.OnFileRecieved = (uin, path, size) =>
            {
                /*progressBar1.Value = (int)100;
                label4.Text = (int)100 + "%";
                listView1.Items.Add(new ListViewItem(new string[]
            {
                                    DateTime.Now.ToLongTimeString(),
                                    uin,
                                   (long)Math.Round(size/1024f) + "Kb",
                                    "file recieved: " + path + "(size: " + size/1024 + "Kb)" +
                                    Environment.NewLine,
            })
                { Tag = new FileInfo(path) });*/
            };
            client.OnFileChunkRecieved = (uin, path, chunkSize, size, perc) =>
            {
                /*Invoke(listView1, () =>
                {
                    progressBar1.Value = (int)perc;
                    label4.Text = (int)perc + "%";
                });*/
            };
            client.OnMsgRecieved = (user, str) =>
            {
                /*     Invoke((Action)(() =>
                     {
                         listView1.Items.Add(new ListViewItem(new string[]
                     {
                         DateTime.Now.ToLongTimeString() ,
                         user+"",
                         str.Length+"",
                         str
                     })
                         { Tag = str });


                         richTextBox2.Invoke((Action)(() =>
                         {
                             richTextBox2.Text = str;
                         }));
                     }));*/

            };
            client.Connect(ipAddr, port);
            //   toolStripSplitButton1.Enabled = false;
            client.FetchClients();
        }
        ChatsListControl clc;
        private void Form1_Resize(object sender, EventArgs e)
        {
       
            if (FormWindowState.Minimized == this.WindowState)
            {
                ShowInTaskbar = false;

                captionCaptured = false;
                notifyIcon1.Visible = true;
                //notifyIcon1.ShowBalloonTip(500);
                //this.Hide();
            }
            else if (FormWindowState.Normal == this.WindowState)
            {
                
                captionCaptured = false;
                //Show();
                UIResize();
                Refresh();
                
                //notifyIcon1.Visible = false;
            }
        }

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


            if (!Elements[0].Rect.Contains(pos))
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
                if (pos.Y < Height && pos.Y > (Height - 10))
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
                if (pos.X < Width && pos.X > (Width - 10) && pos.Y < Height && pos.Y > (Height - 10))
                {
                    captureCursorX = Cursor.Position.X;
                    startWindowPositionX = Left;
                    startWindowPositionY = Top;
                    captureCursorY = Cursor.Position.Y;
                    startWidth = Width;
                    startHeight = Height;
                    resizeDirection = ResizeDirectionEnum.BottomRight;

                    resizeCaptured = true;
                }
            }

            var bd = new UIMouseButtonDown() { Position = ctx.GetCursor(), Button = e.Button };
            foreach (var item in Elements)
            {
                if (bd.Handled) continue;
                item.Event(bd);
            }
            if (Visible && e.Y < headerHeight && !Elements[0].Rect.Contains(pos))
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
                    Height = startHeight - (captureCursorY - pos.Y);

                    UIResize();
                }
                if (resizeDirection == ResizeDirectionEnum.BottomRight)
                {
                    Height = startHeight - (captureCursorY - pos.Y);
                    Width = startWidth - (captureCursorX - pos.X);

                    UIResize();
                }

            }
            if (!Elements[0].Rect.Contains(pos2))
            {
                if ((resizeCaptured && (resizeDirection == ResizeDirectionEnum.Left || resizeDirection == ResizeDirectionEnum.Right)) || (pos2.X < 10 || (pos2.X < Width && pos2.X > (Width - 10))))
                {
                    ctx.SetTempCursor(Cursors.SizeWE);
                }
                if ((pos2.Y < 5 || (pos2.Y < Height && pos2.Y > (Height - 5))))
                {
                    ctx.SetTempCursor(Cursors.SizeNS);
                }
                if (((pos2.X < Width && pos2.X > (Width - 10)) && (pos2.Y < Height && pos2.Y > (Height - 5))))
                {
                    ctx.SetTempCursor(Cursors.SizeNWSE);
                }
            }

            if ( WindowState==FormWindowState.Normal && captionCaptured && !resizeCaptured)
            {
                Left = startWindowPositionX - (captureCursorX - pos.X);
                Top = startWindowPositionY - (captureCursorY - pos.Y);
            }

            gr.Clear(Color.LightBlue);

            gr.FillRectangle((captionCaptured || (pos2.Y > 0 && pos2.Y < headerHeight)) ? Brushes.DarkGreen : Brushes.Navy, 0, 0, Width, headerHeight);

            gr.FillEllipse(client.Connected ? Brushes.Green : Brushes.Yellow, 1, 1, 15, 15);
            gr.DrawEllipse(Pens.Black, 1, 1, 15, 15);
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
            ctx.ApplyCursor();

        }

        private void UIResize()
        {
            Elements[0].Rect = new Rectangle(Width - headerHeight - 1, 0, headerHeight, headerHeight - 1);
            Elements[1].Rect = new Rectangle(Width - headerHeight*2 - 1, 0, headerHeight, headerHeight - 1);
            mainPanel.Rect = new Rectangle(0, headerHeight, Width, Height - headerHeight);
            root.Rect = mainPanel.Rect;

        }

        private void notifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            ShowInTaskbar = true;
            
            //notifyIcon1.Visible = false;
            WindowState = FormWindowState.Normal;
            //Show();
        }

        private void closeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }
    }

    public class UITextBox : UIElement
    {
        public override void Draw(DrawingContext ctx)
        {

        }

        public override void Event(UIEvent ev)
        {

        }
    }
    public class ChatTextBoxControl : UITextBox
    {
        public override void Draw(DrawingContext ctx)
        {
            if (!Visible) return;
            var bound = Parent.GetRectangleOfChild(this).Value;
            ctx.Graphics.FillRectangle(Brushes.White, bound.X, bound.Y, bound.Width, bound.Height);
            var pos = ctx.GetCursor();
            if (new Rectangle(bound.X, bound.Bottom - 50, bound.Width, 50).Contains(pos))
            {
                ctx.SetTempCursor(Cursors.IBeam);
            }
            ctx.Graphics.DrawString("Напишите сообщение", SystemFonts.DefaultFont, Brushes.Gray, bound.X + 10, bound.Y + 5);
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
    public class MinimizeButton : Button
    {
        public MinimizeButton(Form owner)
        {
            Click = (x) =>
            {

                owner.WindowState = FormWindowState.Minimized;

            };
        }

        public override void Draw(DrawingContext ctx)
        {
            var cursor = ctx.GetCursor();
            int gap = 3;
            if (Rect.Contains(cursor))
            {
                ctx.Graphics.FillRectangle(Brushes.AliceBlue, Rect);
                ctx.Graphics.DrawLine(new Pen(Color.Red, 3), Rect.X + gap, Rect.Bottom - gap, Rect.Right - gap, Rect.Bottom - gap);
            }
            else
            {
                ctx.Graphics.DrawLine(new Pen(Color.LightGray, 3), Rect.X + gap, Rect.Bottom - gap, Rect.Right - gap, Rect.Bottom - gap);
            }
            ctx.Graphics.DrawRectangle(Pens.Black, Rect);
        }
    }

    public class ChatItem
    {
        public string Name;
        public List<ChatMessage> Messages = new List<ChatMessage>();

    }
    public abstract class ChatMessage
    {
        public UserInfo Owner;
    }

    public class TextChatMessage : ChatMessage
    {
        public string Text;
    }
    public class ImageChatMessage : ChatMessage
    {
        public Bitmap Thumbnail;
        public string Path;
    }
 
    public class UserInfo
    {
        public string Name;
    }
   
}
