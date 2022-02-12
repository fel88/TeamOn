using System.Drawing;

namespace TeamOn
{
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
}
