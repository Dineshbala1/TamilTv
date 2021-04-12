using Android.App;
using Android.Content.Res;
using Android.OS;
using Android.Views;
using Com.Longtailvideo.Jwplayer;
using Com.Longtailvideo.Jwplayer.Media.Playlists;

namespace TamilSerial.Droid
{
    [Activity(Label = "JwPlayer Activity Xamarin.Forms")]
    public class JwPlayerActivity : Activity
    {
        JWPlayerView mPlayerView;
        //private JWEventHandler mEventHandler;
        //private CastManager mCastManager;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.JwPlayer);
            mPlayerView = FindViewById<JWPlayerView>(Resource.Id.jwplayer);
            PlaylistItem pi = new PlaylistItem("http://playertest.longtailvideo.com/adaptive/bipbop/gear4/prog_index.m3u8");
            mPlayerView.Load(pi);
            // Create your application here
        }
        public override void OnConfigurationChanged(Configuration newConfig)
        {
            // Set fullscreen when the device is rotated to landscape
            mPlayerView.SetFullscreen(newConfig.Orientation == Android.Content.Res.Orientation.Landscape, true);
            base.OnConfigurationChanged(newConfig);
        }
        protected override void OnResume()
        {
            // Let JW Player know that the app has returned from the background
            mPlayerView.OnResume();
            base.OnResume();
        }

        protected override void OnPause()
        {
            mPlayerView.OnPause();
            base.OnPause();
        }


        protected override void OnDestroy()
        {
            // Let JW Player know that the app is being destroyed
            mPlayerView.OnDestroy();
            base.OnDestroy();
        }
        public override bool OnKeyDown(Keycode keyCode, KeyEvent events)
        {
            // Exit fullscreen when the user pressed the Back button
            if (keyCode == Keycode.Back)
            {
                if (mPlayerView.Fullscreen)
                {
                    mPlayerView.SetFullscreen(true, true);
                    //return false;
                }
            }
            return base.OnKeyDown(keyCode, events);
        }
    }
}
