using System;
using Xamarin.Forms;

namespace QuickReserve.Converter
{
    public static class ImageConverter
    {
        public static ImageSource ConvertBase64ToImageSource(string base64)
        {
            if (string.IsNullOrEmpty(base64)) return null;

            byte[] imageBytes = Convert.FromBase64String(base64);
            return ImageSource.FromStream(() => new System.IO.MemoryStream(imageBytes));
        }
    }
}
