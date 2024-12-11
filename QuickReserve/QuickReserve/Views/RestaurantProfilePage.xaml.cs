using System;
using Xamarin.Forms;
using QuickReserve.Models;
using QuickReserve.Services;
using QuickReserve.Converter;
using System.Collections.Generic;

namespace QuickReserve.Views
{
    public partial class RestaurantProfilePage : ContentPage
    {
        public RestaurantProfilePage(string userName)
        {
            InitializeComponent();
            Title = "Restaurant Profile"; // Az automatikus fejléc címe
            LoadRestaurantData(userName); // Aszinkron adatbetöltés
        }

        private async void LoadRestaurantData(string userName)
        {
            try
            {
                // Aktiváld a betöltési képernyőt
                loadingIndicator.IsVisible = true;
                contentLayout.IsVisible = false;

                // Étterem adatok aszinkron lekérése
                var restaurantService = new RestaurantService();
                var restaurant = await restaurantService.GetRestaurantByName(userName);

                if (restaurant != null)
                {
                    // Base64 képek konvertálása ImageSource-ra az étteremhez
                    if (restaurant.ImageBase64List != null && restaurant.ImageBase64List.Count > 0)
                    {
                        restaurant.ImageSourceUri = ImageConverter.ConvertBase64ToImageSource(restaurant.ImageBase64List[0]);

                        // Az összes kép konvertálása (ha szükséges a ListView-hoz)
                        foreach (var base64Image in restaurant.ImageBase64List)
                        {
                            restaurant.ImageSourceList.Add(ImageConverter.ConvertBase64ToImageSource(base64Image));
                        }
                    }

                    // Képek konvertálása az ételekhez
                    if (restaurant.Foods != null)
                    {
                        foreach (var food in restaurant.Foods)
                        {
                            if (food.Pictures != null && food.Pictures.Count > 0)
                            {
                                var foodImages = new List<ImageSource>();
                                foreach (var base64Picture in food.Pictures)
                                {
                                    foodImages.Add(ImageConverter.ConvertBase64ToImageSource(base64Picture));
                                }
                                food.ImageSources = foodImages;
                            }
                        }
                    }

                    BindingContext = restaurant;
                }
                else
                {
                    await DisplayAlert("Error", "No restaurant found with the given name.", "OK");
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"An error occurred while loading the restaurant: {ex.Message}", "OK");
            }
            finally
            {
                // Tedd láthatóvá a valódi tartalmat és rejtsd el a betöltési képernyőt
                loadingIndicator.IsVisible = false;
                contentLayout.IsVisible = true;
            }
        }
    }
}
