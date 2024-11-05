using Firebase.Database;
using Firebase.Database.Query;
using Newtonsoft.Json;
using QuickReserve.Models;
using System;
using System.Threading.Tasks;

namespace QuickReserve.Services
{
    public class UserService
    {
        public async Task<bool> AddUser(User user)
        {
            try
            {
                // Felhasználó adatainak mentése a "Users" gyűjteménybe a felhasználói azonosítóval
                await FirebaseService
                    .Client
                    .Child("Users")
                    .Child(user.UserId.ToString())
                    .PutAsync(JsonConvert.SerializeObject(user));

                return true; // Sikeres hozzáadás esetén true értéket adunk vissza
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error adding user: {ex.Message}");
                return false; // Hiba esetén false értéket adunk vissza
            }
        }
    }
}
