using System.ComponentModel;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Com.Longtailvideo.Jwplayer;
using Com.Longtailvideo.Jwplayer.Configuration;
using TamilSerial.Presentation.Controls;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using ARelativeLayout = Android.Widget.RelativeLayout;

namespace TamilSerial.Droid.Renderers
{
    [Preserve(AllMembers = true)]
    public class JwPlayerRenderer : Xamarin.Forms.Platform.Android.AppCompat.ViewRenderer<JwPlayerControlView, ARelativeLayout>
    {
        Context context;
        private JWPlayerView mPlayerView;
        ARelativeLayout frameLayout;
        ViewGroup mainpage;
        public JwPlayerRenderer(Context context) : base(context)
        {
            this.context = context;
        }
        protected override void OnElementChanged(ElementChangedEventArgs<JwPlayerControlView> e)
        {
            base.OnElementChanged(e);
            if (e.OldElement == null)
            {
                if (Control == null)
                {
                    InitializePlayer(e.NewElement.Souce);
                    frameLayout = new ARelativeLayout(context);
                    ARelativeLayout.LayoutParams layoutParams =
                            new ARelativeLayout.LayoutParams(LayoutParams.MatchParent, LayoutParams.MatchParent);
                    layoutParams.AddRule(LayoutRules.CenterInParent);

                    //mPlayerView.LayoutParameters = layoutParams;
                    mPlayerView.LayoutParameters = layoutParams;
                    frameLayout.AddView(mPlayerView);
                    SetNativeControl(frameLayout);
                    mainpage = ((ViewGroup)mPlayerView.Parent);
                }
                SetAreTransportControlsEnabled();
                SetSource();
            }
        }

        private void InitializePlayer(string Source)
        {
            mPlayerView = new JWPlayerView(context, new PlayerConfig.Builder()
                .File(Source)
                .Build());

        }

        void SetSource()
        {
            //after you can implement  fullscreen, loopvideo and more..
        }
        void SetAreTransportControlsEnabled()
        {

        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);
        }
        protected override void Dispose(bool disposing)
        {
            mPlayerView?.Dispose();
            mPlayerView = null;
            base.Dispose(disposing);

        }
    }
}
