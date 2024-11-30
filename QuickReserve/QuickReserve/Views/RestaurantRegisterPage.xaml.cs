using System;
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

        public async void RegisterAsRestaurant(object sender, EventArgs e)
        {
            // Register as restaurant logic here
        }

        private async void OnChooseImageClicked(object sender, EventArgs e)
        {
            try
            {
                PermissionStatus status = PermissionStatus.Unknown;

                if (DeviceInfo.Version.Major >= 33) // Android 13+
                {
                    status = await Permissions.CheckStatusAsync<Permissions.Media>();
                    if (status != PermissionStatus.Granted)
                    {
                        status = await Permissions.RequestAsync<Permissions.Media>();
                    }
                }
                else // Android 12 vagy korábbi
                {
                    status = await Permissions.CheckStatusAsync<Permissions.StorageRead>();
                    if (status != PermissionStatus.Granted)
                    {
                        status = await Permissions.RequestAsync<Permissions.StorageRead>();
                    }
                }

                if (status == PermissionStatus.Granted)
                {
                    var result = await FilePicker.PickAsync(new PickOptions
                    {
                        PickerTitle = "Please select an image",
                        FileTypes = FilePickerFileType.Images
                    });

                    if (result != null)
                    {
                        restaurantImagePreview.Source = ImageSource.FromFile(result.FullPath);
                    }
                }
                else
                {
                    await DisplayAlert("Permission Denied", "Media access is required to choose an image.", "OK");
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"An  error occurred: {ex.Message}", "OK");
            }
        }

    }
}
