using System;
using QuickReserve.Services;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Firebase.Auth;

namespace QuickReserve.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class UserRegisterPage : ContentPage
    {
        private readonly FirebaseAuthService _authService;

        public UserRegisterPage()
        {
            InitializeComponent();

            _authService = new FirebaseAuthService();
        }

        protected void GoToLoginPage(object sender, EventArgs e)
        {
            App.Current.MainPage = new NavigationPage(new LoginPage());
        }


        protected void GoToRestaurantRegisterPage(object sender, EventArgs e)
        {
            App.Current.MainPage = new NavigationPage(new RestaurantRegisterPage());
        }

        private async void RegisterAsGuest(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(txtEmail.Text) ||
                    string.IsNullOrWhiteSpace(txtPassword.Text) ||
                    string.IsNullOrWhiteSpace(txtConfirmPassword.Text))
                {
                    await DisplayAlert("Error", "Please fill in all fields.", "OK");
                    return;
                }

                if (txtPassword.Text != txtConfirmPassword.Text)
                {
                    await DisplayAlert("Error", "Passwords do not match.", "OK");
                    return;
                }

                // Felhasználó létrehozása Firebase-ben
                var userCredential = await _authService.AuthClient.CreateUserWithEmailAndPasswordAsync(
                    txtEmail.Text.Trim(), txtPassword.Text.Trim());

                await DisplayAlert("Success", "User registered successfully!", "OK");

                // Navigálás a bejelentkezési oldalra
                App.Current.MainPage = new NavigationPage(new LoginPage());
            }
            catch (FirebaseAuthException ex)
            {
                await DisplayAlert("Registration Failed", $"Error: {ex.Reason}", "OK");
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"An unexpected error occurred: {ex.Message}", "OK");
            }
        }
    }
}