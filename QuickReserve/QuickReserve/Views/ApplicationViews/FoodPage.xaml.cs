using QuickReserve.Models;
using QuickReserve.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace QuickReserve.Views.ApplicationViews
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class FoodPage : ContentPage
    {
        private Food _food;
        private string _currentUserId;
        private string _currentRestaurantId;
        private int _selectedRating = 0;
        private RestaurantService _restaurantService;
        private UserService _userService; // Hozzáadva a UserService
        private bool _isFavorited; // Kedvenc állapot követése

        public FoodPage(Food food, string currentRestaurantId)
        {
            InitializeComponent();

            _food = food;
            _currentUserId = Application.Current.Properties["userId"].ToString();
            _currentRestaurantId = currentRestaurantId;
            _restaurantService = RestaurantService.Instance;
            _userService = UserService.Instance; // Inicializálás

            // Ha van kép, konvertáljuk ImageSource-ra
            if (!string.IsNullOrEmpty(food.Picture))
            {
                food.ImageSource = ImageSource.FromStream(() =>
                    new System.IO.MemoryStream(Convert.FromBase64String(food.Picture)));
            }

            BindingContext = food;

            // Inicializáljuk a csillagokat és a gomb szövegét
            InitializeRating();

            // Kedvenc állapot ellenőrzése és inicializálása
            Task.Run(async () => await InitializeFavoriteStatus()).Wait(); // Szinkron várakozás az inicializálásra
        }

        private async Task InitializeFavoriteStatus()
        {
            // Lekérjük a felhasználó adatait, hogy ellenőrizzük a kedvenceket
            var user = await _userService.GetUserById(_currentUserId);
            if (user != null && user.LikedFoods != null)
            {
                _isFavorited = user.LikedFoods.Contains(_food.FoodId);
            }
            else
            {
                _isFavorited = false;
            }

            // Szív ikon beállítása az állapot alapján
            heartButton.Source = _isFavorited
                ? ImageSource.FromFile("heart_filled_icon.png")
                : ImageSource.FromFile("heart_not_filled_icon.png");
        }

        private void InitializeRating()
        {
            var existingRating = _food.Ratings.FirstOrDefault(r => r.UserId == _currentUserId);
            if (existingRating != null)
            {
                _selectedRating = (int)existingRating.Value;
                submitButton.Text = "Update Rating";
            }
            else
            {
                _selectedRating = 0;
                submitButton.Text = "Submit Rating";
            }
            UpdateStars();
        }

        private void SetupDescription()
        {
            var fullText = _food.Description ?? "";
            var lineCount = GetLineCount(fullText, descriptionLabel.Width, descriptionLabel.FontSize);

            if (lineCount > 3)
            {
                var shortText = TrimToApproximateLines(fullText, descriptionLabel.Width, descriptionLabel.FontSize, 3);
                descriptionLabel.FormattedText = new FormattedString
                {
                    Spans =
                    {
                        new Span { Text = shortText + "... " },
                        new Span { Text = "Read more", ForegroundColor = Color.FromHex("#FFD700"), FontAttributes = FontAttributes.Bold, FontSize = 18 }
                    }
                };
                descriptionLabel.IsEnabled = true;
            }
            else
            {
                descriptionLabel.Text = fullText;
                descriptionLabel.IsEnabled = false;
            }
        }

        private void OnReadMoreTapped(object sender, EventArgs e)
        {
            descriptionLabel.IsVisible = false;
            fullDescriptionLabel.IsVisible = true;
        }

        private int GetLineCount(string text, double width, double fontSize)
        {
            if (string.IsNullOrEmpty(text) || width <= 0) return 0;
            var approxCharWidth = fontSize * 0.6;
            var charsPerLine = width / approxCharWidth;
            var totalLines = (int)Math.Ceiling(text.Length / charsPerLine);
            return totalLines;
        }

        private string TrimToApproximateLines(string text, double width, double fontSize, int maxLines)
        {
            if (string.IsNullOrEmpty(text)) return "";
            var approxCharWidth = fontSize * 0.55;
            var charsPerLine = (int)(width / approxCharWidth);
            var maxChars = charsPerLine * maxLines;
            return text.Length <= maxChars ? text : text.Substring(0, maxChars - 3);
        }

        protected override void OnSizeAllocated(double width, double height)
        {
            base.OnSizeAllocated(width, height);
            SetupDescription();
        }

        private void UpdateStars()
        {
            star1.Source = _selectedRating >= 1 ? "star_filled.png" : "star_empty.png";
            star2.Source = _selectedRating >= 2 ? "star_filled.png" : "star_empty.png";
            star3.Source = _selectedRating >= 3 ? "star_filled.png" : "star_empty.png";
            star4.Source = _selectedRating >= 4 ? "star_filled.png" : "star_empty.png";
            star5.Source = _selectedRating >= 5 ? "star_filled.png" : "star_empty.png";
        }

        private async void OnBackButtonClicked(object sender, EventArgs e)
        {
            await Navigation.PopAsync();
        }

        private async void OnHeartButtonClicked(object sender, EventArgs e)
        {
            var button = sender as ImageButton;
            if (button != null)
            {
                _isFavorited = !_isFavorited; // Állapot váltása

                if (_isFavorited)
                {
                    // Hozzáadás a kedvencekhez
                    var success = await _userService.AddFavoriteFood(_currentUserId, _food.FoodId);
                    if (success)
                    {
                        button.Source = ImageSource.FromFile("heart_filled_icon.png");
                    }
                    else
                    {
                        _isFavorited = false; // Visszaállítás, ha sikertelen
                        await DisplayAlert("Error", "Failed to add to favorites.", "OK");
                    }
                }
                else
                {
                    // Eltávolítás a kedvencekből
                    var success = await _userService.RemoveFavoriteFood(_currentUserId, _food.FoodId);
                    if (success)
                    {
                        button.Source = ImageSource.FromFile("heart_not_filled_icon.png");
                    }
                    else
                    {
                        _isFavorited = true; // Visszaállítás, ha sikertelen
                        await DisplayAlert("Error", "Failed to remove from favorites.", "OK");
                    }
                }
            }
        }

        private void OnStarClicked(object sender, EventArgs e)
        {
            if (sender is ImageButton starButton && int.TryParse(starButton.CommandParameter.ToString(), out int rating))
            {
                _selectedRating = rating;
                UpdateStars();
            }
        }

        private async void OnSubmitRatingClicked(object sender, EventArgs e)
        {
            if (_selectedRating < 1 || _selectedRating > 5)
            {
                await DisplayAlert("Error", "Please select a rating by clicking the stars (1-5).", "OK");
                return;
            }

            var existingRating = _food.Ratings.FirstOrDefault(r => r.UserId == _currentUserId);
            if (existingRating != null)
            {
                existingRating.Value = _selectedRating;
            }
            else
            {
                _food.Ratings.Add(new Rating
                {
                    UserId = _currentUserId,
                    Value = _selectedRating
                });
            }

            await _restaurantService.UpdateFoodDetails(_currentRestaurantId, _food.FoodId, _food);
            BindingContext = null;
            BindingContext = _food;
            submitButton.Text = "Update Rating";
        }
    }

    // Konverterek
    public class BoolToAvailabilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return (bool)value ? "Availability: In Stock" : "Availability: Out of Stock";
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class ListToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is List<string> list && list != null && list.Any())
            {
                return string.Join(" | ", list);
            }
            return "N/A";
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class PriceConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is double price)
            {
                return $"Price: {price:F2} €";
            }
            return "Price: N/A";
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class PreparationTimeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is int time)
            {
                return $"Preparation Time: {time} minutes";
            }
            return "Preparation Time: N/A";
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class RatingConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is double rating)
            {
                return $"{rating.ToString("F1", System.Globalization.CultureInfo.InvariantCulture)}";
            }
            return "Rating: N/A";
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }       
    } 
}