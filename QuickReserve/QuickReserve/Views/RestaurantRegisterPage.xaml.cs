using QuickReserve.Models;
using QuickReserve.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace QuickReserve.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class RestaurantRegisterPage : ContentPage
    {
        public RestaurantRegisterPage()
        {
            InitializeComponent();
        }

        protected void GoToUserRegisterPage(object sender, EventArgs e)
        {
            App.Current.MainPage = new NavigationPage(new UserRegisterPage());
        }
        protected void GoToLoginPage(object sender, EventArgs e)
        {
            App.Current.MainPage = new NavigationPage(new LoginPage());
        }
        protected async void GoToAddMenuPage(object sender, EventArgs e)
        {
            UserService userService = new UserService();
            RestaurantService restaurantService = new RestaurantService();

            string password = txtPassword.Text?.Trim();  // Trim() eltávolítja a szóközöket a végéről és elejéről
            string confirmPassword = txtConfirmPassword.Text?.Trim();

            if (!string.IsNullOrEmpty(txtUsername.Text) &&
                !string.IsNullOrEmpty(txtPassword.Text) &&
                !string.IsNullOrEmpty(txtConfirmPassword.Text) &&
                !string.IsNullOrEmpty(txtEmail.Text) &&
                !string.IsNullOrEmpty(txtPhonenum.Text) &&
                !string.IsNullOrEmpty(txtCity.Text) &&
                !string.IsNullOrEmpty(txtStreet.Text) &&
                !string.IsNullOrEmpty(txtPhonenum.Text) &&
                !string.IsNullOrEmpty(txtCountry.Text) &&
                !string.IsNullOrEmpty(txtNumber.Text) &&
                !string.IsNullOrEmpty(txtShortDescriprion.Text) &&
                !string.IsNullOrEmpty(txtLongDescriprion.Text))
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
                            Role = "RESTAURANT"
                        };
                        await userService.AddUser(user);

                        Restaurant restaurant = new Restaurant()
                        {
                            Name = txtUsername.Text.Trim(),
                            Address = new Location
                            {
                                City = txtCity.Text.Trim(),
                                Country = txtCountry.Text.Trim(),
                                Street = txtStreet.Text.Trim(),
                                Number = txtNumber.Text.Trim()
                            },
                            PhoneNumber = txtPhonenum.Text.Trim(),
                            Email = txtEmail.Text.Trim(),
                            Rating = 0,
                            ShortDescription = txtShortDescriprion.Text.Trim(),
                            LongDescription = txtLongDescriprion.Text.Trim()
                        };

                        string restaurantId = await restaurantService.AddRestaurantAndGetId(restaurant); // Új metódus

                        if (!string.IsNullOrEmpty(restaurantId))
                        {
                            await DisplayAlert("SAVING SUCCESS", "Restaurant added successfully", "OK");
                            App.Current.MainPage = new NavigationPage(new AddMenuPage(restaurantId)); // Továbbadjuk az ID-t
                        }
                        else
                            await DisplayAlert("SAVING ERROR", "There was an error adding the restaurant", "OK");
                    }
                    else
                        await DisplayAlert("SAVING ERROR", "Your password is not matching", "OK");
                }
                else
                    await DisplayAlert("SAVING ERROR", "This Username is already used", "OK");
            }
            else await DisplayAlert("SAVING ERROR", "Something is empty", "OK");           
        }
    }
}