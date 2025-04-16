using System;
using Xamarin.Forms;
using QuickReserve.Services;
using Plugin.Media.Abstractions;
using Plugin.Media;
using System.IO;
using System.Reflection;
using Xamarin.Essentials;
using QuickReserve.Views.ApplicationViews;
using Firebase.Auth;

namespace QuickReserve.Views
{
    public partial class UserProfilePage : ContentPage
    {
        private User _user; // Privát mező a user objektum tárolására

        public UserProfilePage()
        {
            InitializeComponent();
            LoadUserData();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            LoadUserData(); // Frissíti az adatokat PopAsync után
        }

        private async void LoadUserData()
        {
            var userService = UserService.Instance;
            string userID = App.Current.Properties["userId"].ToString();
            _user = await userService.GetUserById(userID);

            if (_user != null)
            {
                // Ha a felhasználónak nincs profilképe, használjuk a placeholder képet
                if (string.IsNullOrEmpty(_user.ProfileImage))
                {
                    _user.ProfileImage = Convert.ToBase64String(GetPlaceholderImageBytes());
                }
                else
                {
                    _user.ProfileImageSource = ImageSource.FromStream(() =>
                        new MemoryStream(Convert.FromBase64String(_user.ProfileImage)));
                }

                // A BindingContext-et közvetlenül a _user objektumra állítjuk
                BindingContext = _user;
            }
            else
            {
                await DisplayAlert("Error", "User not found", "OK");
            }
        }

        // Placeholder kép beolvasása az erőforrásokból
        private byte[] GetPlaceholderImageBytes()
        {
            var assembly = typeof(UserProfilePage).GetTypeInfo().Assembly;
            using (var stream = assembly.GetManifestResourceStream("QuickReserve.Resources.placeholder.jpg"))
            {
                if (stream == null)
                    return null;

                using (var memoryStream = new MemoryStream())
                {
                    stream.CopyTo(memoryStream);
                    return memoryStream.ToArray();
                }
            }
        }

        private async void OnEditProfileClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new EditUserProfilePage(_user));
        }

        private async void OnFavoriteFoodsClicked(object sender, EventArgs e)
        {
            // Javítsd ki, hogy a FavoriteFoodsPage-re navigáljon, ne az EditUserProfilePage-re
            await Navigation.PushAsync(new FavoritesPage(_user.UserId));
        }

        private async void OnLogoutClicked(object sender, EventArgs e)
        {
            Preferences.Clear();
            App.Current.MainPage = new NavigationPage(new LoginPage());
        }

        private async void OnEditProfileImageClicked(object sender, EventArgs e)
        {
            try
            {
                if (!CrossMedia.Current.IsPickPhotoSupported)
                {
                    await DisplayAlert("Not Supported", "Photo picking is not supported on this device.", "OK");
                    return;
                }

                var photo = await CrossMedia.Current.PickPhotoAsync(new PickMediaOptions
                {
                    PhotoSize = PhotoSize.Medium,
                    CompressionQuality = 80
                });

                if (photo == null)
                    return;

                using (var stream = photo.GetStream())
                using (var memoryStream = new MemoryStream())
                {
                    await stream.CopyToAsync(memoryStream);
                    byte[] imageBytes = memoryStream.ToArray();
                    string base64Image = Convert.ToBase64String(imageBytes);
                    _user.ProfileImage = base64Image; // Frissíti a ProfileImage-t, ami automatikusan frissíti a ProfileImageSource-t
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"An error occurred while selecting the image: {ex.Message}", "OK");
            }
        }
    }
}