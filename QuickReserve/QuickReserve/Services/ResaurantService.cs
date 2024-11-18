using Firebase.Database;
using Firebase.Database.Query;
using Newtonsoft.Json;
using QuickReserve.Models;
using System;
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

        // Az ID generálása, amely az utolsó használt ID + 1
        private async Task<int> GenerateNextId()
        {
            var firebaseClient = FirebaseService.Client;

            try
            {
                // Próbáljuk lekérdezni a 'lastUsedId' értékét a Firebase-ből
                var lastUsedId = await firebaseClient
                    .Child("Restaurant")
                    .Child("lastUsedId")
                    .OnceSingleAsync<int>();

                // Az új ID a legutolsó ID + 1
                int newId = lastUsedId + 1;

                // Elmentjük az új ID-t a Firebase-be
                await firebaseClient
                    .Child("Restaurant")
                    .Child("lastUsedId")
                    .PutAsync(newId);

                return newId;  // Az új generált ID-t visszaadjuk
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error generating next ID: {ex.Message}");

                // Ha nem található a lastUsedId kulcs, vagy más hiba történik, akkor kezdjük 1-től
                // Érdemes lehet a Firebase konzolban manuálisan beállítani az első 'lastUsedId' értéket is
                await firebaseClient
                    .Child("Restaurant")
                    .Child("lastUsedId")
                    .PutAsync(1); // Kezdjük az ID-t 1-től

                return 1;  // Ha hiba történik, akkor az első ID-t (1) használjuk
            }
        }
    }
}
