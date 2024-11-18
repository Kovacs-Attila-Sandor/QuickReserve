using Firebase.Database;
using QuickReserve.Models;
using QuickReserve.Services;
using QuickReserve.Views;
using System;
using System.Collections.Generic;
using System.Data;
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
        }

        protected override async void OnStart()
        {
            /*
            var restaurant = new Restaurant
            {
                RestaurantId = "restaurant123",
                Name = "The Gourmet Kitchen",
                Address = new Location
                {
                    Country = "Hungary",
                    City = "Budapest",
                    Street = "Andrássy Avenue",
                    Number = "12"
                },
                PhoneNumber = "+1 555-1234",
                Email = "contact@gourmetkitchen.com",
                Rating = 4.5,
                ShortDescription = "A fine dining experience with exquisite flavors.",
                LongDescription = "At The Gourmet Kitchen, we provide a variety of gourmet dishes made with the finest ingredients. Enjoy our comfortable, elegant setting while savoring our delicious menu items.",
                Foods = new List<Food>
                {
                    new Food
                    {
                        FoodId = "food001",
                        Name = "Grilled Salmon",
                        Price = 19.99,
                        Description = "Fresh salmon grilled to perfection.",
                        Category = "Main Course"
                    },
                    new Food
                    {
                        FoodId = "food002",
                        Name = "Caesar Salad",
                        Price = 8.99,
                        Description = "Crisp romaine lettuce with Caesar dressing.",
                        Category = "Appetizer"
                    }
                },
                Tables = new List<Table>
                {
                    new Table
                    {
                        TableId = "table001",
                        TableNumber = 1,
                        Capacity = 4,
                        AvailabilityStatus = "Available",
                        Location = new TableLocation
                        {
                            StartRow = 1,
                            StartColumn = 1,
                            EndRow = 1,
                            EndColumn = 2
                        }
                    },
                    new Table
                    {
                        TableId = "table002",
                        TableNumber = 2,
                        Capacity = 2,
                        AvailabilityStatus = "Occupied",
                        Location = new TableLocation
                        {
                            StartRow = 1,
                            StartColumn = 3,
                            EndRow = 1,
                            EndColumn = 4
                        }
                    }
                }
            };
            RestaurantService restaurantService = new RestaurantService();
            await restaurantService.addRestaurant(restaurant);
            */
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
