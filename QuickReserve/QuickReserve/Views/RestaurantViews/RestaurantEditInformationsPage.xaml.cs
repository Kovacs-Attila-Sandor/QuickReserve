using QuickReserve.Converter;
using QuickReserve.Models;
using QuickReserve.Services;
using System;
using System.IO;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace QuickReserve.Views.RestaurantViews
{
    public partial class RestaurantEditInformationsPage : ContentPage
    {
        private Restaurant _restaurant;
        private RestaurantService restaurantService;

        public RestaurantEditInformationsPage(Restaurant restaurant)
        {
            InitializeComponent();
            _restaurant = restaurant;
            restaurantService = RestaurantService.Instance;
            BindRestaurantData();
        }

        private void BindRestaurantData()
        {
            // Fill UI elements with restaurant data
            NameEntry.Text = _restaurant.Name;
            // Update based on your Address model
            AddressCountry.Text = _restaurant.Address.Country;
            AddressCity.Text = _restaurant.Address.City;
            AddressStreet.Text = _restaurant.Address.Street;
            AddressNumber.Text = _restaurant.Address.Number;
            Longitude.Text = _restaurant.Address.Longitude.ToString();
            Latitude.Text = _restaurant.Address.Latitude.ToString();
            PhoneNumberEntry.Text = _restaurant.PhoneNumber;
            EmailEntry.Text = _restaurant.Email;
            ShortDescriptionEditor.Text = _restaurant.ShortDescription;
            LongDescriptionEditor.Text = _restaurant.LongDescription;

            // Load existing images
            ImageList.Children.Clear();
            foreach (var imageBase64 in _restaurant.ImageBase64List)
            {
                DisplayImageWithDeleteOption(imageBase64);
            }
        }

        private async void OnAddImageClicked(object sender, EventArgs e)
        {
            try
            {
                var photo = await MediaPicker.PickPhotoAsync(new MediaPickerOptions
                {
                    Title = "Select an image"
                });

                if (photo != null)
                {
                    using (var stream = await photo.OpenReadAsync())
                    {
                        var memoryStream = new MemoryStream();
                        await stream.CopyToAsync(memoryStream);
                        byte[] imageBytes = memoryStream.ToArray();
                        string base64Image = Convert.ToBase64String(imageBytes);

                        _restaurant.ImageBase64List.Add(base64Image);
                        DisplayImageWithDeleteOption(base64Image);
                    }
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Unable to pick image: {ex.Message}", "OK");
            }
        }

        private void DisplayImageWithDeleteOption(string base64Image)
        {
            var imageLayout = new StackLayout
            {
                Orientation = StackOrientation.Horizontal,
                Margin = new Thickness(5),
                Spacing = 10  // Margin between image and delete button
            };

            // Kép megjelenítése
            var image = new Image
            {
                Source = ImageConverter.ConvertBase64ToImageSource(base64Image),
                HeightRequest = 150, // Smaller size for better UI
                WidthRequest = 150,  // Adjust size
                Aspect = Aspect.AspectFill, // Ensures the image fills the allocated space nicely
                Margin = new Thickness(0, 5)  // Small margin for aesthetics
            };

            // Törlés gomb hozzáadása
            var deleteButton = new Button
            {
                Text = "Delete",
                BackgroundColor = Color.Red,
                TextColor = Color.White,
                HeightRequest = 40,
                WidthRequest = 80,
                Margin = new Thickness(5),
                CornerRadius = 5  // Rounded corners for the delete button
            };

            deleteButton.Clicked += (sender, args) =>
            {
                // Törlés a listából
                _restaurant.ImageBase64List.Remove(base64Image);
                // Eltávolítás a UI-ból
                ImageList.Children.Remove(imageLayout);
            };

            imageLayout.Children.Add(image);
            imageLayout.Children.Add(deleteButton);

            ImageList.Children.Add(imageLayout);
        }



        private async void OnSaveChangesClicked(object sender, EventArgs e)
        {
            // Update restaurant details
            _restaurant.Name = NameEntry.Text.Trim();
            _restaurant.Address = new RestaurantLocation
            {
                Country = AddressCountry.Text.Trim(),
                City = AddressCity.Text.Trim(),
                Street = AddressStreet.Text.Trim(),
                Number = AddressNumber.Text.Trim(),
                Latitude = Double.Parse(Latitude.Text.Trim()),
                Longitude = Double.Parse(Longitude.Text.Trim())
            };
            _restaurant.PhoneNumber = PhoneNumberEntry.Text.Trim();
            _restaurant.Email = EmailEntry.Text.Trim();
            _restaurant.ShortDescription = ShortDescriptionEditor.Text.Trim();
            _restaurant.LongDescription = LongDescriptionEditor.Text.Trim();

            bool result = await restaurantService.UpdateRestaurantAsync(_restaurant);
            if (result)
            {
                await DisplayAlert("Success", "Restaurant details updated successfully!", "OK");
                App.Current.Properties["ReloadRestaurantProfilePage"] = "yes";
                await Navigation.PopAsync();
            }
            else
            {
                await DisplayAlert("Error", "Failed to update restaurant details.", "OK");
            }
        }
    }
}
