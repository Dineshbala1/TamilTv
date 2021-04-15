using System;
using System.Diagnostics;
using System.Reactive.Linq;
using MarcTron.Plugin;
using TamilSerial;
using TamilSerial.Presentation.Controls;
using Xamarin.Forms;

namespace TamilTv.Presentation.Behaviors
{
    public class PageBehavior : Behavior<Page>
    {
        private double _secondsToWait = 0;
        private IDisposable _disposable = null;
        private Random _random = new Random();

        public static readonly BindableProperty FullScreenWebViewProperty =
            BindableProperty.Create(
                "FullScreenWebView",
                typeof(FullScreenEnabledWebView),
                typeof(PageBehavior), null);

        public FullScreenEnabledWebView FullScreenWebView
        {
            get => (FullScreenEnabledWebView) GetValue(FullScreenWebViewProperty);
            set => SetValue(FullScreenWebViewProperty, value);
        }

        protected override void OnAttachedTo(Page bindable)
        {
            base.OnAttachedTo(bindable);
            bindable.Appearing += BindableOnAppearing;
        }

        private void BindableOnAppearing(object sender, EventArgs e)
        {
            if (_secondsToWait != 0)
            {
                return;
            }

            var randomSeconds = _random.Next(7, 10) * 60;
            _secondsToWait = TimeSpan.FromSeconds(randomSeconds).TotalSeconds;
            CrossMTAdmob.Current.OnInterstitialLoaded += OnCurrentOnOnInterstitialLoaded;
            _disposable = Observable.Interval(TimeSpan.FromSeconds(randomSeconds - 10)).Subscribe(l =>
            {
                Debug.WriteLine($"{l} seconds has elapsed");
                if (App.InForeground && !FullScreenWebView.Pause)
                {
                    // Implement the logic to send command or invoke advertisement within the webview
                    FullScreenWebView?.StartAdNoticeCommand.Execute(null);
                }
            });
        }

        private void OnCurrentOnOnInterstitialLoaded(object o, EventArgs args)
        {
            CrossMTAdmob.Current.ShowInterstitial();
        }

        protected override void OnDetachingFrom(Page bindable)
        {
            base.OnDetachingFrom(bindable);
            bindable.Appearing -= BindableOnAppearing;
            CrossMTAdmob.Current.OnInterstitialLoaded -= OnCurrentOnOnInterstitialLoaded;
            _disposable.Dispose();
        }
    }
}
