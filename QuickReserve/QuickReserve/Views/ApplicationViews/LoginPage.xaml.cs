using Firebase.Auth;
using Firebase.Auth.Providers;
using QuickReserve.Services;
using System;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace QuickReserve.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class LoginPage : ContentPage
    {
        public string WEB_API_KEY = "AIzaSyBrYyBcrfzwaWYVU1XbUG7CZ660XTwSmyU";

        private readonly FirebaseAuthService _authService;

        private UserService _userService = new UserService();

        public LoginPage()
        {
            InitializeComponent();

            _authService = new FirebaseAuthService();
            _userService = new UserService();

            // Ellenőrizzük, hogy a felhasználó be van-e jelentkezve
            CheckLoggedInUser();
        }

        protected void GoToUserRegisterPage(object sender, EventArgs e)
        {
            App.Current.MainPage = new NavigationPage(new UserRegisterPage());
        }

        protected void GoToRestaurantRegisterPage(object sender, EventArgs e)
        {
            App.Current.MainPage = new NavigationPage(new RestaurantRegisterPage());
        }
        private void TogglePasswordVisibility(object sender, EventArgs e)
        {
            txtPassword.IsPassword = !txtPassword.IsPassword;
            imgTogglePassword.Source = txtPassword.IsPassword ? "eye_closed.png" : "eye_open.png";
        }

        private void CheckLoggedInUser()
        {
            if (Preferences.ContainsKey("userId"))
            {
                string userId = Preferences.Get("userId", null);
                string userEmail = Preferences.Get("userEmail", null);

                if (!string.IsNullOrEmpty(userId))
                {
                    Device.BeginInvokeOnMainThread(() =>
                    {
                        App.Current.MainPage = new NavigationPage(new AboutPage());
                    });
                }      
            }        
        }

        public async void Login(object sender, EventArgs e)
        {
            LoadingOverlay.IsVisible = true;
            MainGrid.IsEnabled = false;

            try
            {
                if (string.IsNullOrWhiteSpace(txtEmail.Text) || string.IsNullOrWhiteSpace(txtPassword.Text))
                {
                    await DisplayAlert("Error", "Please fill in all fields.", "OK");
                    return;
                }

                var userCredential = await _authService.AuthClient.SignInWithEmailAndPasswordAsync(
                    txtEmail.Text.Trim(), txtPassword.Text.Trim());

                string userID = await _userService.GetUserIdByEmail(txtEmail.Text);

                if (!string.IsNullOrEmpty(userID))
                {
                    if (chkRememberMe.IsChecked)
                    {
                        Preferences.Set("userId", userID);
                        Preferences.Set("userEmail", txtEmail.Text.Trim());
                    }

                    // Sikeres bejelentkezés → főoldalra irányítás
                    App.Current.MainPage = new NavigationPage(new AboutPage());
                }
                else
                {
                    await DisplayAlert("Error", "User not found.", "OK");
                }
            }
            catch (FirebaseAuthException ex)
            {
                await DisplayAlert("Login Failed", $"Error: {ex.Reason}", "OK");
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"An unexpected error occurred: {ex.Message}", "OK");
            }
            finally
            {
                LoadingOverlay.IsVisible = false;
                MainGrid.IsEnabled = true;
            }
        }
    }
}
