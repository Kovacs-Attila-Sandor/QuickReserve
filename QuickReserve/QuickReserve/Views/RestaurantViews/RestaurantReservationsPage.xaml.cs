using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using QuickReserve.Models;
using QuickReserve.Services;
using System.Linq;

namespace QuickReserve.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class RestaurantReservationsPage : ContentPage
    {
        private ReservationService _reservationService;
        private RestaurantService _restaurantService;
        private string _restaurantId;
        private readonly string _userId;

        private bool _isBusy;
        public bool IsBusy
        {
            get => _isBusy;
            set
            {
                _isBusy = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(IsNotBusy));
            }
        }

        public bool IsNotBusy => !IsBusy;
        public bool _isDoneVisible = false;

        List<Reservation> InProgressReservations = new List<Reservation>();
        List<Reservation> DoneReservations = new List<Reservation>();

        public RestaurantReservationsPage()
        {
            InitializeComponent();

            _reservationService = ReservationService.Instance;
            _restaurantService = RestaurantService.Instance;
            _userId = App.Current.Properties["userId"].ToString();
            BindingContext = this;

            // Start initialization in constructor
            Task.Run(async () => await InitializePage());
        }

        private async Task InitializePage()
        {
            try
            {
                IsBusy = true;

                // Initialize restaurant ID
                _restaurantId = await _restaurantService.GetRestaurantIdByUserId(_userId);

                // Load reservations
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
                if (string.IsNullOrEmpty(_restaurantId))
                    throw new InvalidOperationException("Restaurant ID not initialized");

                List<Reservation> reservations = await _reservationService.GetReservationsByRestaurantId(_restaurantId);

                Device.BeginInvokeOnMainThread(() =>
                {
                    InProgressReservations = reservations.Where(r => r.Status == "In progress").ToList();
                    DoneReservations = reservations.Where(r => r.Status == "DONE").ToList();
                    ReservationsListView.ItemsSource = InProgressReservations;
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


        private async void OnMarkAsDoneClicked(object sender, EventArgs e)
        {
            var button = (Button)sender;
            var reservation = (Reservation)button.BindingContext;

            if (reservation == null)
            {
                Console.WriteLine("Reservation is null");
                return;
            }

            if (string.IsNullOrEmpty(reservation.Status))
            {
                Console.WriteLine("Reservation status is null or empty");
                return;
            }

            reservation.Status = "DONE";

            try
            {
                if (string.IsNullOrEmpty(reservation.RestaurantId) || string.IsNullOrEmpty(reservation.TableId))
                {
                    Console.WriteLine("Restaurant or Table ID is null or empty.");
                    return;
                }
                bool succes = await _restaurantService.MarkTableAsAvailable(reservation.RestaurantId, reservation.TableId);
                if (!succes)
                {
                    return;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in MarkTableAsAvailable: {ex.Message}");
            }

            // Update in the database
            try
            {
                bool result = await _reservationService.UpdateReservationStatus(reservation.ReservationId, reservation.Status);
                if (result)
                {
                    // If successful, update the reservations list
                    await DisplayAlert("Successful Update", "The reservation status has been successfully updated.", "OK");


                    LoadReservations(); // Reload the list
                }
                else
                {
                    await DisplayAlert("Error", "Failed to update the reservation status.", "OK");
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", "An error occurred while updating the status. Please try again.", "OK");
                Console.WriteLine($"Error while updating the status: {ex.Message}");
            }

        }
        private void OnViewDoneReservationsClicked(object sender, EventArgs e)
        {
            var button = (Button)sender;

            if (!_isDoneVisible)
            {
                button.Text = "View In Progress Reservations";
                ReservationsListView.ItemsSource = DoneReservations;
            }
            else
            {
                button.Text = "View Done Reservations";
                ReservationsListView.ItemsSource = InProgressReservations;
            }
            _isDoneVisible = !_isDoneVisible;
        }

        private async void OnViewReservationClicked(object sender, EventArgs e)
        {
            
        }
    }
     
}
