using Firebase.Database.Query;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace QuickReserve.Services
{
    public class UserService
    {
        private static UserService _instance;

        private UserService() { }

        public static UserService Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new UserService();
                }
                return _instance;
            }
        }

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

        public async Task<(string UserId, string UserStatus)> GetUserIdAndUserTypeByEmail(string Email)
        {
            try
            {
                // Lekérjük az összes felhasználót a "Users" gyűjteményből
                var allUsers = await FirebaseService
                    .Client
                    .Child("Users")
                    .OnceAsync<User>();

                // Megkeressük az első olyan felhasználót, amelynek Email tulajdonsága megegyezik a keresett névvel
                var user = allUsers
                    .Select(u => u.Object)  // Csak a felhasználói objektumokat vesszük figyelembe
                    .FirstOrDefault(u => u.Email == Email);  // Feltétel a Email-re

                if (user != null)
                {
                    // Ha találtunk egyezést, visszaadjuk a UserId és UserStatus értékeket
                    return (user.UserId, user.Role);
                }
                else
                {
                    return (null, null); // Ha nem találunk felhasználót
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching user by email: {ex.Message}");
                return (null, null);  // Hiba esetén null értékeket adunk vissza
            }
        }

        public async Task<string> GetUserTypeByUserId(string userId)
        {
            var allUsers = await FirebaseService
                .Client
                .Child("Users")
                .OnceAsync<User>();

            var user = allUsers
                .Select(u => u.Object)
                .FirstOrDefault(u => u.UserId == userId);

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
