using Xamarin.Essentials;

namespace TamilTv.Views
{
    public partial class NoInternetPage
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

        protected override void OnAppearing()
        {
            base.OnAppearing();
        }
    }
}