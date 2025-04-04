using Xamarin.Forms;
using QuickReserve.Models;
using System.Collections.Generic;
using System;
using QuickReserve.Services;
using System.Threading.Tasks;

namespace QuickReserve.Views
{
    public partial class ReservationSummaryPage : ContentPage
    {
        public string ReservationDateTime { get; set; }
        public string TableId { get; set; }
        public string TableNumber { get; set; }
        public int GuestCount { get; set; }
        public string UserId { get; set; }
        public string RestaurantId { get; set; }
        public List<Food> OrderItemsForSummaryPage { get; set; } = new List<Food>();

        public bool IsItemsVisible => OrderItemsForSummaryPage.Count > 0;
        public double TotalAmount => CalculateTotalAmount();

        private RestaurantService _restaurantService;

        // Konstruktor
        public ReservationSummaryPage(List<Food> orderItems, string reservationDateTime, string tableId, int guestCount)
        {
            InitializeComponent();

            if (string.IsNullOrEmpty(reservationDateTime) || string.IsNullOrEmpty(tableId) || guestCount <= 0)
            {
                throw new ArgumentNullException("Reservation data is not properly passed.");
            }

            OrderItemsForSummaryPage = orderItems; // Rendelési tételek inicializálása
            ReservationDateTime = reservationDateTime;
            TableId = tableId;
            GuestCount = guestCount;
            UserId = Application.Current.Properties["userId"].ToString();
            RestaurantId = Application.Current.Properties["restaurantId"].ToString();

            _restaurantService = RestaurantService.Instance;
        }

        // Aszinkron műveletek a megjelenéskor
        protected override async void OnAppearing()
        {
            base.OnAppearing();

            // Mutatjuk a töltőt és elrejtjük a tartalmat
            loadingIndicator.IsRunning = true;
            loadingIndicator.IsVisible = true;
            contentStack.IsVisible = false;

            // Aszinkron művelet a táblázat információinak lekérésére
            TableNumber = await GetTableNumber();

            // Elrejtjük a töltőt és megjelenítjük a tartalmat
            loadingIndicator.IsRunning = false;
            loadingIndicator.IsVisible = false;
            contentStack.IsVisible = true;

            loadingIndicator.IsVisible = false;
            contentStack.IsVisible = true;
            finalContentStack.IsVisible = true;

            BindingContext = this; // Beállítjuk a BindingContext-et a megfelelő működéshez
        }

        // Tábla információk lekérése
        public async Task<string> GetTableNumber()
        {
            Table selectedTable = await _restaurantService.GetTableById(RestaurantId, TableId);
            return selectedTable.TableNumber.ToString();
        }

        // A véglegesítés kezelése
        private async void OnFinalizeReservation(object sender, EventArgs e)
        {
            UserService userService = UserService.Instance;
            var reservation = new Reservation()
            {
                UserId = this.UserId,
                RestaurantId = this.RestaurantId,
                TableId = this.TableId,
                ReservationDateTime = this.ReservationDateTime,
                GuestCount = this.GuestCount,
                CreatedAt = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"),
                Foods = this.OrderItemsForSummaryPage,
                Status = "In progress",
                TableNumber = this.TableNumber,
                UserName = await userService.GetUserNameByUserId(UserId)
            };

            try
            {
                // Foglalás mentése az adatbázisba
                ReservationService reservationService = ReservationService.Instance;
                await reservationService.AddReservation(reservation);

                await _restaurantService.MarkTableAsReserved(RestaurantId, TableId);

                // Üzenet a felhasználónak a sikeres véglegesítésről
                await DisplayAlert("Success", "Your reservation has been finalized!", "OK");

                await Navigation.PushAsync(new AboutPage());

                // Ha sikeres a foglalás mentése, akkor kiürítjük a rendelési tételeket
                OrderItemsForSummaryPage.Clear();
                OrderItemsListView.ItemsSource = null; // Frissítjük a ListView-t
            }
            catch (Exception ex)
            {
                // Hiba kezelése, ha a foglalás mentése nem sikerül
                await DisplayAlert("Error", "There was an error finalizing your reservation. Please try again.", "OK");
                Console.WriteLine($"Error finalizing reservation: {ex.Message}");
            }
        }

        // Összeg számítása
        private double CalculateTotalAmount()
        {
            double total = 0;
            foreach (var item in OrderItemsForSummaryPage)
            {
                total += item.Price;
            }
            return total;
        }
    }
}
