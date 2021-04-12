using System;
using System.Threading.Tasks;
using Microsoft.AppCenter;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;
using TamilSerial.Models;
using TamilSerial.Presentation.Navigation;
using TamilSerial.ViewModels.Base;
using TamilSerial.Views;
using Xamarin.CommunityToolkit.Extensions;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace TamilSerial
{
    public partial class App
    {
        private readonly INavigationService _navigationService;
        private bool isAppLoaded = false;

        public App()
        {
            InitializeComponent();

            AppCenter.Start("android=68b6cca3-b5b7-44b7-8c69-de201a8ca1f6;",
                typeof(Analytics), typeof(Crashes));

            Sharpnado.MaterialFrame.Initializer.Initialize(loggerEnable: true, debugLogEnable: true);

            App.Current.UserAppTheme = OSAppTheme.Unspecified;
            _navigationService = ViewModelLocator.Resolve<INavigationService>();
            MainPage = new MainPage();
        }

        protected override async void OnStart()
        {
            if (!await CanLoadApp())
            {
                isAppLoaded = false;
                return;
            }

            await InitApp();

            Connectivity.ConnectivityChanged += Connectivity_ConnectivityChanged;
        }

        protected override void OnSleep()
        {
            Connectivity.ConnectivityChanged -= Connectivity_ConnectivityChanged;
        }

        protected override async void OnResume()
        {
            if (!isAppLoaded)
            {
                if (await CanLoadApp())
                {
                    await InitApp();
                }
            }

            Connectivity.ConnectivityChanged += Connectivity_ConnectivityChanged;
        }

        private async Task InitApp()
        {
            await _navigationService.NavigateAsync(NavigationKeys.HomePage).ConfigureAwait(false);
        }

        private async Task<bool> CanLoadApp()
        {
            bool canLoadApp = false;
            try
            {
                var storageWriteStatus =
                    await CheckAndRequestStorageWritePermission(
                        new Permissions.StorageWrite());

                if (storageWriteStatus != PermissionStatus.Granted)
                {
                    canLoadApp = await MainPage.DisplaySnackBarAsync(
                        $" StorageWrite permission is required to read and write logs, can't access the app without the permission closing the app",
                        "close", () =>
                        {
                            System.Diagnostics.Process.GetCurrentProcess().CloseMainWindow();
                            return Task.CompletedTask;
                        });
                }
                else
                {
                    canLoadApp = true;
                }
            }
            catch (Exception exception)
            {
                Crashes.TrackError(exception);
            }
            finally
            {
                if (!canLoadApp)
                {
                    await MainPage.DisplayAlert("Warning",
                        "Enable storage permissions to access the app, Go to Settings -> Apps -> Tamil Tv -> Permissions.",
                        "Ok");
                    AppInfo.ShowSettingsUI();
                }
            }

            return canLoadApp;
        }


        private async void Connectivity_ConnectivityChanged(object sender,
            ConnectivityChangedEventArgs e)
        {
            if (e.NetworkAccess == NetworkAccess.None)
            {
                await _navigationService.NavigateAsync(NavigationKeys.NoInternetPage, new NavigationParameters(), true,
                    false);
            }
        }

        private async Task<PermissionStatus> CheckAndRequestStorageWritePermission<T>(T permission)
            where T : Permissions.BasePermission, new()
        {
            var status = await CheckAndRequestPermissionAsync(permission);

            if (status == PermissionStatus.Granted)
                return status;

            if (status == PermissionStatus.Denied && DeviceInfo.Platform == DevicePlatform.iOS)
            {
                // Prompt the user to turn on in settings
                // On iOS once a permission has been denied it may not be requested again from the application
                return status;
            }

            if (Permissions.ShouldShowRationale<T>())
            {
                // Prompt the user with additional information as to why the permission is needed
                await MainPage.DisplayAlert("Warning",
                    $"{permission} permission is required to read and write logs", "Ok");
            }

            status = await Permissions.RequestAsync<T>();

            return status;
        }

        private async Task<PermissionStatus> CheckAndRequestPermissionAsync<T>(T permission)
            where T : Permissions.BasePermission
        {
            var status = await permission.CheckStatusAsync();
            if (status != PermissionStatus.Granted)
            {
                status = await permission.RequestAsync();
            }

            return status;
        }
    }
}
