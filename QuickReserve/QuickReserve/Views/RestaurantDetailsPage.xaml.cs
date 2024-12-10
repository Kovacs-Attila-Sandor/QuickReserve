using Xamarin.Forms;
using QuickReserve.Models;
using System;

namespace QuickReserve.Views
{
    public partial class RestaurantDetailsPage : ContentPage
    {
        public RestaurantDetailsPage(Restaurant selectedRestaurant)
        {
            InitializeComponent();
            BindingContext = selectedRestaurant; // Bind restaurant details to the page
        }
        protected void GoToAboutpage(object sender, EventArgs e)
        {
            App.Current.MainPage = new NavigationPage(new AboutPage());
        }

        protected void GoToRestaurantLayoutpage(object sender, EventArgs e)
        {
            App.Current.MainPage = new NavigationPage(new RestaurantLayoutPage());
        }
    }
}
