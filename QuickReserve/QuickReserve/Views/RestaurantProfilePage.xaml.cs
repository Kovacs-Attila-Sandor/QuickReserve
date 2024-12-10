using System;
using Xamarin.Forms;
using QuickReserve.Models;
using QuickReserve.Services;

namespace QuickReserve.Views
{
    public partial class RestaurantProfilePage : ContentPage
    {
        public RestaurantProfilePage(string userName)
        {
            InitializeComponent();

            // Aszinkron módon töltjük be az étterem adatokat
            LoadRestaurantData(userName);
        }

        private async void LoadRestaurantData(string userName)
        {
            try
            {
                // Étterem adatok aszinkron lekérése
                var restaurantService = new RestaurantService();
                var restaurant = await restaurantService.GetRestaurantByName(userName);

                // Ha sikerült éttermet találni, beállítjuk a binding contextet
                if (restaurant != null)
                {
                    BindingContext = restaurant;
                }
                else
                {
                    // Ha nem találunk éttermet, figyelmeztethetjük a felhasználót
                    await DisplayAlert("Hiba", "Nem található étterem a megadott névvel.", "OK");
                }
            }
            catch (Exception ex)
            {
                // Hibakezelés
                await DisplayAlert("Hiba", $"Hiba történt az étterem betöltésekor: {ex.Message}", "OK");
            }
        }

        private async void OnBackClicked(object sender, EventArgs e)
        {
            await Navigation.PopAsync();
        }
    }
}
