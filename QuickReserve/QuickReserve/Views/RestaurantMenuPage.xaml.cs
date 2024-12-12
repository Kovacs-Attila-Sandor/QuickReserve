using Xamarin.Forms;
using QuickReserve.Models;
using System.Collections.Generic;
using System.Linq;
using System;
using QuickReserve.Converter;

namespace QuickReserve.Views
{
    public partial class RestaurantMenuPage : ContentPage
    {
        public Restaurant CurrentRestaurant { get; set; }
        public List<Food> MenuItems { get; set; }
        public List<Food> OrderItems { get; set; } = new List<Food>(); // Rendeléshez hozzáadott ételek

        public RestaurantMenuPage(Restaurant restaurant)
        {
            InitializeComponent();
            CurrentRestaurant = restaurant;
            MenuItems = restaurant.Foods;
            BindingContext = this;

            // Étterem képének beállítása, ha van
            if (!string.IsNullOrEmpty(restaurant.ImageSourceUri?.ToString()))
            {
                RestaurantImage.Source = restaurant.ImageSourceUri;
            }

            // Étkezések képeinek beállítása
            if (restaurant.Foods != null)
            {
                foreach (var food in restaurant.Foods)
                {
                    if (!string.IsNullOrEmpty(food.Picture))
                    {
                        food.ImageSource = ImageConverter.ConvertBase64ToImageSource(food.Picture); // Base64 átalakítása ImageSource-ra
                    }
                }
            }

            // Az étkezések betöltése
            MenuListView.ItemsSource = MenuItems;
        }

        // Kategória szerinti szűrés
        private void OnCategorySelected(object sender, EventArgs e)
        {
            var button = sender as Button;
            string category = button?.Text;

            if (string.IsNullOrEmpty(category))
                return;

            // Szűrés a kategória alapján
            if (category == "Main Courses")
            {
                MenuItems = CurrentRestaurant.Foods.Where(f => f.Category == "Main Course").ToList();
            }
            else if (category == "Drinks")
            {
                MenuItems = CurrentRestaurant.Foods.Where(f => f.Category == "Drink").ToList();
            }
            else if (category == "Desserts")
            {
                MenuItems = CurrentRestaurant.Foods.Where(f => f.Category == "Dessert").ToList();
            }
            else
            {
                MenuItems = CurrentRestaurant.Foods;  // Ha nincs szűrési kategória, akkor mindent megjelenítünk
            }

            // Frissítjük a ListView-t
            MenuListView.ItemsSource = MenuItems;
        }

        // Étkezés hozzáadása a rendeléshez
        private void OnAddToOrderClicked(object sender, EventArgs e)
        {
            var button = sender as Button;
            var food = button?.BindingContext as Food;
            if (food != null)
            {
                OrderItems.Add(food); // Étkezés hozzáadása
                DisplayAlert("Added to Order", $"{food.Name} has been added to your order.", "OK");
            }
        }

        // A rendelés megtekintése
        private async void OnViewOrderClicked(object sender, EventArgs e)
        {
            if (OrderItems.Count == 0)
            {
                await DisplayAlert("No items", "You haven't added any items to the order yet.", "OK");
            }
            else
            {
                // Megjeleníthetjük a rendelést (pl. egy új oldalon)
                await Navigation.PushAsync(new OrderPage(OrderItems));
            }
        }

        // A rendelés leadása
        private async void OnPlaceOrderClicked(object sender, EventArgs e)
        {
            if (OrderItems.Count == 0)
            {
                await DisplayAlert("No items", "You need to add items to the order before placing it.", "OK");
                return;
            }

            // Itt a rendelést leadhatjuk, például egy API hívás vagy egy adatbázis írása
            await DisplayAlert("Order Placed", "Your order has been placed successfully!", "OK");

            // Üresíthetjük a rendelési listát a rendelés után
            OrderItems.Clear();
        }

        private async void OnItemTapped(object sender, ItemTappedEventArgs e)
        {
            if (e.Item != null)
            {
                var food = e.Item as Food;
                if (food != null)
                {
                    // A teljes étkezés leírásának megjelenítése egy felugró ablakban
                    await DisplayAlert(food.Name, $"{food.Description}\nPrice: {food.Price} €", "OK");
                }

                // Az elem kiválasztásának törlése
                ((ListView)sender).SelectedItem = null;
            }
        }
    }
}