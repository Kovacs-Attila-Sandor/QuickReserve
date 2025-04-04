using Firebase.Auth;
using Firebase.Auth.Providers;
using QuickReserve.Services;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace QuickReserve.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class LoginPage : ContentPage
    {
        private readonly FirebaseAuthService _authService;

        private readonly UserService _userService;
        private readonly RestaurantService _restaurantService;

        public LoginPage()
        {
            InitializeComponent();

            _authService = FirebaseAuthService.Instance;
            _userService = UserService.Instance;
            _restaurantService = RestaurantService.Instance;
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

        public async void Login(object sender, EventArgs e)
        {
            LoadingOverlay.IsVisible = true;
            MainPage.IsEnabled = false;

            try
            {
                if (string.IsNullOrWhiteSpace(txtEmail.Text) || string.IsNullOrWhiteSpace(txtPassword.Text))
                {
                    await DisplayAlert("Error", "Please fill in all fields.", "OK");
                    return;
                }

                var userCredential = await _authService.AuthClient.SignInWithEmailAndPasswordAsync(
                    txtEmail.Text.Trim(), txtPassword.Text.Trim());

                var  (userId, userType) = await _userService.GetUserIdAndUserTypeByEmail(txtEmail.Text);
                

                if (!string.IsNullOrEmpty(userId))
                {
                    if (chkRememberMe.IsChecked)
                    {
                        Preferences.Set("userId", userId);
                        Preferences.Set("userStatus", userType);
                        Preferences.Set("userEmail", txtEmail.Text.Trim());
                    }
                    App.Current.Properties["userId"] = userId;
                    App.Current.Properties["userType"] = userType;


                    // Sikeres bejelentkezés → főoldalra irányítás
                    try
                    {
                        App.Current.MainPage = new NavigationPage(new MainPage(userType));
                    }
                    catch (Exception ex)
                    {
                        await DisplayAlert("Error", ex.Message, "OK");
                    }
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
                MainPage.IsEnabled = true;
            }
        }

        private void OnTextChanged(object sender, TextChangedEventArgs e)
        {
            if (sender is Entry entry)
            {
                Label placeholderLabel = entry == txtEmail ? lblEmailPlaceholder : lblPasswordPlaceholder;

                if (!string.IsNullOrWhiteSpace(entry.Text))
                {
                    placeholderLabel.IsVisible = true;
                    placeholderLabel.FadeTo(1, 160);
                    placeholderLabel.TranslateTo(2, -3, 170);
                    entry.Placeholder = "";
                }
                else
                {
                    placeholderLabel.FadeTo(0, 150, Easing.Linear);
                    placeholderLabel.IsVisible = false;
                    entry.Placeholder = entry == txtEmail ? "Email" : "Password";
                }
            }
        }

        private void OnFocused(object sender, FocusEventArgs e)
        {
            if (sender is Entry entry)
            {
                Label placeholderLabel = entry == txtEmail ? lblEmailPlaceholder : lblPasswordPlaceholder;

                if (!string.IsNullOrWhiteSpace(entry.Text))
                {
                    placeholderLabel.IsVisible = true;
                    placeholderLabel.FadeTo(1, 150);
                    placeholderLabel.TranslateTo(0, 0, 150);
                }
            }
        }

        private void OnUnfocused(object sender, FocusEventArgs e)
        {
            if (sender is Entry entry)
            {
                Label placeholderLabel = entry == txtEmail ? lblEmailPlaceholder : lblPasswordPlaceholder;

                if (string.IsNullOrWhiteSpace(entry.Text))
                {
                    placeholderLabel.FadeTo(0, 150);
                    placeholderLabel.IsVisible = false;
                    entry.Placeholder = entry == txtEmail ? "Email" : "Password";
                }
            }
        }
    }
}
