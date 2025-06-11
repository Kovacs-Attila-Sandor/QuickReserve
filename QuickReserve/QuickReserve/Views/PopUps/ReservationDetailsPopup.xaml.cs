using Rg.Plugins.Popup.Pages;
using Rg.Plugins.Popup.Services;
using QuickReserve.Models;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using QuickReserve.Services;

namespace QuickReserve.Views.PopUps
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ReservationDetailsPopup : PopupPage
    {
        private Reservation _reservation;
        private ObservableCollection<FoodQuantity> _foodDetails = new ObservableCollection<FoodQuantity>();
       
        private RestaurantService _restaurantService;
        private ReservationService _reservationService;
       

        public ObservableCollection<FoodQuantity> FoodDetails
        {
            get => _foodDetails;
            set
            {
                _foodDetails = value;
                OnPropertyChanged();
            }
        }

        public bool HasFoods => _reservation?.HasFoods ?? false;

        public ReservationDetailsPopup(Reservation reservation)
        {
            InitializeComponent();
            _reservation = reservation ?? throw new ArgumentNullException(nameof(reservation), "Reservation cannot be null");
            _restaurantService = RestaurantService.Instance;
            _reservationService = ReservationService.Instance;
            BindingContext = _reservation;
            LoadFoodDetails();
        }

        private async void LoadFoodDetails()
        {
            if (_reservation == null || !_reservation.HasFoods)
            {
                Console.WriteLine($"No foods to load. HasFoods: {_reservation?.HasFoods}, ReservationId: {_reservation?.ReservationId}");
                Device.BeginInvokeOnMainThread(() =>
                {
                    FoodsLabel.IsVisible = false;
                    FoodCollectionView.IsVisible = false;
                    ContentStack.IsVisible = true;
                });
                return;
            }

            Device.BeginInvokeOnMainThread(() =>
            {
                LoadingIndicator.IsRunning = true;
                LoadingIndicator.IsVisible = true;
                ContentStack.IsVisible = false;
            });

            Console.WriteLine($"Loading food details for reservation {_reservation.ReservationId} with {(_reservation.FoodIdsAndQuantity?.Count ?? 0)} items at {DateTime.Now:HH:mm:ss} EEST");

            try
            {
                FoodDetails.Clear();
                foreach (var item in _reservation.FoodIdsAndQuantity)
                {
                    Console.WriteLine($"Processing FoodId: {item.FoodId}, Quantity: {item.Quantity}");
                    var food = await GetFoodById(item.FoodId);
                    if (food != null)
                    {
                        Console.WriteLine($"Found food: {food.Name} for FoodId: {item.FoodId}");
                        FoodDetails.Add(new FoodQuantity { Name = food.Name, Quantity = item.Quantity });
                    }
                    else
                    {
                        Console.WriteLine($"No food found for FoodId: {item.FoodId}");
                    }
                }
                Console.WriteLine($"FoodDetails count after loading: {FoodDetails.Count}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading food details: {ex.Message}");
                Device.BeginInvokeOnMainThread(() =>
                {
                    DisplayAlert("Error", "Failed to load food details.", "OK");
                });
            }
            finally
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    LoadingIndicator.IsRunning = false;
                    LoadingIndicator.IsVisible = false;
                    ContentStack.IsVisible = true;
                    FoodsLabel.IsVisible = _reservation.HasFoods && FoodDetails.Any();
                    FoodCollectionView.IsVisible = _reservation.HasFoods && FoodDetails.Any();
                    Console.WriteLine($"UI updated. FoodsLabel.IsVisible: {FoodsLabel.IsVisible}, FoodCollectionView.IsVisible: {FoodCollectionView.IsVisible}");
                });
            }
        }

        private async Task<Food> GetFoodById(string foodId)
        {
            try
            {
                Console.WriteLine($"Fetching food with ID: {foodId} at {DateTime.Now:HH:mm:ss} EEST");
                var restaurantData = await FirebaseService
                    .Client
                    .Child("Restaurant")
                    .OnceAsync<Restaurant>();

                Console.WriteLine($"Restaurant data count: {restaurantData?.Count ?? 0}");
                foreach (var item in restaurantData)
                {
                    var restaurant = item.Object;
                    if (restaurant?.Foods != null)
                    {
                        Console.WriteLine($"Restaurant has {restaurant.Foods.Count} foods");
                        var food = restaurant.Foods.FirstOrDefault(f => f.FoodId == foodId);
                        if (food != null)
                        {
                            Console.WriteLine($"Food found: {food.Name}");
                            return food;
                        }
                    }
                    else
                    {
                        Console.WriteLine($"Restaurant or Foods is null for restaurant: {item.Key}");
                    }
                }

                Console.WriteLine($"No food found for FoodId: {foodId}");
                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching food by ID: {ex.Message}");
                return null;
            }
        }

        private async void OnCloseButtonClicked(object sender, EventArgs e)
        {
            await PopupNavigation.Instance.PopAsync(true);
        }

        private async void OnMarkAsDoneButtonClicked(object sender, EventArgs e)
        {
            MarkAsDoneButton.IsEnabled = false;
            await _restaurantService.MarkTableAsAvailable(_reservation.RestaurantId, _reservation.TableId);
            await _reservationService.UpdateReservationStatus(_reservation.ReservationId, "DONE");
            await PopupNavigation.Instance.PopAsync(true);
            MarkAsDoneButton.IsEnabled = true;

        }
    }

    public class FoodQuantity
    {
        public string Name { get; set; }
        public int Quantity { get; set; }
    }
}