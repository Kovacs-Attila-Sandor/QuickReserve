using Firebase.Database;
using QuickReserve.Models;
using QuickReserve.Services;
using QuickReserve.Views;
using System;
using System.Collections.Generic;
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
            UserService userService = new UserService();
            // Lekérdezés a Firebase adatbázisból
            User fetchedUser = await userService.GetUserById(1);  // Az ID alapján kérdezzük le

            if (fetchedUser != null)
            {
                Console.WriteLine($"User fetched successfully: {fetchedUser.Name}, {fetchedUser.Email}, {fetchedUser.Password}");
            }
            else
            {
                Console.WriteLine("Failed to fetch user.");
            }
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
