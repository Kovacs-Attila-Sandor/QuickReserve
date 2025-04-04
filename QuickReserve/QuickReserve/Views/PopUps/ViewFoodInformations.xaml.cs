using Rg.Plugins.Popup.Pages;
using Rg.Plugins.Popup.Services;
using Xamarin.Forms;
using System.Threading.Tasks;
using System;
using QuickReserve.Models;
using QuickReserve.Converter; // Ha az ImageConverter-t használod

namespace QuickReserve.Views.PopUps
{
    public partial class ViewFoodInformations : PopupPage
    {
        private Food _food; // Tároljuk a Food objektumot

        public ViewFoodInformations(Food food)
        {
            InitializeComponent();

            _food = food;

            // Ha van kép, konvertáljuk ImageSource-ra
            if (!string.IsNullOrEmpty(food.Picture))
            {
                food.ImageSource = ImageConverter.ConvertBase64ToImageSource(food.Picture);
            }

            // A BindingContext-et az ételre állítjuk
            BindingContext = _food;

            // Popup mérete
            var frame = this.Content as Frame;
            if (frame != null)
            {
                frame.WidthRequest = App.Current.MainPage.Width * 0.8;
                frame.HeightRequest = App.Current.MainPage.Height * 0.4; // Magasságot növeltem a tartalomhoz
            }
        }

        private async void ClosePopup(object sender, EventArgs e)
        {
            await PopupNavigation.Instance.PopAsync();
        }
    }
}