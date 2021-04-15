using System;
using Xamarin.Forms;

namespace TamilTv.Presentation.Pages
{
    public partial class VideoAdPage
    {
        private readonly Action _raiseShowAd;

        public VideoAdPage()
        {
            
        }

        public VideoAdPage(View view, Action raiseShowAd) : this()
        {
            InitializeComponent();
            ContentView.Content = view;
            _raiseShowAd = raiseShowAd;
        }

        public void ShowTimerAdBeginningInSeconds(int seconds)
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                ProgressRing.IsVisible = true;
                Label.IsVisible = true;
                int counter = seconds;
                Device.StartTimer(TimeSpan.FromSeconds(1), () =>
                {
                    counter--;
                    ProgressRing.Progress = counter / 10f;
                    if (counter <= 0)
                    {
                        Label.IsVisible = false;
                        ProgressRing.IsVisible = false;
                        ProgressRing.Progress = 1;
                        _raiseShowAd();
                        return false;
                    }

                    return true;
                });
            });
        }
    }
}