using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;
using TeamOn.Controls;

namespace TeamOn
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            client = new ChatClient();

            FormClosing += Form1_FormClosing;
            pictureBox1.AllowDrop = true;
            pictureBox1.DragEnter += PictureBox1_DragEnter;
            pictureBox1.DragDrop += PictureBox1_DragDrop;


            bmp = new Bitmap(Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height);
            ctx.Graphics = Graphics.FromImage(bmp);
            ctx.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            ctx.PictureBox = pictureBox1;
            Elements.Add(new CloseButton(this) { AbsoluteRectPosition = true, Rect = new Rectangle(Width - headerHeight - 1, 0, headerHeight, headerHeight - 1) });
            Elements.Add(new MinimizeButton(this) { AbsoluteRectPosition = true, Rect = new Rectangle(Width - headerHeight * 2 - 1, 0, headerHeight, headerHeight - 1) });
            Elements.Add(new SettingsButton(this) { AbsoluteRectPosition = true, Rect = new Rectangle(headerHeight, 0, headerHeight, headerHeight - 1) });
            Elements.Add(new NewGroupButton(this) { AbsoluteRectPosition = true, Rect = new Rectangle(headerHeight * 2, 0, headerHeight, headerHeight - 1) });

            //TopMost = true;

            notifyIcon1.BalloonTipIcon = ToolTipIcon.Info;
            notifyIcon1.ContextMenuStrip = contextMenuStrip1;
            notifyIcon1.Icon = TeamOn.Properties.Resources.smiley_mr_green;
            Icon = TeamOn.Properties.Resources.smiley_mr_green;

            root = new RootElement(this);
            root.Rect = new Rectangle(0, headerHeight, Width, Height - headerHeight);

            mainPanel = new TwoColumnPanel() { Rect = new Rectangle(0, headerHeight, Width, Height - headerHeight), Parent = root };
            mainPanel.FirstPanelWidth = 200;
            mainPanel.FirstPanelFixed = true;

            TeamScreen.TeamScreenServer.StartServer();

            clc = new ChatsListControl() { Parent = mainPanel };

            /*ChatsListControl.Chats.Add(new OnePersonChatItem() { Name = "chat1" });
            ChatsListControl.Chats.Add(new GroupChatItem() { Name = "group1" });
            ChatsListControl.Chats.Add(new GroupChatItem() { Name = "group2" });*/

            //ChatMessageAreaControl.CurrentChat = ChatsListControl.Chats.First();
            //var chat1 = ChatsListControl.Chats.First();
            /*   chat1.Messages.Add(new TextChatMessage() { Text = "Hello!  123 :)", Owner = ChatMessageAreaControl.CurrentUser });
               chat1.Messages.Add(new TextChatMessage() { Text = "Hi", Owner = ChatMessageAreaControl.CurrentUser });
               chat1.Messages.Add(new TextChatMessage() { Text = "how are you?", Owner = new UserInfo() { Name = "user2" } });

               */
            mainPanel.Elements.Add(clc);
            chatControl = new ChatControl() { Parent = mainPanel };
            mainPanel.Elements.Add(chatControl);
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

            Settings.LoadSettings();
            ChatMessageAreaControl.CurrentUser = new UserInfo() { Name = Settings.Nickname };
            MouseWheel += Form1_MouseWheel;
            settings = new SettingsControl() { Parent = mainPanel };
            groupEdit = new GroupEditControl() { Parent = mainPanel };
            connectStart();
            Load += Form1_Load;
            if (Settings.PermanentChats)
            {
                LoadChats();
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (Settings.PermanentChats)
            {
                //save all chats
                SaveChats();
            }
        }
        public void LoadChats()
        {
            if (!File.Exists("chats.xml")) return;

            var doc = XDocument.Load("chats.xml");
            var root = doc.Descendants("root").First();
            foreach (var item in root.Element("users").Elements("user"))
            {
                var uid = int.Parse(item.Attribute("id").Value);
                var nameid = item.Attribute("name").Value;
                if (!ChatClient.Instance.Users.Any(z => z.Name == nameid))
                {
                    ChatClient.Instance.Users.Add(new UserInfo() { Id = uid, Name = nameid });
                }
            }

            foreach (var chat in root.Elements("chat"))
            {
                var type = chat.Attribute("type").Value;
                ChatItem _chat = null;
                if (type == "simple")
                {
                    _chat = new OnePersonChatItem();
                    var person = chat.Attribute("person").Value;
                    _chat.Name = person;
                    (_chat as OnePersonChatItem).Person = ChatClient.Instance.Users.First(z => z.Name == person);
                }
                if (type == "group")
                {
                    _chat = new GroupChatItem();
                    var name = chat.Attribute("name").Value;
                    var owner = chat.Attribute("owner").Value;
                    if (ChatMessageAreaControl.CurrentUser.Name == owner)
                    {
                        (_chat as GroupChatItem).Owner = ChatMessageAreaControl.CurrentUser;
                    }
                    else
                    {
                        (_chat as GroupChatItem).Owner = ChatClient.Instance.Users.First(z => z.Name == owner);
                    }
                    foreach (var uitem in chat.Descendants("user"))
                    {
                        var unm = uitem.Attribute("name").Value;
                        UserInfo ur = ChatClient.Instance.Users.FirstOrDefault(z => z.Name == unm);
                        if(ur==null && unm == ChatMessageAreaControl.CurrentUser.Name)
                        {
                            ur = ChatMessageAreaControl.CurrentUser;
                        }
                        (_chat as GroupChatItem).Users.Add(ur);
                    }

                    _chat.Name = name;
                }
                ChatsListControl.Chats.Add(_chat);
                foreach (var msg in chat.Descendants("message"))
                {
                    var mtype = msg.Attribute("type").Value;
                    if (mtype == "text")
                    {
                        var dt = DateTime.Parse(msg.Attribute("stamp").Value);
                        var owner = msg.Attribute("owner").Value;
                        if (ChatMessageAreaControl.CurrentUser.Name == owner)
                        {
                            _chat.Messages.Add(new TextChatMessage(dt, msg.Value) { Owner = ChatMessageAreaControl.CurrentUser });
                        }
                        else
                        {
                            _chat.Messages.Add(new TextChatMessage(dt, msg.Value) { Owner = ChatClient.Instance.Users.First(z => z.Name == owner) });
                        }

                    }
                    if (mtype == "image_link")
                    {
                        var path = msg.Value;
                        var dt = DateTime.Parse(msg.Attribute("stamp").Value);
                        var owner = msg.Attribute("owner").Value;

                        if (ChatMessageAreaControl.CurrentUser.Name == owner)
                        {
                            _chat.Messages.Add(new ImageLinkChatMessage() { DateTime = dt, Path = path, Owner = ChatMessageAreaControl.CurrentUser });
                        }
                        else
                        {
                            _chat.Messages.Add(new ImageLinkChatMessage() { DateTime = dt, Path = path, Owner = ChatClient.Instance.Users.First(z => z.Name == owner) });
                        }
                    }
                }
            }
        }

        public void SaveChats()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("<?xml version=\"1.0\"?>");
            sb.AppendLine("<root>");
            sb.AppendLine("<users>");
            foreach (var item in ChatsListControl.Chats.OfType<OnePersonChatItem>())
            {
                sb.AppendLine($"<user id=\"{item.Person.Id}\" name=\"{item.Person.Name}\"/>");
            }
            sb.AppendLine("</users>");
            foreach (var item in ChatsListControl.Chats)
            {
                if (item is OnePersonChatItem op)
                {

                    sb.AppendLine($"<chat type=\"simple\" person=\"{op.Person.Name}\">");
                    sb.AppendLine("<messages>");
                    foreach (var msg in item.Messages)
                    {
                        if (msg is TextChatMessage tcm)
                        {
                            sb.AppendLine($"<message type=\"text\" stamp=\"{tcm.DateTime}\" owner=\"{tcm.Owner.Name}\">");
                            sb.AppendLine($"<![CDATA[{tcm.Text}]]>");
                            sb.AppendLine("</message>");
                        }
                        if (msg is ImageLinkChatMessage ilcm)
                        {
                            sb.AppendLine($"<message type=\"image_link\" stamp=\"{ilcm.DateTime}\" owner=\"{ilcm.Owner.Name}\">");
                            sb.AppendLine($"<![CDATA[{ilcm.Path}]]>");
                            sb.AppendLine("</message>");
                        }

                    }
                    sb.AppendLine("</messages>");
                    sb.AppendLine("</chat>");
                }
                if (item is GroupChatItem gc)
                {
                    sb.AppendLine($"<chat type=\"group\" name=\"{gc.Name}\" owner=\"{gc.Owner.Name}\">");
                    sb.AppendLine("<users>");
                    foreach (var user in gc.Users)
                    {
                        sb.AppendLine($"<user name=\"{user.Name}\" id=\"{user.Id}\"/>");
                    }
                    sb.AppendLine("</users>");
                    sb.AppendLine("<messages>");
                    foreach (var msg in item.Messages)
                    {
                        if (msg is TextChatMessage tcm)
                        {
                            sb.AppendLine($"<message type=\"text\" stamp=\"{tcm.DateTime}\" owner=\"{tcm.Owner.Name}\">");
                            sb.AppendLine($"<![CDATA[{tcm.Text}]]>");
                            sb.AppendLine("</message>");
                        }
                        if (msg is ImageLinkChatMessage ilcm)
                        {
                            sb.AppendLine($"<message type=\"image_link\" stamp=\"{ilcm.DateTime}\" owner=\"{ilcm.Owner.Name}\">");
                            sb.AppendLine($"<![CDATA[{ilcm.Path}]]>");
                            sb.AppendLine("</message>");
                        }

                    }
                    sb.AppendLine("</messages>");
                    sb.AppendLine("</chat>");
                }
            }
            sb.AppendLine("</root>");
            File.WriteAllText("chats.xml", sb.ToString());
        }

        Stack<UIPanel> stack = new Stack<UIPanel>();

        public void BackToControl()
        {
            if (stack.Any())
            {
                var p = stack.Pop();
                mainPanel.Elements[1] = p;
            }
        }

        internal void SwitchLayoutToControl(UIPanel panel)
        {
            stack.Push(mainPanel.Elements[1] as UIPanel);
            panel.Parent = mainPanel;
            mainPanel.Elements[1] = panel;
        }

        private void PictureBox1_DragDrop(object sender, DragEventArgs e)
        {
            var dt = e.Data.GetData(DataFormats.FileDrop);
            if (dt != null)
            {
                var dt2 = dt as string[];
                bool good = false;
                foreach (var item in dt2)
                {
                    try
                    {
                        var bmp = Bitmap.FromFile(item) as Bitmap;
                        ChatTextBoxControl.Instance.BitmapContent = bmp;
                        good = true;
                        break;
                    }
                    catch (Exception ex)
                    {

                    }
                }
                if (!good)
                {
                    ChatTextBoxControl.Instance.AttachedFiles = dt2;
                }
            }
            else
            {
                var bmp = (Bitmap)e.Data.GetData(DataFormats.Bitmap);
            }

        }

        private void PictureBox1_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.Bitmap))
                e.Effect = DragDropEffects.Move;

            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effect = DragDropEffects.Copy;
            }
        }

        private void Form1_MouseWheel(object sender, MouseEventArgs e)
        {
            UIMouseWheel w = new UIMouseWheel() { Delta = e.Delta };
            foreach (var item in Elements)
            {
                if (w.Handled) break;
                item.Event(w);
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            mf = new MessageFilter();
            Application.AddMessageFilter(mf);
        }

        MessageFilter mf = null;
        ChatControl chatControl;
        SettingsControl settings;
        GroupEditControl groupEdit;

        public void SwitchToChat()
        {
            mainPanel.Elements[1] = chatControl;

        }

        public void SwitchLayoutSettings()
        {
            if (mainPanel.Elements[1] != settings)
            {
                mainPanel.Elements[1] = settings;
            }
            else
            {
                Settings.SaveSettings();
                mainPanel.Elements[1] = chatControl;
            }
        }

        public void SwitchLayoutGroupEdit(GroupChatItem group = null)
        {
            if (mainPanel.Elements[1] != groupEdit)
            {
                mainPanel.Elements[1] = groupEdit;
                groupEdit.Init(group);
            }
            else
            {
                mainPanel.Elements[1] = chatControl;
            }
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            var ev = new UIKeyDown() { Key = e };
            if (root.CurrentFocusOwner != null)
            {
                (root.CurrentFocusOwner as UIElement).Event(ev);
            }
            base.OnKeyDown(e);
        }

        private void Form1_SizeChanged(object sender, EventArgs e)
        {
            if (Left < 0)
            {

            }
        }

        RootElement root;


        private void connectStart()
        {
            Thread th = new Thread(() =>
            {
                while (true)
                {
                    try
                    {
                        connect(Settings.ServerIP, Settings.ServerPort);
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
            Invoke(this, () =>
            {
                foreach (var chat in ChatsListControl.Chats.OfType<OnePersonChatItem>())
                {
                    chat.Person.Online = false;
                }
                foreach (var item in ChatClient.Instance.Users)
                {
                    if (item.Name == ChatMessageAreaControl.CurrentUser.Name) continue;
                    var fr = ChatsListControl.Chats.OfType<OnePersonChatItem>().FirstOrDefault(z => z.Person.Name == item.Name);
                    if (fr != null)
                        fr.Person.Online = true;
                    if (fr == null)
                    {
                        ChatsListControl.Chats.Add(new OnePersonChatItem() { Person = item, Name = item.Name });
                        item.Online = true;
                    }
                }

                /* listView2.Items.Clear();
                 foreach (var userInfo in client.Users)
                 {
                     listView2.Items.Add(new ListViewItem(new string[] { userInfo.Name }) { Tag = userInfo });
                 }*/
            });
        }
        ChatClient client;
        void connect(string ipAddr, int port)
        {

            //client.Nickname = textBox3.Text;
            client.OnClientsListUpdate = UpdateClientsList;
            client.OnError = (msg) =>
            {
                /*richTextBox1.Invoke((Action)(() =>
                {
                    richTextBox1.Text += $"{DateTime.Now.ToLongTimeString()}: {msg}";

                }));*/
            };
            client.OnFileRecieved = (user, path, size) =>
            {
                Invoke((Action)(() =>
                {
                    var fr = ChatsListControl.Chats.OfType<OnePersonChatItem>().FirstOrDefault(z => z.Person.Name == user);
                    if (fr != null)
                    {
                        fr.Person.Online = true;
                        var fi = new FileInfo(path);

                        var uri = new Uri(fi.FullName);
                        if (uri.LocalPath.EndsWith(".jpg"))
                        {
                            fr.AddMessage(new ImageLinkChatMessage() { Path = uri.LocalPath.ToString(), Owner = ChatClient.Instance.Users.First(z => z.Name == user) });
                        }
                        else
                        {
                            fr.AddMessage(new TextChatMessage(DateTime.Now, uri.AbsoluteUri.ToString()) { Owner = ChatClient.Instance.Users.First(z => z.Name == user) });
                        }
                    }
                }));

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
                Invoke((Action)(() =>
                {
                    var fr = ChatsListControl.Chats.OfType<OnePersonChatItem>().FirstOrDefault(z => z.Person.Name == user);
                    if (fr != null)
                    {
                        fr.Person.Online = true;
                        fr.AddMessage(new TextChatMessage(DateTime.Now, str) { Owner = ChatClient.Instance.Users.First(z => z.Name == user) });
                    }
                }));

            };
            client.OnGroupMsgRecieved = (user, group, str) =>
            {
                Invoke((Action)(() =>
                {
                    var fr = ChatsListControl.Chats.OfType<GroupChatItem>().FirstOrDefault(z => z.Name == group);
                    if (fr != null)
                    {
                        fr.AddMessage(new TextChatMessage(DateTime.Now, str) { Owner = ChatClient.Instance.Users.First(z => z.Name == user) });
                    }
                }));

            };
            client.OnTyping = (user) =>
            {
                Invoke((Action)(() =>
                {
                    var fr = ChatsListControl.Chats.OfType<OnePersonChatItem>().FirstOrDefault(z => z.Person.Name == user);
                    if (fr != null)
                    {
                        fr.LastTyping = DateTime.Now;
                    }
                }));

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
            if (Elements.OfType<UIButton>().Any(z =>
             {
                 return z.Rect.Contains(ctx.GetCursor());
             })) return;
            var pos = ctx.GetCursor();
            if (pos.Y < headerHeight)
            {
                if (pos.X < 20)
                {
                    Close();
                    return;
                }
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
            if (pictureBox1.IsDisposed) return;
            var bd = new UIMouseButtonUp() { Position = ctx.GetCursor(), Button = e.Button, Context = ctx };
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

            if (!Elements.OfType<UIButton>().Any(z =>
            {
                return z.Rect.Contains(ctx.GetCursor());
            }))
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

            var bd = new UIMouseButtonDown() { Position = ctx.GetCursor(), Button = e.Button, Context = ctx };
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
                    ctx.SetTempCursor(Cursors.SizeWE, 2);
                }
                if ((pos2.Y < 5 || (pos2.Y < Height && pos2.Y > (Height - 5))))
                {
                    ctx.SetTempCursor(Cursors.SizeNS, 2);
                }
                if (((pos2.X < Width && pos2.X > (Width - 10)) && (pos2.Y < Height && pos2.Y > (Height - 5))))
                {
                    ctx.SetTempCursor(Cursors.SizeNWSE, 2);
                }
            }

            if (WindowState == FormWindowState.Normal && captionCaptured && !resizeCaptured)
            {
                Left = startWindowPositionX - (captureCursorX - pos.X);
                Top = startWindowPositionY - (captureCursorY - pos.Y);
            }

            gr.Clear(Color.LightBlue);

            gr.FillRectangle((captionCaptured || (pos2.Y > 0 && pos2.Y < headerHeight)) ? Brushes.DarkGreen : Brushes.Navy, 0, 0, Width, headerHeight);

            gr.FillEllipse(client.Connected ? Brushes.LawnGreen : Brushes.Yellow, 1, 1, 15, 15);

            if (!client.Connected)
            {
                foreach (var item in ChatsListControl.Chats.OfType<OnePersonChatItem>())
                {
                    item.Person.Online = false;
                }
            }
            gr.DrawEllipse(Pens.Black, 1, 1, 15, 15);
            var ms = gr.MeasureString($"Team Online ({Settings.Nickname})", SystemFonts.DefaultFont);
            gr.DrawString($"Team Online ({Settings.Nickname})", SystemFonts.DefaultFont, Brushes.Azure, Width / 2 - ms.Width / 2, 3);

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
            Elements[1].Rect = new Rectangle(Width - headerHeight * 2 - 1, 0, headerHeight, headerHeight - 1);
            mainPanel.Rect = new Rectangle(0, headerHeight, Width, Height - headerHeight);
            root.Rect = new Rectangle(0, headerHeight, Width, Height - headerHeight);

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

    public abstract class ChatItem
    {
        public string Name;
        public List<ChatMessage> Messages = new List<ChatMessage>();
        public void AddMessage(ChatMessage msg)
        {
            Messages.Add(msg);
            if (msg.Owner.Name != ChatMessageAreaControl.CurrentUser.Name)
                NewMessagesCounter++;
        }
        public int NewMessagesCounter;
    }

    public class OnePersonChatItem : ChatItem
    {
        public UserInfo Person;
        public DateTime LastTyping;
    }

    public class GroupChatItem : ChatItem
    {
        public List<UserInfo> Users = new List<UserInfo>();
        public UserInfo Owner;
    }

    public abstract class ChatMessage
    {
        public ChatItem Parent;
        public UserInfo Owner;
        public DateTime DateTime;
    }

    public class TextChatMessage : ChatMessage
    {
        public TextChatMessage(DateTime time, string text)
        {
            Text = text;
            DateTime = time;
        }
        public string Text;
    }
    public class ImageChatMessage : ChatMessage
    {
        public Bitmap Thumbnail;
        public string Path;
    }
    public class ImageLinkChatMessage : ChatMessage
    {
        public string Path;
        public Bitmap Thumbnail;
        public void GenerateThumbnail()
        {
            if (Thumbnail != null) return;
            var bmp = Bitmap.FromFile(Path);
            var aspect = bmp.Height / (float)bmp.Width;
            Thumbnail = new Bitmap(120, (int)(120 * aspect));
            var gr = Graphics.FromImage(Thumbnail);
            gr.DrawImage(bmp, new RectangleF(0, 0, Thumbnail.Width, Thumbnail.Height), new RectangleF(0, 0, bmp.Width, bmp.Height), GraphicsUnit.Pixel);
            gr.Dispose();
            bmp.Dispose();
        }
    }

    public class UserInfo
    {
        public int Id;
        public string Name;
        public bool Online;
    }


    public class RemoteControlVisualizer : UIElement
    {
        public override void Draw(DrawingContext ctx)
        {

        }

        public override void Event(UIEvent ev)
        {

        }
    }
}
