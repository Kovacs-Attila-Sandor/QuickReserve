using Plugin.Media.Abstractions;
using Plugin.Media;
using QuickReserve.Models;
using QuickReserve.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Rg.Plugins.Popup.Services;
using QuickReserve.Views.PopUps;

namespace QuickReserve.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AddMenuPage : ContentPage
    {
        private readonly string _restaurantId;
        private readonly RestaurantService _restaurantService;
        private List<string> _categories;
        private List<Food> _pendingFoods;
        private MediaFile _selectedImageFile;

        public AddMenuPage(string restaurantId)
        {
            InitializeComponent();
            _restaurantId = restaurantId ?? throw new ArgumentNullException(nameof(restaurantId));
            _restaurantService = RestaurantService.Instance;
            _categories = new List<string>();
            _pendingFoods = new List<Food>();
            categoryPicker.ItemsSource = _categories;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            var restaurant = await _restaurantService.GetRestaurantById(_restaurantId);
            if (restaurant != null && restaurant.Foods != null)
            {
                _categories = restaurant.Foods.Select(f => f.Category).Distinct().ToList();
                categoryPicker.ItemsSource = _categories;
            }
        }

        private async void OnAddCategoryClicked(object sender, EventArgs e)
        {
            string newCategory = await DisplayPromptAsync("New Category", "Enter a new category name:",
                maxLength: 50, keyboard: Keyboard.Text);

            newCategory = newCategory?.Trim();

            if (string.IsNullOrWhiteSpace(newCategory))
                return;

            if (_categories.Contains(newCategory, StringComparer.OrdinalIgnoreCase))
            {
                await DisplayAlert("Error", "Category already exists.", "OK");
                return;
            }

            _categories.Add(newCategory);
            categoryPicker.ItemsSource = null;
            categoryPicker.ItemsSource = _categories;
            categoryPicker.SelectedItem = newCategory;
        }

        private async void AddItem(object sender, EventArgs e)
        {
            if (!ValidateInput(out double price, out int preparationTime, out int stockQuantity, out double calories, out double weight))
            {
                await DisplayAlert("Error", "Please fill all required fields correctly", "OK");
                return;
            }

            string base64Image = null;
            if (_selectedImageFile != null)
            {
                using (var memoryStream = new System.IO.MemoryStream())
                {
                    var stream = _selectedImageFile.GetStream();
                    await stream.CopyToAsync(memoryStream);
                    byte[] imageBytes = memoryStream.ToArray();
                    base64Image = Convert.ToBase64String(imageBytes);
                }
            }

            var newFood = new Food
            {
                FoodId = Guid.NewGuid().ToString(),
                RestaurantId = _restaurantId,
                Name = txtMenuItemName.Text.Trim(),
                Description = txtMenuItemDescription.Text.Trim(),
                Price = price,
                Category = categoryPicker.SelectedItem?.ToString(),
                Picture = base64Image,
                IsAvailable = swIsAvailable.IsToggled,
                PreparationTime = preparationTime,
                Ingredients = txtIngredients.Text.Split(',').Select(i => i.Trim()).ToList(),
                Allergens = txtAllergens.Text.Split(',').Select(a => a.Trim()).ToList(),
                Calories = calories,
                Tags = txtTags.Text.Split(',').Select(t => t.Trim()).ToList(),
                Weight = weight,
                CreatedDate = DateTime.UtcNow,
                LastUpdated = DateTime.UtcNow,
                OrderCount = 0
            };

            _pendingFoods.Add(newFood);
            ClearForm();
            await PopupNavigation.Instance.PushAsync(new CustomAlert("Success", "Menu item added successfully"));
        }

        private bool ValidateInput(out double price, out int preparationTime, out int stockQuantity, out double calories, out double weight)
        {
            price = 0;
            preparationTime = 0;
            stockQuantity = 0;
            calories = 0;
            weight = 0;

            return !string.IsNullOrWhiteSpace(txtMenuItemName.Text) &&
                   !string.IsNullOrWhiteSpace(txtMenuItemDescription.Text) &&
                   double.TryParse(txtPrice.Text, out price) && price >= 0 &&
                   categoryPicker.SelectedIndex != -1 &&
                   int.TryParse(txtPreparationTime.Text, out preparationTime) && preparationTime >= 0 &&
                   int.TryParse(txtStockQuantity.Text, out stockQuantity) && stockQuantity >= 0 &&
                   double.TryParse(txtCalories.Text, out calories) && calories >= 0 &&
                   double.TryParse(txtWeight.Text, out weight) && weight >= 0;
        }

        private void ClearForm()
        {
            txtMenuItemName.Text = string.Empty;
            txtMenuItemDescription.Text = string.Empty;
            txtPrice.Text = string.Empty;
            txtPreparationTime.Text = string.Empty;
            txtStockQuantity.Text = string.Empty;
            txtIngredients.Text = string.Empty;
            txtAllergens.Text = string.Empty;
            txtCalories.Text = string.Empty;
            txtTags.Text = string.Empty;
            swIsAvailable.IsToggled = true;
            _selectedImageFile = null;
            imgFood.Source = null;
            // Don't clear category selection
        }

        private async void AddLayout(object sender, EventArgs e)
        {
            try
            {
                if (!_pendingFoods.Any() && !_categories.Any())
                {
                    await DisplayAlert("Warning", "Please add at least one menu item or category before proceeding", "OK");
                    return;
                }

                LoadingOverlay.IsVisible = true;

                var restaurant = await _restaurantService.GetRestaurantById(_restaurantId);
                if (restaurant == null)
                {
                    await DisplayAlert("Error", "Restaurant not found", "OK");
                    return;
                }

                if (_categories.Any())
                {
                    await _restaurantService.AddCategoryToRestaurant(_restaurantId, _categories);
                }

                foreach (var food in _pendingFoods)
                {
                    await _restaurantService.AddFoodToRestaurant(_restaurantId, food);
                }
                await PopupNavigation.Instance.PushAsync(new CustomAlert("Success", "Menu saved successfully"));
                _pendingFoods.Clear();

                await Navigation.PushAsync(new CreateLayoutPage(_restaurantId));
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Failed to save menu: {ex.Message}", "OK");
            }
            finally
            {
                LoadingOverlay.IsVisible = false;
            }
        }

        private void OnTextChanged(object sender, TextChangedEventArgs e)
        {
            if (!(sender is Entry entry)) return;

            Label placeholderLabel = GetPlaceholderLabel(entry);
            if (placeholderLabel == null) return;

            bool hasText = !string.IsNullOrWhiteSpace(entry.Text);

            placeholderLabel.IsVisible = hasText;
            placeholderLabel.FadeTo(hasText ? 1 : 0, 150);
            placeholderLabel.TranslateTo(2, hasText ? -3 : -7, 170);
            entry.Placeholder = hasText ? string.Empty : GetPlaceholderText(entry);
        }

        private void OnCategorySelected(object sender, EventArgs e)
        {
            // Selection handled by Picker
        }

        private Label GetPlaceholderLabel(Entry entry)
        {
            if (entry == txtMenuItemName) return lblMenuItemNamePlaceholder;
            if (entry == txtMenuItemDescription) return lblMenuItemDescriptionPlaceholder;
            if (entry == txtPrice) return lblMenuItemPrice;
            if (entry == txtPreparationTime) return lblPreparationTime;
            if (entry == txtStockQuantity) return lblStockQuantity;
            if (entry == txtIngredients) return lblIngredients;
            if (entry == txtAllergens) return lblAllergens;
            if (entry == txtCalories) return lblCalories;
            if (entry == txtTags) return lblTags;
            return null;
        }

        private string GetPlaceholderText(Entry entry)
        {
            if (entry == txtMenuItemName) return "Name";
            if (entry == txtMenuItemDescription) return "Description";
            if (entry == txtPrice) return "Price (in €)";
            if (entry == txtPreparationTime) return "Preparation Time (minutes)";
            if (entry == txtStockQuantity) return "Stock Quantity";
            if (entry == txtIngredients) return "Ingredients (comma separated)";
            if (entry == txtAllergens) return "Allergens (comma separated)";
            if (entry == txtCalories) return "Calories (kcal)";
            if (entry == txtTags) return "Tags (comma separated)";
            return string.Empty;
        }

        private async void OnUploadImageClicked(object sender, EventArgs e)
        {
            try
            {
                await CrossMedia.Current.Initialize();

                if (!CrossMedia.Current.IsPickPhotoSupported)
                {
                    await DisplayAlert("Error", "Photo picking is not supported on this device", "OK");
                    return;
                }

                _selectedImageFile = await CrossMedia.Current.PickPhotoAsync(new PickMediaOptions
                {
                    PhotoSize = PhotoSize.Medium,
                    CompressionQuality = 80
                });

                if (_selectedImageFile != null)
                {
                    imgFood.Source = ImageSource.FromStream(() => _selectedImageFile.GetStream());
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Failed to upload image: {ex.Message}", "OK");
            }
        }
    }
}