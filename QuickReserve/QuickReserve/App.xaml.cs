using Firebase.Database;
using QuickReserve.Models;
using QuickReserve.Services;
using QuickReserve.Views;
using System;
using System.Collections.Generic;
using System.Data;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace QuickReserve
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            DependencyService.Register<MockDataStore>();
            MainPage = new NavigationPage(new LoginPage());
        }

        protected override async void OnStart()
        {
           
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
