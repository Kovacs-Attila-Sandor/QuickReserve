using QuickReserve.Models;
using QuickReserve.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace QuickReserve.Views.ApplicationViews
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class FavoritesPage : ContentPage, INotifyPropertyChanged
    {
        private UserService _userService;
        private string _userId;
        private List<Food> _favoriteFoods;
        private bool _isLoading;

        public List<Food> FavoriteFoods
        {
            get => _favoriteFoods;
            set
            {
                _favoriteFoods = value;
                OnPropertyChanged(nameof(FavoriteFoods));
            }
        }

        public bool IsLoading
        {
            get => _isLoading;
            set
            {
                _isLoading = value;
                OnPropertyChanged(nameof(IsLoading));
            }
        }

        public FavoritesPage(string userId)
        {
            InitializeComponent();
            _userId = userId;
            _userService = UserService.Instance;
            BindingContext = this;
        }

        private async void LoadContent()
        {
            IsLoading = true;

            if (string.IsNullOrEmpty(_userId))
            {
                FavoriteFoods = null;
                Console.WriteLine("User not authenticated");
                IsLoading = false;
                return;
            }

            try
            {
                var foods = await _userService.GetFavoriteFoods(_userId);
                Console.WriteLine($"Foods loaded: {(foods != null ? foods.Count : 0)} items");
                FavoriteFoods = foods ?? new List<Food>();
            }
            catch (Exception ex)
            {
                FavoriteFoods = null;
                Console.WriteLine($"Error loading favorite foods: {ex.Message}");
            }
            finally
            {
                IsLoading = false;
            }
        }

        private async void OnFoodTapped(object sender, EventArgs e)
        {
            if (sender is Frame frame && frame.BindingContext is Food food)
            {
                await Navigation.PushAsync(new FoodPage(food, food.RestaurantId, true));
            }
        }

        private async void OnBackClicked(object sender, EventArgs e)
        {
            await Navigation.PopAsync();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            Device.BeginInvokeOnMainThread(LoadContent);
        }
    }
}