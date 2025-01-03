using System;
using QuickReserve.Models;
using QuickReserve.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Xamarin.Essentials;

namespace QuickReserve.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class UserRegisterPage : ContentPage
    {

        public UserRegisterPage()
        {
            InitializeComponent();
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
            UserService userService = new UserService();

            string password = txtPassword.Text?.Trim();  // Trim() eltávolítja a szóközöket a végéről és elejéről
            string confirmPassword = txtConfirmPassword.Text?.Trim();

            if (!string.IsNullOrEmpty(txtUsername.Text) && !string.IsNullOrEmpty(txtPassword.Text) && !string.IsNullOrEmpty(txtConfirmPassword.Text) && !string.IsNullOrEmpty(txtEmail.Text) && !string.IsNullOrEmpty(txtPhonenum.Text))
            {
                if (await userService.GetUserByName(txtUsername.Text.Trim()) == null)
                {
                    if (string.Equals(password, confirmPassword))
                    {
                        User user = new User()
                        {
                            Name = txtUsername.Text.Trim(),
                            Password = txtPassword.Text.Trim(),
                            Email = txtEmail.Text.Trim(),
                            PhoneNumber = txtPhonenum.Text.Trim(),
                            Role = "USER"
                        };
                        bool succes = await userService.AddUser(user);
                        if (succes)
                            await DisplayAlert("SAVING SUCCESS", "User added successfully", "OK");
                        else
                            await DisplayAlert("SAVING ERROR", "There was an error adding the user", "OK");
                    }
                    else await DisplayAlert("SAVING ERROR", "Your password is not matching", "OK");
                }
                else await DisplayAlert("SAVING ERROR", "This Username is already used", "OK");
            }
            else await DisplayAlert("SAVING ERROR", "Something is empty", "OK");
        }
    }
}