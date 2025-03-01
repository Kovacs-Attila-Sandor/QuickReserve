using System;
using Xamarin.Forms;
using QuickReserve.Services;
using QuickReserve.Models;
using System.Collections.Generic;
using System.IO;
using QuickReserve.Converter;
using System.Windows.Input;
using System.Threading.Tasks;
using Firebase.Auth;

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
                Buttons.IsVisible = true;
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

        // Method to navigate to the profile page
        protected async void GoToProfilePage(object sender, EventArgs e)
        {

            string userId = App.Current.Properties["userId"].ToString();

            var userService = new UserService();
            var userType = await userService.GetUserTypeByUserId(userId);

            if (userType == "RESTAURANT")
            {
                // Ha Restaurant típusú felhasználó, navigálunk a RestaurantProfilePage-re
                await Navigation.PushAsync(new RestaurantProfilePage(userId));
            }
            else
            {
                // Ha nem Restaurant típusú felhasználó, navigálunk a UserProfilePage-re
                await Navigation.PushAsync(new UserProfilePage(userId));
            }
        }

        private async void GoToReservations(object sender, EventArgs e)
        {
            var userId = App.Current.Properties["userId"].ToString();
            User user = await _userService.GetUserById(userId);
            if (user.Role == "RESTAURANT")
            {
                await Navigation.PushAsync(new RestaurantReservationsPage(_restaurant.RestaurantId));
            }
            else
            {
                await Navigation.PushAsync(new UserReservationsPage(userId));
            }
        }
    }
}