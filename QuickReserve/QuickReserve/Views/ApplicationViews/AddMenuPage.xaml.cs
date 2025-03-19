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

namespace QuickReserve.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AddMenuPage : ContentPage
    {
        private readonly string _restaurantId;
        private readonly RestaurantService _restaurantService;
        private List<string> _categories;
        private List<Food> _pendingFoods;
        private bool isDropdownVisible = false;
        private MediaFile _selectedImageFile;

        public AddMenuPage(string restaurantId)
        {
            InitializeComponent();
            _restaurantId = restaurantId ?? throw new ArgumentNullException(nameof(restaurantId));
            _restaurantService = new RestaurantService();
            _categories = new List<string>();
            _pendingFoods = new List<Food>();
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
        }    

        private async void OnAddCategoryClicked(object sender, EventArgs e)
        {
            string newCategory = await DisplayPromptAsync("New Category", "Enter a new category name:",
                maxLength: 50, keyboard: Keyboard.Text);

            newCategory.Trim();

            if (string.IsNullOrWhiteSpace(newCategory))
                return;

            if (_categories.Contains(newCategory, StringComparer.OrdinalIgnoreCase))
            {
                await DisplayAlert("Error", "Category already exists.", "OK");
                return;
            }

            _categories.Add(newCategory);
            categoryCollectionView.ItemsSource = null;
            categoryCollectionView.ItemsSource = _categories;
        }

        private async void OnDeleteCategoryClicked(object sender, EventArgs e)
        {
            if (sender is ImageButton button && button.CommandParameter is string categoryToDelete)
            {
                bool confirm = await DisplayAlert("Confirm Delete",
                    $"Are you sure you want to delete the category '{categoryToDelete}'?",
                    "Yes", "No");

                if (!confirm)
                    return;

                _categories.Remove(categoryToDelete);
                categoryCollectionView.ItemsSource = null;
                categoryCollectionView.ItemsSource = _categories;

                if (lblSelectedItem.Text == categoryToDelete)
                    lblSelectedItem.Text = "Select a category";

                // Remove any pending foods with this category
                _pendingFoods.RemoveAll(food => food.Category == categoryToDelete);
            }
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
                Category = lblSelectedItem.Text != "Select a category" ? lblSelectedItem.Text : null,
                FoodId = Guid.NewGuid().ToString(),
                Picture = base64Image
            };

            _pendingFoods.Add(newFood);
            ClearForm();
        }

        private bool ValidateInput(out double price)
        {
            price = 0;
            return !string.IsNullOrEmpty(txtMenuItemName.Text) &&
                   double.TryParse(txtPrice.Text, out price) &&
                   price >= 0 &&
                   !string.IsNullOrEmpty(txtMenuItemDescription.Text) &&
                   lblSelectedItem.Text != "Select a category";
        }

        private void ClearForm()
        {
            txtMenuItemDescription.Text = string.Empty;
            txtPrice.Text = string.Empty;
            txtMenuItemName.Text = string.Empty;
            dropdownFrame.IsVisible = false;
            isDropdownVisible = false;
            _selectedImageFile = null;
            imgFood.Source = null;
        }

        private async void AddLayout(object sender, EventArgs e)
        {
            try
            {
                LoadingOverlay.IsVisible = true;

                // Save all pending changes to the database
                var restaurant = await _restaurantService.GetRestaurantById(_restaurantId);
                if (restaurant == null)
                {
                    await DisplayAlert("Error", "Restaurant not found", "OK");
                    return;
                }

                await _restaurantService.AddCategoryToRestaurant(_restaurantId, _categories);

                // Add all pending foods
                foreach (var food in _pendingFoods)
                {
                    await _restaurantService.AddFoodToRestaurant(_restaurantId, food);
                }

                await DisplayAlert("Success", "Menu saved successfully", "OK");
                _pendingFoods.Clear(); // Clear after successful save

                // Navigate to next page
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

        private void OnCategoryTapped(object sender, EventArgs e)
        {
            isDropdownVisible = !isDropdownVisible;
            dropdownFrame.IsVisible = isDropdownVisible;
        }

        private void OnCategorySelected(object sender, SelectionChangedEventArgs e)
        {
            if (e.CurrentSelection.FirstOrDefault() is string selectedCategory)
            {
                lblSelectedItem.Text = selectedCategory;
                dropdownFrame.IsVisible = false;
                isDropdownVisible = false;
            }
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
                _selectedImageFile = await CrossMedia.Current.PickPhotoAsync(new PickMediaOptions
                {
                    PhotoSize = PhotoSize.Medium
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