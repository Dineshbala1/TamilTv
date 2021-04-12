using System;
using System.ComponentModel;
using Android.Content;
using TamilSerial.Droid.Renderers;
using TamilSerial.Presentation.Controls;
using Xamarin.Forms.Platform.Android;
using XFAndroidFullScreen.Droid.Renderers;
using WebView = Xamarin.Forms.WebView;

[assembly: Xamarin.Forms.ExportRenderer(
    typeof(FullScreenEnabledWebView),
    typeof(FullScreenEnabledWebViewRenderer))]
namespace TamilSerial.Droid.Renderers
{
    /// <summary>
    /// An Android renderer for <see cref="FullScreenEnabledWebView"/>.
    /// </summary>
    public class FullScreenEnabledWebViewRenderer : WebViewRenderer
    {
        private FullScreenEnabledWebView _webView;

        /// <summary>
        /// Initializes a new instance of the <see cref="FullScreenEnabledWebViewRenderer"/> class.
        /// </summary>
        /// <param name="context">An Android context.</param>
        public FullScreenEnabledWebViewRenderer(Context context) : base(context)
        {
        }

        /// <inheritdoc/>
        protected override void OnElementChanged(ElementChangedEventArgs<WebView> e)
        {
            base.OnElementChanged(e);
            
            _webView = (FullScreenEnabledWebView) e.NewElement;
            if (Control != null)
            {
                Control.Settings.JavaScriptEnabled = true;
                Control.Settings.JavaScriptCanOpenWindowsAutomatically = true;
                Control.Settings.BuiltInZoomControls = false;
                Control.SetWebViewClient(new PlayerClient(this));
            }
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);

            if (e.PropertyName == FullScreenEnabledWebView.PauseProperty.PropertyName)
            {
                this.Control.LoadUrl("javascript:jwplayer().pause()");
            }
        }

        /// <summary>
        /// Creates a <see cref="FormsWebChromeClient"/> that implements the necessary callbacks to support
        /// full-screen operation.
        /// </summary>
        /// <returns>A <see cref="FullScreenEnabledWebChromeClient"/>.</returns>
        protected override FormsWebChromeClient GetFormsWebChromeClient()
        {
            var client = new FullScreenEnabledWebChromeClient();
            client.EnterFullscreenRequested += OnEnterFullscreenRequested;
            client.ExitFullscreenRequested += OnExitFullscreenRequested;
            return client;
        }

        /// <summary>
        /// Executes the full-screen command on the <see cref="FullScreenEnabledWebView"/> if available. The
        /// Xamarin view to display in full-screen is sent as a command parameter.
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="eventArgs">The event arguments.</param>
        private void OnEnterFullscreenRequested(
            object sender,
            EnterFullScreenRequestedEventArgs eventArgs)
        {
            this.Control.OnResume();
            if (_webView.EnterFullScreenCommand != null && _webView.EnterFullScreenCommand.CanExecute(null))
            {
                _webView.EnterFullScreenCommand.Execute(eventArgs.View.ToView());
            }
        }

        /// <summary>
        /// Executes the exit full-screen command on th e <see cref="FullScreenEnabledWebView"/> if available.
        /// The command is passed no parameters.
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="eventArgs">The event arguments.</param>
        private void OnExitFullscreenRequested(object sender, EventArgs eventArgs)
        {
            this.Control.OnPause();
            if (_webView.ExitFullScreenCommand != null && _webView.ExitFullScreenCommand.CanExecute(null))
            {
                _webView.ExitFullScreenCommand.Execute(null);
            }

            this.Control.OnResume();
        }
    }
}