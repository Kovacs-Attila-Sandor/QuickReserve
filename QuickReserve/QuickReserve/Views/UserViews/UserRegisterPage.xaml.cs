using System;
using QuickReserve.Services;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Firebase.Auth;
using System.Threading.Tasks;

namespace QuickReserve.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class UserRegisterPage : ContentPage
    {
        private readonly FirebaseAuthService _authService;
        private readonly UserService _userService;

        public UserRegisterPage()
        {
            InitializeComponent();

            _authService = FirebaseAuthService.Instance;
            _userService = UserService.Instance;
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

            LoadingOverlay.IsVisible = true;
            MainPage.IsEnabled = false;

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

                User user = new User()
                {
                    Name = txtUsername.Text.Trim(),
                    Email = txtEmail.Text.Trim(),
                    PhoneNumber = txtPhonenum.Text.Trim(),
                    Role = "USER"
                };

                await _userService.AddUser(user);

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
            finally
            {
                LoadingOverlay.IsVisible = false;
                MainPage.IsEnabled = true;
            }
        }

        private void TogglePasswordVisibility(object sender, EventArgs e)
        {
            txtPassword.IsPassword = !txtPassword.IsPassword;
            imgTogglePassword.Source = txtPassword.IsPassword ? "eye_closed.png" : "eye_open.png";
        }

        private void ToggleConfirmPasswordVisibility(object sender, EventArgs e)
        {
            txtConfirmPassword.IsPassword = !txtConfirmPassword.IsPassword;
            imgToggleConfirmPassword.Source = txtConfirmPassword.IsPassword ? "eye_closed.png" : "eye_open.png";
        }

        private void OnTextChanged(object sender, TextChangedEventArgs e)
        {
            if (sender is Entry entry)
            {
                Label placeholderLabel = GetPlaceholderLabel(entry);

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
                    entry.Placeholder = GetPlaceholderText(entry);
                }
            }
        }

        private void OnFocused(object sender, FocusEventArgs e)
        {
            if (sender is Entry entry)
            {
                Label placeholderLabel = GetPlaceholderLabel(entry);

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
                Label placeholderLabel = GetPlaceholderLabel(entry);

                if (string.IsNullOrWhiteSpace(entry.Text))
                {
                    placeholderLabel.FadeTo(0, 150);
                    placeholderLabel.IsVisible = false;
                    entry.Placeholder = GetPlaceholderText(entry);
                }
            }
        }

        private Label GetPlaceholderLabel(Entry entry)
        {
            return entry == txtUsername ? lblUsernamePlaceholder :
                   entry == txtEmail ? lblEmailPlaceholder :
                   entry == txtPassword ? lblPasswordPlaceholder :
                   entry == txtConfirmPassword ? lblConfirmPasswordPlaceholder :
                   entry == txtPhonenum ? lblPhonePlaceholder : null;
        }

        private string GetPlaceholderText(Entry entry)
        {
            return entry == txtUsername ? "Username" :
                   entry == txtEmail ? "Email" :
                   entry == txtPassword ? "Password" :
                   entry == txtConfirmPassword ? "Confirm Password" :
                   entry == txtPhonenum ? "Phone Number" : "";
        }

    }
} 