using System.Threading.Tasks;
using TamilSerial.Models;
using TamilSerial.Presentation.Navigation;
using TamilTv.Contracts;
using Xamarin.Essentials;

namespace TamilTv.Services
{
    public class NoInternetService : IResumableService
    {
        private readonly INavigationService _navigationService;

        private bool _inOfflineState = false;

        public NoInternetService(INavigationService navigationService)
        {
            _navigationService = navigationService;
        }

        public void OnSleep()
        {
            Connectivity.ConnectivityChanged -= OnConnectivityChanged;
        }

        public async void OnResume()
        {
            Connectivity.ConnectivityChanged += OnConnectivityChanged;
            await CheckForBackgroundSwitching();
        }

        public void OnStart()
        {
            Connectivity.ConnectivityChanged += OnConnectivityChanged;
        }

        private async void OnConnectivityChanged(object sender, ConnectivityChangedEventArgs e)
        {
            if (Connectivity.NetworkAccess != NetworkAccess.Internet)
            {
                await _navigationService.NavigateAsync(NavigationKeys.NoInternetPage, new NavigationParameters(), true, true);
                _inOfflineState = true;
            }
            else
            {
                _inOfflineState = false;
                await _navigationService.GoBackAsync(new NavigationParameters(), true, true);
            }
        }

        private async Task CheckForBackgroundSwitching()
        {
            if (Connectivity.NetworkAccess == NetworkAccess.Internet)
            {
                if (_inOfflineState)
                {
                    await _navigationService.GoBackAsync(new NavigationParameters(), true, true);
                }
            }
        }
    }
}
