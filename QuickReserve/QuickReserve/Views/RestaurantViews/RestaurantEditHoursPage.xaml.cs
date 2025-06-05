using QuickReserve.Models;
using QuickReserve.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace QuickReserve.Views
{
    public partial class RestaurantEditHoursPage : ContentPage
    {
        private readonly Restaurant _restaurant;
        private readonly RestaurantService _restaurantService;

        public RestaurantEditHoursPage(Restaurant restaurant)
        {
            InitializeComponent();
            _restaurantService = RestaurantService.Instance;
            _restaurant = restaurant ?? new Restaurant
            {
                Hours = new List<RestaurantHours>()
            };

            // Inicializáljuk a Hours gyűjteményt minden nappal, ha üres
            InitializeHours();
            BindingContext = _restaurant;
        }

        private void InitializeHours()
        {
            if (_restaurant.Hours == null || !_restaurant.Hours.Any())
            {
                _restaurant.Hours = new List<RestaurantHours>(); // Change ObservableCollection to List
                var days = new[] { "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday", "Sunday" };
                foreach (var day in days)
                {
                    _restaurant.Hours.Add(new RestaurantHours
                    {
                        Day = day,
                        OpenTime = new TimeSpan(9, 0, 0), // Default opening: 9:00
                        CloseTime = new TimeSpan(17, 0, 0), // Default closing: 17:00
                        IsClosed = false
                    });
                }
            }
            else
            {
                // Ensure all days are present
                var days = new[] { "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday", "Sunday" };
                foreach (var day in days)
                {
                    if (!_restaurant.Hours.Any(h => h.Day == day))
                    {
                        _restaurant.Hours.Add(new RestaurantHours
                        {
                            Day = day,
                            OpenTime = new TimeSpan(9, 0, 0),
                            CloseTime = new TimeSpan(17, 0, 0),
                            IsClosed = false
                        });
                    }
                }
                // Sort by day order
                var orderedHours = _restaurant.Hours.OrderBy(h => Array.IndexOf(days, h.Day)).ToList();
                _restaurant.Hours = orderedHours; // Assign back to List<RestaurantHours>
            }
        }

        private async void OnSaveButtonClicked(object sender, EventArgs e)
        {
            SaveButton.IsEnabled = false;
            try
            {
                // Zárva lévő napoknál null-ra állítjuk az időpontokat
                foreach (var hour in _restaurant.Hours)
                {
                    if (hour.IsClosed)
                    {
                        hour.OpenTime = null;
                        hour.CloseTime = null;
                    }
                }

                // Mentés a backend-re
                await _restaurantService.SaveHours(_restaurant.RestaurantId, _restaurant.Hours);

                // Sikeres mentés visszajelzése
                await DisplayAlert("Sikeres", "A nyitvatartási idő sikeresen mentve.", "OK");
                await Navigation.PopAsync();
            }
            catch (Exception ex)
            {
                // Hibaüzenet
                
                await DisplayAlert("Hiba", $"Hiba történt a mentés közben: {ex.Message}", "OK");
                Console.WriteLine($"Hiba a mentés közben: {ex.Message}");
            }
            finally
            {
                SaveButton.IsEnabled = true;
            }
        }
    }
}