using Android.Content.PM;
using Android.Runtime;
using Android.Webkit;
using TamilSerial.Droid;
using TamilSerial.Droid.Renderers;

namespace XFAndroidFullScreen.Droid.Renderers
{
    using System;
    using Android.Views;
    using Xamarin.Forms.Platform.Android;

    /// <summary>
    /// A <see cref="FormsWebChromeClient"/> that implements full-screen callbacks and raises corresponding
    /// .NET events.
    /// </summary>
    public class FullScreenEnabledWebChromeClient : FormsWebChromeClient
    {
        /// <summary>
        /// Triggered when the content requests full-screen.
        /// </summary>
        public event EventHandler<EnterFullScreenRequestedEventArgs> EnterFullscreenRequested;

        /// <summary>
        /// Triggered when the content requests exiting full-screen.
        /// </summary>
        public event EventHandler ExitFullscreenRequested;

        /// <inheritdoc />
        public override void OnHideCustomView()
        {
            (Xamarin.Forms.Forms.Context as MainActivity).RequestedOrientation = ScreenOrientation.UserPortrait;
            if ((Xamarin.Forms.Forms.Context as MainActivity).Window != null)
            {
                (Xamarin.Forms.Forms.Context as MainActivity).Window.ClearFlags(WindowManagerFlags.Fullscreen);
                (Xamarin.Forms.Forms.Context as MainActivity).Window.AddFlags(WindowManagerFlags.ForceNotFullscreen);
                if ((Xamarin.Forms.Forms.Context as MainActivity).Window.DecorView != null)
                {
                    var uiOptions =
                        SystemUiFlags.LayoutStable |
                        SystemUiFlags.LightNavigationBar;

                    (Xamarin.Forms.Forms.Context as MainActivity).Window.DecorView.SystemUiVisibility =
                        (StatusBarVisibility) uiOptions;
                }
            }

            ExitFullscreenRequested?.Invoke(this, EventArgs.Empty);
        }

        /// <inheritdoc />
        public override void OnShowCustomView(View view, ICustomViewCallback callback)
        {
            ((MainActivity) Xamarin.Forms.Forms.Context).RequestedOrientation = ScreenOrientation.UserLandscape;
           
            if ((Xamarin.Forms.Forms.Context as MainActivity).Window != null)
            {
                (Xamarin.Forms.Forms.Context as MainActivity).Window.AddFlags(WindowManagerFlags.Fullscreen);
                if ((Xamarin.Forms.Forms.Context as MainActivity).Window.DecorView != null)
                {
                    var uiOptions =
                        SystemUiFlags.LowProfile |
                        SystemUiFlags.LayoutFullscreen |
                        SystemUiFlags.Fullscreen |
                        SystemUiFlags.LayoutStable |
                        SystemUiFlags.LayoutHideNavigation |
                        SystemUiFlags.HideNavigation |
                        SystemUiFlags.ImmersiveSticky;

                    (Xamarin.Forms.Forms.Context as MainActivity).Window.DecorView.SystemUiVisibility = (StatusBarVisibility)uiOptions;
                }
            }
            EnterFullscreenRequested?.Invoke(this, new EnterFullScreenRequestedEventArgs(view));
        }
    }

    public class PlayerClient : FormsWebViewClient
    {
        public PlayerClient(WebViewRenderer renderer) : base(renderer)
        {
        }

        protected PlayerClient(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer)
        {
        }

        public override void OnPageFinished(WebView view, string url)
        {
            base.OnPageFinished(view, url);

            var javascript = "javascript:jwplayer().getDuration()";
            view.LoadUrl(javascript);
        }
    }
}