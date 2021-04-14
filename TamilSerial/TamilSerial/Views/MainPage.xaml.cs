using TamilSerial.Presentation.Navigation;
using TamilSerial.ViewModels.Base;

namespace TamilSerial.Views
{
    public partial class MainPage
    {
        private readonly INavigationService _navigationService;

        public MainPage()
        {
            InitializeComponent();
            this.BindingContext = this;

            _navigationService = ViewModelLocator.Resolve<INavigationService>();
        }

        protected override bool OnBackButtonPressed()
        {
            Xamarin.Forms.Device.BeginInvokeOnMainThread(async () => await _navigationService.GoBackAsync());
            return true;
        }
    }
}