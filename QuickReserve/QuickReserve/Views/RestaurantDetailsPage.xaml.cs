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
        protected void GoToAboupage(object sender, EventArgs e)
        {
            App.Current.MainPage = new NavigationPage(new AboutPage());
        }
    }
}
