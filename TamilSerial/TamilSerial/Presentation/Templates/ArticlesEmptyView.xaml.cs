using System;
using System.Resources;
using TamilTv.Resources;
using Xamarin.Essentials;

namespace TamilTv.Presentation.Templates
{
    public partial class ArticlesEmptyView
    {
        public ArticlesEmptyView()
        {
            InitializeComponent();
        }

        private void LifecycleEffect_OnLoaded(object sender, EventArgs e)
        {
            if (Connectivity.NetworkAccess != NetworkAccess.Internet)
            {
                ErrorStatus.Text = AppResources.NoInternet;
            }
        }

        private void LifecycleEffect_OnUnloaded(object sender, EventArgs e)
        {
            if (Connectivity.NetworkAccess != NetworkAccess.Internet)
            {
                ErrorStatus.Text = AppResources.NoInternet;
            }
        }
    }
}