using QuickReserve.Models;
using QuickReserve.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace QuickReserve.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class RestaurantLayoutPage : ContentPage
    {
        private Button _selectedTableButton; // Tárolja az aktuálisan kiválasztott gombot
        private Restaurant _restaurant;

        public RestaurantLayoutPage(Restaurant restaurant)
        {
            InitializeComponent();
            _restaurant = restaurant;

            var dynamicGrid = DynamicGrid;

            // Define 10 rows
            for (int i = 0; i < 10; i++)
            {
                dynamicGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Star });
            }

            // Define 6 columns
            for (int j = 0; j < 6; j++)
            {
                dynamicGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Star });
            }

            // Extract tables from the restaurant
            List<(int row, int col)> tables = new List<(int, int)>();
            for (int i = 0; i < restaurant.Tables.Count; i++)
            {
                var table = (restaurant.Tables[i].Location.Row, restaurant.Tables[i].Location.Column);
                tables.Add(table);
            }

            // Add 10x6 buttons dynamically
            for (int row = 0; row < 10; row++)
            {
                for (int col = 0; col < 6; col++)
                {
                    var button = new Button
                    {
                        Text = null,
                        FontSize = 13,
                        BorderWidth = 3,
                        FontAttributes = FontAttributes.Bold,
                        TextColor = Color.Black,
                        BackgroundColor = Color.LightGray,
                        HorizontalOptions = LayoutOptions.FillAndExpand,
                        VerticalOptions = LayoutOptions.FillAndExpand,
                        IsVisible = true,
                        WidthRequest = 50,
                        HeightRequest = 50
                    };

                    // Check if the current position matches a table
                    if (tables.Exists(t => t.row == row && t.col == col))
                    {
                        button.IsEnabled = true;
                        button.BorderColor = Color.Green;
                        button.Clicked += OnButtonClicked;
                    }
                    else
                    {
                        button.IsEnabled = false;
                        button.BorderColor = Color.Black;
                    }

                    // Add the button to the grid
                    dynamicGrid.Children.Add(button, col, row);
                }
            }
        }

        private async void OnButtonClicked(object sender, EventArgs e)
        {
            var button = sender as Button;

            if (button.Text == null)
            {
                // Ellenőrizzük, hogy már van-e kiválasztott asztal
                if (_selectedTableButton != null && _selectedTableButton != button)
                {
                    await DisplayAlert("Error", "You can only select one table at a time.", "OK");
                    return;
                }

                // Prompt the user for input
                string result = await DisplayPromptAsync("Number of seats:", "Enter the number of people:",
                                                         placeholder: "Number of people", maxLength: 3, keyboard: Keyboard.Text);

                if (!string.IsNullOrEmpty(result))
                {
                    button.Text = result; // Set the button text
                    button.BorderColor = Color.Black;
                    _selectedTableButton = button; // Mentjük az aktuálisan kiválasztott gombot
                }
            }
            else
            {
                // Ha a gombra kattintunk és már van szám, akkor töröljük azt
                button.BorderColor = Color.Transparent;
                button.Text = null;
                _selectedTableButton = null; // Töröljük a kiválasztott gombot
            }
        }

        protected void GoToAboutPage(object sender, EventArgs e)
        {
            App.Current.MainPage = new NavigationPage(new AboutPage());
        }

        protected async void ConfirmButton(object sender, EventArgs e)
        {
            var selectedDate = reservationDatePicker.Date;
            var selectedTime = myPicker.SelectedItem as string;

            if (_selectedTableButton == null || selectedTime == null)
            {
                await DisplayAlert("Error", "Please select a time and a table before confirming.", "OK");
                return;
            }

            if (!int.TryParse(_selectedTableButton.Text, out int guestCount))
            {
                await DisplayAlert("Error", "Invalid table selection.", "OK");
                return;
            }

            // Határozzuk meg a gomb sorát és oszlopát
            var row = Grid.GetRow(_selectedTableButton);
            var col = Grid.GetColumn(_selectedTableButton);

            // Keresd meg az asztalt a pozíció alapján
            var table = _restaurant.Tables.FirstOrDefault(t =>
                t.Location.Row == row && t.Location.Column == col);

            if (table == null)
            {
                await DisplayAlert("Error", $"No table found at position ({row}, {col}).", "OK");
                return;
            }

            if (table.Capacity < guestCount || table.AvailabilityStatus != "Available")
            {
                await DisplayAlert("Error", $"The selected table cannot accommodate {guestCount} persons or is unavailable.", "OK");
                return;
            }

            var reservationDateTime = $"{selectedDate.ToShortDateString()} {selectedTime}";
            string tableId = table.TableId;

            var menuPage = new RestaurantMenuPage(_restaurant)
            {
                ReservationDateTime = reservationDateTime,
                TableId = tableId,
                GuestCount = guestCount
            };

            await Navigation.PushAsync(menuPage);
        }
    }
}
