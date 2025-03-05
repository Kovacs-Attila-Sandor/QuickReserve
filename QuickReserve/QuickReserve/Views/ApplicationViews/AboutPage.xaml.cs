using System;
using Xamarin.Forms;
using QuickReserve.Services;
using QuickReserve.Models;
using QuickReserve.Converter;

namespace QuickReserve.Views
{
    public partial class AboutPage : ContentPage

    {
        private UserService _userService;
        private RestaurantService _restaurantService;
        private Restaurant _restaurant;

        public AboutPage()
        {
            InitializeComponent();

            // A Content és ListView kezdetben nem láthatóak
            Content.IsVisible = false;
            lstmoments.IsVisible = false;
            loadingIndicator.IsVisible = true;
            LoadingLabel.IsVisible = true;

            _userService = new UserService();
            _restaurantService = new RestaurantService();

            //App.Current.Properties["restaurantId"] = _restaurant.RestaurantId;

            // Éttermek betöltése
            DisplayRestaurants();
        }

        // Fetch restaurants from Firebase and display them in ListView
        public async void DisplayRestaurants()
        {
            var allRestaurants = await _restaurantService.GetAllRestaurants();

            if (allRestaurants != null)
            {
                // Éttermek képeinek dekódolása
                foreach (var restaurant in allRestaurants)
                {
                    if (!string.IsNullOrEmpty(restaurant.FirstImageBase64))
                    {
                        restaurant.ImageSourceUri = ImageConverter.ConvertBase64ToImageSource(restaurant.FirstImageBase64);
                    }
                }

                // A ListView adatainak beállítása
                lstmoments.ItemsSource = allRestaurants;

                // A tartalom megjelenítése
                Content.IsVisible = true;
                lstmoments.IsVisible = true;
                loadingIndicator.IsVisible = false;
                LoadingLabel.IsVisible = false;
            }
            else
            {
                Console.WriteLine("No restaurants found.");
                // Ha nincs étterem, elrejtheted a betöltést és megjelenítheted a hibaüzenetet
                loadingIndicator.IsVisible = false;
                LoadingLabel.IsVisible = false;
                await DisplayAlert("Error", "No restaurants found.", "OK");
            }
        }

        private void OnItemTapped(object sender, ItemTappedEventArgs e)
        {
            if (e.Item is Restaurant selectedRestaurant)
            {
                // Navigálás a RestaurantDetailsPage-re
                Navigation.PushAsync(new RestaurantDetailsPage(selectedRestaurant));
            }

             // A ListView automatikus kijelölésének eltávolítása
             ((ListView)sender).SelectedItem = null;
        }

      
    }
}