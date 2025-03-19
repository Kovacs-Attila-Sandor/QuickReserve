using Xamarin.Forms;
using QuickReserve.Models;
using System;
using System.Collections.Generic;
using QuickReserve.Converter;

namespace QuickReserve.Views
{
    public partial class RestaurantDetailsPage : ContentPage
    {
        Restaurant restaurant;

        public RestaurantDetailsPage(Restaurant selectedRestaurant)
        {
            InitializeComponent();
            restaurant = new Restaurant();
            restaurant = selectedRestaurant;

            Application.Current.Properties["restaurantId"] = restaurant.RestaurantId;

            // Convert images for the selected restaurant
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

            BindingContext = selectedRestaurant;
        }

        private int _currentIndex = 0;

        private void OnCarouselItemChanged(object sender, CurrentItemChangedEventArgs e)
        {
            var carousel = (CarouselView)sender;
            var newIndex = carousel.Position;

            // Ha a felhasználó vissza akarna lépni, állítsd vissza az előző pozícióra
            if (newIndex < _currentIndex)
            {
                carousel.Position = _currentIndex;
            }
            else
            {
                // Frissítsd a jelenlegi indexet, ha előre lép
                _currentIndex = newIndex;
            }
        }
        protected async void GoToAboutpage(object sender, EventArgs e)
        {
            await Navigation.PopAsync();
        }

        protected async void GoToRestaurantLayoutpage(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new RestaurantLayoutPage(restaurant.RestaurantId));
        }

        protected async void GoToMenuPage(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new RestaurantMenuPage(restaurant));
        }

        private async void OnDatePickerClicked(object sender, EventArgs e)
        {
            DatePicker datePicker = new DatePicker
            {
                Format = "D",
                MinimumDate = DateTime.Today,
                MaximumDate = DateTime.Today.AddMonths(6)
            };

            datePicker.DateSelected += (s, args) =>
            {
                DisplayAlert("Selected Date", args.NewDate.ToString("D"), "OK");
            };

            await Navigation.PushModalAsync(new ContentPage
            {
                Content = datePicker
            });
        }

        private void OnHeartClicked(object sender, EventArgs e)
        {
            var button = sender as ImageButton; // ImageButton-ra konvertáljuk, nem Image-re
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

    }
}