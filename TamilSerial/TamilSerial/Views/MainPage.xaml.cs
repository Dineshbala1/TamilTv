using System;
using TamilSerial.Models;
using TamilSerial.Presentation.Navigation;
using TamilSerial.ViewModels.Base;

namespace TamilSerial.Views
{
    public partial class MainPage
    {
        public MainPage()
        {
            InitializeComponent();
            this.BindingContext = this;
        }

        private async void MenuItem_OnClicked(object sender, EventArgs e)
        {
            var navService = ViewModelLocator.Resolve<INavigationService>();
            await navService.NavigateAsync(NavigationKeys.SearchPage);
        }
    }
}