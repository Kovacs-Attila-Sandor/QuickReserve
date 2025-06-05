using System;
using System.Collections.Generic;
using System.Linq;
using Xamarin.Forms;
using QuickReserve.Services;
using QuickReserve.Models;
using QuickReserve.Converter;
using QuickReserve.Views.ApplicationViews;

namespace QuickReserve.Views
{
    public partial class AboutPage : ContentPage
    {
        private UserService _userService;
        private RestaurantService _restaurantService;
        private List<Restaurant> originalItems;
        private List<Food> DiscountedFoods;

        public AboutPage()
        {
            InitializeComponent();

            // A Content és ListView kezdetben nem láthatóak
            Content.IsVisible = false;
            lstmoments.IsVisible = false;
            DiscountedFoodsContainer.IsVisible = false;
            loadingIndicator.IsVisible = true;
            LoadingLabel.IsVisible = true;

            _userService = UserService.Instance;
            _restaurantService = RestaurantService.Instance;

            // Éttermek és kedvezményes ételek betöltése
            DisplayRestaurants();
        }

        // Éttermek és kedvezményes ételek betöltése Firebase-ból és megjelenítése
        public async void DisplayRestaurants()
        {
            try
            {
                var allRestaurants = await _restaurantService.GetAllRestaurants();

                if (allRestaurants != null)
                {
                    // Éttermek képeinek dekódolása
                    foreach (var restaurant in allRestaurants)
                    {
                        if (!string.IsNullOrEmpty(restaurant.FirstImageBase64))
                        {
                            restaurant.ImageSourceUri = ImageConverter.ConvertBase64ToImageSource(restaurant.FirstImageBase64);
                        }
                    }

                    // Az eredeti lista frissítése
                    originalItems = allRestaurants;

                    // Kedvezményes ételek betöltése és képek dekódolása
                    DiscountedFoods = await _restaurantService.GetAllFoodsWithDicount();
                    if (DiscountedFoods != null && DiscountedFoods.Any())
                    {
                        foreach (var food in DiscountedFoods)
                        {
                            if (!string.IsNullOrEmpty(food.Picture))
                            {
                                try
                                {
                                    food.ImageSource = ImageConverter.ConvertBase64ToImageSource(food.Picture);
                                }
                                catch (Exception ex)
                                {
                                    Console.WriteLine($"Failed to convert image for food {food.Name}: {ex.Message}");
                                    food.ImageSource = ImageSource.FromFile("image_placeholder.png");
                                }
                            }
                            else
                            {
                                food.ImageSource = ImageSource.FromFile("image_placeholder.png");
                            }
                        }
                    }
                    else
                    {
                        Console.WriteLine("No discounted foods found.");
                        DiscountedFoods = new List<Food>(); // Initialize empty list to avoid null issues
                    }

                    // A ListView és CollectionView adatainak beállítása
                    lstmoments.ItemsSource = originalItems;
                    discountedFoodsCollectionView.ItemsSource = DiscountedFoods;

                    // A tartalom megjelenítése
                    Content.IsVisible = true;
                    lstmoments.IsVisible = true;
                    DiscountedFoodsContainer.IsVisible = DiscountedFoods.Any(); // Only show if there are discounted foods
                    loadingIndicator.IsVisible = false;
                    LoadingLabel.IsVisible = false;
                }
                else
                {
                    Console.WriteLine("No restaurants found.");
                    loadingIndicator.IsVisible = false;
                    LoadingLabel.IsVisible = false;
                    await DisplayAlert("Error", "No restaurants found.", "OK");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading data: {ex.Message}");
                loadingIndicator.IsVisible = false;
                LoadingLabel.IsVisible = false;
                await DisplayAlert("Error", "Failed to load data.", "OK");
            }
        }

        private void OnItemTapped(object sender, ItemTappedEventArgs e)
        {
            if (e.Item is Restaurant selectedRestaurant)
            {
                Navigation.PushAsync(new RestaurantDetailsPage(selectedRestaurant));
            }

            ((ListView)sender).SelectedItem = null;
        }

        private void OnFoodItemTapped(object sender, SelectionChangedEventArgs e)
        {
            if (e.CurrentSelection.FirstOrDefault() is Food selectedFood)
            {
                // Navigálás a FoodDetailsPage-re
                Navigation.PushAsync(new FoodPage(selectedFood, selectedFood.RestaurantId, true));
                Console.WriteLine($"Selected food: {selectedFood.Name}");
            }

            ((CollectionView)sender).SelectedItem = null;
        }

        // Keresési szöveg változásakor futó eseménykezelő
        private void OnSearchTextChanged(object sender, TextChangedEventArgs e)
        {
            var searchText = e.NewTextValue;
            if (string.IsNullOrWhiteSpace(searchText))
            {
                lstmoments.ItemsSource = originalItems;
                discountedFoodsCollectionView.ItemsSource = DiscountedFoods;
            }
            else
            {
                lstmoments.ItemsSource = originalItems
                    .Where(item => item.Name.IndexOf(searchText, StringComparison.OrdinalIgnoreCase) >= 0)
                    .ToList();
                discountedFoodsCollectionView.ItemsSource = DiscountedFoods
                    .Where(food => food.Name.IndexOf(searchText, StringComparison.OrdinalIgnoreCase) >= 0)
                    .ToList();
            }
        }
    }
}