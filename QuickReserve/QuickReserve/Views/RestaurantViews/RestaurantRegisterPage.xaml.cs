using Plugin.Media;
using QuickReserve.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace QuickReserve.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class RestaurantRegisterPage : ContentPage
    {
        public ObservableCollection<string> ImageBase64List { get; private set; } = new ObservableCollection<string>();
        public ObservableCollection<ImageSource> ImageSourceList { get; private set; } = new ObservableCollection<ImageSource>();

        private readonly FirebaseAuthService _authService;
        private readonly UserService _userService;
        private readonly RestaurantService _restaurantService;
        public ICommand DeleteCommand { get; private set; }


        public RestaurantRegisterPage()
        {
            InitializeComponent();
            _authService = FirebaseAuthService.Instance;
            _userService = UserService.Instance;
            _restaurantService = RestaurantService.Instance;

            DeleteCommand = new Command<ImageSource>(OnDeleteImage);
            BindingContext = this;
        }

        private void UpdateImageVisibility()
        {
            NoImagesLabel.Text = ImageSourceList.Count == 0 ? "No images selected." : "";
        }

        private void OnDeleteImage(ImageSource image)
        {
            int index = ImageSourceList.IndexOf(image);
            if (index >= 0)
            {
                ImageSourceList.RemoveAt(index);
                ImageBase64List.RemoveAt(index);
                UpdateImageVisibility();
            }
        }

        protected async void PickImageButton_Clicked(object sender, EventArgs e)
        {
            try
            {
                var media = await CrossMedia.Current.PickPhotosAsync();
                if (media != null && media.Count > 0)
                {
                    foreach (var file in media)
                    {
                        using (var stream = file.GetStream())
                        {
                            byte[] imageBytes = new byte[stream.Length];
                            await stream.ReadAsync(imageBytes, 0, (int)stream.Length);

                            string base64Image = Convert.ToBase64String(imageBytes);
                            ImageBase64List.Add(base64Image);

                            ImageSource imageSource = ImageSource.FromStream(() => new MemoryStream(imageBytes));
                            ImageSourceList.Add(imageSource);
                        }
                    }
                }
                UpdateImageVisibility();
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"An error occurred: {ex.Message}", "OK");
            }
        }

        protected async void GoToAddMenuPage(object sender, EventArgs e)
        {

            //LoadingOverlay.IsVisible = true;
            //MainPage.IsEnabled = false;

            //string password = txtPassword.Text?.Trim();
            //string confirmPassword = txtConfirmPassword.Text?.Trim();
            //try
            //{
            //    if (!string.IsNullOrEmpty(txtUsername.Text) &&
            //        !string.IsNullOrEmpty(password) &&
            //        !string.IsNullOrEmpty(confirmPassword) &&
            //        !string.IsNullOrEmpty(txtEmail.Text) &&
            //        !string.IsNullOrEmpty(txtPhonenum.Text) &&
            //        !string.IsNullOrEmpty(txtCity.Text) &&
            //        !string.IsNullOrEmpty(txtStreet.Text) &&
            //        !string.IsNullOrEmpty(txtCountry.Text) &&
            //        !string.IsNullOrEmpty(txtNumber.Text) &&
            //        !string.IsNullOrEmpty(txtShortDescription.Text) &&
            //        !string.IsNullOrEmpty(txtLongDescription.Text) &&
            //        ImageBase64List.Count > 0)
            //    {

            //        if (password == confirmPassword)
            //        {
            //            User user = new User
            //            {
            //                Name = txtUsername.Text.Trim(),
            //                Email = txtEmail.Text.Trim(),
            //                PhoneNumber = txtPhonenum.Text.Trim(),
            //                Role = "RESTAURANT"
            //            };
            //            var userId = await _userService.AddUserAndGetId(user);

            //            Restaurant restaurant = new Restaurant
            //            {
            //                Name = txtUsername.Text.Trim(),
            //                PhoneNumber = txtPhonenum.Text.Trim(),
            //                Email = txtEmail.Text.Trim(),
            //                ShortDescription = txtShortDescription.Text.Trim(),
            //                LongDescription = txtLongDescription.Text.Trim(),
            //                Address = new RestaurantLocation
            //                {
            //                    City = txtCity.Text.Trim(),
            //                    Country = txtCountry.Text.Trim(),
            //                    Street = txtStreet.Text.Trim(),
            //                    Number = txtNumber.Text.Trim()
            //                },
            //                ImageBase64List = new List<string>(ImageBase64List),
            //                UserId = userId
            //            };

            //            // Felhasználó létrehozása Firebase-ben
            //            var userCredential = await _authService.AuthClient.CreateUserWithEmailAndPasswordAsync(txtEmail.Text.Trim(), txtPassword.Text.Trim());

            //            string restaurantId = await _restaurantService.AddRestaurantAndGetId(restaurant);

            //            if (!string.IsNullOrEmpty(restaurantId))
            //            {
            //                await Navigation.PushAsync(new AddMenuPage(restaurantId));
            //            }
            //            else
            //            {
            //                await DisplayAlert("Error", "Error saving restaurant.", "OK");
            //            }
            //        }
            //        else
            //        {
            //            await DisplayAlert("Error", "Passwords do not match.", "OK");
            //        }
            //    }
            //    else
            //    {
            //        await DisplayAlert("Error", "All fields are required.", "OK");
            //    }
            //}
            //catch (FirebaseAuthException ex)
            //{
            //    await DisplayAlert("Registration Failed", $"Error: {ex.Reason}", "OK");
            //}
            //finally
            //{
            //    LoadingOverlay.IsVisible = false;
            //    MainPage.IsEnabled = true;
            //}
            await Navigation.PushAsync(new AddMenuPage("700d36c6-81d9-4bab-aba1-d7fbefa413bc")); //Mamma Mia etterem Id-ja
        }

        // Törlés funkció
        protected void DeleteImage(int index)
        {
            // Kép eltávolítása a listából és a CollectionView-ból
            ImageBase64List.RemoveAt(index);
            (ImagePreviewCollection.ItemsSource as IList<ImageSource>)?.RemoveAt(index);
        }

        protected async void GoToLoginPage(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new LoginPage());
        }

        private void OnTextChanged(object sender, TextChangedEventArgs e)
        {
            if (sender is Entry entry)
            {
                Label placeholderLabel = GetPlaceholderLabel(entry);

                if (!string.IsNullOrWhiteSpace(entry.Text))
                {
                    placeholderLabel.IsVisible = true;
                    placeholderLabel.FadeTo(1, 160);
                    placeholderLabel.TranslateTo(2, -3, 170);
                    entry.Placeholder = "";
                }
                else
                {
                    placeholderLabel.FadeTo(0, 150, Easing.Linear);
                    placeholderLabel.IsVisible = false;
                    entry.Placeholder = GetPlaceholderText(entry);
                }
            }
        }

        private void OnFocused(object sender, FocusEventArgs e)
        {
            if (sender is Entry entry)
            {
                Label placeholderLabel = GetPlaceholderLabel(entry);

                if (!string.IsNullOrWhiteSpace(entry.Text))
                {
                    placeholderLabel.IsVisible = true;
                    placeholderLabel.FadeTo(1, 150);
                    placeholderLabel.TranslateTo(0, 0, 150);
                }
            }
        }

        private void OnUnfocused(object sender, FocusEventArgs e)
        {
            if (sender is Entry entry)
            {
                Label placeholderLabel = GetPlaceholderLabel(entry);

                if (string.IsNullOrWhiteSpace(entry.Text))
                {
                    placeholderLabel.FadeTo(0, 150);
                    placeholderLabel.IsVisible = false;
                    entry.Placeholder = GetPlaceholderText(entry);
                }
            }
        }

        private Label GetPlaceholderLabel(Entry entry)
        {
            return entry == txtUsername ? lblUserNamePlaceholder :
                   entry == txtEmail ? lblEmailPlaceholder :
                   entry == txtPassword ? lblPasswordPlaceholder :
                   entry == txtConfirmPassword ? lblConfirmPasswordPlaceholder :
                   entry == txtCountry ? lblCountryPlaceholder :
                   entry == txtCity ? lblCityPlaceholder :
                   entry == txtStreet ? lblStreetPlaceholder :
                   entry == txtNumber ? lblStreetNumberPlaceholder :
                   entry == txtShortDescription ? lblShortDescriptionPlaceholder :
                   entry == txtLongDescription ? lblLongDescriptionPlaceholder :
                   entry == txtPhonenum ? lblPhoneNumberPlaceholder : null;
        }

        private string GetPlaceholderText(Entry entry)
        {
            return entry == txtUsername ? "Username" :
                   entry == txtEmail ? "Email" :
                   entry == txtPassword ? "Password" :
                   entry == txtConfirmPassword ? "Confirm Password" :
                   entry == txtCountry ? "Country" :
                   entry == txtCity ? "City" :
                   entry == txtStreet ? "Street" :
                   entry == txtNumber ? "Street Number" :
                   entry == txtShortDescription ? "Short Description" :
                   entry == txtLongDescription ? "Long Description" :
                   entry == txtPhonenum ? "Phone Number" : "";
        }

        private void TogglePasswordVisibility(object sender, EventArgs e)
        {
            txtPassword.IsPassword = !txtPassword.IsPassword;
            imgTogglePassword.Source = txtPassword.IsPassword ? "eye_closed.png" : "eye_open.png";
        }

        private void ToggleConfirmPasswordVisibility(object sender, EventArgs e)
        {
            txtConfirmPassword.IsPassword = !txtConfirmPassword.IsPassword;
            imgToggleConfirmPassword.Source = txtConfirmPassword.IsPassword ? "eye_closed.png" : "eye_open.png";
        }
    }
}