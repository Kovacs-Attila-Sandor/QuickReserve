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
    public partial class CreateLayoutPage : ContentPage
    {
        private string _restaurantId;
        public CreateLayoutPage(string restaurantId)
        {
            _restaurantId = restaurantId;
            InitializeComponent();

            // Create the grid dynamically
            var dynamicGrid = DynamicGrid;

            // Define 15 rows
            for (int i = 0; i < 10; i++)
            {
                dynamicGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Star });
            }

            // Define 10 columns
            for (int j = 0; j < 6; j++)
            {
                dynamicGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Star });
            }

            // Add 15x10 buttons dynamically
            for (int row = 0; row < 10; row++)
            {
                for (int col = 0; col < 6; col++)
                {
                    var button = new Button
                    {
                        Text =  null,
                        FontSize=13,
                        BorderWidth=3,
                        FontAttributes = FontAttributes.Bold,
                        TextColor =Color.Black,
                        BackgroundColor=Color.LightGray,
                        HorizontalOptions = LayoutOptions.FillAndExpand,
                        VerticalOptions = LayoutOptions.FillAndExpand,
                        IsVisible =true,
                        WidthRequest = 50, // Adjust the width of the buttons
                        HeightRequest = 50 // Adjust the height of the buttons
                    };
                    button.Clicked += OnButtonClicked;

                

                    // Add both elements to the grid in the same position
                    dynamicGrid.Children.Add(button, col, row);
                    

                    

                }
            }
        }

        private async void OnButtonClicked(object sender, EventArgs e)
        {
            var button = sender as Button;
        

            if (button.Text == null)
            {
                button.BorderColor = Color.Black;

                // Prompt the user for input
                string result = await DisplayPromptAsync("Number of seats", "Enter the number of seats:",
                                                         placeholder: "Number of seats", maxLength: 3, keyboard: Keyboard.Text);

                if (!string.IsNullOrEmpty(result))
                {
                    button.Text = result; // Set the button text
                    button.BorderColor = Color.Black;
                }
            }
            else
            {
                button.BorderColor = Color.Transparent;
                button.Text = null; // Reset button text
            }
        }

        // Navigate to the LoginPage when "Finish Registration" button is clicked
        protected async void GoToLoginPage(object sender, EventArgs e)
        {
            try
            {
                // Gyűjtsük össze az összes asztalt a DynamicGrid-ből
                var tables = new List<Table>();

                int tableCounter = 1; // Az asztalok számozásához

                foreach (var child in DynamicGrid.Children)
                {
                    if (child is Button button && !string.IsNullOrEmpty(button.Text))
                    {
                        // Hozzunk létre egy új asztal objektumot
                        var table = new Table
                        {
                            TableId = Guid.NewGuid().ToString(),
                            TableNumber = tableCounter++, // Automatikusan növekvő asztalszám
                            Capacity = int.Parse(button.Text), // A gomb szövegéből kapjuk meg a kapacitást
                            AvailabilityStatus = "Available", // Alapértelmezett státusz
                            Location = new TableLocation
                            {
                                Row = Grid.GetRow(button),
                                Column = Grid.GetColumn(button)
                            }
                        };

                        tables.Add(table); // Adjunk hozzá az asztalt a listához
                    }
                }

                // Hívjuk meg a mentési metódust
                var restaurantService = new RestaurantService();
                bool isSaved = await restaurantService.AddTablesToRestaurant(_restaurantId, tables);

                if (isSaved)
                {
                    await DisplayAlert("Success", "Tables saved successfully!", "OK");
                    App.Current.MainPage = new NavigationPage(new LoginPage());
                }
                else
                {
                    await DisplayAlert("Error", "Failed to save tables.", "OK");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving tables: {ex.Message}");
                await DisplayAlert("Error", "Failed to save tables.", "OK");
            }
            App.Current.MainPage = new NavigationPage(new LoginPage());
        }
    }
}

