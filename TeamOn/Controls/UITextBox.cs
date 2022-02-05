using System;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using TeamOn.Controls;

namespace TeamOn.Controls
{
    public class UITextBox : UIElement
    {
        public string Text = string.Empty;
        int blinkCounter = 0;
        int blinkPeriod = 40;
        public override void Draw(DrawingContext ctx)
        {
            if (!Visible) return;
            blinkCounter++;
            blinkCounter %= blinkPeriod;
            var bound = Parent.GetRectangleOfChild(this).Value;
            ctx.Graphics.FillRectangle(Brushes.White, bound.X, bound.Y, bound.Width, bound.Height);
            var pos = ctx.GetCursor();
            if (new Rectangle(bound.X, bound.Bottom - 50, bound.Width, 50).Contains(pos))
            {
                ctx.SetTempCursor(Cursors.IBeam);
            }
            if (Text.Length > 0)
            {
                var ms = ctx.Graphics.MeasureString(Text, SystemFonts.DefaultFont);
                var ms2 = ctx.Graphics.MeasureString(Text.Substring(0,curretPosition), SystemFonts.DefaultFont);
                ctx.Graphics.DrawString(Text, SystemFonts.DefaultFont, Brushes.Black, bound.X + 10, bound.Y + 5);
                if (blinkCounter < blinkPeriod / 2)
                {
                    ctx.Graphics.DrawLine(Pens.Black, bound.X + 10 + ms2.Width, bound.Y + 5, bound.X + 10 + ms2.Width, bound.Y + ms.Height + 5);
                }
            }
            else
            {
                ctx.Graphics.DrawString("Напишите сообщение", SystemFonts.DefaultFont, Brushes.Gray, bound.X + 10, bound.Y + 5);
            }

        }
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
        public override void Event(UIEvent ev)
        {
            KeysConverter kc = new KeysConverter();

            if (ev is UIKeyDown kd)
            {
                //int nonVirtualKey = MapVirtualKey((uint)kd.Key.KeyCode, 2);
                // char mappedChar = Convert.ToChar(nonVirtualKey);
                //  string keyChar = kc.ConvertToString(kd.Key.KeyCode);
                // Text += mappedChar;
                
                if (kd.Key.KeyCode == Keys.Left)
                {
                    curretPosition--;
                    if (curretPosition < 0) curretPosition = 0;
                }else if
                 (kd.Key.KeyCode == Keys.Back)
                {
                    if (Text.Length > 0)
                    {
                        Text = Text.Remove(curretPosition - 1, 1);
                        curretPosition--;
                    }
                }
                else
                {
                    var tt = KeyCodeToUnicode(kd.Key.KeyData);
                    if(tt.Any()){
                        Text = Text.Insert(curretPosition, tt);
                        curretPosition++;
                    }
                    
                }
            }
        }
    }

}
