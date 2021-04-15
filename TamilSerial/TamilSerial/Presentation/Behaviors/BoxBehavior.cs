using MarcTron.Plugin;
using TamilSerial;
using TamilTv.Resources;
using Xamarin.Forms;

namespace TamilTv.Presentation.Behaviors
{
    public class BoxBehavior : Behavior<BoxView>
    {
        private IGestureRecognizer gesture;

        protected override void OnAttachedTo(BoxView bindable)
        {
            base.OnAttachedTo(bindable);

            gesture = new TapGestureRecognizer(TappedCallback);
            bindable.GestureRecognizers.Add(gesture);
        }

        private void TappedCallback(View view, object arg2)
        {
            if (App.InForeground)
            {
                CrossMTAdmob.Current.LoadInterstitial(AppResources.VideoInterimAd);
                view.InputTransparent = true;
            }
        }

        protected override void OnDetachingFrom(BoxView bindable)
        {
            base.OnDetachingFrom(bindable);
            bindable.GestureRecognizers.Remove(gesture);
        }
    }
}