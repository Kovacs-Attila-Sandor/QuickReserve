using Firebase.Auth;
using Plugin.Media.Abstractions;
using Plugin.Media;
using QuickReserve.Models;
using QuickReserve.Services;
using QuickReserve.Views.PopUps;
using Rg.Plugins.Popup.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace QuickReserve.Views.ApplicationViews
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AddEditFoodPage : ContentPage
    {
        private Restaurant _restaurant;
        private RestaurantService _restaurantService;
        private Food _food;
        private bool _isEditMode;

        public AddEditFoodPage(Restaurant restaurant, Food food = null)
        {
            InitializeComponent();
            _restaurant = restaurant;
            _restaurantService = RestaurantService.Instance;
            _food = food ?? new Food
            {
                Ingredients = new List<string>(),
                Allergens = new List<string>(),
                Tags = new List<string>(),
                Ratings = new List<Rating>()
            };

            _isEditMode = food != null;

            // Set BindingContext
            BindingContext = new FoodViewModel
            {
                Food = _food,
                Title = _isEditMode ? "Edit Food" : "Add Food",
                IsEditMode = _isEditMode,
                IngredientsText = _food.Ingredients?.Any() == true ? string.Join(", ", _food.Ingredients) : "",
                AllergensText = _food.Allergens?.Any() == true ? string.Join(", ", _food.Allergens) : ""
            };

            LoadFoodInformations();
        }

        private void LoadFoodInformations()
        {
            if (_isEditMode)
            {
                NameEntry.Text = _food.Name;
                DescriptionEditor.Text = _food.Description;
                PriceEntry.Text = _food.Price.ToString();
                CategoryPicker.SelectedItem = _food.Category;
                FoodImage.Source = _food.ImageSource;
                IngredientsEntry.Text = _food.Ingredients?.Any() == true ? string.Join(", ", _food.Ingredients) : "";
                AllergensEntry.Text = _food.Allergens?.Any() == true ? string.Join(", ", _food.Allergens) : "";
                CaloriesEntry.Text = _food.Calories.ToString("F0");
                WeightEntry.Text = _food.Weight.ToString();
                PreparationTimeEntry.Text = _food.PreparationTime.ToString();
                swIsAvailable.IsToggled = _food.IsAvailable;
                CategoryPicker.ItemsSource = _restaurant.Categories;
            }
        }

        private async void OnSaveClicked(object sender, EventArgs e)
        {
            var viewModel = (FoodViewModel)BindingContext;
            var food = viewModel.Food;

            try
            {
                if (!ValidateInput(out double price, out int preparationTime, out double calories, out double weight))
                {
                    await PopupNavigation.Instance.PushAsync(new CustomAlert("Error", "Please fill all required fields correctly."));
                    return;
                }

                // Update food properties
                food.Name = NameEntry.Text;
                food.Description = DescriptionEditor.Text;
                food.Price = price;
                food.Category = CategoryPicker.SelectedItem?.ToString() ?? "Main Course";
                food.Ingredients = IngredientsEntry.Text?.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(i => i.Trim()).ToList() ?? new List<string>();
                food.Allergens = AllergensEntry.Text?.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(i => i.Trim()).ToList() ?? new List<string>();
                food.Calories = calories;
                food.Weight = weight;
                food.PreparationTime = preparationTime;
                food.IsAvailable = swIsAvailable.IsToggled;
                food.LastUpdated = DateTime.UtcNow;

                SaveButton.IsEnabled = false;
                DeleteButton.IsEnabled = false;

                if (_isEditMode)
                {
                    // Update existing food
                    await _restaurantService.UpdateFoodDetails(_restaurant.RestaurantId, _food.FoodId, food);
                    await PopupNavigation.Instance.PushAsync(new CustomAlert("Success", "Food updated successfully."));
                }
                else
                {
                    // Add new food
                    food.FoodId = Guid.NewGuid().ToString();
                    food.CreatedDate = DateTime.UtcNow;
                    _restaurant.Foods.Add(food);
                    await _restaurantService.AddFoodToRestaurant(_restaurant.RestaurantId, food);
                    await PopupNavigation.Instance.PushAsync(new CustomAlert("Success", "Food added successfully."));
                }

                // Navigate back
                await Navigation.PopAsync();
            }
            catch (Exception ex)
            {
                await PopupNavigation.Instance.PushAsync(new CustomAlert("Error", $"Failed to save food: {ex.Message}"));
            }
            finally
            {
                SaveButton.IsEnabled = true;
                DeleteButton.IsEnabled = true;
            }
        }

        private bool ValidateInput(out double price, out int preparationTime, out double calories, out double weight)
        {
            price = 0;
            preparationTime = 0;
            calories = 0;
            weight = 0;

            return !string.IsNullOrWhiteSpace(NameEntry.Text) &&
                   !string.IsNullOrWhiteSpace(DescriptionEditor.Text) &&
                   double.TryParse(PriceEntry.Text, out price) && price >= 0 &&
                   CategoryPicker.SelectedIndex != -1 &&
                   int.TryParse(PreparationTimeEntry.Text, out preparationTime) && preparationTime >= 0 &&
                   double.TryParse(CaloriesEntry.Text, out calories) && calories >= 0 &&
                   double.TryParse(WeightEntry.Text, out weight) && weight >= 0;
        }

        private async void OnDeleteClicked(object sender, EventArgs e)
        {
            var viewModel = (FoodViewModel)BindingContext;
            var food = viewModel.Food;

            bool confirm = await DisplayAlert("Confirm Delete",
                $"Are you sure you want to delete {food.Name}?",
                "Delete", "Cancel");

            if (confirm)
            {
                try
                {
                    SaveButton.IsEnabled = false;
                    DeleteButton.IsEnabled = false;

                    _restaurant.Foods.Remove(food);
                    await _restaurantService.DeleteFoodFromRestaurant(_restaurant.RestaurantId, food.FoodId);
                    await PopupNavigation.Instance.PushAsync(new CustomAlert("Success", $"{food.Name} has been deleted."));
                    await Navigation.PopAsync();
                }
                catch (Exception ex)
                {
                    await PopupNavigation.Instance.PushAsync(new CustomAlert("Error", $"Failed to delete food: {ex.Message}"));
                }
                finally
                {
                    SaveButton.IsEnabled = true;
                    DeleteButton.IsEnabled = true;
                }
            }
        }

        private async void OnCancelClicked(object sender, EventArgs e)
        {
            await Navigation.PopAsync();
        }

        private async void OnEditFoodImageClicked(object sender, EventArgs e)
        {
            try
            {
                if (!CrossMedia.Current.IsPickPhotoSupported)
                {
                    await PopupNavigation.Instance.PushAsync(new CustomAlert("Error", "Photo picking is not supported on this device."));
                    return;
                }

                var photo = await CrossMedia.Current.PickPhotoAsync(new PickMediaOptions
                {
                    PhotoSize = PhotoSize.Medium,
                    CompressionQuality = 80
                });

                if (photo == null)
                    return;

                var viewModel = (FoodViewModel)BindingContext;
                var food = viewModel.Food;

                FoodImage.Source = ImageSource.FromStream(() => photo.GetStream());

                using (var stream = photo.GetStream())
                using (var memoryStream = new MemoryStream())
                {
                    await stream.CopyToAsync(memoryStream);
                    byte[] imageBytes = memoryStream.ToArray();
                    food.Picture = Convert.ToBase64String(imageBytes);
                    food.ImageSource = ImageSource.FromStream(() => new MemoryStream(imageBytes));
                }
            }
            catch (Exception ex)
            {
                await PopupNavigation.Instance.PushAsync(new CustomAlert("Error", $"An error occurred while selecting the image: {ex.Message}"));
            }
        }
    }

    public class FoodViewModel
    {
        public Food Food { get; set; }
        public string Title { get; set; }
        public bool IsEditMode { get; set; }
        public string IngredientsText { get; set; }
        public string AllergensText { get; set; }
    }
}