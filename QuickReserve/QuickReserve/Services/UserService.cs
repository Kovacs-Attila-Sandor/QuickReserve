using Firebase.Database;
using Firebase.Database.Query;
using Newtonsoft.Json;
using QuickReserve.Models;
using System;
using System.Linq;
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
                // Az új felhasználó ID-jának automatikus generálása
                user.UserId = Guid.NewGuid().ToString();    

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
        public async Task<User> GetUserById(string userId)
        {
            try
            {
                // Lekérjük a felhasználót a "Users" gyűjteményből a megadott UserId alapján
                var userData = await FirebaseService
                    .Client
                    .Child("Users")  // "Users" gyűjtemény
                    .Child(userId)  // A UserId már string
                    .OnceSingleAsync<User>();  // Az adatokat User típusra deszerializáljuk

                return userData;  // A felhasználó adatait visszaadjuk
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching user: {ex.Message}");
                return null;  // Hiba esetén null-t adunk vissza
            }
        }

        // Felhasználó lekérése a Firebase-ből a UserName alapján
        public async Task<User> GetUserByName(string userName)
        {
            try
            {
                // Lekérjük az összes felhasználót a "Users" gyűjteményből
                var allUsers = await FirebaseService
                    .Client
                    .Child("Users")
                    .OnceAsync<User>();

                // Megkeressük az első olyan felhasználót, amelynek UserName tulajdonsága megegyezik a keresett névvel
                var user = allUsers
                    .Select(u => u.Object)  // Csak a felhasználói objektumokat vesszük figyelembe
                    .FirstOrDefault(u => u.Name == userName);  // Feltétel a UserName-re

                return user;  // Ha találtunk egyezést, visszaadjuk a felhasználót
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching user by name: {ex.Message}");
                return null;  // Hiba esetén null-t adunk vissza
            }
        }

        public async Task<bool> ValidateUserCredentials(string userName, string password)
        {
            try
            {
                // Lekérjük az összes felhasználót a "Users" gyűjteményből
                var allUsers = await FirebaseService
                    .Client
                    .Child("Users")
                    .OnceAsync<User>();

                // Megkeressük az első olyan felhasználót, amelynek UserName és Password tulajdonságai megegyeznek a keresett értékekkel
                var user = allUsers
                    .Select(u => u.Object)  // Csak a felhasználói objektumokat vesszük figyelembe
                    .FirstOrDefault(u => u.Name == userName && u.Password == password);  // Feltételek a UserName és Password-ra

                // Ha találtunk egyezést, visszatérünk true-val, ellenkező esetben false
                return user != null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error validating user credentials: {ex.Message}");
                return false;  // Hiba esetén false-t adunk vissza
            }
        }

    }
}
