using System;
using System.Linq;
using Xamarin.Forms;
using QuickReserve.Models;
using QuickReserve.Services;
using QuickReserve.Converter;
using System.Threading.Tasks;
using QuickReserve.Views.RestaurantViews;

namespace QuickReserve.Views
{
    public partial class RestaurantProfilePage : ContentPage
    {
        private RestaurantService _restaurantService;
        private Restaurant _restaurant;

        public RestaurantProfilePage(string restaurantId)
        {
            InitializeComponent();
            _restaurantService = new RestaurantService();
            LoadRestaurantData(restaurantId);
        }

        private async void LoadRestaurantData(string restaurantId)
        {
            try
            {
                // Show loading indicator
                loadingIndicator.IsVisible = true;
                contentLayout.IsVisible = false;

                // Fetch restaurant data asynchronously
                _restaurant = await _restaurantService.GetRestaurantByUserId(restaurantId);

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
                    await DisplayAlert("Error", "No restaurant found with the given name.", "OK");
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
                LoadRestaurantData(_restaurant.Name);  // Reload restaurant data
            }
        }

        // Delete food logic (for handling delete button click event)
        private async void OnDeleteFoodClicked(object sender, EventArgs e)
        {
            // Get the FoodId from the CommandParameter
            var button = (Button)sender;
            var foodId = button.CommandParameter.ToString();

            // Display confirmation alert
            bool isConfirmed = await DisplayAlert("Confirm Deletion", "Are you sure you want to delete this food item?", "Yes", "No");

            if (isConfirmed)
            {
                // If confirmed, proceed with the deletion
                await DeleteFood(foodId);
            }
            else
            {
                // If not confirmed, do nothing
                await DisplayAlert("Cancelled", "Food deletion was cancelled.", "OK");
            }
        }

        private async Task DeleteFood(string foodId)
        {
            try
            {
                bool result = await _restaurantService.DeleteFoodFromRestaurant(_restaurant.RestaurantId, foodId);
                if (result)
                {
                    await DisplayAlert("Success", "Food deleted successfully!", "OK");
                    // Refresh restaurant data after deletion
                    LoadRestaurantData(_restaurant.Name);  // Refresh using the updated restaurant data
                }
                else
                {
                    await DisplayAlert("Error", "Failed to delete food.", "OK");
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"An error occurred: {ex.Message}", "OK");
            }
        }


        // Edit food logic
        private async void OnEditMenuClicked(object sender, EventArgs e)
        {
            var button = (Button)sender;
            var foodId = button.CommandParameter.ToString();

            // Navigáljunk az EditFoodPage-re a FoodId paraméterrel
            await Navigation.PushAsync(new EditFoodPage(_restaurant.RestaurantId, foodId));
        }


        private async void OnEditHoursClicked(object sender, EventArgs e)
        {
            // Navigáljunk az EditFoodPage-re a FoodId paraméterrel
            await Navigation.PushAsync(new RestaurantEditHoursPage(_restaurant));
        }

        private void OnLogoutClicked(object sender, EventArgs e)
        {
            App.Current.MainPage = new LoginPage();
        }


        private async void OnUpdateRestaurantInfoClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new RestaurantEditInformationsPage(_restaurant));
        }
    }
}