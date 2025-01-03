using System;
using Xamarin.Forms;
using QuickReserve.Services;
using QuickReserve.Models;
using QuickReserve.Converter;
using System.IO;
using System.Reflection;

namespace QuickReserve.Views
{
    public partial class UserProfilePage : ContentPage
    {
        public User user { get; set; }

        public UserProfilePage(string userName)
        {
            InitializeComponent();
            LoadUserData(userName);
        }

        private async void LoadUserData(string userName)
        {
            var userService = new UserService();
            user = await userService.GetUserByName(userName);

            if (user != null)
            {
                // If the user doesn't have a profile image, use the placeholder image
                if (string.IsNullOrEmpty(user.ProfileImage))
                {
                    user.ProfileImageSource = ImageSource.FromFile("placeholder.jpg");
                }
                else
                {
                    // If the user has a profile image, use it
                    user.ProfileImageSource = ImageSource.FromStream(() =>
                        new MemoryStream(Convert.FromBase64String(user.ProfileImage)));
                }

                BindingContext = this; // Set data binding
            }
            else
            {
                await DisplayAlert("Error", "User not found", "OK");
            }
        }

        // Edit profile button click handler
        private async void OnEditProfileClicked(object sender, EventArgs e)
        {
            // Navigate to the edit profile page
            await Navigation.PushAsync(new EditUserProfilePage(user));
        }

        private void OnLogoutClicked(object sender, EventArgs e)
        {
            App.Current.MainPage = new LoginPage();
        }
    }
}