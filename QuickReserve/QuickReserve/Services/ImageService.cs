using System;
using System.Threading.Tasks;
using Firebase.Database.Query;
using QuickReserve.Services;

public class ImageService
{
    public async Task SaveImageAsync(string imagePath, string imageName)
    {
        try
        {
            // Konvertáld a képet Base64 formátumba
            string base64Image = ImageConverter.ConvertToBase64(imagePath);

            // Mentés a Firebase Realtime Database-be
            await FirebaseService.Client
                .Child("Images") // Gyökérkulcs az adatbázisban
                .Child(imageName) // Kép neve
                .PutAsync(base64Image); // Base64 adat mentése

            Console.WriteLine("Kép sikeresen mentve!");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Hiba történt a mentés során: {ex.Message}");
        }
    }
}
