using System;
using System.IO;

public static class ImageConverter
{
    public static string ConvertToBase64(string imagePath)
    {
        byte[] imageBytes = File.ReadAllBytes(imagePath);
        return Convert.ToBase64String(imageBytes);
    }
}
