using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace TeamOn.Controls
{
    public class UITextBox : UIElement, IFocusContainer
    {
        public UITextBox()
        {
            RootElement.Instance.RegisterFocusContainer(this);
        }

        public Color BackColor = Color.White;
        public Color ForeColor = Color.Black;

        //string _text = string.Empty;
        public void SetText(string str)
        {
            Text = str;
            curretPosition = 0;
        }
        public string Text = string.Empty;

        int blinkCounter = 0;
        int blinkPeriod = 40;
        public bool Focused { get; set; }
        public string[] AttachedFiles;

        public int TopMargin = 3;
        public override void Draw(DrawingContext ctx)
        {
            if (!Visible) return;

            blinkCounter++;
            blinkCounter %= blinkPeriod;
            var bound = Parent.GetRectangleOfChild(this).Value;
            ctx.Graphics.SetClip(bound);
            ctx.Graphics.FillRectangle(new SolidBrush(BackColor), bound.X, bound.Y, bound.Width, bound.Height);
            var pos = ctx.GetCursor();
            if (selectionMode)
            {
                var symb = GetSymbolUnderCursor(pos.X, ctx);
                endSelection = symb;
                curretPosition = endSelection;
                /*  if (endSelection < startSelection)
                  {
                      endSelection = startSelection;
                      startSelection = symb;
                      curretPosition = startSelection;
                  }*/

            }
            int imgShift = 0;
            if (BitmapContent != null || (AttachedFiles != null && AttachedFiles.Any()))
            {
                imgShift = bound.Height + 10;
            }
            if (selectionLegth > 0)
            {
                var ss = Math.Min(startSelection, endSelection);
                var es = Math.Max(startSelection, endSelection);
                var mss = MeasureDisplayString(ctx.Graphics, Text.Substring(ss, es - ss), SystemFonts.DefaultFont);
                int right = bound.X + 10 + imgShift;
                if (ss > 0)
                    right += MeasureDisplayStringWidth(ctx.Graphics, Text.Substring(0, ss), SystemFonts.DefaultFont);

                ctx.Graphics.FillRectangle(Brushes.Blue, right, mss.Y + bound.Y + TopMargin, mss.Width, mss.Height);
            }

            if (bound.Contains(pos))
            {
                ctx.SetTempCursor(Cursors.IBeam);
            }

            if (Text.Length > 0 || Focused)
            {
                var ms = ctx.Graphics.MeasureString(Text, SystemFonts.DefaultFont);
                ctx.Graphics.DrawString(Text, SystemFonts.DefaultFont, new SolidBrush(ForeColor), bound.X + 10 + imgShift, bound.Y + TopMargin, StringFormat.GenericTypographic);
            }
            else
            {
                if (!string.IsNullOrEmpty(WatermarkText))
                    ctx.Graphics.DrawString(WatermarkText, SystemFonts.DefaultFont, Brushes.Gray, bound.X + 10 + imgShift, bound.Y + TopMargin);
            }

            if (Focused && blinkCounter < blinkPeriod / 2)
            {
                ctx.Graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
                var mms = ctx.Graphics.MeasureCharacterRanges(Text.Substring(0, curretPosition), SystemFonts.DefaultFont, bound, StringFormat.GenericDefault);
                float max = bound.X;
                if (mms.Any())
                {
                    max = mms.Max(z => z.GetBounds(ctx.Graphics).Right);
                }
                var ms2 = TextRenderer.MeasureText(Text.Substring(0, curretPosition), SystemFonts.DefaultFont);
                int right = bound.X + 10 + imgShift;
                if (curretPosition > 0)
                {
                    right += MeasureDisplayStringWidth(ctx.Graphics, Text.Substring(0, curretPosition), SystemFonts.DefaultFont);
                }
                var ms3 = ctx.Graphics.MeasureString("Aa", SystemFonts.DefaultFont);
                //ctx.Graphics.DrawLine(Pens.Black, max, bound.Y + 5, max, bound.Y + ms3.Height + 5);
                ctx.Graphics.DrawLine(Pens.Black, right, bound.Y + TopMargin, right, bound.Y + ms3.Height + TopMargin);
            }


            if (BitmapContent != null)
            {
                ctx.Graphics.DrawImage(BitmapContent, new RectangleF(bound.X, bound.Y, bound.Height, bound.Height), new RectangleF(0, 0, BitmapContent.Width, BitmapContent.Height), GraphicsUnit.Pixel);

            }

            ctx.Graphics.ResetClip();

        }
        static public int MeasureDisplayStringWidth(Graphics graphics, string text,
                                            Font font)
        {
            System.Drawing.StringFormat format = new System.Drawing.StringFormat();
            System.Drawing.RectangleF rect = new System.Drawing.RectangleF(0, 0,
                                                                          1000, 1000);
            System.Drawing.CharacterRange[] ranges =       {
                new System.Drawing.CharacterRange(0,
text.Length) };
            System.Drawing.Region[] regions = new System.Drawing.Region[1];

            format.SetMeasurableCharacterRanges(ranges);

            regions = graphics.MeasureCharacterRanges(text, font, rect, format);
            rect = regions[0].GetBounds(graphics);

            return (int)(rect.Width);
        }
        static public RectangleF MeasureDisplayString(Graphics graphics, string text,
                                            Font font)
        {
            System.Drawing.StringFormat format = new System.Drawing.StringFormat();
            System.Drawing.RectangleF rect = new System.Drawing.RectangleF(0, 0,
                                                                          1000, 1000);
            System.Drawing.CharacterRange[] ranges =       {
                new System.Drawing.CharacterRange(0,
text.Length) };
            System.Drawing.Region[] regions = new System.Drawing.Region[1];

            format.SetMeasurableCharacterRanges(ranges);

            regions = graphics.MeasureCharacterRanges(text, font, rect, format);
            rect = regions[0].GetBounds(graphics);

            return rect;
        }
        public string WatermarkText;
        public string KeyCodeToUnicode(Keys key)
        {
            byte[] keyboardState = new byte[255];
            bool keyboardStateStatus = GetKeyboardState(keyboardState);

            if (!keyboardStateStatus)
            {
                return "";
            }

            uint virtualKeyCode = (uint)key;
            uint scanCode = MapVirtualKey(virtualKeyCode, 0);
            IntPtr inputLocaleIdentifier = GetKeyboardLayout(0);

            StringBuilder result = new StringBuilder();
            ToUnicodeEx(virtualKeyCode, scanCode, keyboardState, result, (int)5, (uint)0, inputLocaleIdentifier);

            return result.ToString();
        }

        [DllImport("user32.dll")]
        static extern bool GetKeyboardState(byte[] lpKeyState);

        [DllImport("user32.dll")]
        static extern uint MapVirtualKey(uint uCode, uint uMapType);

        [DllImport("user32.dll")]
        static extern IntPtr GetKeyboardLayout(uint idThread);

        [DllImport("user32.dll")]
        static extern int ToUnicodeEx(uint wVirtKey, uint wScanCode, byte[] lpKeyState, [Out, MarshalAs(UnmanagedType.LPWStr)] StringBuilder pwszBuff, int cchBuff, uint wFlags, IntPtr dwhkl);

        protected int curretPosition = 0;
        /*[DllImport("user32.dll")]
        static extern int MapVirtualKey(uint uCode, uint uMapType);*/

        public Action<UITextBox> TextChanged;

        bool selectionMode = false;
        int startSelection;
        int endSelection;
        int selectionLegth => Math.Abs(endSelection - startSelection);
        public bool ImagePastedAllowed = false;
        public Bitmap BitmapContent;


        public override void Event(UIEvent ev)
        {
            KeysConverter kc = new KeysConverter();
            var bound = GetBound();
            if (ev is UIMouseButtonUp mup)
            {
                selectionMode = false;
            }
            if (ev is UIMouseButtonDown md)
            {
                if (bound.Contains(md.Position))
                {
                    selectionMode = true;
                    //determine curret position here
                    RootElement.Instance.CaptureFocus(this);
                    blinkCounter = 0;
                    curretPosition = GetSymbolUnderCursor(md.Position.X, md.Context);
                    startSelection = curretPosition;
                    endSelection = curretPosition;
                }
            }
            if (ev is UIKeyDown kd)
            {
                if (kd.Key.KeyCode == Keys.Left)
                {
                    curretPosition--;



                    if (curretPosition < 0)
                    {
                        curretPosition = 0;
                    }
                    //if (kd.Key.Modifiers == Keys.Shift)
                    {

                    }
                    //else
                    {
                        startSelection = curretPosition;
                        endSelection = startSelection;
                    }
                    blinkCounter = 0;
                }
                else
                if (kd.Key.KeyCode == Keys.Right)
                {

                    if (curretPosition < Text.Length)
                    {
                        curretPosition++;
                    }
                    startSelection = curretPosition;
                    endSelection = startSelection;
                    blinkCounter = 0;

                }
                else if (kd.Key.KeyCode == Keys.End)
                {
                    if (kd.Key.Modifiers == Keys.Shift)
                    {
                        startSelection = curretPosition;
                        endSelection = Text.Length;
                    }
                    else
                    {
                        curretPosition = Text.Length;
                        blinkCounter = 0;
                    }


                }
                else if (kd.Key.KeyCode == Keys.Home)
                {
                    if (kd.Key.Modifiers == Keys.Shift)
                    {
                        startSelection = 0;
                        endSelection = curretPosition;
                    }
                    else
                    {
                        curretPosition = 0;
                        blinkCounter = 0;
                    }

                }

                else if (kd.Key.KeyCode == Keys.A && kd.Key.Modifiers == Keys.Control)
                {
                    startSelection = 0;
                    endSelection = Text.Length;
                }
                else if (kd.Key.KeyCode == Keys.V && kd.Key.Modifiers == Keys.Control)
                {
                    var str = Clipboard.GetText();
                    if (!string.IsNullOrEmpty(str))
                    {
                        if (selectionLegth > 0)
                        {
                            curretPosition = startSelection;

                            Text = Text.Remove(startSelection, selectionLegth);
                            Text = Text.Insert(curretPosition, str);
                            TextChanged?.Invoke(this);
                            endSelection = startSelection;
                        }
                        else
                        {
                            Text = Text.Insert(curretPosition, str);
                            curretPosition += str.Length;
                            TextChanged?.Invoke(this);
                        }
                    }
                    var img = Clipboard.GetImage();
                    if (img != null && ImagePastedAllowed)
                    {
                        BitmapContent = img as Bitmap;
                    }
                }
                else if
                (kd.Key.KeyCode == Keys.Delete)
                {
                    if (selectionLegth > 0)
                    {

                        var ss = Math.Min(startSelection, endSelection);
                        curretPosition = ss;
                        Text = Text.Remove(ss, selectionLegth);
                        TextChanged?.Invoke(this);
                        endSelection = ss;
                        startSelection = ss;
                    }
                    else
                  if (curretPosition < Text.Length)
                    {
                        Text = Text.Remove(curretPosition, 1);
                        TextChanged?.Invoke(this);
                        endSelection = startSelection;

                    }
                }
                else if
               (kd.Key.KeyCode == Keys.Back)
                {
                    if (curretPosition == 0 && BitmapContent != null)
                    {
                        BitmapContent = null;
                    }
                    if (selectionLegth > 0)
                    {

                        var ss = Math.Min(startSelection, endSelection);
                        curretPosition = ss;
                        Text = Text.Remove(ss, selectionLegth);
                        TextChanged?.Invoke(this);
                        endSelection = ss;
                        startSelection = ss;
                    }
                    else
                    if (Text.Length > 0 && curretPosition > 0)
                    {
                        Text = Text.Remove(curretPosition - 1, 1);
                        curretPosition--;
                        TextChanged?.Invoke(this);
                        startSelection = endSelection;
                    }


                }
                else
                {
                    var tt = KeyCodeToUnicode(kd.Key.KeyData);
                    if (tt.Any())
                    {
                        if (selectionLegth > 0)
                        {
                            var ss = Math.Min(startSelection, endSelection);
                            var es = Math.Max(startSelection, endSelection);
                            Text = Text.Remove(ss, selectionLegth);
                            endSelection = ss;
                            startSelection = ss;
                            curretPosition = ss;
                        }
                        Text = Text.Insert(curretPosition, tt);
                        curretPosition++;
                        TextChanged?.Invoke(this);
                    }
                }
            }
        }

        private int GetSymbolUnderCursor(int positionX, DrawingContext ctx)
        {
            var bound = GetBound();
            //get all shiftsx
            List<int> shifts = new List<int>();
            shifts.Add(0);
            for (int i = 1; i < Text.Length; i++)
            {
                shifts.Add(MeasureDisplayStringWidth(ctx.Graphics, Text.Substring(0, i), SystemFonts.DefaultFont));
            }
            if (!string.IsNullOrEmpty(Text))
                shifts.Add(MeasureDisplayStringWidth(ctx.Graphics, Text, SystemFonts.DefaultFont));

            int imgShift = 0;
            if (BitmapContent != null)
            {
                imgShift = bound.Height + 10;
            }

            var diffx = positionX - (bound.X + 10 + imgShift);
            int mindist = Math.Abs(shifts[0] - diffx);
            int minindex = 0;
            for (int i = 0; i < shifts.Count; i++)
            {
                var distx = Math.Abs(shifts[i] - diffx);
                if (distx < mindist)
                {
                    mindist = distx;
                    minindex = i;
                }
            }
            return minindex;
        }

        public void LostFocus()
        {
            endSelection = startSelection;
            curretPosition = endSelection;
        }
    }
}
