using Firebase.Database.Query;
using Newtonsoft.Json;
using QuickReserve.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace QuickReserve.Services
{
    public class RestaurantService
    {
        private static RestaurantService _instance;

        private RestaurantService() { }

        public static RestaurantService Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new RestaurantService();
                }
                return _instance;
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

        public async Task<Restaurant> GetRestaurantByUserId(string userId)
        {
            try
            {
                // Lekérjük az étterem adatokat a "Restaurant" gyűjteményből a megadott RestaurantId alapján
                var allRestaurant = await FirebaseService
                    .Client
                    .Child("Restaurant")  // "Restaurant" gyűjtemény                  
                    .OnceAsync<Restaurant>();  // Az adatokat Restaurant típusra deszerializáljuk

                var restaurant = allRestaurant
                    .Select(u => u.Object)
                    .FirstOrDefault(u => u.UserId == userId);

                return restaurant;  // Az étterem adatainak visszaadása
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching restaurant: {ex.Message}");
                return null;  // Hiba esetén null-t adunk vissza
            }
        }

        public async Task<string> GetRestaurantIdByUserId(string userId)
        {
            try
            {
                // Lekérjük az étterem adatokat a "Restaurant" gyűjteményből a megadott RestaurantId alapján
                var allRestaurant = await FirebaseService
                    .Client
                    .Child("Restaurant")  // "Restaurant" gyűjtemény                  
                    .OnceAsync<Restaurant>();  // Az adatokat Restaurant típusra deszerializáljuk

                var restaurant = allRestaurant
                    .Select(u => u.Object)
                    .FirstOrDefault(u => u.UserId == userId);

                return restaurant.RestaurantId;  // Az étterem adatainak visszaadása
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching restaurant: {ex.Message}");
                return null;  // Hiba esetén null-t adunk vissza
            }
        }

        public async Task<List<Restaurant>> GetAllRestaurants()
        {
            try
            {
                // Fetch all the restaurants from the "Restaurant" collection
                var restaurantData = await FirebaseService
                    .Client
                    .Child("Restaurant")  // "Restaurant" collection
                    .OnceAsync<Restaurant>();  // Fetch all restaurants as a list of Restaurant objects

                // Convert the fetched data into a List<Restaurant>
                List<Restaurant> restaurants = new List<Restaurant>();
                foreach (var item in restaurantData)
                {
                    var restaurant = item.Object;
                    restaurants.Add(restaurant);
                }

                return restaurants;  // Return the list of all restaurants
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching all restaurants: {ex.Message}");
                return null;  // Return null in case of an error
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

        public async Task<bool> DeleteFoodFromRestaurant(string restaurantId, string foodId)
        {
            try
            {
                var restaurant = await GetRestaurantById(restaurantId);
                if (restaurant != null)
                {
                    var foodToDelete = restaurant.Foods.FirstOrDefault(f => f.FoodId == foodId);
                    if (foodToDelete != null)
                    {
                        restaurant.Foods.Remove(foodToDelete); // Remove the food from the list

                        // Update the restaurant data in Firebase
                        await FirebaseService
                            .Client
                            .Child("Restaurant")
                            .Child(restaurantId)
                            .PutAsync(JsonConvert.SerializeObject(restaurant));

                        return true; // Successfully deleted the food
                    }
                }
                return false; // Food not found
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting food: {ex.Message}");
                return false;
            }
        }

        public async Task<Food> GetFoodById(string foodId)
        {
            try
            {
                // Lekérjük az étterem adatait az adott RestaurantId alapján
                var restaurantData = await FirebaseService
                    .Client
                    .Child("Restaurant")  // "Restaurant" gyűjtemény
                    .OnceAsync<Restaurant>();  // Lekérjük az összes étterem adatot

                // Keresés a megfelelő étteremhez tartozó étel alapján
                foreach (var item in restaurantData)
                {
                    var restaurant = item.Object;
                    var food = restaurant.Foods.FirstOrDefault(f => f.FoodId == foodId);  // Az ételt keresem az étterem ételei között

                    if (food != null)
                    {
                        return food;  // Ha megtaláltuk az ételt, visszaadjuk
                    }
                }

                return null;  // Ha nem találunk ételt a megadott FoodId alapján
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching food by ID: {ex.Message}");
                return null;  // Hiba esetén null-t adunk vissza
            }
        }

        public async Task<bool> UpdateFoodDetails(string restaurantId, string foodId, Food updatedFood)
        {
            try
            {
                // Validáció
                if (string.IsNullOrEmpty(restaurantId) || string.IsNullOrEmpty(foodId) || updatedFood == null)
                {
                    Console.WriteLine("Invalid input parameters for UpdateFoodDetails.");
                    return false;
                }

                // Lekérjük az étterem adatokat
                var restaurant = await GetRestaurantById(restaurantId);
                if (restaurant == null || restaurant.Foods == null)
                {
                    Console.WriteLine($"Restaurant with ID {restaurantId} not found or has no foods.");
                    return false;
                }

                // Megkeressük a módosítandó ételt
                var foodToUpdate = restaurant.Foods.FirstOrDefault(f => f.FoodId == foodId);
                if (foodToUpdate == null)
                {
                    Console.WriteLine($"Food with ID {foodId} not found in restaurant {restaurantId}.");
                    return false;
                }

                // Frissítjük az étel adatait (csak a szükséges mezőket)
                UpdateFoodProperties(foodToUpdate, updatedFood);

                // Frissítjük az étterem adatait a Firebase-ban
                await FirebaseService
                    .Client
                    .Child("Restaurant")
                    .Child(restaurantId)
                    .PutAsync(JsonConvert.SerializeObject(restaurant));

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating food details: {ex.Message}");
                return false;
            }
        }

        // Segédfüggvény az étel tulajdonságainak frissítésére
        private void UpdateFoodProperties(Food target, Food source)
        {
            target.Name = source.Name ?? target.Name;
            target.Description = source.Description ?? target.Description;
            target.Price = source.Price != 0 ? source.Price : target.Price;
            target.Category = source.Category ?? target.Category;
            target.Picture = source.Picture ?? target.Picture;
            target.IsAvailable = source.IsAvailable; // Ez feltételezi, hogy false az alapértelmezett
            target.PreparationTime = source.PreparationTime != 0 ? source.PreparationTime : target.PreparationTime;
            target.Ingredients = source.Ingredients ?? target.Ingredients;
            target.Allergens = source.Allergens ?? target.Allergens;
            target.NutritionalInfo = source.NutritionalInfo ?? target.NutritionalInfo;
            target.Tags = source.Tags ?? target.Tags;
            target.Ratings = source.Ratings ?? target.Ratings;
        }

        public async Task<Table> GetTableById(string restaurantId, string tableId)
        {
            try
            {
                // Lekérjük az étterem adatokat a Firebase adatbázisból a megadott RestaurantId alapján
                var restaurant = await GetRestaurantById(restaurantId);
                if (restaurant != null)
                {
                    // Keresés az étterem asztalai között a megadott TableId alapján
                    var table = restaurant.Tables?.FirstOrDefault(t => t.TableId == tableId);
                    return table; // Ha megtaláltuk, visszaadjuk az asztalt
                }
                return null; // Ha az étterem nem található
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching table by ID: {ex.Message}");
                return null; // Hiba esetén null-t adunk vissza
            }
        }

        public async Task<bool> MarkTableAsReserved(string restaurantId, string tableId)
        {
            try
            {
                // Lekérjük az adott éttermet
                var restaurant = await GetRestaurantById(restaurantId);
                if (restaurant != null)
                {
                    // Megkeressük az asztalt a Tables listában
                    var table = restaurant.Tables?.FirstOrDefault(t => t.TableId == tableId);
                    if (table != null)
                    {
                        // Állapot frissítése "Reserved"-re
                        table.AvailabilityStatus = "Reserved";

                        // Frissítjük az étterem adatait a Firebase adatbázisban
                        await FirebaseService
                            .Client
                            .Child("Restaurant")
                            .Child(restaurantId)
                            .PutAsync(JsonConvert.SerializeObject(restaurant));

                        return true; // Sikeres frissítés
                    }

                    Console.WriteLine($"Table with ID {tableId} not found.");
                    return false; // Ha az asztal nem található
                }

                Console.WriteLine($"Restaurant with ID {restaurantId} not found.");
                return false; // Ha az étterem nem található
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error marking table as reserved: {ex.Message}");
                return false; // Hiba esetén false
            }
        }

        public async Task<bool> MarkTableAsAvailable(string restaurantId, string tableId)
        {
            try
            {
                Console.WriteLine($"ResId: {restaurantId}, TableID: {tableId}");
                // Lekérjük az adott éttermet
                var restaurant = await GetRestaurantById(restaurantId);
                if (restaurant != null)
                {
                    // Megkeressük az asztalt a Tables listában
                    var table = restaurant.Tables?.FirstOrDefault(t => t.TableId == tableId);
                    if (table != null)
                    {
                        // Állapot frissítése "Available"-re
                        table.AvailabilityStatus = "Available";

                        // Frissítjük az étterem adatait a Firebase adatbázisban
                        await FirebaseService
                            .Client
                            .Child("Restaurant")
                            .Child(restaurantId)
                            .PutAsync(JsonConvert.SerializeObject(restaurant));

                        return true; // Sikeres frissítés
                    }

                    Console.WriteLine($"Table with ID {tableId} not found.");
                    return false; // Ha az asztal nem található
                }

                Console.WriteLine($"Restaurant with ID {restaurantId} not found.");
                return false; // Ha az étterem nem található
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error marking table as active: {ex.Message}");
                return false; // Hiba esetén false
            }
        }

        public async Task<bool> SaveHours(string restaurantId, List<RestaurantHours> hours)
        {
            try
            {
                // Fetch the restaurant by its ID
                var restaurant = await GetRestaurantById(restaurantId);
                if (restaurant != null)
                {
                    // Update the restaurant's operating hours
                    restaurant.Hours = hours;

                    // Save the updated restaurant data to Firebase
                    await FirebaseService
                        .Client
                        .Child("Restaurant")
                        .Child(restaurantId)
                        .PutAsync(JsonConvert.SerializeObject(restaurant));

                    return true; // Successfully updated the hours
                }

                Console.WriteLine($"Restaurant with ID {restaurantId} not found.");
                return false; // Restaurant not found
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving hours: {ex.Message}");
                return false; // Return false in case of an error
            }
        }

        public async Task<bool> UpdateRestaurantAsync(Restaurant updatedRestaurant)
        {
            try
            {
                // Az étterem adatainak frissítése a Firebase adatbázisban
                await FirebaseService
                    .Client
                    .Child("Restaurant")
                    .Child(updatedRestaurant.RestaurantId)
                    .PutAsync(JsonConvert.SerializeObject(updatedRestaurant));

                return true; // Sikeres frissítés
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating restaurant: {ex.Message}");
                return false; // Hiba esetén false
            }
        }

        public async Task<bool> AddCategoryToRestaurant(string restaurantId, List<string> newCategories)
        {
            try
            {
                // Az étterem lekérése a Firebase-ból
                var restaurantData = await FirebaseService
                    .Client
                    .Child("Restaurant")  // "Restaurant" gyűjtemény
                    .Child(restaurantId)  // Az étterem egyedi azonosítója
                    .OnceSingleAsync<Restaurant>();  // Az étterem adatainak lekérése

                if (restaurantData != null)
                {
                    // Ha a kategóriák listája null, inicializáljuk
                    if (restaurantData.Categories == null)
                    {
                        restaurantData.Categories = new List<string>();
                    }

                    // Új kategóriák hozzáadása (csak ha még nem léteznek)
                    bool categoryAdded = false;
                    foreach (var category in newCategories)
                    {
                        if (!restaurantData.Categories.Contains(category))
                        {
                            restaurantData.Categories.Add(category);
                            categoryAdded = true;
                        }
                    }

                    // Csak akkor frissítjük az adatbázist, ha új kategória került hozzáadásra
                    if (categoryAdded)
                    {
                        await FirebaseService
                            .Client
                            .Child("Restaurant")
                            .Child(restaurantId)
                            .PutAsync(restaurantData);  // Az étterem adatainak frissítése

                        return true;  // Sikeres frissítés
                    }
                    else
                    {
                        Console.WriteLine("No new categories were added.");
                        return false;  // Nem volt új kategória
                    }
                }
                else
                {
                    Console.WriteLine("Restaurant not found.");
                    return false;  // Az étterem nem található
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error adding categories to restaurant: {ex.Message}");
                return false;  // Hiba esetén false értékkel térünk vissza
            }
        }

        public IDisposable ListenForRestaurantChanges(string restaurantId, Action<Restaurant> onRestaurantUpdated)
        {
            var firebaseClient = FirebaseService.Client;
            Debug.WriteLine($"Restaurant ID: {restaurantId}");

            var subscription = firebaseClient
                .Child("Restaurant")
                .Child(restaurantId)
                .AsObservable<Restaurant>()
                .Subscribe(d =>
                {
                    Debug.WriteLine($"🔥 Firebase adat érkezett: {JsonConvert.SerializeObject(d.Object)}");

                    var restaurant = JsonConvert.DeserializeObject<Restaurant>(JsonConvert.SerializeObject(d.Object));
                    if (restaurant != null)
                    {
                        Device.BeginInvokeOnMainThread(() =>
                        {
                            onRestaurantUpdated?.Invoke(restaurant);
                        });
                    }
                });
            return subscription;
        }
    }
}