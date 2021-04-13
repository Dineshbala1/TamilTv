using TamilSerial.Models;
using TamilSerial.Presentation.Navigation;
using TamilTv.Contracts;
using Xamarin.Essentials;

namespace TamilTv.Services
{
    public class NoInternetService : IResumableService
    {
        private readonly INavigationService _navigationService;

        public NoInternetService(INavigationService navigationService)
        {
            _navigationService = navigationService;
        }

        public void OnSleep()
        {
            Connectivity.ConnectivityChanged -= OnConnectivityChanged;
        }

        public void OnResume()
        {
            Connectivity.ConnectivityChanged += OnConnectivityChanged;
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
            }
            else
            {
                await _navigationService.GoBackAsync(new NavigationParameters(), true, true);
            }
        }
    }
}
