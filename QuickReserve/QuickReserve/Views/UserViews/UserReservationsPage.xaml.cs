using Rg.Plugins.Popup.Services;
using QuickReserve.Models;
using QuickReserve.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using QuickReserve.Views.PopUps;

namespace QuickReserve.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class UserReservationsPage : ContentPage
    {
        private ReservationService _reservationService;
        private string _userId;

        private bool _isBusy;
        public bool IsBusy
        {
            get => _isBusy;
            set
            {
                _isBusy = value;
                OnPropertyChanged();
            }
        }

        private bool _isDoneVisible;
        private List<Reservation> FinishedReservations = new List<Reservation>();
        private List<Reservation> ActiveReservations = new List<Reservation>();

        public UserReservationsPage()
        {
            InitializeComponent();
            _reservationService = ReservationService.Instance;
            _userId = App.Current.Properties["userId"].ToString();
            BindingContext = this;

            // Start initialization
            Task.Run(async () => await InitializePage());
        }

        private async Task InitializePage()
        {
            try
            {
                IsBusy = true;
                await LoadReservations();
            }
            catch (Exception ex)
            {
                Device.BeginInvokeOnMainThread(async () =>
                {
                    await DisplayAlert("Error", $"Failed to initialize: {ex.Message}", "OK");
                });
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async Task LoadReservations()
        {
            try
            {
                List<Reservation> reservations = await _reservationService.GetReservationsByUserId(_userId);

                Device.BeginInvokeOnMainThread(() =>
                {
                    ActiveReservations = reservations.Where(r => r.Status == "In progress").ToList();
                    FinishedReservations = reservations.Where(r => r.Status == "DONE").ToList();
                    ReservationsListView.ItemsSource = ActiveReservations;
                });
            }
            catch (Exception ex)
            {
                Device.BeginInvokeOnMainThread(async () =>
                {
                    await DisplayAlert("Error", $"Failed to load reservations: {ex.Message}", "OK");
                });
            }
        }

        private void OnViewDoneReservationsClicked(object sender, EventArgs e)
        {
            var button = (Button)sender;

            if (!_isDoneVisible)
            {
                button.Text = "View Active Reservations";
                ReservationsListView.ItemsSource = FinishedReservations;
            }
            else
            {
                button.Text = "View Done Reservations";
                ReservationsListView.ItemsSource = ActiveReservations;
            }
            _isDoneVisible = !_isDoneVisible;
        }

        private async void OnViewReservationDetailsClicked(object sender, EventArgs e)
        {
            var button = (Button)sender;
            var reservation = (Reservation)button.BindingContext;

            if (reservation != null)
            {
                await PopupNavigation.Instance.PushAsync(new ReservationDetailsPopup(reservation), true);
            }
            else
            {
                await DisplayAlert("Error", "No reservation selected.", "OK");
            }
        }
    }
}