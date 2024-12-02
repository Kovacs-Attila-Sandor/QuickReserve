using Firebase.Database;
using Firebase.Database.Query;
using Newtonsoft.Json;
using QuickReserve.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace QuickReserve.Services
{
    public class RestaurantService
    {
        // Étterem hozzáadása a Firebase adatbázishoz
        public async Task<bool> addRestaurant(Restaurant restaurant)
        {
            try
            {
                // Az új étterem ID-jának automatikus generálása
                restaurant.RestaurantId = Guid.NewGuid().ToString();

                // Étterem adatainak mentése a "Restaurant" gyűjteménybe a RestaurantId alapján
                await FirebaseService
                    .Client
                    .Child("Restaurant")  // "Restaurant" gyűjtemény
                    .Child(restaurant.RestaurantId.ToString())  // Az int típusú RestaurantId stringgé konvertálva
                    .PutAsync(JsonConvert.SerializeObject(restaurant));  // Az adatokat JSON formátumban mentjük

                return true;  // Ha sikeres volt a mentés
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error adding restaurant: {ex.Message}");
                return false;  // Hiba esetén false
            }
        }

        // Étterem lekérése a Firebase-ből a RestaurantId alapján
        public async Task<Restaurant> GetRestaurantById(string restaurantId)
        {
            try
            {
                // Lekérjük az étterem adatokat a "Restaurant" gyűjteményből a megadott RestaurantId alapján
                var restaurantData = await FirebaseService
                    .Client
                    .Child("Restaurant")  // "Restaurant" gyűjtemény
                    .Child(restaurantId)  // A RestaurantId már string
                    .OnceSingleAsync<Restaurant>();  // Az adatokat Restaurant típusra deszerializáljuk

                return restaurantData;  // Az étterem adatainak visszaadása
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching restaurant: {ex.Message}");
                return null;  // Hiba esetén null-t adunk vissza
            }
        }
        
        public async Task<string> AddRestaurantAndGetId(Restaurant restaurant)
        {
            try
            {
                // Generálj egy egyedi azonosítót az étteremhez
                restaurant.RestaurantId = Guid.NewGuid().ToString();

                // Az étterem adatainak mentése a Firebase adatbázisba
                await FirebaseService
                    .Client
                    .Child("Restaurant")
                    .Child(restaurant.RestaurantId)
                    .PutAsync(JsonConvert.SerializeObject(restaurant));

                return restaurant.RestaurantId; // Visszaadjuk az étterem ID-t
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error adding restaurant: {ex.Message}");
                return null; // Hiba esetén null-t adunk vissza
            }
        }
        public async Task<bool> AddFoodToRestaurant(string restaurantId, Food food)
        {
            try
            {
                // Lekérjük az adott éttermet a Firebase adatbázisból
                var restaurant = await GetRestaurantById(restaurantId);
                if (restaurant != null)
                {
                    // Hozzáadjuk az új ételt az étterem Foods listájához
                    restaurant.Foods.Add(food);

                    // Frissítjük az étterem adatait a Firebase adatbázisban
                    await FirebaseService
                        .Client
                        .Child("Restaurant")
                        .Child(restaurantId)
                        .PutAsync(JsonConvert.SerializeObject(restaurant));

                    return true; // Sikeres hozzáadás
                }
                return false; // Ha az étterem nem található
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error adding food to restaurant: {ex.Message}");
                return false; // Hiba esetén false
            }
        }
        public async Task<bool> AddTablesToRestaurant(string restaurantId, List<Table> tables)
        {
            try
            {
                // Lekérjük az adott éttermet
                var restaurant = await GetRestaurantById(restaurantId);
                if (restaurant != null)
                {
                    // Hozzáadjuk az asztalokat az étteremhez
                    if (restaurant.Tables == null)
                        restaurant.Tables = new List<Table>();

                    restaurant.Tables.AddRange(tables);

                    // Frissítjük az étterem adatait a Firebase adatbázisban
                    await FirebaseService
                        .Client
                        .Child("Restaurant")
                        .Child(restaurantId)
                        .PutAsync(JsonConvert.SerializeObject(restaurant));

                    return true; // Sikeres mentés
                }
                return false; // Ha az étterem nem található
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error adding tables to restaurant: {ex.Message}");
                return false; // Hiba esetén false
            }
        }
    }
}
