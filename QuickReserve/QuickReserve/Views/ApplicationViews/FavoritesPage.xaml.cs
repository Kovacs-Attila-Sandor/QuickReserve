using QuickReserve.Models;
using QuickReserve.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace QuickReserve.Views.ApplicationViews
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class FavoritesPage : ContentPage, INotifyPropertyChanged
    {
        private string _selectedSegment = "Restaurants"; // Kezdésként "Restaurants" legyen aktív
        private UserService _userService;
        private string _userId;

        // Segment color properties
        private Color _restaurantSegmentColor = Color.FromHex("#4CAF50");
        private Color _foodSegmentColor = Color.FromHex("#333333");
        private Color _restaurantTextColor = Color.White;
        private Color _foodTextColor = Color.LightGray;

        public string SelectedSegment
        {
            get => _selectedSegment;
            set
            {
                if (_selectedSegment != value)
                {
                    _selectedSegment = value;
                    Console.WriteLine($"SelectedSegment changed to: {_selectedSegment}");
                    UpdateSegmentColors();
                    OnPropertyChanged(nameof(SelectedSegment));
                    OnPropertyChanged(nameof(RestaurantSegmentColor));
                    OnPropertyChanged(nameof(FoodSegmentColor));
                    OnPropertyChanged(nameof(RestaurantTextColor));
                    OnPropertyChanged(nameof(FoodTextColor));
                    Device.BeginInvokeOnMainThread(LoadContent);
                }
            }
        }

        public Color RestaurantSegmentColor => _restaurantSegmentColor;
        public Color FoodSegmentColor => _foodSegmentColor;
        public Color RestaurantTextColor => _restaurantTextColor;
        public Color FoodTextColor => _foodTextColor;

        public ICommand SegmentSelectedCommand { get; }

        public FavoritesPage(string userId)
        {
            InitializeComponent();
            _userId = userId;
            _userService = UserService.Instance;
            BindingContext = this;

            SegmentSelectedCommand = new Command<string>((segment) =>
            {
                Console.WriteLine($"Segment selected: {segment}");
                SelectedSegment = segment;
            });
        }

        private void UpdateSegmentColors()
        {
            if (SelectedSegment == "Restaurants")
            {
                _restaurantSegmentColor = Color.FromHex("#4CAF50");
                _foodSegmentColor = Color.FromHex("#333333");
                _restaurantTextColor = Color.White;
                _foodTextColor = Color.LightGray;
            }
            else
            {
                _restaurantSegmentColor = Color.FromHex("#333333");
                _foodSegmentColor = Color.FromHex("#4CAF50");
                _restaurantTextColor = Color.LightGray;
                _foodTextColor = Color.White;
            }
            Console.WriteLine($"Colors updated: Restaurants={_restaurantSegmentColor}, Foods={_foodSegmentColor}");
        }

        private void OnRestaurantsButtonClicked(object sender, EventArgs e)
        {
            Console.WriteLine("Restaurants button clicked");
            SelectedSegment = "Restaurants";
        }

        private void OnFoodsButtonClicked(object sender, EventArgs e)
        {
            Console.WriteLine("Foods button clicked");
            SelectedSegment = "Foods";
        }

        private async void LoadContent()
        {
            if (ContentArea == null)
            {
                Console.WriteLine("ContentArea is null");
                return;
            }

            ContentArea.Children.Clear();
            Console.WriteLine($"Loading content for: {SelectedSegment}");

            if (string.IsNullOrEmpty(_userId))
            {
                ContentArea.Children.Add(new Label
                {
                    Text = "User not authenticated",
                    TextColor = Color.White,
                    HorizontalOptions = LayoutOptions.Center,
                    VerticalOptions = LayoutOptions.Center
                });
                return;
            }

            try
            {
                if (SelectedSegment == "Restaurants")
                {
                    var restaurants = await _userService.GetFavoriteRestaurants(_userId);
                    Console.WriteLine($"Restaurants loaded: {(restaurants != null ? restaurants.Count : 0)} items");
                    DisplayRestaurants(restaurants);
                }
                else if (SelectedSegment == "Foods")
                {
                    var foods = await _userService.GetFavoriteFoods(_userId);
                    Console.WriteLine($"Foods loaded: {(foods != null ? foods.Count : 0)} items");
                    DisplayFoods(foods);
                }
            }
            catch (Exception ex)
            {
                ContentArea.Children.Add(new Label
                {
                    Text = "Error loading favorites",
                    TextColor = Color.White,
                    HorizontalOptions = LayoutOptions.Center,
                    VerticalOptions = LayoutOptions.Center
                });
                Console.WriteLine($"Error loading favorites: {ex.Message}");
            }
        }

        private void DisplayRestaurants(List<Restaurant> restaurants)
        {
            if (restaurants == null || restaurants.Count == 0)
            {
                ContentArea.Children.Add(new Label
                {
                    Text = "No favorite restaurants yet",
                    TextColor = Color.White,
                    HorizontalOptions = LayoutOptions.Center,
                    VerticalOptions = LayoutOptions.Center
                });
                Console.WriteLine("No restaurants to display");
                return;
            }

            foreach (var restaurant in restaurants)
            {
                var card = new Frame
                {
                    BackgroundColor = Color.FromHex("#333333"),
                    CornerRadius = 10,
                    Padding = 15,
                    Margin = new Thickness(0, 0, 0, 10)
                };

                var stack = new StackLayout();
                stack.Children.Add(new Label
                {
                    Text = restaurant.Name ?? "Unknown",
                    FontSize = 18,
                    FontAttributes = FontAttributes.Bold,
                    TextColor = Color.White
                });
                stack.Children.Add(new Label
                {
                    Text = restaurant.Address?.ToString() ?? "No address",
                    FontSize = 14,
                    TextColor = Color.LightGray,
                    Margin = new Thickness(0, 5, 0, 0)
                });

                var tapGesture = new TapGestureRecognizer();
                tapGesture.Tapped += async (s, e) =>
                {
                    // await Navigation.PushAsync(new RestaurantDetailsPage(restaurant));
                };
                card.GestureRecognizers.Add(tapGesture);

                card.Content = stack;
                ContentArea.Children.Add(card);
            }
            Console.WriteLine($"Displayed {restaurants.Count} restaurants");
        }

        private void DisplayFoods(List<Food> foods)
        {
            if (foods == null || foods.Count == 0)
            {
                ContentArea.Children.Add(new Label
                {
                    Text = "No favorite foods yet",
                    TextColor = Color.White,
                    HorizontalOptions = LayoutOptions.Center,
                    VerticalOptions = LayoutOptions.Center
                });
                Console.WriteLine("No foods to display");
                return;
            }

            foreach (var food in foods)
            {
                var card = new Frame
                {
                    BackgroundColor = Color.FromHex("#333333"),
                    CornerRadius = 10,
                    Padding = 15,
                    Margin = new Thickness(0, 0, 0, 10)
                };

                var stack = new StackLayout();
                stack.Children.Add(new Label
                {
                    Text = food.Name ?? "Unknown",
                    FontSize = 18,
                    FontAttributes = FontAttributes.Bold,
                    TextColor = Color.White
                });             
                stack.Children.Add(new Label
                {
                    Text = $"Price: {food.Price}",
                    FontSize = 14,
                    TextColor = Color.LightGray,
                    Margin = new Thickness(0, 5, 0, 0)
                });

                var tapGesture = new TapGestureRecognizer();
                tapGesture.Tapped += async (s, e) =>
                {
                    // await Navigation.PushAsync(new FoodDetailsPage(food));
                };
                card.GestureRecognizers.Add(tapGesture);

                card.Content = stack;
                ContentArea.Children.Add(card);
            }
            Console.WriteLine($"Displayed {foods.Count} foods");
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