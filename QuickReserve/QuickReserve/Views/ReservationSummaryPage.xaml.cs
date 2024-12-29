using Xamarin.Forms;
using QuickReserve.Models;
using System.Collections.Generic;
using System;
using Xamarin.Essentials;
using System.Linq;

namespace QuickReserve.Views
{
    public partial class ReservationSummaryPage : ContentPage
    {
        public string ReservationDateTime { get; set; }
        public string TableId { get; set; }
        public int GuestCount { get; set; }
        public List<Food> OrderItems { get; set; } = new List<Food>();

        public ReservationSummaryPage(string reservationDateTime, string tableId, int guestCount)
        {
            InitializeComponent();

            // Adatok ellenőrzése
            if (string.IsNullOrEmpty(reservationDateTime) || string.IsNullOrEmpty(tableId) || guestCount <= 0)
            {
                throw new ArgumentNullException("Reservation data is not properly passed.");
            }

            ReservationDateTime = reservationDateTime;
            TableId = tableId;
            GuestCount = guestCount;

            BindingContext = this;
        } 

        private async void OnFinalizeReservation(object sender, EventArgs e)
        {
            if (OrderItems.Count == 0)
            {
                await DisplayAlert("No items", "You need to add items to finalize the reservation.", "OK");
                return;
            }

            string message = $"Your reservation at table {TableId} for {GuestCount} guests on {ReservationDateTime} has been finalized with {OrderItems.Count} items.";
            await DisplayAlert("Reservation Finalized", message, "OK");

            // A rendelés törlése véglegesítés után
            OrderItems.Clear();

            // Frissítjük a ListView-t a törlés után
            OrderItemsListView.ItemsSource = null;
        }
    }
}
