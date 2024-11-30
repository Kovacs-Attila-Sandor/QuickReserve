using System;
using System.ComponentModel;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace QuickReserve.Views
{
    public partial class AboutPage : ContentPage
    {
        public AboutPage()
        {
            InitializeComponent();
        }

        protected void GoToProfilePage(object sender, EventArgs e)
        {
            App.Current.MainPage = new NavigationPage(new ProfilePage());
        }
    }
}