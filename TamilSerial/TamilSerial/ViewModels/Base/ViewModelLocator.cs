using System;
using System.Globalization;
using System.Reflection;
using MonkeyCache;
using MonkeyCache.FileStore;
using Refit;
using TamilSerial.Contracts;
using TamilSerial.Presentation.Dialog;
using TamilSerial.Presentation.Navigation;
using TamilSerial.Presentation.Navigation.Base;
using TamilSerial.Services;
using TamilSerial.Views;
using TamilTv.Contracts;
using TamilTv.Models;
using TamilTv.Services;
using TamilTv.ViewModels;
using TamilTv.Views;
using TinyIoC;
using Xamarin.Forms;
using HomePage = TamilSerial.Views.HomePage;
using Logger = TamilTv.Services.Logger;

namespace TamilSerial.ViewModels.Base
{
    public class ViewModelLocator
    {
        private static TinyIoCContainer _container;

        public static readonly BindableProperty AutoWireViewModelProperty =
            BindableProperty.CreateAttached("AutoWireViewModel", typeof(bool), typeof(ViewModelLocator), default(bool), propertyChanged: OnAutoWireViewModelChanged);

        public static bool GetAutoWireViewModel(BindableObject bindable)
        {
            return (bool)bindable.GetValue(ViewModelLocator.AutoWireViewModelProperty);
        }

        public static void SetAutoWireViewModel(BindableObject bindable, bool value)
        {
            bindable.SetValue(ViewModelLocator.AutoWireViewModelProperty, value);
        }

        static ViewModelLocator()
        {
            _container = new TinyIoCContainer();

            RegisterDependencies();
            RegisterView();
            RegisterViewModel();
        }

        public static void UpdateDependencies(bool useMockServices)
        {
            // Change injected dependencies
        }

        public static void RegisterSingleton<TInterface, T>() where TInterface : class where T : class, TInterface
        {
            _container.Register<TInterface, T>().AsSingleton();
        }

        public static T Resolve<T>() where T : class
        {
            return _container.Resolve<T>();
        }

        public static T Resolve<T>(string name) where T : class
        {
            return _container.Resolve<T>(name);
        }

        public static Object Resolve(string name)
        {
            var pageinfo = PageNavigationRegistry.GetPageNavigationInfo(name);
            return _container.Resolve(pageinfo.Type, pageinfo.Name);
        }

        private static void OnAutoWireViewModelChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var view = bindable as Element;
            if (view == null)
            {
                return;
            }

            var viewType = view.GetType();
            var viewName = viewType.FullName.Replace(".Views.", ".ViewModels.");
            var viewAssemblyName = viewType.GetTypeInfo().Assembly.FullName;
            var viewModelName = string.Format(CultureInfo.InvariantCulture, "{0}ViewModel, {1}", viewName, viewAssemblyName);

            var viewModelType = Type.GetType(viewModelName);
            if (viewModelType == null)
            {
                return;
            }
            var viewModel = _container.Resolve(viewModelType);
            view.BindingContext = viewModel;
        }

        private static void RegisterDependencies()
        {
            _container.Register<ILogger, Logger>().AsSingleton();
            _container.Register<IResumableService, NoInternetService>().AsSingleton();
            _container.Register<INavigationService, NavigationService>();
            _container.Register<IDialogService, DialogService>().AsSingleton();
            _container.Register(typeof(IApiService),
                (container, overloads) => RestService.For<IApiService>(AppConstants.ApiUrl));
            _container.Register(typeof(IBarrel), Barrel.Current);
            _container.Register<ICachedBigbossService, CachedBigbossService>().AsSingleton();
            _container.Register<IBigBossService, BigBossService>().AsSingleton();
            _container.Register<IPagedArticlesHandler, PagedArticlesHandler>().AsSingleton();
        }

        private static void RegisterView()
        {
            PageNavigationRegistry.Register(NavigationKeys.HomePage, typeof(HomePage));
            PageNavigationRegistry.Register(NavigationKeys.ArticlePage, typeof(ArticlePage));
            PageNavigationRegistry.Register(NavigationKeys.SearchPage, typeof(SearchPage));
            PageNavigationRegistry.Register(NavigationKeys.NoInternetPage, typeof(NoInternetPage));

            _container.Register(typeof(HomePage), NavigationKeys.HomePage);
            _container.Register(typeof(ArticlePage), NavigationKeys.ArticlePage);
            _container.Register(typeof(SearchPage), NavigationKeys.SearchPage);
            _container.Register(typeof(NoInternetPage), NavigationKeys.NoInternetPage);
        }

        private static void RegisterViewModel()
        {
            _container.Register<HomePageViewModel>();
            _container.Register<ArticlePageViewModel>();
            _container.Register<SearchPageViewModel>();
            _container.Register<NoInternetPageViewModel>();
        }
    }
}
