using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Xamarin.Forms;

namespace QuickReserve.Converter
{
    public class ImageConverter
    {
        public static ImageSource ConvertBase64ToImageSource(string base64String)
        {
            try
            {
                byte[] imageBytes = Convert.FromBase64String(base64String);
                return ImageSource.FromStream(() => new MemoryStream(imageBytes));
            }
            catch (Exception)
            {
                // Hibakezelés, ha a Base64 string hibás vagy a konvertálás nem sikerül
                return ImageSource.FromFile("default_image.png");  // Visszaadunk egy alapértelmezett képet hiba esetén
            }
        }

    }
}
