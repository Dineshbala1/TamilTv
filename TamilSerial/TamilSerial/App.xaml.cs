using System.Threading.Tasks;
using Microsoft.AppCenter;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;
using TamilSerial.Presentation.Navigation;
using TamilSerial.ViewModels.Base;
using TamilSerial.Views;
using TamilTv.Contracts;
using TamilTv.Models;

namespace TamilSerial
{
    public partial class App
    {
        private readonly INavigationService _navigationService;
        private readonly IResumableService _resumableService;

        public App()
        {
            InitializeComponent();

            AppCenter.Start("android=68b6cca3-b5b7-44b7-8c69-de201a8ca1f6;",
                typeof(Analytics), typeof(Crashes));
            Sharpnado.MaterialFrame.Initializer.Initialize(loggerEnable: true, debugLogEnable: true);
            _navigationService = ViewModelLocator.Resolve<INavigationService>();
            _resumableService = ViewModelLocator.Resolve<IResumableService>();

            MainPage = new MainPage();
        }

        protected override async void OnStart()
        {
            _resumableService.OnStart();
            await InitApp();
        }

        protected override void OnSleep()
        {
            _resumableService.OnSleep();
        }

        protected override void OnResume()
        {
            _resumableService.OnResume();
        }

        private async Task InitApp()
        {
            await _navigationService.NavigateAsync(NavigationKeys.HomePage).ConfigureAwait(false);
        }
    }
}
