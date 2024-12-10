using Xamarin.Forms;
using QuickReserve.Models;
using QuickReserve.Converter;
using System.Collections.Generic;

namespace QuickReserve.Views
{
    public partial class RestaurantDetailsPage : ContentPage
    {
        public RestaurantDetailsPage(Restaurant selectedRestaurant)
        {
            InitializeComponent();

            // Convert images for the selected restaurant
            selectedRestaurant.ImageSourceList = new List<ImageSource>();
            if (selectedRestaurant.ImageBase64List != null)
            {
                foreach (var base64Image in selectedRestaurant.ImageBase64List)
                {
                    selectedRestaurant.ImageSourceList.Add(ImageConverter.ConvertBase64ToImageSource(base64Image));
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
    }
}
