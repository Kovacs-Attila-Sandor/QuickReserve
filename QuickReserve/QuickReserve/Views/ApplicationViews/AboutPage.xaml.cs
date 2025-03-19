using System;
using System.Collections.Generic;
using System.Linq;
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
        private List<Restaurant> originalItems; // Az összes étterem listája

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

        // Éttermek betöltése Firebase-ból és megjelenítése ListView-ban
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

                // Az eredeti lista frissítése
                originalItems = allRestaurants;

                // A ListView adatainak beállítása
                lstmoments.ItemsSource = originalItems;

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

        // Keresési szöveg változásakor futó eseménykezelő
        private void OnSearchTextChanged(object sender, TextChangedEventArgs e)
        {
            var searchText = e.NewTextValue;
            if (string.IsNullOrWhiteSpace(searchText))
            {
                // Ha nincs keresési szöveg, visszaállítjuk az eredeti listát
                lstmoments.ItemsSource = originalItems;
            }
            else
            {
                // Szűrés a keresési szöveg alapján (kis- és nagybetűk figyelmen kívül hagyása)
                lstmoments.ItemsSource = originalItems
                    .Where(item => item.Name.IndexOf(searchText, StringComparison.OrdinalIgnoreCase) >= 0)
                    .ToList();
            }
        }
    }
}