using System;
using Xamarin.Forms;
using QuickReserve.Services; // Importálni kell a UserService-hez
using QuickReserve.Models;   // Importálni kell a User osztályhoz
using QuickReserve.Converter; // Importálni kell a konverterhez

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

        // Aszinkron metódus a felhasználó adatainak betöltésére
        private async void LoadUserData(string userName)
        {
            var userService = new UserService();
            User = await userService.GetUserByName(userName); // Felhasználó adatainak lekérése

            if (User != null)
            {
                // Ha a profilkép base64 formátumban van, átalakítjuk ImageSource típusra
                if (!string.IsNullOrEmpty(User.ProfileImage))
                {
                    User.ProfileImageSource = ImageSource.FromResource("QuickReserve.Android.Resources.drawable.placeholder.png");
                }
                else
                {
                    // Ha nincs profilkép, beállítjuk az alapértelmezett képet
                    User.ProfileImageSource = ImageSource.FromFile("placeholder.png"); // Tedd a megfelelő mappába
                }

                // A felhasználó adatainak megjelenítése a UI-ban
                BindingContext = this; // A bindingContext beállítása a User objektumra
            }
            else
            {
                await DisplayAlert("Error", "User not found", "OK");
            }
        }
    }
}
