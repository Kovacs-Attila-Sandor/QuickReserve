using QuickReserve.Models;
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
	public partial class UserRegisterPage: ContentPage
	{
		public UserRegisterPage()
		{
            InitializeComponent();
        }

        protected void GoToLoginPage(object sender, EventArgs e)
        {
            App.Current.MainPage = new NavigationPage(new LoginPage());
        }

        private async void Regiser(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(txtUsername.Text) && !string.IsNullOrEmpty(txtPasswod.Text) && !string.IsNullOrEmpty(txtConfirmPasswod.Text) && !string.IsNullOrEmpty(txtEmail.Text) && !string.IsNullOrEmpty(txtPhonenum.Text))
            {
                //if (await App.SQLiteDb.GetEventsAsync(txtUsername.Text) == null)    //FIREBASE DATABASE
                //{
                    if (string.Equals(txtPasswod.Text, txtConfirmPasswod))
                    {
                        User user = new User()
                        {
                            Name = txtUsername.Text,
                            Password = txtPasswod.Text,
                            Email = txtEmail.Text,
                            PhoneNumber = txtPhonenum.Text,
                            Role = "USER"
                        };
                        //await App.SQLiteDb.SaveItemAsync(events);
                    }
                    else await DisplayAlert("SAVING ERROR", "Your password is incorrect", "OK");

                // }
                // else await DisplayAlert("SAVING ERROR", "This Username is already used", "OK");
            }
            else await DisplayAlert("SAVING ERROR", "Something is empty", "OK");
        }
    }
}