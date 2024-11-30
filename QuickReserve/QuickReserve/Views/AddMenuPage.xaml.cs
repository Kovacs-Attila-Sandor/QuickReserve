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
    public partial class AddMenuPage : ContentPage
    {
        public AddMenuPage()
        {
            InitializeComponent();
        }
        protected void AddItem(object sender, EventArgs e)
        {
            
        }
        protected void AddLayout(object sender, EventArgs e)
        {
            App.Current.MainPage = new NavigationPage(new CreateLayoutPage());
        }
        protected void GoToLoginPage(object sender, EventArgs e)
        {
            App.Current.MainPage = new NavigationPage(new LoginPage());
        }
    }
}