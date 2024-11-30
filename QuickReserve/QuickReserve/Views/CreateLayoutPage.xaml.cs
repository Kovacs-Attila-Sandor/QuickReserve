using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace QuickReserve.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CreateLayoutPage : ContentPage
    {
        public CreateLayoutPage()
        {
            InitializeComponent();

            // Create the grid dynamically
            var dynamicGrid = DynamicGrid;

            // Define 15 rows
            for (int i = 0; i < 12; i++)
            {
                dynamicGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Star });
            }

            // Define 10 columns
            for (int j = 0; j < 7; j++)
            {
                dynamicGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Star });
            }

            // Add 15x10 buttons dynamically
            for (int row = 0; row < 12; row++)
            {
                for (int col = 0; col < 7; col++)
                {


                    var button = new Button
                    {
                        Text =  null,
                        FontSize=12,
                        BorderWidth=3,
                        BackgroundColor=Color.LightGray,
                        HorizontalOptions = LayoutOptions.FillAndExpand,
                        VerticalOptions = LayoutOptions.FillAndExpand,
                        IsVisible =true,
                        WidthRequest = 40, // Adjust the width of the buttons
                        HeightRequest = 40 // Adjust the height of the buttons
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
        protected void GoToLoginPage(object sender, EventArgs e)
        {
            App.Current.MainPage = new NavigationPage(new LoginPage());
        }
    }
}
