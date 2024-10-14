using QuickReserve.ViewModels;
using System.ComponentModel;
using Xamarin.Forms;

namespace QuickReserve.Views
{
    public partial class ItemDetailPage : ContentPage
    {
        public ItemDetailPage()
        {
            InitializeComponent();
            BindingContext = new ItemDetailViewModel();
        }
    }
}