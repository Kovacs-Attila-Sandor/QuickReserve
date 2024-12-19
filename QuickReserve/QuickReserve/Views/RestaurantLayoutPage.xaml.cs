using QuickReserve.Models;
using QuickReserve.Services;
using System;
using System.Collections.Generic;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace QuickReserve.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class RestaurantLayoutPage : ContentPage
    {
        public RestaurantLayoutPage(Restaurant restaurant)
        {
            InitializeComponent();

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
                        WidthRequest = 50, // Adjust the width of the buttons
                        HeightRequest = 50 // Adjust the height of the buttons
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
                button.BorderColor = Color.Black;

                // Prompt the user for input
                string result = await DisplayPromptAsync("Number of seats:", "Enter the number of people:",
                                                         placeholder: "Number of people", maxLength: 3, keyboard: Keyboard.Text);

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

        protected void GoToAboutPage(object sender, EventArgs e)
        {
            App.Current.MainPage = new NavigationPage(new AboutPage());
        }
    }
}
