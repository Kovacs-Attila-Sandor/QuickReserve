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

        // Lekérdezi a felhasználó típusát az adatbázisból
        public async Task<string> GetUserType(string userName)
        {
            var allUsers = await FirebaseService
                .Client
                .Child("Users")
                .OnceAsync<User>();

            var user = allUsers
                .Select(u => u.Object)
                .FirstOrDefault(u => u.Name == userName);

            if (user != null)
            {
                return user.Role; // "User" vagy "Restaurant"
            }

            return null;
        }

        public async Task<string> GetUserNameByUserId(string userId)
        {
            try
            {
                // Lekérjük az összes felhasználót a Firebase "Users" gyűjteményből
                var allUsers = await FirebaseService
                    .Client
                    .Child("Users")
                    .OnceAsync<User>();

                // Megkeressük azt a felhasználót, akinek az ID-ja megegyezik az adott userId-vel
                var user = allUsers
                    .Select(u => u.Object)
                    .FirstOrDefault(u => u.UserId == userId);

                if (user != null)
                {
                    return user.Name; // Visszaadjuk a felhasználó nevét
                }

                return null; // Ha nem található a felhasználó
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Hiba a GetUserNameByUserId során: {ex.Message}");
                return null; // Hiba esetén visszatérünk null-lal
            }
        }


        public async Task<bool> UpdateUserProfile(string userId, User updatedUser)
        {
            try
            {
                // We check if the user exists by trying to fetch it from the database
                var existingUser = await GetUserById(userId);

                if (existingUser == null)
                {
                    // If the user doesn't exist, return false
                    Console.WriteLine("User not found.");
                    return false;
                }

                // Update the existing user's data with the new updatedUser data
                updatedUser.UserId = userId; // Ensure the updatedUser's UserId is set to the correct value

                // We update the user data in the Firebase "Users" collection using the UserId
                await FirebaseService
                    .Client
                    .Child("Users")  // "Users" collection
                    .Child(userId)  // Using the provided userId
                    .PutAsync(JsonConvert.SerializeObject(updatedUser));  // Saving the updated data as JSON

                return true;  // Return true if update was successful
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating user profile: {ex.Message}");
                return false;  // Return false if there was an error
            }
        }


    }
}
