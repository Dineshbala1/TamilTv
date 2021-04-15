using System;
using Xamarin.Forms;

namespace TamilTv.Presentation.Pages
{
    public class TimerPage : ContentPage
    {
        private readonly Action _raiseShowAd = null;
        private Label _label = new Label {IsVisible = false};

        public TimerPage(View view, Action raiseShowAd)
        {
            _raiseShowAd = raiseShowAd;
            var grid = new Grid();
            grid.Children.Add(view);
            grid.Children.Add(_label, 0, 1, 0, 1);

            Content = grid;
        }

        public void ShowTimerAdBeginningInSeconds(int seconds)
        {
            int counter = seconds;
            Device.StartTimer(TimeSpan.FromSeconds(1), () =>
            {
                counter--;
                if (counter >= 0)
                {
                    _label.IsVisible = false;
                    _raiseShowAd();
                    return false;
                }

                _label.Text = $"Showing ad in {counter} seconds";
                _label.IsVisible = true;
                return true;
            });
        }
    }
}
