using QuickReserve.Models;
using QuickReserve.Services;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace QuickReserve.Views
{
    public partial class RestaurantEditHoursPage : ContentPage
    {
        public Restaurant _restaurant { get; set; }

        RestaurantService restaurantService;
        public RestaurantEditHoursPage(Restaurant restaurant)
        {
            InitializeComponent();
            restaurantService = new RestaurantService();
            _restaurant = restaurant; // Az aktuális étterem
            BindingContext = _restaurant; // BindingContext beállítása a helyes modellre
        }

        private async void OnSaveButtonClicked(object sender, EventArgs e)
        {
            try
            {
                // Attempt to save the modified hours
                await restaurantService.SaveHours(_restaurant.RestaurantId, _restaurant.Hours);

                // Display success message
                await DisplayAlert("Success", "The opening hours were successfully saved.", "OK");
                await Navigation.PopAsync();
            }
            catch (Exception ex)
            {
                // Display error message if something goes wrong
                await DisplayAlert("Error", "An error occurred while saving!", "OK");
                Console.WriteLine($"Error: An error occurred while saving: {ex.Message}");
            }
        }
    }
}
