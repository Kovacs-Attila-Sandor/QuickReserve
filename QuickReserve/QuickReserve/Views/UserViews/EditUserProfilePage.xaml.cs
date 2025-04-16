using Xamarin.Forms;
using QuickReserve.Models;
using QuickReserve.Services;
using System;
using Firebase.Auth;
using System.IO;
using Xamarin.Essentials;
using Plugin.Media.Abstractions;
using Plugin.Media;
using Rg.Plugins.Popup.Services;
using QuickReserve.Views.PopUps;

namespace QuickReserve.Views
{
    public partial class EditUserProfilePage : ContentPage
    {
        public User _user { get; set; }
        private UserService userService;

        public EditUserProfilePage(User user)
        {
            InitializeComponent();
            this._user = user;  // Hozzárendeljük a paramétert az osztály mezőjéhez
            userService = UserService.Instance;  // Feltételezzük, hogy a UserService kezeli az adatokat
            BindingContext = this._user;  // A BindingContext-ot a user objektumra állítjuk be
        }

        // Save Changes Button Clicked
        private async void OnSaveChangesClicked(object sender, EventArgs e)
        {
            User newUser = new User() { 
                UserId = _user.UserId,
                PhoneNumber = PhoneNumberEntry.Text.Trim(),
                Email = _user.Email,
                Name = NameEntry.Text.Trim(),
                Role = _user.Role,
                CreatedAt = _user.CreatedAt,
                ProfileImage = _user.ProfileImage,
                ProfileImageSource = _user.ProfileImageSource
            };

            bool success = await userService.UpdateUserProfile(_user.UserId, newUser);

            if (success)
            {
                await PopupNavigation.Instance.PushAsync(new CustomAlert("Success", "Your profile has been updated."));
                await Navigation.PopAsync();
            }
            else
            {
                await DisplayAlert("Error", "Failed to update your profile. Please try again later.", "OK");               
            }
        }

        private async void OnEditProfileImageClicked(object sender, EventArgs e)
        {
            try
            {
                // Ellenőrizzük, hogy a galéria hozzáférés elérhető-e
                if (!CrossMedia.Current.IsPickPhotoSupported)
                {
                    await DisplayAlert("Not Supported", "Photo picking is not supported on this device.", "OK");
                    return;
                }

                // Kép kiválasztása a galériából a CrossMedia segítségével
                var photo = await CrossMedia.Current.PickPhotoAsync(new PickMediaOptions
                {
                    PhotoSize = PhotoSize.Medium, // Kép átméretezése közepes méretre a teljesítmény érdekében
                    CompressionQuality = 80 // Kép tömörítése (0-100, 100 a legjobb minőség)
                });

                if (photo == null)
                    return; // A felhasználó nem választott képet

                ProfileImage.Source = ImageSource.FromStream(() =>
                {
                    // A kiválasztott kép streamjének visszaadása
                    var stream = photo.GetStream();
                    return stream;
                });

                // A kiválasztott kép beolvasása byte tömbként
                using (var stream = photo.GetStream())
                using (var memoryStream = new MemoryStream())
                {
                    await stream.CopyToAsync(memoryStream);
                    byte[] imageBytes = memoryStream.ToArray();

                    // Byte tömb konvertálása base64 stringgé
                    string base64Image = Convert.ToBase64String(imageBytes);

                    // A User objektum ProfileImage tulajdonságának frissítése
                    _user.ProfileImage = base64Image;
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"An error occurred while selecting the image: {ex.Message}", "OK");
            }
        }
        private async void OnCancelClicked(object sender, EventArgs e)
        {
            await Navigation.PopAsync();
        }
    }
}