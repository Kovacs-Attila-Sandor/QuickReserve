using Xamarin.Forms;
using QuickReserve.Models;
using System.Collections.Generic;
using System.Linq;
using System;
using QuickReserve.Converter;
using QuickReserve.Views.PopUps;
using Rg.Plugins.Popup.Services;
using QuickReserve.Views.ApplicationViews;

namespace QuickReserve.Views
{
    public partial class RestaurantMenuPage : ContentPage
    {
        public Restaurant CurrentRestaurant { get; set; }
        public List<Food> MenuItems { get; set; }
        public List<Food> OrderItems { get; set; } = new List<Food>();
        public string ReservationDateTime { get; set; }
        public string TableId { get; set; }
        public int GuestCount { get; set; }

      
        public RestaurantMenuPage(Restaurant restaurant)
        {
            InitializeComponent();

            CurrentRestaurant = restaurant;
            MenuItems = restaurant.Foods;

            // Csoportosítás
            CurrentRestaurant.GroupedFoods = restaurant.Foods
                .GroupBy(f => f.Category ?? "Uncategorized");


            if (!string.IsNullOrEmpty(restaurant.ImageSourceUri?.ToString()))
            {
                RestaurantImage.Source = restaurant.ImageSourceUri;
            }

            if (restaurant.Foods != null && restaurant.Foods.Any())
            {
                foreach (var food in restaurant.Foods)
                {
                    if (!string.IsNullOrEmpty(food.Picture))
                    {
                        food.ImageSource = ImageConverter.ConvertBase64ToImageSource(food.Picture);
                        if (food.ImageSource == null)
                        {
                            Console.WriteLine($"Failed to load image for {food.Name}");
                        }
                    }
                    else
                    {
                        food.ImageSource = ImageSource.FromFile("image_placeholder.png");
                    }
                }
            }
            else
            {
                Console.WriteLine("No food items available.");
            }
            BindingContext = CurrentRestaurant;

        }

        private async void OnNoPreOrderClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new ReservationSummaryPage(new Dictionary<Food, int>(), ReservationDateTime, TableId, GuestCount));
        }     
        private void OnCategorySelected(object sender, EventArgs e)
        {
            var button = sender as Button;
            string category = button?.Text;

            if (string.IsNullOrEmpty(category))
                return;

            if (category == "Main Courses")
            {
                CurrentRestaurant.GroupedFoods = CurrentRestaurant.Foods
                    .Where(f => f.Category == "Main Course")
                    .GroupBy(f => f.Category ?? "Uncategorized");
            }
            else if (category == "Drinks")
            {
                CurrentRestaurant.GroupedFoods = CurrentRestaurant.Foods
                    .Where(f => f.Category == "Drink")
                    .GroupBy(f => f.Category ?? "Uncategorized");
            }
            else if (category == "Desserts")
            {
                CurrentRestaurant.GroupedFoods = CurrentRestaurant.Foods
                    .Where(f => f.Category == "Dessert")
                    .GroupBy(f => f.Category ?? "Uncategorized");
            }
            else
            {
                CurrentRestaurant.GroupedFoods = CurrentRestaurant.Foods
                    .GroupBy(f => f.Category ?? "Uncategorized");
            }

            BindingContext = null;
            BindingContext = CurrentRestaurant;
        }

        private async void OnAddToOrderClicked(object sender, EventArgs e)
        {
            var button = sender as Button;
            var food = button?.BindingContext as Food;
            if (food != null)
            {
                OrderItems.Add(food);
                await PopupNavigation.Instance.PushAsync(new CustomAlert("Successful Addition", $"{food.Name} has been added to your order."));
            }
        }

        private async void OnViewOrderClicked(object sender, EventArgs e)
        {
            if (OrderItems.Count == 0)
            {
                await PopupNavigation.Instance.PushAsync(new CustomAlert("No Items", "You haven't added any items to the order yet"));
            }
            else
            {
                await Navigation.PushAsync(new OrderPage(OrderItems, ReservationDateTime, TableId, GuestCount));
            }
        }

        private async void OnPlaceOrderClicked(object sender, EventArgs e)
        {
            if (OrderItems.Count == 0)
            {
                await DisplayAlert("No items", "You need to add items to the order before placing it.", "OK");
                return;
            }

            string message = $"Order placed for Table {TableId} at {ReservationDateTime} with {OrderItems.Count} items.";
            await DisplayAlert("Order Placed", message, "OK");

            OrderItems.Clear();
        }

        private async void OnItemTapped(object sender, ItemTappedEventArgs e)
        {
            if (e.Item != null)
            {
                var food = e.Item as Food;
                if (food != null)
                {
                    await Navigation.PushAsync(new FoodPage(food, CurrentRestaurant.RestaurantId, false));
                }

                ((ListView)sender).SelectedItem = null;
            }
        }
        private async void OnBackButtonClicked(object sender, EventArgs e)
        {
            await Navigation.PopAsync();
        }
    }
}