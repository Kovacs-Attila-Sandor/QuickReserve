using QuickReserve.Models;
using QuickReserve.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace QuickReserve.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class RestaurantRegisterPage : ContentPage
    {
        private List<string> imageBase64List = new List<string>();

        public RestaurantRegisterPage()
        {
            InitializeComponent();
        }

        protected async void PickImageButton_Clicked(object sender, EventArgs e)
        {
            try
            {
                var result = await MediaPicker.PickPhotoAsync(new MediaPickerOptions
                {
                    Title = "Select a photo"
                });

                if (result != null)
                {
                    // Convert the selected image to Base64
                    var stream = await result.OpenReadAsync();
                    byte[] imageBytes = new byte[stream.Length];
                    await stream.ReadAsync(imageBytes, 0, (int)stream.Length);
                    string base64Image = Convert.ToBase64String(imageBytes);

                    // Add the Base64 encoded image to the list
                    imageBase64List.Add(base64Image);

                    // Refresh the CollectionView
                    var imageSourceList = new List<ImageSource>();
                    foreach (var base64 in imageBase64List)
                    {
                        var bytes = Convert.FromBase64String(base64);
                        imageSourceList.Add(ImageSource.FromStream(() => new MemoryStream(bytes)));
                    }
                    ImagePreviewCollection.ItemsSource = imageSourceList;
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"An error occurred: {ex.Message}", "OK");
            }
        }

        protected async void GoToAddMenuPage(object sender, EventArgs e)
        {
            UserService userService = new UserService();
            RestaurantService restaurantService = new RestaurantService();
            string password = txtPassword.Text?.Trim();
            string confirmPassword = txtConfirmPassword.Text?.Trim();

            // Ellenőrizzük, hogy minden mező ki van-e töltve és hogy van-e legalább egy kép
            if (!string.IsNullOrEmpty(txtUsername.Text) &&
                !string.IsNullOrEmpty(txtPassword.Text) &&
                !string.IsNullOrEmpty(txtConfirmPassword.Text) &&
                !string.IsNullOrEmpty(txtEmail.Text) &&
                !string.IsNullOrEmpty(txtPhonenum.Text) &&
                !string.IsNullOrEmpty(txtCity.Text) &&
                !string.IsNullOrEmpty(txtStreet.Text) &&
                !string.IsNullOrEmpty(txtCountry.Text) &&
                !string.IsNullOrEmpty(txtNumber.Text) &&
                !string.IsNullOrEmpty(txtShortDescriprion.Text) &&
                !string.IsNullOrEmpty(txtLongDescriprion.Text) &&
                imageBase64List.Count > 0)  // Képek jelenléte is szükséges
            {
                // Ellenőrizzük, hogy a felhasználónév már létezik-e
                if (await userService.GetUserByName(txtUsername.Text.Trim()) == null)
                {
                    // Ellenőrizzük, hogy a két jelszó egyezik-e
                    if (string.Equals(password, confirmPassword))
                    {
                        // Létrehozzuk a User objektumot
                        User user = new User()
                        {
                            Name = txtUsername.Text.Trim(),
                            Password = txtPassword.Text.Trim(),
                            Email = txtEmail.Text.Trim(),
                            PhoneNumber = txtPhonenum.Text.Trim(),
                            Role = "RESTAURANT"
                        };
                        await userService.AddUser(user);

                        // Létrehozzuk a Restaurant objektumot
                        Restaurant restaurant = new Restaurant()
                        {
                            Name = txtUsername.Text.Trim(),
                            PhoneNumber = txtPhonenum.Text.Trim(),
                            Email = txtEmail.Text.Trim(),
                            ShortDescription = txtShortDescriprion.Text.Trim(),
                            LongDescription = txtLongDescriprion.Text.Trim(),
                            Address = new RestaurantLocation
                            {
                                City = txtCity.Text.Trim(),
                                Country = txtCountry.Text.Trim(),
                                Street = txtStreet.Text.Trim(),
                                Number = txtNumber.Text.Trim()
                            },
                            ImageBase64List = new List<string>(imageBase64List) // Több kép hozzáadása
                        };

                        // Étterem mentése és ID lekérése
                        string restaurantId = await restaurantService.AddRestaurantAndGetId(restaurant);

                        // Ha sikerült menteni, navigáljunk az AddMenuPage-re
                        if (!string.IsNullOrEmpty(restaurantId))
                        {
                            await DisplayAlert("SAVING SUCCESS", "Restaurant added successfully", "OK");
                            App.Current.MainPage = new NavigationPage(new AddMenuPage(restaurantId));
                        }
                        else
                            await DisplayAlert("SAVING ERROR", "There was an error adding the restaurant", "OK");
                    }
                    else
                    {
                        await DisplayAlert("SAVING ERROR", "Your password is not matching", "OK");
                    }
                }
                else
                {
                    await DisplayAlert("SAVING ERROR", "This Username is already used", "OK");
                }
            }
            else
            {
                await DisplayAlert("SAVING ERROR", "Please fill in all fields and add at least one image.", "OK");
            }
        }

        protected async void GoToLoginPage(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new LoginPage());
        }

        protected async void GoToUserRegisterPage(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new UserRegisterPage());
        }
    }
}
