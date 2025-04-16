using QuickReserve.Converter;
using QuickReserve.Models;
using QuickReserve.Services;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace QuickReserve.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class RestaurantLayoutPage : ContentPage
    {
        private Button _selectedTableButton;
        private Restaurant _restaurant;
        private RestaurantService restaurantService;
        private string _restaurantId;
        private TimeSpan minTime = new TimeSpan(9, 0, 0);
        private TimeSpan maxTime = new TimeSpan(22, 0, 0);

        public RestaurantLayoutPage(string restaurantId)
        {
            InitializeComponent();
            restaurantService = RestaurantService.Instance;
            _restaurantId = restaurantId;

            reservationTimePicker.Time = minTime;
            reservationTimePicker.PropertyChanged += ReservationTimePicker_PropertyChanged;

            Buttons.IsVisible = false;
            ConfirmButton.IsVisible = false;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            try
            {
                LoadingOverlay.IsVisible = true;
                DynamicGrid.Children.Clear();
                DynamicGrid.RowDefinitions.Clear();
                DynamicGrid.ColumnDefinitions.Clear();
                _selectedTableButton = null;

                _restaurant = await restaurantService.GetRestaurantById(_restaurantId);
                if (_restaurant?.FirstImageBase64 != null)
                {
                    _restaurant.ImageSourceUri = ImageConverter.ConvertBase64ToImageSource(_restaurant.FirstImageBase64);
                }

                SetupGrid();
                await SetupTablesAsync();
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Failed to load restaurant: {ex.Message}", "OK");
            }
            finally
            {
                LoadingOverlay.IsVisible = false;
                Buttons.IsVisible = true;
                ConfirmButton.IsVisible = true;
            }
        }

        private void SetupGrid()
        {
            for (int i = 0; i < 10; i++)
            {
                DynamicGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Star });
            }

            for (int j = 0; j < 6; j++)
            {
                DynamicGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Star });
            }
        }

        private Task SetupTablesAsync()
        {
            if (_restaurant?.Tables == null) return Task.CompletedTask;

            var tables = _restaurant.Tables.Select(t =>
                (t.Location.Row, t.Location.Column, t.AvailabilityStatus)).ToList();

            for (int row = 0; row < 10; row++)
            {
                for (int col = 0; col < 6; col++)
                {
                    var button = CreateTableButton();
                    var table = tables.FirstOrDefault(t => t.Row == row && t.Column == col);

                    if (table != default)
                    {
                        ConfigureTableButton(button, table.AvailabilityStatus);
                    }
                    else
                    {
                        button.IsEnabled = false;
                        button.BorderColor = Color.Black;
                    }

                    DynamicGrid.Children.Add(button, col, row);
                }
            }
            return Task.CompletedTask;
        }

        private Button CreateTableButton()
        {
            return new Button
            {
                FontSize = 13,
                BorderWidth = 3,
                FontAttributes = FontAttributes.Bold,
                TextColor = Color.Black,
                BackgroundColor = Color.LightGray,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.FillAndExpand,
                WidthRequest = 50,
                HeightRequest = 50
            };
        }

        private void ConfigureTableButton(Button button, string status)
        {
            button.IsEnabled = status == "Available";
            button.BorderColor = status == "Available" ? Color.Green : Color.Red;
            if (status == "Available")
            {
                button.Clicked += OnButtonClicked;
            }
        }

        private async void OnButtonClicked(object sender, EventArgs e)
        {
            var button = sender as Button;

            // Get the table position
            var row = Grid.GetRow(button);
            var col = Grid.GetColumn(button);

            // Find the corresponding table
            var table = _restaurant.Tables.FirstOrDefault(t =>
                t.Location.Row == row && t.Location.Column == col);

            if (table == null)
            {
                await DisplayAlert("Error", "Table not found!", "OK");
                return;
            }

            if (string.IsNullOrEmpty(button.Text))
            {
                if (_selectedTableButton != null && _selectedTableButton != button)
                {
                    await DisplayAlert("Error", "You can only select one table at a time!", "OK");
                    return;
                }

                // Show the prompt with table capacity information
                string result = await DisplayPromptAsync(
                    $"Select Table (max {table.Capacity} people)",
                    $"Enter number of guests:",
                    placeholder: "Guest count",
                    maxLength: 3,
                    keyboard: Keyboard.Numeric
                );

                if (!string.IsNullOrEmpty(result) && int.TryParse(result, out int guestCount))
                {
                    if (guestCount > table.Capacity)
                    {
                        await DisplayAlert("Error", $"This table can only accommodate {table.Capacity} people!", "OK");
                        return;
                    }

                    button.Text = $"{guestCount}/{table.Capacity}";
                    button.BorderColor = Color.Black;
                    _selectedTableButton = button;
                }
            }
            else
            {
                button.BorderColor = Color.Green;
                button.Text = null;
                _selectedTableButton = null;
            }
        }

        private async void ReservationTimePicker_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(TimePicker.Time))
            {
                var selectedTime = reservationTimePicker.Time;

                if (selectedTime < minTime || selectedTime > maxTime)
                {
                    await DisplayAlert("Invalid time", "Please select a time between 9:00 and 22:00!", "OK");
                    reservationTimePicker.Time = minTime;
                }
            }
        }

        private async void ConfirmButtonClicked(object sender, EventArgs e)
        {
            try
            {
                if (_selectedTableButton == null)
                {
                    await DisplayAlert("Error", "Please select a table before confirming!", "OK");
                    return;
                }

                var selectedTime = reservationTimePicker.Time;
                if (selectedTime < minTime || selectedTime > maxTime)
                {
                    await DisplayAlert("Error", "Please select a time between 9:00 and 22:00!", "OK");
                    return;
                }

                if (!int.TryParse(_selectedTableButton.Text.Split('/')[0], out int guestCount))
                {
                    await DisplayAlert("Error", "Invalid guest count!", "OK");
                    return;
                }

                var row = Grid.GetRow(_selectedTableButton);
                var col = Grid.GetColumn(_selectedTableButton);
                var table = _restaurant.Tables.FirstOrDefault(t =>
                    t.Location.Row == row && t.Location.Column == col);

                if (table == null)
                {
                    await DisplayAlert("Error", "Selected table not found!", "OK");
                    return;
                }

                if (table.Capacity < guestCount || table.AvailabilityStatus != "Available")
                {
                    await DisplayAlert("Error", "Table capacity is insufficient or not available!", "OK");
                    return;
                }

                var reservationDateTime = new DateTime(
                    reservationDatePicker.Date.Year,
                    reservationDatePicker.Date.Month,
                    reservationDatePicker.Date.Day,
                    reservationTimePicker.Time.Hours,
                    reservationTimePicker.Time.Minutes,
                    0);

                var menuPage = new RestaurantMenuPage(_restaurant)
                {
                    ReservationDateTime = reservationDateTime.ToString("g"),
                    TableId = table.TableId,
                    GuestCount = guestCount
                };

                await Navigation.PushAsync(menuPage);
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Confirmation failed: {ex.Message}", "OK");
            }
        }

        private async void GoToAboutPage(object sender, EventArgs e)
        {
            await Navigation.PopAsync();
        }
    }
}