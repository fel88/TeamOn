using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Text;

namespace TeamOn.TeamScreen
{
    public static class Stuff
    {
        public static byte[] GetRGBValues(Bitmap bmp)
        {
            Rectangle rect = new Rectangle(0, 0, bmp.Width, bmp.Height);
            BitmapData bmpData = bmp.LockBits(rect, ImageLockMode.ReadOnly, bmp.PixelFormat);

            IntPtr ptr = bmpData.Scan0;

            int bytes = bmpData.Stride * bmp.Height;
            byte[] rgbValues = new byte[bytes];

            Marshal.Copy(ptr, rgbValues, 0, bytes);
            bmp.UnlockBits(bmpData);

            return rgbValues;
        }

        public static Bitmap SetRGBValues(Bitmap bmp, byte[] bb1)
        {
            Rectangle rect = new Rectangle(0, 0, bmp.Width, bmp.Height);
            BitmapData bmpData = bmp.LockBits(rect, ImageLockMode.ReadOnly, bmp.PixelFormat);

            IntPtr ptr = bmpData.Scan0;

            int bytes = bmpData.Stride * bmp.Height;

            Marshal.Copy(bb1, 0, ptr, bytes);
            bmp.UnlockBits(bmpData);
            return bmp;
        }

        public static Bitmap BmpFromByteArray(int imageWidth, int imageHeight, byte[] bb1)
        {
            var ret = new Bitmap(imageWidth, imageHeight, PixelFormat.Format32bppArgb);
            SetRGBValues(ret, bb1);

            return ret;
        }

        public static Bitmap CloneViaCopyBytes(Bitmap bb)
        {
            var data = GetRGBValues(bb);
            return BmpFromByteArray(bb.Width, bb.Height, data);
        }

        public static Bitmap AddDiff(Bitmap b1, Bitmap d1)
        {
            var bb1 = GetRGBValues(b1);
            var bb2 = GetRGBValues(d1);
            for (int i = 0; i < bb1.Length; i += 4)
            {
                if (bb2[i + 3] != 0)
                {
                    for (int j = 0; j < 4; j++)
                    {
                        bb1[i + j] = bb2[i + j];
                    }
                }
            }
            return BmpFromByteArray(b1.Width, b1.Height, bb1);
        }

        public static string CreateMD5(string input)
        {
            using (System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create())
            {
                byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(input);
                byte[] hashBytes = md5.ComputeHash(inputBytes);

                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < hashBytes.Length; i++)
                {
                    sb.Append(hashBytes[i].ToString("X2"));
                }
                return sb.ToString();
            }
        }
    }
}
