using ReelBuy.Backend.Helpers;
using System.Drawing;
using System.Drawing.Imaging;

namespace Orders.Backend.Helpers;

public class ImageResizer : IImageResizer
{
    public byte[] ResizeImage(byte[] imageBytes, int maxWidth, int maxHeight, int quality)
    {
        using (var ms = new MemoryStream(imageBytes))
        {
            using (var originalImage = new Bitmap(ms))
            {
                int newWidth = originalImage.Width;
                int newHeight = originalImage.Height;

                if (newWidth > maxWidth || newHeight > maxHeight)
                {
                    float ratioX = (float)maxWidth / originalImage.Width;
                    float ratioY = (float)maxHeight / originalImage.Height;
                    float ratio = Math.Min(ratioX, ratioY);

                    newWidth = (int)(originalImage.Width * ratio);
                    newHeight = (int)(originalImage.Height * ratio);
                }

                // Create new resize image
                var resizedImage = new Bitmap(originalImage, new Size(newWidth, newHeight));

                // Image to array
                using (var outputMs = new MemoryStream())
                {
                    // Save image to PNG
                    resizedImage.Save(outputMs, ImageFormat.Png);
                    return outputMs.ToArray();
                }
            }
        }
    }
}