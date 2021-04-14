using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using TamilSerial.Contracts;
using TamilSerial.Presentation.Navigation;
using TamilSerial.Presentation.Navigation.Base;
using TamilSerial.ViewModels.Base;
using TamilTv.Contracts;
using TamilTv.Extensions;
using TamilTv.Models;
using TamilTv.Resources;
using Xamarin.CommunityToolkit.ObjectModel;

namespace TamilSerial.ViewModels
{
    /// <inheritdoc />
    public class HomePageViewModel : ViewModelBase
    {
        private readonly ICachedBigbossService _cachedBigBossService;
        private readonly IPagedArticlesHandler _pagedArticlesHandler;
        private readonly ILogger _logger;

        private bool _isFlyOutOpen;
        private int _thresholdNumber = 2;
        private bool _isDummyLoaded = true;
        private bool _endOfItem;
        private Category _selectedCategory;
        private bool _isRefreshing;
        private ObservableCollection<CategoryGrouping<string, Category>> _categories =
            new ObservableCollection<CategoryGrouping<string, Category>>();
        private ObservableCollection<ProgramInformationModel> _programInformationListList =
            ProgramInformationModel.GenerateDummyProgramInformationModel(12).ToObservableCollection();

        public HomePageViewModel(
            ICachedBigbossService cachedBigBossService,
            IPagedArticlesHandler pagedArticlesHandler,
            ILogger logger)
        {
            _cachedBigBossService = cachedBigBossService;
            _pagedArticlesHandler = pagedArticlesHandler;
            _logger = logger;

            Title = AppResources.HomePage;

            MenuCommand = new AsyncCommand<Category>((category) => ExecuteMenuCommand(category));
            ThresholdReachedCommand = new AsyncCommand(ExecuteThresholdReachedCommand);
            NavigateToArticleCommand = new AsyncCommand<ProgramInformationModel>(ExecuteNavigateToArticleCommand);
            RefreshCommand = new AsyncCommand(ExecuteRefreshCommand);
        }

        public ICommand MenuCommand { get; }

        public ICommand ThresholdReachedCommand { get; }

        public ICommand NavigateToArticleCommand { get; }

        public ICommand RefreshCommand { get; }

        public bool IsFlyOutOpen
        {
            get => _isFlyOutOpen;
            set
            {
                _isFlyOutOpen = value;
                RaisePropertyChanged(() => IsFlyOutOpen);
            }
        }

        public int ThresholdNumber
        {
            get => _thresholdNumber;
            set
            {
                _thresholdNumber = value;
                RaisePropertyChanged(() => ThresholdNumber);
            }
        }

        public bool EndOfItem
        {
            get => _endOfItem;
            set
            {
                _endOfItem = value;
                RaisePropertyChanged(() => EndOfItem);
            }
        }

        public bool IsRefreshing
        {
            get => _isRefreshing;
            set
            {
                _isRefreshing = value;
                RaisePropertyChanged(() => IsRefreshing);
            }
        }

        public ObservableCollection<CategoryGrouping<string, Category>> Categories
        {
            get => _categories;
            set
            {
                _categories = value;
                RaisePropertyChanged(() => Categories);
            }
        }

        public ObservableCollection<ProgramInformationModel> ProgramInformationList
        {
            get => _programInformationListList;
            set
            {
                _programInformationListList = value;
                RaisePropertyChanged(() => ProgramInformationList);
            }
        }

        public override async void OnNavigatedTo(INavigationParameters navigationParameters)
        {
            base.OnNavigatedTo(navigationParameters);
            if (navigationParameters.ContainsKey(NavigationParameterKeys.__NavigationMode) &&
                navigationParameters.GetValue<NavigationMode>(NavigationParameterKeys.__NavigationMode) ==
                NavigationMode.New)
            {
                await InitializeData();
            }
        }

        private Task InitializeData(bool showLoading = true)
        {
            if (showLoading)
            {
                return DialogService.LoadingAsync(async () =>
                {
                    await LoadHomePagePrograms();

                    _isDummyLoaded = false;
                });
            }

            return LoadHomePagePrograms();
        }
        
        private async Task ExecuteMenuCommand(Category categoryUrl, bool showLoading = true)
        {
            IsFlyOutOpen = false;
            _selectedCategory = categoryUrl;
            if (!showLoading)
            {
                await LoadCategoryPrograms(categoryUrl);
                return;
            }

            await DialogService.LoadingAsync(async () => { await LoadCategoryPrograms(categoryUrl); });
        }

        private async Task ExecuteThresholdReachedCommand()
        {
            try
            {
                if (IsBusy || _isDummyLoaded)
                {
                    await Task.Delay(1000);
                    return;
                }

                await DialogService.LoadingAsync(async () => { await LoadNextPagePrograms(); }, AppResources.DownloadingOldEpisodes);
            }
            catch (System.Exception exception)
            {
                _logger.Error(exception, exception.Message);
            }
        }

        private async Task ExecuteNavigateToArticleCommand(ProgramInformationModel programInformation)
        {
            if (string.IsNullOrEmpty(programInformation.Url))
            {
                await DialogService.ShowAlertAsync(
                    string.Format(AppResources.InvalidNavigationUrl, programInformation.Title),
                    AppResources.Warning,
                    AppResources.Okay);
            }

            await NavigationService.NavigateAsync(NavigationKeys.ArticlePage,
                new NavigationParameters {{NavigationParameterKeys.ArticleUrl, programInformation}});
        }

        private async Task ExecuteRefreshCommand()
        {
            await _cachedBigBossService.InvalidateCacheToRefresh();
            if (_selectedCategory == null)
            {
                await InitializeData(false);
            }
            else
            {
                await ExecuteMenuCommand(_selectedCategory, false);
            }

            IsRefreshing = false;
        }

        private async Task LoadHomePagePrograms()
        {
            try
            {
                var result = _cachedBigBossService.GetCategories();
                var articlesResponse = _pagedArticlesHandler.LoadArticles(AppConstants.HostUrl);

                await Task.WhenAll(result, articlesResponse);

                Categories = result.Result.GetCategoryGrouping();
                ProgramInformationList = articlesResponse.Result.ToObservableCollection();
            }
            catch (System.Exception exception)
            {
                _logger.Error(exception, exception.Message);
                Categories = new ObservableCollection<CategoryGrouping<string, Category>>();
                ProgramInformationList = new ObservableCollection<ProgramInformationModel>();
            }
        }

        private async Task LoadCategoryPrograms(Category categoryUrl)
        {
            try
            {
                Title = categoryUrl.Title;
                var articlesResponse = await _pagedArticlesHandler.LoadArticles(categoryUrl.Url).ConfigureAwait(false);
                ProgramInformationList = articlesResponse.ToObservableCollection();
            }
            catch (System.Exception exception)
            {
                _logger.Error(exception, exception.Message);
                ProgramInformationList = new ObservableCollection<ProgramInformationModel>();
            }
        }

        private async Task LoadNextPagePrograms()
        {
            if (_pagedArticlesHandler.HasNextPage)
            {
                var result = await _pagedArticlesHandler.ExecuteGetNextPage();
                if (result.Any())
                {
                    ProgramInformationList.AddBulk(result);
                }
                else
                {
                    EndOfItem = true;
                }
            }
            else
            {
                EndOfItem = true;
            }
        }
    }
}
