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
        // Felhasználó hozzáadása a Firebase adatbázishoz
        public async Task<bool> AddUser(User user)
        {
            try
            {
                // Felhasználó adatainak mentése a "Users" gyűjteménybe a felhasználói azonosítóval
                await FirebaseService
                    .Client
                    .Child("Users")  // "Users" gyűjtemény
                    .Child(user.UserId.ToString())  // Az int típusú UserId stringgé konvertálva
                    .PutAsync(JsonConvert.SerializeObject(user));  // Az adatokat JSON formátumban mentjük

                return true;  // Ha sikeres volt a mentés
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error adding user: {ex.Message}");
                return false;  // Hiba esetén false
            }
        }

        // Felhasználó lekérése a Firebase-ből a UserId alapján
        public async Task<User> GetUserById(int userId)
        {
            try
            {
                // Lekérjük a felhasználót a "Users" gyűjteményből a megadott UserId alapján
                var userData = await FirebaseService
                    .Client
                    .Child("Users")  // "Users" gyűjtemény
                    .Child(userId.ToString())  // Az int típusú UserId stringgé konvertálva
                    .OnceSingleAsync<User>();  // Az adatokat User típusra deszerializáljuk

                return userData;  // A felhasználó adatait visszaadjuk
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching user: {ex.Message}");
                return null;  // Hiba esetén null-t adunk vissza
            }
        }
    }
}
