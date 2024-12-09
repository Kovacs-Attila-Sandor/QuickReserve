using QuickReserve.Models;
using QuickReserve.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
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

            string password = txtPassword.Text?.Trim();
            string confirmPassword = txtConfirmPassword.Text?.Trim();
            App.Current.MainPage = new NavigationPage(new AddMenuPage("02"));
            /*
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

                        // Create the restaurant object
                        Restaurant restaurant = new Restaurant()
                        {
                            Name = txtUsername.Text.Trim(),
                            Address = new LocationRes
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
                            LongDescription = txtLongDescriprion.Text.Trim(),
                            ImageBase64 = txtBase64Image.Text?.Trim() // Add the Base64 image
                        };

                        string restaurantId = await restaurantService.AddRestaurantAndGetId(restaurant);

                        if (!string.IsNullOrEmpty(restaurantId))
                        {
                            await DisplayAlert("SAVING SUCCESS", "Restaurant added successfully", "OK");
                            App.Current.MainPage = new NavigationPage(new AddMenuPage(restaurantId));
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
            */
        }

        protected async void PickImageButton_Clicked(object sender, EventArgs e)
        {
            try
            {
                await Permissions.RequestAsync<Permissions.StorageRead>();
                await Permissions.RequestAsync<Permissions.StorageWrite>();
                // Check for storage permission to access photos
                var status1 = await Permissions.CheckStatusAsync<Permissions.Photos>();
                if (status1 != PermissionStatus.Granted)
                {
                    var result1 = await Permissions.RequestAsync<Permissions.Photos>();
                    status1 = result1;
                }

                // Check for media permission (required in Android 33+)
                var status2 = await Permissions.CheckStatusAsync<Permissions.Media>();
                if (status2 != PermissionStatus.Granted)
                {
                    var result2 = await Permissions.RequestAsync<Permissions.Media>();
                    status2 = result2;
                }

                // Make sure all permissions are granted
                if (status1 == PermissionStatus.Granted && status2 == PermissionStatus.Granted)
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

                        // Store the Base64 encoded image in the hidden Entry field
                        txtBase64Image.Text = base64Image;

                        // Show the image immediately
                        SelectedImage.Source = ImageSource.FromStream(() => new MemoryStream(imageBytes));
                        SelectedImage.IsVisible = true; // Make the Image visible
                    }
                }
                else
                {
                    // Both permissions are not granted
                    string permissionStr = (status1 != PermissionStatus.Granted) ? "Photos" : "";
                    permissionStr += (status2 != PermissionStatus.Granted) ? ((permissionStr.Length > 0) ? " and " : "") + "Storage" : "";
                    await DisplayAlert("Permission Denied", "You need to grant permission to access " + permissionStr + ".", "OK");
                }
            }
            catch (Exception ex)
            {
                // Handle errors during the image selection process
                await DisplayAlert("Error", "An error occurred while selecting the image: " + ex.Message, "OK");
            }
        }
    }
}