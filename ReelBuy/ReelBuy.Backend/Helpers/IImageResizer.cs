namespace ReelBuy.Backend.Helpers;

public interface IImageResizer
{
    byte[] ResizeImage(byte[] imageBytes, int maxWidth, int maxHeight, int quality);
}