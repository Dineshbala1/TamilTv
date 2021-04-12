using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace TamilTv.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class NoInternetPage : ContentPage
    {
        public NoInternetPage()
        {
            InitializeComponent();
        }

        protected override bool OnBackButtonPressed()
        {
            if (Connectivity.NetworkAccess != NetworkAccess.None)
            {
                return base.OnBackButtonPressed();
            }

            return true;
        }
    }
}