using Xamarin.Forms;
using QuickReserve.Models;
using QuickReserve.Services;
using System;

namespace QuickReserve.Views
{
    public partial class EditUserProfilePage : ContentPage
    {
        public User user { get; set; }
        private UserService userService;

        public EditUserProfilePage(User user)
        {
            InitializeComponent();
            this.user = user;  // Hozzárendeljük a paramétert az osztály mezőjéhez
            userService = UserService.Instance;  // Feltételezzük, hogy a UserService kezeli az adatokat
            BindingContext = this.user;  // A BindingContext-ot a user objektumra állítjuk be
        }

        // Save Changes Button Clicked
        private async void OnSaveChangesClicked(object sender, EventArgs e)
        {
            User newUser = new User() { 
                UserId = user.UserId,
                PhoneNumber = PhoneNumberEntry.Text.Trim(),
                Email = EmailEntry.Text.Trim(),
                Name = NameEntry.Text.Trim(),
                Role = user.Role,
                CreatedAt = user.CreatedAt,
                ProfileImage = user.ProfileImage,
                ProfileImageSource = user.ProfileImageSource
            };

            bool success = await userService.UpdateUserProfile(user.UserId, newUser);

            if (success)
            {
                await DisplayAlert("Success", "Your profile has been updated.", "OK");
                App.Current.MainPage = new NavigationPage(new MainPage(App.Current.Properties["userType"].ToString()));
            }
            else
            {
                await DisplayAlert("Error", "Failed to update your profile. Please try again later.", "OK");               
            }
        }
    }
}