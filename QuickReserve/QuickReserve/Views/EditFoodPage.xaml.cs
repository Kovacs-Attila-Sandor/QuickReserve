using Xamarin.Forms;
using QuickReserve.Models;
using QuickReserve.Services;
using System;

namespace QuickReserve.Views
{
    public partial class EditFoodPage : ContentPage
    {
        private string _foodId;
        private string _restaurantId;
        private RestaurantService _restaurantService;

        public EditFoodPage(string restaurantId, string foodId)
        {
            InitializeComponent();
            _foodId = foodId;
            _restaurantId = restaurantId;
            _restaurantService = new RestaurantService();
            LoadFoodDetails(foodId);
        }

        private async void LoadFoodDetails(string foodId)
        {
            var food = await _restaurantService.GetFoodById(foodId);

            if (food != null)
            {
                foodNameEntry.Text = food.Name;
                foodPriceEntry.Text = food.Price.ToString();
                foodDescriptionEntry.Text = food.Description;
            }
        }

        private async void OnSaveClicked(object sender, EventArgs e)
        {
            // Az új étel adatok összegyűjtése
            var updatedFood = new Food
            {
                FoodId = _foodId, // Az étel id-ja
                Name = foodNameEntry.Text,
                Price = double.Parse(foodPriceEntry.Text),
                Description = foodDescriptionEntry.Text,
                //Picture = updatedFoodImage // Ha van kép is
            };

            // Használjuk a restaurantId-t és foodId-t a frissítéshez
            bool result = await _restaurantService.UpdateFoodDetails(_restaurantId, _foodId, updatedFood);
            if (result)
            {
                await DisplayAlert("Success", "Food details updated successfully.", "OK");
                // Térj vissza vagy frissítsd az adatokat
            }
            else
            {
                await DisplayAlert("Error", "Failed to update food details.", "OK");
            }
        }

        private async void OnCancelClicked(object sender, EventArgs e)
        {
            await Navigation.PopAsync();  // Navigate back to the previous page without saving
        }
    }
}
