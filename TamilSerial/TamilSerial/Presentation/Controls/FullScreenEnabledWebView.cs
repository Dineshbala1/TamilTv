using System.Threading.Tasks;
using System.Windows.Input;
using MarcTron.Plugin;
using TamilTv.Presentation.Pages;
using TamilTv.Resources;
using Xamarin.Forms;

namespace TamilSerial.Presentation.Controls
{
    public class FullScreenEnabledWebView : WebView
    {
        private static VideoAdPage _timerPage;

        private bool _firstTouch;

        /// <summary>
        /// Bindable property for <see cref="EnterFullScreenCommand"/>.
        /// </summary>
        public static readonly BindableProperty EnterFullScreenCommandProperty =
            BindableProperty.Create(
                nameof(EnterFullScreenCommand),
                typeof(ICommand),
                typeof(FullScreenEnabledWebView),
                defaultValue: new Command(async (view) => await DefaultEnterAsync((View)view)));

        /// <summary>
        /// Bindable property for <see cref="ExitFullScreenCommand"/>.
        /// </summary>
        public static readonly BindableProperty ExitFullScreenCommandProperty =
            BindableProperty.Create(
                nameof(ExitFullScreenCommand),
                typeof(ICommand),
                typeof(FullScreenEnabledWebView),
                defaultValue: new Command(async (view) => await DefaultExitAsync()));

        public static readonly BindableProperty PauseProperty =
            BindableProperty.Create(
                nameof(Pause),
                typeof(bool),
                typeof(FullScreenEnabledWebView),
                defaultValue: false);

        public static readonly BindableProperty ShowAdProperty =
            BindableProperty.Create(
                nameof(ShowAd),
                typeof(bool),
                typeof(FullScreenEnabledWebView),
                defaultValue: false,
                BindingMode.TwoWay,
                propertyChanged: PropertyChanged);

        /// <summary>
        /// Gets or sets the command executed when the web view content requests entering full-screen.
        /// The command is passed a <see cref="View"/> containing the content to display.
        /// The default command displays the content as a modal page.
        /// </summary>
        public ICommand EnterFullScreenCommand
        {
            get => (ICommand)GetValue(EnterFullScreenCommandProperty);
            set => SetValue(EnterFullScreenCommandProperty, value);
        }

        /// <summary>
        /// Gets or sets the command executed when the web view content requests exiting full-screen.
        /// The command is passed no parameters.
        /// The default command pops a modal page off the navigation stack.
        /// </summary>
        public ICommand ExitFullScreenCommand
        {
            get => (ICommand)GetValue(ExitFullScreenCommandProperty);
            set => SetValue(ExitFullScreenCommandProperty, value);
        }

        public bool Pause
        {
            get => (bool)GetValue(PauseProperty);
            set => SetValue(PauseProperty, value);
        }

        public bool ShowAd
        {
            get => (bool)GetValue(ShowAdProperty);
            set => SetValue(ShowAdProperty, value);
        }

        public ICommand StartAdNoticeCommand { get; }

        public FullScreenEnabledWebView()
        {
            StartAdNoticeCommand = new Command(Execute);
        }

        private void Execute(object obj)
        {
            if (_timerPage == null)
            {
                ShowAd = true;
                return;
            }

            _timerPage.ShowTimerAdBeginningInSeconds(10);
        }

        private static async Task DefaultEnterAsync(View view)
        {
            if (_timerPage != null)
            {
                _timerPage = null;
            }

            _timerPage = new VideoAdPage(view,
                () =>
                {
                    if (App.InForeground)
                    {
                        CrossMTAdmob.Current.LoadInterstitial(AppResources.VideoInterimAd);
                    }
                });

            await Application.Current.MainPage.Navigation.PushModalAsync(_timerPage);
        }

        private static void PropertyChanged(BindableObject bindable, object oldvalue, object newvalue)
        {
            if (newvalue is bool result && result)
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    if (App.InForeground)
                    {
                        CrossMTAdmob.Current.LoadInterstitial(AppResources.VideoInterimAd);
                    }
                });

                ((FullScreenEnabledWebView)bindable).ShowAd = false;
            }
        }

        private static async Task DefaultExitAsync()
        {
            await Application.Current.MainPage.Navigation.PopModalAsync();
        }
    }
}
