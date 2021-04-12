using System;
using System.Threading.Tasks;
using TamilSerial.Presentation.Navigation.Base;
using TamilSerial.ViewModels.Base;
using Xamarin.Forms;
using NavigationMode = TamilSerial.Presentation.Navigation.Base.NavigationMode;

namespace TamilSerial.Presentation.Navigation
{
    public class NavigationService : INavigationService
    {
        private INavigation Navigation => ((NavigationPage)Application.Current.MainPage).Navigation;

        private Page _page;

        public Task<INavigationResult> GoBackAsync()
        {
            return GoBackAsync(new NavigationParameters());
        }

        public Task<INavigationResult> GoBackAsync(INavigationParameters parameters)
        {
            return GoBackAsync(parameters, null, false);
        }

        public Task<INavigationResult> GoBackAsync(INavigationParameters parameters, bool? useModalNavigation,
            bool animated)
        {
            return GoBackInternal(parameters, useModalNavigation, animated);
        }

        public Task<INavigationResult> GoBackToRootAsync(INavigationParameters parameters)
        {
            throw new NotImplementedException();
        }

        public Task<INavigationResult> NavigateAsync(string name)
        {
            return NavigateAsync(name, null);
        }

        public Task<INavigationResult> NavigateAsync(string name, INavigationParameters parameters)
        {
            return NavigateAsync(name, parameters, null, false);
        }

        public Task<INavigationResult> NavigateAsync(string name, INavigationParameters parameters,
            bool? useModalNavigation, bool animated)
        {
            return NavigateInternal(name, parameters, useModalNavigation, animated);
        }

        protected virtual Page GetCurrentPage()
        {
            return _page ?? Application.Current.MainPage;
        }

        private async Task<INavigationResult> NavigateInternal(string name, INavigationParameters parameters,
            bool? useModalNavigation, bool animated)
        {
            var result = new NavigationResult();
            try
            {
                var currentPage = (Application.Current.MainPage as NavigationPage)?.RootPage ??
                                  Application.Current.MainPage as NavigationPage;
                await NavigateInternal(currentPage, name, parameters ?? new NavigationParameters(), useModalNavigation,
                    animated);
                result.Success = true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                result.Exception = e;
            }

            return result;
        }

        private async Task<INavigationResult> GoBackInternal(INavigationParameters parameters, bool? useModalNavigation, bool animated)
        {
            var result = new NavigationResult();
            try
            {
                var page = (Application.Current.MainPage as NavigationPage)?.CurrentPage ??
                           Application.Current.MainPage as NavigationPage;

                parameters.Add(KnownInternalParameters.NavigationMode, NavigationMode.Back);
                var previousPage =
                    PageUtilities.GetOnNavigatedToTarget(page, Application.Current.MainPage,
                        useModalNavigation ?? false);

                var poppedPage = await DoPop(page.Navigation, useModalNavigation ?? false, animated);
                if (poppedPage != null)
                {
                    OnNavigatedFrom(page, parameters);
                    OnNavigatedTo(previousPage, parameters);
                    PageUtilities.DestroyPage(poppedPage);

                    result.Success = true;
                    return result;
                }
            }
            catch (Exception ex)
            {
                result.Exception = ex;
                return result;
            }

            return result;
        }

        protected virtual Task<Page> DoPop(INavigation navigation, bool useModalNavigation, bool animated)
        {
            return useModalNavigation ? navigation.PopModalAsync(animated) : navigation.PopAsync(animated);
        }

        private async Task NavigateInternal(Page currentPage, string name, INavigationParameters parameters,
            bool? useModalNavigation, bool animated)
        {
            parameters.Add(KnownInternalParameters.NavigationMode, NavigationMode.New);
            var nextPage = (Page) ViewModelLocator.Resolve(name);
            ViewModelLocator.SetAutoWireViewModel(nextPage, true);

            var task = useModalNavigation.HasValue && useModalNavigation.Value
                ? Navigation.PushModalAsync(nextPage, animated)
                : Navigation.PushAsync(nextPage, animated);

            await task.ConfigureAwait(false);

            OnNavigatedFrom(currentPage, parameters);
            await PageUtilities.OnInitializedAsync(nextPage, parameters);
            OnNavigatedTo(nextPage, parameters);
        }

        private void OnNavigatedFrom(Page currentPage, INavigationParameters navigationParameters)
        {
            currentPage.OnNavigatedFrom(navigationParameters);
        }

        private void OnNavigatedTo(Page nextPage, INavigationParameters navigationParameters)
        {
            nextPage.OnNavigatedTo(navigationParameters);
        }
    }
}
