using Xamarin.Forms;
using System.Collections.Generic;
using QuickReserve.Models;
using System;

namespace QuickReserve.Views
{
    public partial class OrderPage : ContentPage
    {
        public string ReservationDateTime { get; set; }
        public string TableId { get; set; }
        public int GuestCount { get; set; }
        public Dictionary<Food, int> OrderItems { get; set; }
        public OrderPage(List<Food> orderItems, string reservationDateTime, string tableId, int guestCount)
        {
            InitializeComponent();
            OrderItems = new Dictionary<Food, int>();
            foreach (var food in orderItems)
            {
                OrderItems[food] = 1; // Alapértelmezett mennyiség: 1
            }
            BindingContext = this;
            ReservationDateTime = reservationDateTime;
            TableId = tableId;
            GuestCount = guestCount;
        }

        // Az eseménykezelő a törléshez
        private void OnRemoveItemClicked(object sender, EventArgs e)
        {
            var button = sender as Button;
            var foodItem = button?.BindingContext as KeyValuePair<Food, int>?;

            if (foodItem.HasValue)
            {
                OrderItems.Remove(foodItem.Value.Key);

                // Oldal frissítése
                BindingContext = null;
                BindingContext = this;

                // Ha már nincs több tétel, navigáljunk vissza
                if (OrderItems.Count == 0)
                {
                    Navigation.PopAsync();
                }
            }
        }

        private async void OnBackButtonClicked(object sender, EventArgs e)
        {
            await Navigation.PopAsync();
        }

        private async void OnPlaceOrderClicked(object sender, EventArgs e)
        {
            if (OrderItems.Count == 0)
            {
                await DisplayAlert("No items", "You need to add items to the order before placing it.", "OK");
                return;
            }
        
            // Navigálás a ReservationSummaryPage oldalra
            var reservationSummaryPage = new ReservationSummaryPage(OrderItems, ReservationDateTime, TableId, GuestCount);
            await Navigation.PushAsync(reservationSummaryPage);
        }

        private void OnIncreaseQuantityClicked(object sender, EventArgs e)
        {
            var button = sender as Button;
            var foodItem = button?.BindingContext as KeyValuePair<Food, int>?;

            if (foodItem.HasValue)
            {
                OrderItems[foodItem.Value.Key] = foodItem.Value.Value + 1;

                // Oldal frissítése
                BindingContext = null;
                BindingContext = this;
            }
        }

        private void OnDecreaseQuantityClicked(object sender, EventArgs e)
        {
            var button = sender as Button;
            var foodItem = button?.BindingContext as KeyValuePair<Food, int>?;

            if (foodItem.HasValue && foodItem.Value.Value > 1)
            {
                OrderItems[foodItem.Value.Key] = foodItem.Value.Value - 1;

                // Oldal frissítése
                BindingContext = null;
                BindingContext = this;
            }
            else if (foodItem.HasValue && foodItem.Value.Value == 1)
            {
                OrderItems.Remove(foodItem.Value.Key);

                // Oldal frissítése
                BindingContext = null;
                BindingContext = this;

                // Ha már nincs több tétel, navigáljunk vissza
                if (OrderItems.Count == 0)
                {
                    Navigation.PopAsync();
                }
            }
        }
    }
}
