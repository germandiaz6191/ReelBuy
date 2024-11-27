using ReelBuy.Backend.Helpers;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace Orders.Backend.Helpers
{
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

                    // Calcular nueva dimensión si la imagen excede las dimensiones máximas
                    if (newWidth > maxWidth || newHeight > maxHeight)
                    {
                        float ratioX = (float)maxWidth / originalImage.Width;
                        float ratioY = (float)maxHeight / originalImage.Height;
                        float ratio = Math.Min(ratioX, ratioY);

                        newWidth = (int)(originalImage.Width * ratio);
                        newHeight = (int)(originalImage.Height * ratio);
                    }

                    // Crear la imagen redimensionada
                    var resizedImage = new Bitmap(originalImage, new Size(newWidth, newHeight));

                    // Convertir la imagen a un byte array en formato JPEG con calidad ajustada
                    using (var outputMs = new MemoryStream())
                    {
                        // Establecer la calidad de la imagen JPEG
                        var jpegEncoder = GetEncoder(ImageFormat.Jpeg);
                        var encoderParameters = new EncoderParameters(1);
                        encoderParameters.Param[0] = new EncoderParameter(Encoder.Quality, quality);

                        resizedImage.Save(outputMs, jpegEncoder, encoderParameters);
                        return outputMs.ToArray();
                    }
                }
            }
        }

        // Obtener el encoder para el formato de imagen
        private ImageCodecInfo GetEncoder(ImageFormat format)
        {
            var codecs = ImageCodecInfo.GetImageDecoders();
            foreach (var codec in codecs)
            {
                if (codec.FormatID == format.Guid)
                {
                    return codec;
                }
            }
            return null;
        }
    }
}