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

            if (!string.IsNullOrEmpty(txtUsername.Text) &&
                !string.IsNullOrEmpty(password) &&
                !string.IsNullOrEmpty(confirmPassword) &&
                !string.IsNullOrEmpty(txtEmail.Text) &&
                !string.IsNullOrEmpty(txtPhonenum.Text) &&
                !string.IsNullOrEmpty(txtCity.Text) &&
                !string.IsNullOrEmpty(txtStreet.Text) &&
                !string.IsNullOrEmpty(txtCountry.Text) &&
                !string.IsNullOrEmpty(txtNumber.Text) &&
                !string.IsNullOrEmpty(txtShortDescriprion.Text) &&
                !string.IsNullOrEmpty(txtLongDescriprion.Text) &&
                imageBase64List.Count > 0)
            {
                if (await userService.GetUserByName(txtUsername.Text.Trim()) == null)
                {
                    if (password == confirmPassword)
                    {
                        User user = new User
                        {
                            Name = txtUsername.Text.Trim(),
                            Password = password,
                            Email = txtEmail.Text.Trim(),
                            PhoneNumber = txtPhonenum.Text.Trim(),
                            Role = "RESTAURANT"
                        };
                        await userService.AddUser(user);

                        Restaurant restaurant = new Restaurant
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
                            ImageBase64List = new List<string>(imageBase64List)
                        };

                        string restaurantId = await restaurantService.AddRestaurantAndGetId(restaurant);

                        if (!string.IsNullOrEmpty(restaurantId))
                        {
                            await DisplayAlert("Success", "Restaurant added successfully", "OK");
                            await Navigation.PushAsync(new AddMenuPage(restaurantId));
                        }
                        else
                        {
                            await DisplayAlert("Error", "Error saving restaurant.", "OK");
                        }
                    }
                    else
                    {
                        await DisplayAlert("Error", "Passwords do not match.", "OK");
                    }
                }
                else
                {
                    await DisplayAlert("Error", "Username already exists.", "OK");
                }
            }
            else
            {
                await DisplayAlert("Error", "All fields are required.", "OK");
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
