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
    public partial class AddMenuPage : ContentPage
    {
        private string _restaurantId;
        private RestaurantService _restaurantService;
        public AddMenuPage(string restaurantId)
        {
            InitializeComponent();
            _restaurantId = restaurantId;
            _restaurantService = new RestaurantService();
        }
        protected async void AddItem(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(txtMenuItemName.Text) &&
                double.TryParse(txtPrice.Text, out double price) &&
                !string.IsNullOrEmpty(txtMenuItemDescription.Text) &&
                !string.IsNullOrEmpty(txtMenuItemType.Text))
            {
                Food newFood = new Food
                {
                    Name = txtMenuItemName.Text.Trim(),
                    Price = price,
                    Description = txtMenuItemDescription.Text.Trim(),
                    Category = txtMenuItemType.Text.Trim(),
                    FoodId = Guid.NewGuid().ToString()
                };

                bool success = await _restaurantService.AddFoodToRestaurant(_restaurantId, newFood);
                if (success)
                    await DisplayAlert("SUCCESS", "Food added successfully", "OK");
                else
                    await DisplayAlert("ERROR", "Failed to add food to restaurant", "OK");
            }
            else
            {
                await DisplayAlert("ERROR", "Please fill all the fields", "OK");
            }
            txtMenuItemDescription.Text = "";
            txtPrice.Text = "";
            txtMenuItemType.Text = "";
            txtMenuItemName.Text = "";
        }
        protected void AddLayout(object sender, EventArgs e)
        {
            App.Current.MainPage = new NavigationPage(new CreateLayoutPage(_restaurantId));
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

        private Label GetPlaceholderLabel(Entry entry)
        {
            return entry == txtMenuItemDescription ? lblMenuItemDescriptionPlaceholder :
                   entry == txtMenuItemName ? lblMenuItemNamePlaceholder :
                   entry == txtMenuItemType ? lblMenuItemTypePlaceholder :
                   entry == txtPrice ? lblMenuItemPrice : null;
        }

        private string GetPlaceholderText(Entry entry)
        {
            return entry == txtMenuItemDescription ? "Menu Item Description" :
                   entry == txtMenuItemName ? "Menu Item Name" :
                   entry == txtMenuItemType ? "Menu Item Type" :
                   entry == txtPrice ? "Price" : "";               
        }
    }
}