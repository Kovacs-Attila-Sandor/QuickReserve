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
using System.Windows.Input;

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
        public ICommand DeleteCategoryCommand { get; }


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
            // Load existing categories if needed
            var restaurant = await _restaurantService.GetRestaurantById(_restaurantId);         
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
            categoryPicker.ItemsSource = null; // Force refresh
            categoryPicker.ItemsSource = _categories;
            categoryPicker.SelectedItem = newCategory;
        }

        private async void OnDeleteCategoryClicked(object sender, EventArgs e)
        {
            if (categoryPicker.SelectedIndex == -1)
            {
                await DisplayAlert("Error", "Please select a category to delete", "OK");
                return;
            }

            string categoryToDelete = _categories[categoryPicker.SelectedIndex];

            bool confirm = await DisplayAlert("Confirm Delete",
                $"Are you sure you want to delete the category '{categoryToDelete}'?",
                "Yes", "No");

            if (!confirm)
                return;

            _categories.Remove(categoryToDelete);
            categoryPicker.ItemsSource = null;
            categoryPicker.ItemsSource = _categories;

            // Remove any pending foods with this category
            _pendingFoods.RemoveAll(food => food.Category == categoryToDelete);
        }

        private async void AddItem(object sender, EventArgs e)
        {
            if (!ValidateInput(out double price))
            {
                await DisplayAlert("Error", "Please fill all fields correctly and select a category", "OK");
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
                Name = txtMenuItemName.Text.Trim(),
                Price = price,
                Description = txtMenuItemDescription.Text.Trim(),
                Category = categoryPicker.SelectedItem?.ToString(),
                FoodId = Guid.NewGuid().ToString(),
                Picture = base64Image
            };

            _pendingFoods.Add(newFood);
            ClearForm();
            await DisplayAlert("Success", "Menu item added successfully", "OK");
        }

        private bool ValidateInput(out double price)
        {
            price = 0;
            return !string.IsNullOrWhiteSpace(txtMenuItemName.Text) &&
                   double.TryParse(txtPrice.Text, out price) &&
                   price >= 0 &&
                   !string.IsNullOrWhiteSpace(txtMenuItemDescription.Text) &&
                   categoryPicker.SelectedIndex != -1;
        }

        private void ClearForm()
        {
            txtMenuItemDescription.Text = string.Empty;
            txtPrice.Text = string.Empty;
            txtMenuItemName.Text = string.Empty;
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

                // Save categories first
                if (_categories.Any())
                {
                    await _restaurantService.AddCategoryToRestaurant(_restaurantId, _categories);
                }

                // Save all pending foods
                foreach (var food in _pendingFoods)
                {
                    await _restaurantService.AddFoodToRestaurant(_restaurantId, food);
                }

                await DisplayAlert("Success", "Menu saved successfully", "OK");
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
            // Selection is automatically handled by the Picker
        }

        private Label GetPlaceholderLabel(Entry entry)
        {
            if (entry == txtMenuItemDescription) return lblMenuItemDescriptionPlaceholder;
            if (entry == txtMenuItemName) return lblMenuItemNamePlaceholder;
            if (entry == txtPrice) return lblMenuItemPrice;
            return null;
        }

        private string GetPlaceholderText(Entry entry)
        {
            if (entry == txtMenuItemDescription) return "Menu Item Description";
            if (entry == txtMenuItemName) return "Menu Item Name";
            if (entry == txtPrice) return "Price";
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