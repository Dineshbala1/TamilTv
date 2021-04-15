using System;
using System.Diagnostics;
using MarcTron.Plugin;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace TamilSerial.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ArticlePage
    {
        public ArticlePage()
        {
            InitializeComponent();

            CrossMTAdmob.Current.OnInterstitialLoaded += (sender, args) =>
            {
                CrossMTAdmob.Current.ShowInterstitial();
            };
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            //Device.StartTimer(TimeSpan.FromSeconds(10), () =>
            //{
               
            //    Debug.WriteLine("Ad loaded from here");
            //    return true;
            //});

            //CrossMTAdmob.Current.LoadInterstitial("ca-app-pub-5213150499806218/3809233328");
        }
    }
}