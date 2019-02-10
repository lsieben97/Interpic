using Interpic.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interpic.Utils
{
    public static class ImageUtils
    {
        public static Image CropImage(Image source, ElementBounds bounds)
        {
            Bitmap nb = new Bitmap(bounds.Size.Width, bounds.Size.Height);
            Graphics g = Graphics.FromImage(nb);
            g.DrawImage(source, -bounds.Location.X, -bounds.Location.Y);
            return nb;
        }

        public static string ImageToBase64(Image image, System.Drawing.Imaging.ImageFormat format)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                // Convert Image to byte[]
                image.Save(ms, format);
                byte[] imageBytes = ms.ToArray();

                // Convert byte[] to Base64 String
                string base64String = Convert.ToBase64String(imageBytes);
                return base64String;
            }
        }
    }
}
