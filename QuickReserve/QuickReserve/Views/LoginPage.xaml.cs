using QuickReserve.Services;
using QuickReserve.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace QuickReserve.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class LoginPage : ContentPage
    {
        public LoginPage()
        {
            InitializeComponent();
            
            this.BindingContext = new LoginViewModel();
        }

        protected void GoToUserRegisterPage(object sender, EventArgs e)
        {
            App.Current.MainPage = new NavigationPage(new UserRegisterPage());
        }

        protected void GoToRestaurantRegisterPage(object sender, EventArgs e)
        {
            App.Current.MainPage = new NavigationPage(new RestaurantRegisterPage());
        }

        public async void Login(object sender, EventArgs e)
        {
            UserService userService = new UserService();

            if (!string.IsNullOrEmpty(txtUsername.Text.Trim()) && !string.IsNullOrEmpty(txtPassword.Text.Trim()))
            {
                // Ellenőrizzük, hogy a felhasználónevet és a jelszót helyesen adták-e meg
                if (await userService.ValidateUserCredentials(txtUsername.Text.Trim(), txtPassword.Text.Trim()))
                {
                    // Ha a bejelentkezés sikeres
                    await DisplayAlert("LOGIN", "Login successful", "OK");
                    App.Current.MainPage = new NavigationPage(new AboutPage());
                }
                else
                {
                    // Ha a felhasználó nem létezik vagy helytelenek a hitelesítő adatok
                    await DisplayAlert("LOGIN ERROR", "This Username does not exist or incorrect password", "OK");
                }
            }
            else
            {
                // Ha a felhasználó nem adott meg minden szükséges adatot
                await DisplayAlert("LOGIN ERROR", "Please enter both username and password", "OK");
            }
        }

    }
}