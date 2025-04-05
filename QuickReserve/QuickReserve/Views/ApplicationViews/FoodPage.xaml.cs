using QuickReserve.Models;
using QuickReserve.Services;
using System;
using System.Collections.Generic;
using System.Linq;
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

        public FoodPage(Food food, string currentRestaurantId)
        {
            InitializeComponent();

            _food = food;
            _currentUserId = Application.Current.Properties["userId"].ToString(); ;         
            _currentRestaurantId = currentRestaurantId;
            _restaurantService = RestaurantService.Instance;

            // Ha van kép, konvertáljuk ImageSource-ra
            if (!string.IsNullOrEmpty(food.Picture))
            {
                food.ImageSource = ImageSource.FromStream(() =>
                    new System.IO.MemoryStream(Convert.FromBase64String(food.Picture)));
            }

            BindingContext = food;

            // Inicializáljuk a csillagokat és a gomb szövegét
            InitializeRating();
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

            // Csillagok inicializálása
            UpdateStars();
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
                // Módosítás: meglévő értékelés frissítése
                existingRating.Value = _selectedRating;
            }
            else
            {
                // Új értékelés hozzáadása
                _food.Ratings.Add(new Rating
                {
                    UserId = _currentUserId,
                    Value = _selectedRating
                });
            }

            await _restaurantService.UpdateFoodDetails(_currentRestaurantId, _food.FoodId, _food);

            // Frissítjük a BindingContext-et és a gomb szövegét
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
                return string.Join(", ", list);
            }
            return "N/A";
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class DictToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is Dictionary<string, double> dict && dict != null && dict.Any())
            {
                return string.Join(", ", dict.Select(kv => $"{kv.Key}: {kv.Value}"));
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
                return $"Rating: {rating.ToString("F1", System.Globalization.CultureInfo.InvariantCulture)}/5";
            }
            return "Rating: N/A";
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}