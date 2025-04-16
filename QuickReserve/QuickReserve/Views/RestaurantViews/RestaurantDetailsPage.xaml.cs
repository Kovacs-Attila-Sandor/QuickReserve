using Xamarin.Forms;
using QuickReserve.Models;
using System;
using System.Collections.Generic;
using QuickReserve.Converter;
using QuickReserve.Services;
using System.Diagnostics;
using System.Linq;
using QuickReserve.Views.ApplicationViews;

namespace QuickReserve.Views
{
    public partial class RestaurantDetailsPage : ContentPage
    {
        private Restaurant restaurant;
        private readonly RestaurantService _restaurantService;
        private List<Food> allFoods;

        public RestaurantDetailsPage(Restaurant selectedRestaurant)
        {
            InitializeComponent();
            restaurant = selectedRestaurant;
            _restaurantService = RestaurantService.Instance;
            allFoods = restaurant.Foods?.ToList() ?? new List<Food>();

            Application.Current.Properties["restaurantId"] = restaurant.RestaurantId;
            ConvertImages(restaurant);
            SetGroupedFoods();

            BindingContext = restaurant;
            
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            SetGroupedFoods();
            BindingContext = restaurant;
        }

        private void SetGroupedFoods()
        {
            foreach (var food in allFoods)
            {
                Debug.WriteLine($"Food: {food.Name}, Category: {food.Category ?? "NULL"}");
            }

            var grouped = allFoods
                .GroupBy(f => f.Category ?? "Uncategorized")
                .OrderBy(g => g.Key)
                .ToList();

            foreach (var group in grouped)
            {
                Debug.WriteLine($"Category: {group.Key}, Items: {group.Count()}");
            }

            restaurant.GroupedFoods = grouped;

            foreach (var group in restaurant.GroupedFoods)
            {
                Debug.WriteLine($"Category: {group.Key}, Items: {group.Count()}");
            }
        }

        private void OnCategorySelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.CurrentSelection.FirstOrDefault() is string selectedCategory)
            {
                var groupIndex = restaurant.GroupedFoods.ToList().FindIndex(g => g.Key == selectedCategory);
                if (groupIndex >= 0)
                {
                    double scrollPosition = 0;
                    for (int i = 0; i < groupIndex; i++)
                    {
                        var group = restaurant.GroupedFoods.ElementAt(i);

                        scrollPosition += 10 + 5 + 24 + 5;

                        scrollPosition += group.Count() * (220 + 5);
                        scrollPosition += 15;
                    }

                    Debug.WriteLine($"Kiválasztott kategória: {selectedCategory}");
                    Debug.WriteLine($"Számított görgetési pozíció: {scrollPosition}");

                    // Görgetés a pontos pozícióhoz
                    mainScrollView.ScrollToAsync(0, scrollPosition, true);
                }
                else
                {
                    Debug.WriteLine($"A kategória '{selectedCategory}' nem található a GroupedFoods-ban.");
                }
            }
        }

        private void ConvertImages(Restaurant selectedRestaurant)
        {
            selectedRestaurant.ImageSourceList = new List<ImageSource>();
            if (selectedRestaurant.ImageBase64List != null)
            {
                foreach (var base64Image in selectedRestaurant.ImageBase64List)
                {
                    selectedRestaurant.ImageSourceList.Add(ImageConverter.ConvertBase64ToImageSource(base64Image));
                }
            }

            if (selectedRestaurant.Foods != null)
            {
                foreach (var food in selectedRestaurant.Foods)
                {
                    if (!string.IsNullOrEmpty(food.Picture))
                    {
                        food.ImageSource = ImageConverter.ConvertBase64ToImageSource(food.Picture);
                    }
                }
            }
        }

        private async void OnDatePickerClicked(object sender, EventArgs e) { }

        private void OnHeartClicked(object sender, EventArgs e)
        {
            var button = sender as ImageButton;
            if (button != null)
            {
                if (button.Source is FileImageSource fileSource)
                {
                    button.Source = fileSource.File == "not_filled_heart_icon.png" ? "filled_heart_icon.png" : "not_filled_heart_icon.png";
                }
            }
        }

        private void OnLabelTapped(object sender, EventArgs e)
        {
            if (sender is Label label)
            {
                label.MaxLines = label.MaxLines == 5 ? int.MaxValue : 5;
            }
        }

        private async void OnBackButtonClicked(object sender, EventArgs e)
        {
            await Navigation.PopAsync();
        }

        private async void ReservationButton_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new RestaurantLayoutPage(restaurant.RestaurantId));
        }

        private async void RefreshView_Refreshing(object sender, EventArgs e)
        {
            try
            {
                var refreshedRestaurant = await _restaurantService.GetRestaurantById(restaurant.RestaurantId);
                if (refreshedRestaurant != null)
                {
                    ConvertImages(refreshedRestaurant);
                    allFoods = refreshedRestaurant.Foods?.ToList() ?? new List<Food>();
                    restaurant = refreshedRestaurant;
                    SetGroupedFoods();
                    BindingContext = restaurant;
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Hiba", $"Hiba történt az adatok frissítése közben: {ex.Message}", "OK");
            }
            finally
            {
                myRefreshView.IsRefreshing = false;
            }
        }

        private async void OnItemTapped(object sender, EventArgs e)
        {
            // A sender a Frame, amelyre a TapGestureRecognizer vonatkozik
            var frame = sender as Frame;
            if (frame != null)
            {
                // A BindingContext-ből kinyerjük a Food objektumot
                var food = frame.BindingContext as Food;
                if (food != null)
                {
                    // Navigáció a FoodPage-re a food és restaurant.RestaurantId paraméterekkel
                    await Navigation.PushAsync(new FoodPage(food, restaurant.RestaurantId));
                }
            }
        }
    }
}