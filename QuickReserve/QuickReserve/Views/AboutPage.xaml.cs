using System;
using Xamarin.Forms;
using QuickReserve.Services;
using QuickReserve.Models;
using System.Collections.Generic;
using System.IO;
using QuickReserve.Converter;
using System.Windows.Input;
using System.Threading.Tasks;

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
            BindingContext = this;
            DisplayRestaurants();
            _userService = new UserService();
            _restaurantService = new RestaurantService();

            InitializeRestaurant();
        }

        private async void InitializeRestaurant()
        {
            string restaurnatName = App.Current.Properties["LoggedInUserName"].ToString();
            _restaurant = await _restaurantService.GetRestaurantByName(restaurnatName);
            lstmoments.IsVisible = true;
            Buttons.IsVisible = true;
            loadingIndicator.IsVisible = false;
            LoadingLabel.IsVisible = false;
        }

        // Method to navigate to the profile page
        protected async void GoToProfilePage(object sender, EventArgs e)
        {
            string loggedInUserName = App.Current.Properties["LoggedInUserName"].ToString();
            var userService = new UserService();
            var userType = await userService.GetUserType(loggedInUserName);

            if (userType == "RESTAURANT")
            {
                // Ha Restaurant típusú felhasználó, navigálunk a RestaurantProfilePage-re
                await Navigation.PushAsync(new RestaurantProfilePage(loggedInUserName));
            }
            else
            {
                // Ha nem Restaurant típusú felhasználó, navigálunk a UserProfilePage-re
                await Navigation.PushAsync(new UserProfilePage(loggedInUserName));
            }
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