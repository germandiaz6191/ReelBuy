using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Formats.Jpeg;
using System.IO;

namespace ReelBuy.Backend.Helpers
{
    public class ImageResizer : IImageResizer
    {
        public byte[] ResizeImage(byte[] imageBytes, int maxWidth, int maxHeight, int quality)
        {
            using (var image = Image.Load(imageBytes))
            {
                // Calcular nueva dimensión si la imagen excede las dimensiones máximas
                if (image.Width > maxWidth || image.Height > maxHeight)
                {
                    var ratioX = (float)maxWidth / image.Width;
                    var ratioY = (float)maxHeight / image.Height;
                    var ratio = Math.Min(ratioX, ratioY);

                    var newWidth = (int)(image.Width * ratio);
                    var newHeight = (int)(image.Height * ratio);

                    image.Mutate(x => x.Resize(newWidth, newHeight));
                }

                // Convertir la imagen a un byte array en formato JPEG con calidad ajustada
                using (var ms = new MemoryStream())
                {
                    var encoder = new JpegEncoder { Quality = quality };
                    image.Save(ms, encoder);
                    return ms.ToArray();
                }
            }
        }
    }
}