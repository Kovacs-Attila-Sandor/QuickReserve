using Firebase.Database;
using QuickReserve.Models;
using QuickReserve.Services;
using QuickReserve.Views;
using System;
using System.Collections.Generic;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace QuickReserve
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            DependencyService.Register<MockDataStore>();
            MainPage = new NavigationPage(new LoginPage());
            Restaurant restaurant = new Restaurant
            {
                RestaurantId = 1,
                Name = "Paradise Restaurant",
                PhoneNumber = "0765742874",
                Email = "info@paradiserestaurant.com",
                Rating = 5,
                ShortDescription = "A charming place with delicious food.",
                LongDescription = "A charming place located in the heart of the city, offering a wide variety of dishes.",
                Location = new Location
                {
                    Country = "Hungary",
                    City = "Budapest",
                    Street = "Main Street",
                    Number = 5
                },
                Products = new List<string> { "Pizza", "Burger", "Pasta" },
                Tables = new List<Table>
            {
                new Table
                {
                    TableId = 1,
                    TableNumber = 1,
                    Capacity = 4,
                    AvailabilityStatus = "Available"
                },
                new Table
                {
                    TableId = 2,
                    TableNumber = 2,
                    Capacity = 2,
                    AvailabilityStatus = "Reserved"
                }
            },
                Foods = new List<Food>
            {
                new Food
                {
                    FoodId = "F001",
                    Name = "Margherita Pizza",
                    Price = 8.99,
                    Description = "Classic Margherita with fresh basil.",
                    Category = "Pizza"
                },
                new Food
                {
                    FoodId = "F002",
                    Name = "Caesar Salad",
                    Price = 5.99,
                    Description = "Fresh Caesar Salad with croutons and cheese.",
                    Category = "Salad"
                }
                }
            };
            //Console.WriteLine("\n\n\n\n\n\n\n" + restaurant.ToString());
            //restaurant.Location.City = "Debrecen";
            //Console.WriteLine(restaurant.ToString());
        }

        protected override void OnStart()
        {

        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
