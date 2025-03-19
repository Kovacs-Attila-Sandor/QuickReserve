using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace QuickReserve.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SplashPage : ContentPage
    {
        public SplashPage()
        {
            InitializeComponent();
            StartAnimation();
        }

        private async void StartAnimation()
        {
            // Kezdeti állapot: logó és szöveg átlátszó, kisebb méretű
            var frame = this.FindByName<Frame>(""); // A Frame-nek nincs neve az XAML-ben, ha kell, adj neki x:Name="LogoFrame"-et
            var label = this.FindByName<Label>(""); // Hasonlóképpen, adj x:Name="TitleLabel"-t a Label-nek, ha használni akarod

            // Egyszerűbb megoldás: az egész oldal animálása
            this.Opacity = 0;
            await this.FadeTo(1, 1000, Easing.CubicInOut); // Fade-in az egész oldalnak

            // Opcionális: logó skálázása (ha x:Name-et adsz a Frame-nek)
            // frame.Scale = 0.5;
            // await frame.ScaleTo(1, 800, Easing.SpringOut);
        }
    }
}