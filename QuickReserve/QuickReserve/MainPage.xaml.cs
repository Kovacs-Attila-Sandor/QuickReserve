using QuickReserve.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace QuickReserve
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class MainPage : TabbedPage
	{
        public MainPage(string userType) // "User" vagy "Restaurant"
        {
            InitializeComponent();

            // Mindenki számára elérhető lapok
            Children.Add(new AboutPage { Title = "Home", IconImageSource = "home_icon.png" });

            if (userType == "USER")
            {
                Children.Add(new UserReservationsPage { Title = "Reservations", IconImageSource = "reservation_icon.png" });
                Children.Add(new UserProfilePage { Title = "Profile", IconImageSource = "profile_icon.png" });
            }
            else if (userType == "RESTAURANT")
            {
                Children.Add(new RestaurantReservationsPage { Title = "Reservations", IconImageSource = "reservation_icon.png" });
                Children.Add(new RestaurantProfilePage { Title = "Restaurant Profile", IconImageSource = "profile_icon.png" });
            }
        }
    }
}