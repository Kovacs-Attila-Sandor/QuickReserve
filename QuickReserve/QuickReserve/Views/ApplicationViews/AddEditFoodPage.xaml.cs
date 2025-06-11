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
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using System.Globalization;

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
                PriceEntry.Text = _food.Price.ToString("0.##", CultureInfo.InvariantCulture).Replace('.', ',');
                DiscountedPriceEntry.Text = _food.DiscountedPrice?.ToString("0.##", CultureInfo.InvariantCulture).Replace('.', ',') ?? string.Empty;
                CategoryPicker.SelectedItem = _food.Category;
                FoodImage.Source = _food.ImageSource;
                IngredientsEntry.Text = _food.Ingredients?.Any() == true ? string.Join(", ", _food.Ingredients) : "";
                AllergensEntry.Text = _food.Allergens?.Any() == true ? string.Join(", ", _food.Allergens) : "";
                CaloriesEntry.Text = _food.Calories.ToString("F0", CultureInfo.InvariantCulture);
                WeightEntry.Text = _food.Weight.ToString("0.##", CultureInfo.InvariantCulture).Replace('.', ',');
                PreparationTimeEntry.Text = _food.PreparationTime.ToString(CultureInfo.InvariantCulture);
                swIsAvailable.IsToggled = _food.IsAvailable;
                CategoryPicker.ItemsSource = _restaurant.Categories;
            }
            else
            {
                CategoryPicker.ItemsSource = _restaurant.Categories;
                CaloriesEntry.Text = string.Empty;
                WeightEntry.Text = string.Empty;
                PreparationTimeEntry.Text = string.Empty;
                PriceEntry.Text = string.Empty;
            }
        }

        private async void OnAddCategoryClicked(object sender, EventArgs e)
        {
            // Show dialog to input a new category
            string newCategory = await DisplayPromptAsync("Új kategória", "Adja meg a kategória nevét:");
            if (!string.IsNullOrWhiteSpace(newCategory))
            {
                try
                {
                    // Normalize and validate the new category
                    newCategory = newCategory.Trim();
                    if (_restaurant.Categories == null)
                    {
                        _restaurant.Categories = new List<string>();
                    }

                    // Check if category already exists (case-insensitive)
                    if (_restaurant.Categories.Any(c => c.Equals(newCategory, StringComparison.OrdinalIgnoreCase)))
                    {
                        await PopupNavigation.Instance.PushAsync(new CustomAlert("Hiba", "Ez a kategória már létezik."));
                        return;
                    }

                    // Add to restaurant categories
                    _restaurant.Categories.Add(newCategory);

                    // Update restaurant in backend
                    await _restaurantService.UpdateRestaurantAsync(_restaurant);

                    // Refresh CategoryPicker
                    CategoryPicker.ItemsSource = null;
                    CategoryPicker.ItemsSource = _restaurant.Categories;

                    // Optionally select the new category
                    CategoryPicker.SelectedItem = newCategory;

                    await PopupNavigation.Instance.PushAsync(new CustomAlert("Siker", $"A '{newCategory}' kategória sikeresen hozzáadva."));
                }
                catch (Exception ex)
                {
                    await PopupNavigation.Instance.PushAsync(new CustomAlert("Hiba", $"Nem sikerült hozzáadni a kategóriát: {ex.Message}"));
                }
            }
        }

        private async void OnSaveClicked(object sender, EventArgs e)
        {
            var viewModel = (FoodViewModel)BindingContext;
            var food = viewModel.Food;

            try
            {
                if (!ValidateInput(out double price, out double? discountedPrice, out int preparationTime, out double calories, out double weight))
                {
                    await PopupNavigation.Instance.PushAsync(new CustomAlert("Hiba", "Kérjük, töltse ki az összes szükséges mezőt helyesen."));
                    return;
                }

                // Update food properties from Entry fields
                food.RestaurantId = _restaurant.RestaurantId;
                food.Name = NameEntry.Text;
                food.Description = DescriptionEditor.Text;
                food.Price = price;
                food.DiscountedPrice = discountedPrice;
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
                    await PopupNavigation.Instance.PushAsync(new CustomAlert("Siker", "Étel sikeresen frissítve."));
                }
                else
                {
                    // Add new food
                    food.FoodId = Guid.NewGuid().ToString();
                    food.CreatedDate = DateTime.UtcNow;
                    food.RestaurantId = _restaurant.RestaurantId;
                    _restaurant.Foods.Add(food);
                    await _restaurantService.AddFoodToRestaurant(_restaurant.RestaurantId, food);
                    await PopupNavigation.Instance.PushAsync(new CustomAlert("Siker", "Étel sikeresen hozzáadva."));
                }

                // Navigate back
                await Navigation.PopAsync();
            }
            catch (Exception ex)
            {
                await PopupNavigation.Instance.PushAsync(new CustomAlert("Hiba", $"Nem sikerült menteni az ételt: {ex.Message}"));
            }
            finally
            {
                SaveButton.IsEnabled = true;
                DeleteButton.IsEnabled = true;
            }
        }

        private bool ValidateInput(out double price, out double? discountedPrice, out int preparationTime, out double calories, out double weight)
        {
            price = 0;
            preparationTime = 0;
            calories = 0;
            weight = 0;
            discountedPrice = null;

            // Replace ',' with '.' for decimal separator to ensure correct parsing
            string priceText = PriceEntry.Text?.Replace(',', '.');
            string discountedPriceText = DiscountedPriceEntry.Text?.Replace(',', '.');
            string caloriesText = CaloriesEntry.Text?.Replace(',', '.');
            string weightText = WeightEntry.Text?.Replace(',', '.');

            bool isPriceValid = double.TryParse(priceText, NumberStyles.Any, CultureInfo.InvariantCulture, out price) && price >= 0;
            bool isDiscountedPriceValid = true;
            double tempDiscountedPrice = 0;

            if (!string.IsNullOrWhiteSpace(discountedPriceText))
            {
                isDiscountedPriceValid = double.TryParse(discountedPriceText, NumberStyles.Any, CultureInfo.InvariantCulture, out tempDiscountedPrice) && tempDiscountedPrice >= 0;
                if (isDiscountedPriceValid)
                    discountedPrice = tempDiscountedPrice;
            }

            return !string.IsNullOrWhiteSpace(NameEntry.Text) &&
                   !string.IsNullOrWhiteSpace(DescriptionEditor.Text) &&
                   isPriceValid &&
                   isDiscountedPriceValid &&
                   CategoryPicker.SelectedItem != null &&
                   int.TryParse(PreparationTimeEntry.Text, NumberStyles.Any, CultureInfo.InvariantCulture, out preparationTime) && preparationTime >= 0 &&
                   double.TryParse(caloriesText, NumberStyles.Any, CultureInfo.InvariantCulture, out calories) && calories >= 0 &&
                   double.TryParse(weightText, NumberStyles.Any, CultureInfo.InvariantCulture, out weight) && weight >= 0;
        }

        private async void OnDeleteClicked(object sender, EventArgs e)
        {
            var viewModel = (FoodViewModel)BindingContext;
            var food = viewModel.Food;

            bool confirm = await DisplayAlert("Törlés megerősítése",
                $"Biztosan törölni szeretné a(z) {food.Name} ételt?",
                "Törlés", "Mégse");

            if (confirm)
            {
                try
                {
                    SaveButton.IsEnabled = false;
                    DeleteButton.IsEnabled = false;

                    _restaurant.Foods.Remove(food);
                    await _restaurantService.DeleteFoodFromRestaurant(_restaurant.RestaurantId, food.FoodId);
                    await PopupNavigation.Instance.PushAsync(new CustomAlert("Siker", $"{food.Name} sikeresen törölve."));
                    await Navigation.PopAsync();
                }
                catch (Exception ex)
                {
                    await PopupNavigation.Instance.PushAsync(new CustomAlert("Hiba", $"Nem sikerült törölni az ételt: {ex.Message}"));
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
                    await PopupNavigation.Instance.PushAsync(new CustomAlert("Hiba", "A fénykép kiválasztása nem támogatott ezen az eszközön."));
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
                await PopupNavigation.Instance.PushAsync(new CustomAlert("Hiba", $"Hiba történt a kép kiválasztása során: {ex.Message}"));
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