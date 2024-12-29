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
        public List<Food> OrderItems { get; set; }

        public OrderPage(List<Food> orderItems, string reservationDateTime, string tableId, int guestCount)
        {
            InitializeComponent();
            OrderItems = orderItems;
            BindingContext = this;
            ReservationDateTime = reservationDateTime;
            TableId = tableId;
            GuestCount = guestCount;
        }

        // Az eseménykezelő a törléshez
        private void OnRemoveItemClicked(object sender, EventArgs e)
        {
            // Az ItemContext (BindingContext) elérése a törölt étkezéshez
            var button = sender as Button;
            var foodItem = button?.BindingContext as Food;

            if (foodItem != null)
            {
                // Eltávolítjuk a rendelési tételek listájából
                OrderItems.Remove(foodItem);

                //Oldal frissitese
                BindingContext = null;
                BindingContext = this;

                // Ha már nincs több tétel, navigáljunk vissza az előző oldalra
                if (OrderItems.Count == 0)
                {
                    Navigation.PopAsync();
                }
            }
        }


        private async void OnPlaceOrderClicked(object sender, EventArgs e)
        {
            if (OrderItems.Count == 0)
            {
                await DisplayAlert("No items", "You need to add items to the order before placing it.", "OK");
                return;
            }

            var reservationSummaryPage = new ReservationSummaryPage(ReservationDateTime, TableId, GuestCount)
            {
                OrderItems = OrderItems
            };
            // Navigálás a ReservationSummaryPage oldalra
            await Navigation.PushAsync(reservationSummaryPage);   
        }
    }
}
