using System;
using Xamarin.Forms;
using QuickReserve.Services;
using QuickReserve.Models;
using System.Collections.Generic;
using System.IO;
using QuickReserve.Converter;

namespace QuickReserve.Views
{
    public partial class AboutPage : ContentPage
    {
        public AboutPage()
        {
            InitializeComponent();
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
                    // Ha van Base64 kép, akkor dekódoljuk és beállítjuk az ImageSourceUri-t
                    if (!string.IsNullOrEmpty(restaurant.ImageBase64))
                    {
                        // Directly call the static method from ImageConverter
                        restaurant.ImageSourceUri = ImageConverter.ConvertBase64ToImageSource(restaurant.ImageBase64);
                    }
                }

                lstmoments.ItemsSource = allRestaurants;  // ListView ItemsSource beállítása
            }
            else
            {
                // Hibaüzenet, ha nincs étterem
                Console.WriteLine("No restaurants found.");
            }
        }


        // Handle the item selection event to navigate to restaurant details
        private async void OnRestaurantSelected(object sender, SelectedItemChangedEventArgs e)
        {
            if (e.SelectedItem != null)
            {
                var selectedRestaurant = e.SelectedItem as Restaurant;
                if (selectedRestaurant != null)
                {
                    await Navigation.PushAsync(new RestaurantDetailsPage(selectedRestaurant));
                }
            }
        }       
    }
}
