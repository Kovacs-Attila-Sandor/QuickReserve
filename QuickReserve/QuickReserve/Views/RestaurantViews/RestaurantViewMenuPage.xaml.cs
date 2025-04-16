using QuickReserve.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using QuickReserve.Views.ApplicationViews;
using QuickReserve.Services;

namespace QuickReserve.Views.RestaurantViews
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class RestaurantViewMenuPage : ContentPage
    {
        private Restaurant _restaurant;
        private RestaurantService _restaurantService;
        private List<Food> _allFoods; // Eredeti ételek tárolása
        private List<Food> _filteredFoods; // Szűrt ételek tárolása

        public RestaurantViewMenuPage(Restaurant restaurant)
        {
            InitializeComponent();
            _restaurant = restaurant;
            _restaurantService = RestaurantService.Instance;
            
        }
        protected override void OnAppearing()
        {
            base.OnAppearing();         
            _allFoods = _restaurant.Foods?.ToList() ?? new List<Food>(); // Másolat készítése
            _filteredFoods = _allFoods; // Kezdetben minden étel látható
            BindingContext = this; // BindingContext beállítása az oldalra
            FoodCollectionView.ItemsSource = _filteredFoods; // CollectionView inicializálása
        }
        private async void OnAddNewFoodClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new AddEditFoodPage(_restaurant));
        }

        private async void OnEditFoodClicked(object sender, EventArgs e)
        {
            if (sender is Button button && button.CommandParameter is Food food)
            {
                await Navigation.PushAsync(new AddEditFoodPage(_restaurant, food));
            }
        }

        private async void OnCancelClicked(object sender, EventArgs e)
        {
            await Navigation.PopAsync();
        }

        private async void OnDeleteFoodClicked(object sender, EventArgs e)
        {
            if (sender is Button button && button.CommandParameter is Food food)
            {
                bool confirm = await DisplayAlert("Confirm Delete",
                    $"Are you sure you want to delete {food.Name}?",
                    "Delete", "Cancel");

                if (confirm)
                {
                    _restaurant.Foods.Remove(food);
                    _allFoods.Remove(food); // Frissítjük az eredeti listát
                    _filteredFoods.Remove(food); // Frissítjük a szűrt listát
                    FoodCollectionView.ItemsSource = null; // CollectionView frissítése
                    FoodCollectionView.ItemsSource = _filteredFoods;
                    await _restaurantService.DeleteFoodFromRestaurant(_restaurant.RestaurantId, food.FoodId);
                }
            }
        }

        private void OnSearchTextChanged(object sender, TextChangedEventArgs e)
        {
            string searchText = e.NewTextValue?.Trim().ToLower() ?? string.Empty;

            if (string.IsNullOrWhiteSpace(searchText))
            {
                _filteredFoods = _allFoods.ToList(); // Minden étel látható, ha üres a keresés
            }
            else
            {
                _filteredFoods = _allFoods
                    .Where(food => food.Name?.ToLower().Contains(searchText) == true)
                    .ToList(); // Szűrés név alapján
            }

            FoodCollectionView.ItemsSource = null; // CollectionView frissítése
            FoodCollectionView.ItemsSource = _filteredFoods;
        }
    }
}