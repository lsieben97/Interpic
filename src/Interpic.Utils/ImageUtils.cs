using Interpic.Models;
using System;
using System.Drawing;
using System.IO;
using System.Windows.Media.Imaging;

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

        public static BitmapImage ImageFromString(string imageString, bool useInterpicUIPackage = true)
        {
            if (useInterpicUIPackage)
            {
                imageString = "/Interpic.UI;component/Icons/" + imageString;
            }
            return new BitmapImage(new Uri(imageString, UriKind.RelativeOrAbsolute));
        }
    }
}
