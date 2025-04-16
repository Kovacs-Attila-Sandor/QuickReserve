using QuickReserve.Services;
using QuickReserve.Views;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace QuickReserve
{
    public partial class App : Application
    {
        UserService userService = UserService.Instance;

        public App()
        {
            InitializeComponent();
            MainPage = new SplashPage();

            // Aszinkron inicializáció indítása
            InitializeAppAsync();
        }

        private async void InitializeAppAsync()
        {
            await CheckUserLoginStatusAsync();
        }

        private async Task CheckUserLoginStatusAsync()
        {
            if (Preferences.ContainsKey("userId"))
            {
                string userId = Preferences.Get("userId", null);
                try
                {
                    string userType = await userService.GetUserTypeByUserId(userId);
                    MainPage = new NavigationPage(new MainPage(userType));
                }
                catch (Exception ex)
                {
                    MainPage = new NavigationPage(new LoginPage());
                    await MainPage.DisplayAlert("Error", $"Failed to load user data: {ex.Message}", "OK");
                }
            }
            else
            {
                MainPage = new NavigationPage(new LoginPage());
            }
        }

        protected override void OnStart()
        {
            // Üresen hagyható, mert az inicializáció a konstruktorban történik
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}