using Xamarin.Forms;
using QuickReserve.Models;
using QuickReserve.Services;
using QuickReserve.Converter;
using System;
using Xamarin.Essentials;

namespace QuickReserve.Views
{
    public partial class EditFoodPage : ContentPage
    {
        private string _foodId;
        private string _restaurantId;
        private RestaurantService _restaurantService;
        private string _base64Image; // A kép Base64 formátumban

        public EditFoodPage(string restaurantId, string foodId)
        {
            InitializeComponent();
            _foodId = foodId;
            _restaurantId = restaurantId;
            _restaurantService = RestaurantService.Instance;
            LoadFoodDetails(foodId);
        }

        private async void LoadFoodDetails(string foodId)
        {
            try
            {
                // Töltőképernyő aktiválása
                loadingIndicator.IsVisible = true;
                loadingIndicator.IsRunning = true;
                mainContent.IsVisible = false;

                var food = await _restaurantService.GetFoodById(foodId);

                if (food != null)
                {
                    foodNameEntry.Text = food.Name;
                    foodPriceEntry.Text = food.Price.ToString();
                    foodDescriptionEntry.Text = food.Description;

                    if (!string.IsNullOrEmpty(food.Picture))
                    {
                        _base64Image = food.Picture; // Base64 tárolása
                        foodImage.Source = ImageConverter.ConvertBase64ToImageSource(food.Picture);
                    }
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Failed to load food details: {ex.Message}", "OK");
            }
            finally
            {
                // Töltőképernyő kikapcsolása
                loadingIndicator.IsVisible = false;
                loadingIndicator.IsRunning = false;
                mainContent.IsVisible = true;
            }
        }

        private async void OnChooseImageClicked(object sender, EventArgs e)
        {
            try
            {
                var result = await FilePicker.PickAsync(new PickOptions
                {
                    FileTypes = FilePickerFileType.Images // A Title mezőt eltávolítottam
                });

                if (result != null)
                {
                    var stream = await result.OpenReadAsync();
                    using (var memoryStream = new System.IO.MemoryStream())
                    {
                        await stream.CopyToAsync(memoryStream);
                        byte[] imageBytes = memoryStream.ToArray();
                        _base64Image = Convert.ToBase64String(imageBytes); // Base64-be alakítás
                        foodImage.Source = ImageSource.FromStream(() => new System.IO.MemoryStream(imageBytes));
                    }
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Image selection failed: {ex.Message}", "OK");
            }
        }


        private async void OnSaveClicked(object sender, EventArgs e)
        {
            try
            {
                var updatedFood = new Food
                {
                    FoodId = _foodId,
                    Name = foodNameEntry.Text,
                    Price = double.Parse(foodPriceEntry.Text),
                    Description = foodDescriptionEntry.Text,
                    Picture = _base64Image // Frissített Base64 kép
                };

                bool result = await _restaurantService.UpdateFoodDetails(_restaurantId, _foodId, updatedFood);
                if (result)
                {
                    await DisplayAlert("Success", "Food details updated successfully.", "OK");
                    Restaurant restaurant = new Restaurant();
                    restaurant = await _restaurantService.GetRestaurantById(_restaurantId);
                    App.Current.MainPage = new NavigationPage(new RestaurantProfilePage());
                }
                else
                {
                    await DisplayAlert("Error", "Failed to update food details.", "OK");
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"An error occurred: {ex.Message}", "OK");
            }
        }

        private async void OnCancelClicked(object sender, EventArgs e)
        {
            await Navigation.PopAsync();
        }
    }
}
