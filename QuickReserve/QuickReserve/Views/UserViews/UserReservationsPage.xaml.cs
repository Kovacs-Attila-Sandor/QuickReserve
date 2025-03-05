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
    public partial class UserReservationsPage : ContentPage
    {

        private ReservationService _reservationService;
        private string _userId;

        public bool _isDoneVisible = false;

        List<Reservation> FinishedReservations = new List<Reservation>();
        List<Reservation> ActiveReservation = new List<Reservation>();

        public UserReservationsPage()
        {
            InitializeComponent();

            _reservationService = new ReservationService();
            _userId = App.Current.Properties["userId"].ToString();

            LoadReservations();
        }


        private async void LoadReservations()
        {
            try
            {
                // Foglalások betöltése az adatbázisból
                List<Reservation> reservations = await _reservationService.GetReservationsByUserId(_userId);

                FinishedReservations = reservations.Where(r => r.Status == "DONE").ToList();
                ActiveReservation = reservations.Where(r => r.Status == "In progress").ToList();

                // A foglalások megjelenítése a ListView-ban           
                ReservationsListView.ItemsSource = ActiveReservation;
            }
            catch (Exception ex)
            {
                await DisplayAlert("Hiba", "Nem sikerült betölteni a foglalásokat. Kérlek, próbáld újra.", "OK");
                Console.WriteLine($"Hiba a foglalások betöltésekor: {ex.Message}");
            }
        }

        private async void OnViewDoneReservationsClicked(object sender, EventArgs e)
        {
            var button = (Button)sender;

            if (!_isDoneVisible)
            {
                button.Text = "View Active Reservations";
                ReservationsListView.ItemsSource = FinishedReservations;
            }
            else
            {
                button.Text = "View Reservations history";
                ReservationsListView.ItemsSource = ActiveReservation;
            }
            _isDoneVisible = !_isDoneVisible;
        }
    }
}