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
        public User User { get; set; }

        public UserProfilePage(string userName)
        {
            InitializeComponent();
            LoadUserData(userName);
        }

        private async void LoadUserData(string userName)
        {
            var userService = new UserService();
            User = await userService.GetUserByName(userName);

            if (User != null)
            {
                // Ha a felhasználó nem rendelkezik profilképpel, használjuk a placeholder képet
                if (string.IsNullOrEmpty(User.ProfileImage))
                {
                    User.ProfileImageSource = ImageSource.FromFile("placeholder.jpg");
                }
                else
                {
                    // Ha van profilkép, azt használjuk
                    User.ProfileImageSource = ImageSource.FromStream(() =>
                        new MemoryStream(Convert.FromBase64String(User.ProfileImage)));
                }

                
                BindingContext = this; // Adatok megjelenítése
            }
            else
            {
                await DisplayAlert("Error", "User not found", "OK");
            }   
        }

        private async void OnBackToAboutClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new AboutPage());
        }
    }
}
