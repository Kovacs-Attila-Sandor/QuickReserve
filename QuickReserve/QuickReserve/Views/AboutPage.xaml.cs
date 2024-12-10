using System;
using Xamarin.Forms;
using QuickReserve.Services;
using QuickReserve.Models;
using System.Collections.Generic;
using System.IO;
using QuickReserve.Converter;
using System.Windows.Input;

namespace QuickReserve.Views
{
    public partial class AboutPage : ContentPage
    {
        public AboutPage()
        {
            InitializeComponent();
            BindingContext = this; // Kötelező ahhoz, hogy a parancs működjön.
            DisplayRestaurants();
        }

        // Method to navigate to the profile page
        protected void GoToProfilePage(object sender, EventArgs e)
        {
            App.Current.MainPage = new NavigationPage(new ProfilePage());
        }

        // Fetch restaurants from Firebase and display them in ListView
        public async void DisplayRestaurants()
        {
            var restaurantService = new RestaurantService();
            var allRestaurants = await restaurantService.GetAllRestaurants();

            if (allRestaurants != null)
            {
                foreach (var restaurant in allRestaurants)
                {
                    if (!string.IsNullOrEmpty(restaurant.FirstImageBase64))
                    {
                        // Az első kép dekódolása és beállítása
                        restaurant.ImageSourceUri = ImageConverter.ConvertBase64ToImageSource(restaurant.FirstImageBase64);
                    }
                }

                lstmoments.ItemsSource = allRestaurants;
            }
            else
            {
                Console.WriteLine("No restaurants found.");
            }
        }


        // Command to handle tapping on a restaurant
        public ICommand RestaurantTappedCommand => new Command<Restaurant>(OnRestaurantTapped);

        private async void OnRestaurantTapped(Restaurant selectedRestaurant)
        {
            if (selectedRestaurant != null)
            {
                await Navigation.PushAsync(new RestaurantDetailsPage(selectedRestaurant));
            }
        }
    }
}
