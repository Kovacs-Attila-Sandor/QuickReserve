using Rg.Plugins.Popup.Pages;
using Rg.Plugins.Popup.Services;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using System;

namespace QuickReserve.Views.PopUps
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CustomAlert : PopupPage
    {
        public string Title { get; set; }
        public string Message { get; set; }

        public CustomAlert(string title, string message)
        {
            InitializeComponent();

            Title = title;
            Message = message;
            BindingContext = this;

            // Popup mérete
            var frame = this.Content as Frame;
            if (frame != null)
            {
                frame.WidthRequest = App.Current.MainPage.Width * 0.8;
                frame.HeightRequest = App.Current.MainPage.Height * 0.2;
            }
        }

        private async void OnCloseClicked(object sender, EventArgs e)
        {
            await PopupNavigation.Instance.PopAsync();
        }
    }
}