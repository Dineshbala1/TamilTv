﻿using System.Windows.Input;
using TamilSerial.Models;
using TamilSerial.Presentation.Dialog;
using TamilSerial.Presentation.Navigation;
using TamilSerial.Presentation.Navigation.Base;
using Xamarin.Forms;

namespace TamilSerial.ViewModels.Base
{
    public abstract class ViewModelBase : ExtendedBindableObject, INavigatedAware
    {
        protected readonly IDialogService DialogService;
        protected readonly INavigationService NavigationService;

        private bool _isBusy;

        public bool IsBusy
        {
            get => _isBusy;

            set
            {
                _isBusy = value;
                RaisePropertyChanged(() => IsBusy);
            }
        }

        public ICommand MenuSearchCommand { get; }

        protected ViewModelBase()
        {
            DialogService = ViewModelLocator.Resolve<IDialogService>();
            NavigationService = ViewModelLocator.Resolve<INavigationService>();

            MenuSearchCommand = new Command(ExecuteSearchCommand);
        }

        private async void ExecuteSearchCommand()
        {
           await NavigationService.NavigateAsync(NavigationKeys.SearchPage);
        }

        public virtual void OnNavigatedFrom(INavigationParameters navigationParameters)
        {
            
        }

        public virtual void OnNavigatedTo(INavigationParameters navigationParameters)
        {
            
        }
    }
}
