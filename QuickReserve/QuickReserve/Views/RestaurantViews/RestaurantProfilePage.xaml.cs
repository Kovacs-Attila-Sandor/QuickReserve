using System;
using System.Linq;
using Xamarin.Forms;
using QuickReserve.Models;
using QuickReserve.Services;
using QuickReserve.Converter;
using System.Threading.Tasks;
using QuickReserve.Views.RestaurantViews;
using Xamarin.Essentials;

namespace QuickReserve.Views
{
    public partial class RestaurantProfilePage : ContentPage
    {
        private RestaurantService _restaurantService;
        private Restaurant _restaurant;
        private string _userId;
        private readonly UserService _userService;

        public RestaurantProfilePage()
        {
            InitializeComponent();
            _restaurantService = RestaurantService.Instance;
            _userService = UserService.Instance;
            _userId = App.Current.Properties["userId"].ToString();
            App.Current.Properties["ReloadRestaurantProfilePage"] = "no";
            LoadRestaurantData();
        }

        private async void LoadRestaurantData()
        {
            try
            {
                // Show loading indicator
                loadingIndicator.IsVisible = true;
                contentLayout.IsVisible = false;

                // Fetch restaurant data asynchronously
                _restaurant = await _restaurantService.GetRestaurantByUserId(_userId);

                if (_restaurant != null)
                {
                    // Convert restaurant images
                    if (_restaurant.ImageBase64List != null && _restaurant.ImageBase64List.Any())
                    {
                        _restaurant.ImageSourceUri = ImageConverter.ConvertBase64ToImageSource(_restaurant.ImageBase64List[0]);
                    }

                    // Convert food images
                    foreach (var food in _restaurant.Foods)
                    {
                        if (!string.IsNullOrEmpty(food.Picture))
                        {
                            food.ImageSource = ImageConverter.ConvertBase64ToImageSource(food.Picture);
                        }
                    }

                    // Bind data to the page
                    BindingContext = _restaurant;  // Use the private _restaurant object
                }
                else
                {
                    await DisplayAlert("Error", "No restaurant found with the given Id.", "OK");
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"An error occurred while loading the restaurant: {ex.Message}", "OK");
            }
            finally
            {
                // Hide loading indicator and show content
                loadingIndicator.IsVisible = false;
                contentLayout.IsVisible = true;
            }
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            // Re-fetch the restaurant data to ensure it's up-to-date
            if (_restaurant != null)
            {
                if (App.Current.Properties["ReloadRestaurantProfilePage"].ToString() == "yes")
                {
                    LoadRestaurantData();  // Reload restaurant data
                    App.Current.Properties["ReloadRestaurantProfilePage"] = "no";
                }
            } 
        }     
         
        private async void OnHoursButtonClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new RestaurantEditHoursPage(_restaurant));
        }

        private async void OnMenuButtonClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new RestaurantViewMenuPage(_restaurant));
        }

        private void OnLogoutClicked(object sender, EventArgs e)
        {
            Preferences.Clear();
            App.Current.MainPage = new LoginPage();
        }

        private async void OnUpdateRestaurantInfoClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new RestaurantEditInformationsPage(_restaurant));
        }
    }
}