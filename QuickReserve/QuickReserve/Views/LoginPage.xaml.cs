using QuickReserve.Services;
using QuickReserve.ViewModels;
using System;
using System.Net.Http;
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

        protected void GoToAboutPage(object sender, EventArgs e)
        {
            App.Current.MainPage = new NavigationPage(new AboutPage());
        }

        public async void Login(object sender, EventArgs e)
        {
            UserService userService = new UserService();

            // Ellenőrizzük, hogy a felhasználónév és a jelszó nem üres
            if (string.IsNullOrWhiteSpace(txtUsername.Text) || string.IsNullOrWhiteSpace(txtPassword.Text))
            {
                await DisplayAlert("LOGIN ERROR", "Please enter both username and password", "OK");
                return;
            }

            try
            {
                // Felhasználói hitelesítő adatok ellenőrzése
                bool isValidUser = await userService.ValidateUserCredentials(txtUsername.Text.Trim(), txtPassword.Text.Trim());

                if (isValidUser)
                {
                    // A sikeres bejelentkezés után elmenthetjük a felhasználó nevét
                    App.Current.Properties["LoggedInUserName"] = txtUsername.Text;

                    App.Current.MainPage = new NavigationPage(new AboutPage());                  
                }
                else
                {
                    // Hibás hitelesítő adatok
                    await DisplayAlert("LOGIN ERROR", "This Username does not exist or incorrect password", "OK");
                }
            }
            catch (TimeoutException)
            {
                await DisplayAlert("LOGIN ERROR", "The login process timed out. Please try again later.", "OK");
            }
            catch (HttpRequestException)
            {
                await DisplayAlert("LOGIN ERROR", "A network error occurred. Please try again later.", "OK");
            }
            catch (Exception ex)
            {
                // Általános hiba
                await DisplayAlert("LOGIN ERROR", "An unexpected error occurred: " + ex.Message, "OK");

                // Ha van naplózási mechanizmus, itt használhatjuk
                Console.WriteLine($"Login error: {ex}");
            }
        }

    }
}
