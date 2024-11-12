using Firebase.Database.Query;
using Newtonsoft.Json;
using QuickReserve.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace QuickReserve.Services
{
    internal class ResaurantService
    {
        public async Task<bool> addRestaurant(Restaurant restaurant)
        {
            try
            {
                // Az int típusú RestaurantId-t stringgé konvertáljuk a kulcshoz
                string restaurantKey = restaurant.RestaurantId.ToString();

                // Felhasználó adatainak mentése a "Restaurant" gyűjteménybe
                await FirebaseService
                    .Client
                    .Child("Restaurant")
                    .Child(restaurantKey)  // Az int típusú RestaurantId stringgé konvertálva
                    .PutAsync(JsonConvert.SerializeObject(restaurant));

                return true; // Sikeres hozzáadás esetén true értéket adunk vissza
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error adding restaurant: {ex.Message}");
                return false; // Hiba esetén false értéket adunk vissza
            }
        }

        public async Task<Restaurant> GetRestaurantById(int restaurantId)
        {
            try
            {
                // Az int típusú RestaurantId-t stringgé konvertáljuk a kulcshoz
                string restaurantKey = restaurantId.ToString();

                // Lekérjük az étterem adatokat az "Restaurant" gyűjteményből az adott RestaurantId alapján
                var restaurantData = await FirebaseService
                    .Client
                    .Child("Restaurant") // Az éttermek gyűjteménye
                    .Child(restaurantKey)  // Az int típusú RestaurantId stringgé konvertálva
                    .OnceSingleAsync<Restaurant>(); // Az étterem adatainak lekérése

                return restaurantData; // Az étterem adatainak visszaadása
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching restaurant: {ex.Message}");
                return null; // Hiba esetén null-t adunk vissza
            }
        }
    }
}
