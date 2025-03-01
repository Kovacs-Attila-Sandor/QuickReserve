using Firebase.Database;
using Firebase.Database.Query;
using Newtonsoft.Json;
using QuickReserve.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QuickReserve.Services
{
    public class RestaurantService
    {
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

        public async Task<Restaurant> GetRestaurantByName(string name)
        {
            try
            {
                // Lekérjük az összes éttermet a "Restaurant" gyűjteményből
                var allRestaurants = await FirebaseService
                    .Client
                    .Child("Restaurant")  // "Restaurant" gyűjtemény
                    .OnceAsync<Restaurant>();  // Az összes étterem lekérése

                // Megkeressük az első olyan éttermet, amelynek Name tulajdonsága megegyezik a keresett névvel
                var restaurant = allRestaurants
                    .Select(u => u.Object)  // Csak az étterem objektumokat vesszük figyelembe
                    .FirstOrDefault(u => u.Name == name);  // Feltétel a Name-re

                return restaurant;  // Ha találtunk egyezést, visszaadjuk az éttermet
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching restaurant by name: {ex.Message}");
                return null;  // Hiba esetén null-t adunk vissza
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
                // Lekérjük az étterem adatokat az adott RestaurantId alapján
                var restaurant = await GetRestaurantById(restaurantId);
                if (restaurant != null)
                {
                    // Megkeressük a módosítandó ételt az étterem Foods listájában
                    var foodToUpdate = restaurant.Foods.FirstOrDefault(f => f.FoodId == foodId);
                    if (foodToUpdate != null)
                    {
                        // Frissítjük az étel adatait
                        foodToUpdate.Name = updatedFood.Name;
                        foodToUpdate.Price = updatedFood.Price;
                        foodToUpdate.Description = updatedFood.Description;
                        foodToUpdate.Picture = updatedFood.Picture; // Ha van kép is, azt is frissítjük

                        // Frissítjük az étterem adatait a Firebase adatbázisban
                        await FirebaseService
                            .Client
                            .Child("Restaurant")
                            .Child(restaurantId)
                            .PutAsync(JsonConvert.SerializeObject(restaurant));

                        return true; // Sikeres frissítés
                    }
                    return false; // Ha nem találjuk az ételt
                }
                return false; // Ha az étterem nem található
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating food details: {ex.Message}");
                return false; // Hiba esetén false
            }
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


    }
}