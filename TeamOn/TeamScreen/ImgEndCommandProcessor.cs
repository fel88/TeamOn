using System;
using System.Drawing;
using System.IO;

namespace TeamOn.TeamScreen
{
    public class ImgEndCommandProcessor : CommandProcessor
    {
        public event Action<Bitmap> ImageCaptured;

        public static Bitmap LastBitmap = null;
        public static int FramesReceived = 0;
        public static float Fps = 0;
        public static DateTime LastTime = DateTime.Now;
        public override bool Process(CommandContext ctx)
        {
            var str = ctx.Command;
            if (str.StartsWith("IMGEND"))
            {
                try
                {
                    MemoryStream ms = new MemoryStream(ChunkCommandProcessor.Data);
                    var bmp = Bitmap.FromStream(ms) as Bitmap;
                    var mss = (DateTime.Now - LastTime).TotalMilliseconds;
                    if (mss >= 1000)
                    {
                        LastTime = DateTime.Now;
                        Fps = (float)(FramesReceived / (mss / 1000f));
                        FramesReceived = 0;
                    }
                    ms.Dispose();
                    FramesReceived++;
                    Bitmap temp = null;
                    temp = LastBitmap;
                    if (LastBitmap != null)
                    {
                        if (ChunkCommandProcessor.IsDelta)
                        {
                            bmp = Stuff.AddDiff(LastBitmap, bmp);
                        }
                    }

                    if (ImageCaptured != null)
                    {
                        ImageCaptured(Stuff.CloneViaCopyBytes(bmp));
                    }

                    LastBitmap = Stuff.CloneViaCopyBytes(bmp);
                    if (temp != null)
                    {
                        //temp.Dispose();
                    }

                    ChunkCommandProcessor.Data = null;
                }
                catch (Exception ex)
                {

                }
                return true;
            }
            return false;
        }
    }
}
