using Firebase.Database.Query;
using Newtonsoft.Json;
using QuickReserve.Converter;
using QuickReserve.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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

        public async Task<bool> AddFavoriteFood(string userId, string foodId)
        {
            try
            {
                // Lekérjük a felhasználó adatait a Firebase-ból
                var user = await GetUserById(userId);
                if (user == null)
                {
                    Console.WriteLine($"User with ID {userId} not found.");
                    return false;
                }

                // Ha a LikedFoods még null, inicializáljuk egy üres listával
                if (user.LikedFoods == null)
                {
                    user.LikedFoods = new List<string>();
                }

                // Ellenőrizzük, hogy az étel már szerepel-e a kedvencek között
                if (user.LikedFoods.Contains(foodId))
                {
                    Console.WriteLine($"Food with ID {foodId} is already a favorite for user {userId}.");
                    return true; // Már kedvenc, nincs szükség frissítésre
                }

                // Hozzáadjuk az új FoodId-t a LikedFoods listához
                user.LikedFoods.Add(foodId);

                // Frissítjük a felhasználó adatait a Firebase-ban
                await FirebaseService
                    .Client
                    .Child("Users")
                    .Child(userId)
                    .PutAsync(JsonConvert.SerializeObject(user));

                return true; // Sikeres mentés
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error adding favorite food: {ex.Message}");
                return false; // Hiba esetén false
            }
        }

        public async Task<bool> RemoveFavoriteFood(string userId, string foodId)
        {
            try
            {
                // Lekérjük a felhasználó adatait
                var user = await GetUserById(userId);
                if (user == null)
                {
                    Console.WriteLine($"User with ID {userId} not found.");
                    return false;
                }

                // Ha a LikedFoods null vagy nem tartalmazza az ételt, nem kell törölni
                if (user.LikedFoods == null || !user.LikedFoods.Contains(foodId))
                {
                    Console.WriteLine($"Food with ID {foodId} is not a favorite for user {userId}.");
                    return true; // Nincs mit törölni, de "sikeres"
                }

                // Eltávolítjuk a FoodId-t a LikedFoods listából
                user.LikedFoods.Remove(foodId);

                // Frissítjük a Firebase-t
                await FirebaseService
                    .Client
                    .Child("Users")
                    .Child(userId)
                    .PutAsync(JsonConvert.SerializeObject(user));

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error removing favorite food: {ex.Message}");
                return false;
            }
        }

        public async Task<List<Food>> GetFavoriteFoods(string userId)
        {
            try
            {
                // 1. Lépés: Felhasználó lekérdezése a LikedFoods lista megszerzéséhez
                var user = await GetUserById(userId);
                if (user == null || user.LikedFoods == null || !user.LikedFoods.Any())
                {
                    Console.WriteLine($"No favorite foods found for user {userId}.");
                    return new List<Food>(); // Üres lista, ha nincs kedvenc étel
                }
                // 2. Lépés: Összes étterem lekérdezése a Firebase-ból
                var restaurantsResponse = await FirebaseService
                    .Client
                    .Child("Restaurant")
                    .OnceAsync<Restaurant>();

                if (restaurantsResponse == null || !restaurantsResponse.Any())
                {
                    Console.WriteLine("No restaurants found in the database.");
                    return new List<Food>(); // Üres lista, ha nincs étterem
                }

                // 3. Lépés: Összes étel összegyűjtése az éttermekből
                var allFoods = new List<Food>();
                foreach (var restaurant in restaurantsResponse)
                {
                    var restaurantData = restaurant.Object;
                    if (restaurantData.Foods != null && restaurantData.Foods.Any())
                    {
                        allFoods.AddRange(restaurantData.Foods);
                    }
                }

                // 4. Lépés: Kedvenc ételek kiválogatása a LikedFoods alapján
                var favoriteFoods = allFoods
                    .Where(food => user.LikedFoods.Contains(food.FoodId))
                    .ToList();

                return favoriteFoods;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting favorite foods: {ex.Message}");
                return new List<Food>(); // Hiba esetén üres lista
            }
        }

        public async Task<List<Restaurant>> GetFavoriteRestaurants(string userId)
        {
            try
            {
                // 1. Lépés: Felhasználó lekérdezése a LikedRestaurants lista megszerzéséhez
                var user = await GetUserById(userId);
                if (user == null || user.LikedRestaurants == null || !user.LikedRestaurants.Any())
                {
                    Console.WriteLine($"No favorite restaurants found for user {userId}.");
                    return new List<Restaurant>(); // Üres lista, ha nincs kedvenc étterem
                }

                // 2. Lépés: Összes étterem lekérdezése a Firebase-ból
                var restaurantsResponse = await FirebaseService
                    .Client
                    .Child("Restaurant")
                    .OnceAsync<Restaurant>();

                if (restaurantsResponse == null || !restaurantsResponse.Any())
                {
                    Console.WriteLine("No restaurants found in the database.");
                    return new List<Restaurant>(); // Üres lista, ha nincs étterem
                }

                // 3. Lépés: Kedvenc éttermek kiválogatása a LikedRestaurants alapján
                var favoriteRestaurants = restaurantsResponse
                    .Select(r => r.Object)
                    .Where(restaurant => user.LikedRestaurants.Contains(restaurant.RestaurantId))
                    .ToList();

                // 4. Lépés: ImageSourceUri beállítása minden étteremhez (opcionális, ha képeket használsz)
                foreach (var restaurant in favoriteRestaurants)
                {
                    if (!string.IsNullOrEmpty(restaurant.FirstImageBase64))
                    {
                        restaurant.ImageSourceUri = ImageConverter.ConvertBase64ToImageSource(restaurant.FirstImageBase64);
                    }
                }

                return favoriteRestaurants;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting favorite restaurants: {ex.Message}");
                return new List<Restaurant>(); // Hiba esetén üres lista
            }
        }
    }
}
