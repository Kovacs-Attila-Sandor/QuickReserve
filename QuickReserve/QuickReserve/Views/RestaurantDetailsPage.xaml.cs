using Xamarin.Forms;
using QuickReserve.Models;

namespace QuickReserve.Views
{
    public partial class RestaurantDetailsPage : ContentPage
    {
        public RestaurantDetailsPage(Restaurant selectedRestaurant)
        {
            InitializeComponent();
            BindingContext = selectedRestaurant; // Bind restaurant details to the page
        }
    }
}
