using Xamarin.Forms;
using QuickReserve.Models;
using System.Collections.Generic;
using System.Linq;
using System;
using QuickReserve.Converter;
using System.Windows.Input;

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

        // BindableProperty for IsButtonVisible
        public static readonly BindableProperty IsButtonVisibleProperty =
         BindableProperty.Create(
             nameof(IsButtonVisible),
             typeof(bool),
             typeof(RestaurantMenuPage),
             default(bool));

        public bool IsButtonVisible
        {
            get => (bool)GetValue(IsButtonVisibleProperty);
            set => SetValue(IsButtonVisibleProperty, value);
        }

        public RestaurantMenuPage(Restaurant restaurant)
        {
            InitializeComponent();

            CurrentRestaurant = restaurant;
            MenuItems = restaurant.Foods;

            BindingContext = this;  // Itt van az összekötés a BindingContext-tel

            if (!string.IsNullOrEmpty(restaurant.ImageSourceUri?.ToString()))
            {
                RestaurantImage.Source = restaurant.ImageSourceUri;
            }

            if (restaurant.Foods != null)
            {
                foreach (var food in restaurant.Foods)
                {
                    if (!string.IsNullOrEmpty(food.Picture))
                    {
                        food.ImageSource = ImageConverter.ConvertBase64ToImageSource(food.Picture);
                    }
                }
            }

            MenuListView.ItemsSource = MenuItems;
        }


        private async void OnNoPreOrderClicked(object sender, EventArgs e)
        {
            //await DisplayAlert("asd", $"{ReservationDateTime}, {TableId}, {GuestCount}", "OK");
            await Navigation.PushAsync(new ReservationSummaryPage(ReservationDateTime, TableId, GuestCount));
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            // Biztosítjuk, hogy a gombok láthatósága megfelelő
            if (!string.IsNullOrEmpty(TableId))
            {
                foreach (var food in MenuItems)
                {
                    food.IsButtonVisible = true;  // Set the button visible for each food item
                }
                IsButtonVisible = true;
            }
            else
            {
                foreach (var food in MenuItems)
                {
                    food.IsButtonVisible = false;  // Hide buttons if no table is selected
                }
                IsButtonVisible = false;
            }

            // Frissítjük az ItemsSource-t, hogy az UI észlelje a változást
            MenuListView.ItemsSource = null;  // Először ürítjük az ItemsSource-t
            MenuListView.ItemsSource = MenuItems;  // Újra hozzárendeljük a listát

            BindingContext = this;
        }



        private void OnCategorySelected(object sender, EventArgs e)
        {
            var button = sender as Button;
            string category = button?.Text;

            if (string.IsNullOrEmpty(category))
                return;

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
                MenuItems = CurrentRestaurant.Foods;
            }

            MenuListView.ItemsSource = MenuItems;
        }

        private void OnAddToOrderClicked(object sender, EventArgs e)
        {
            var button = sender as Button;
            var food = button?.BindingContext as Food;
            if (food != null)
            {
                OrderItems.Add(food);
                DisplayAlert("Added to Order", $"{food.Name} has been added to your order.", "OK");
            }
        }

        private async void OnViewOrderClicked(object sender, EventArgs e)
        {
            if (OrderItems.Count == 0)
            {
                await DisplayAlert("No items", "You haven't added any items to the order yet.", "OK");
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
                    await DisplayAlert(food.Name, $"{food.Description}\nPrice: {food.Price} €", "OK");
                }

                ((ListView)sender).SelectedItem = null;
            }
        }

     

    }
}
