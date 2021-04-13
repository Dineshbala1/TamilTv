using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using TamilSerial.Contracts;
using TamilSerial.Models;
using TamilSerial.Presentation.Navigation;
using TamilSerial.Presentation.Navigation.Base;
using TamilSerial.ViewModels.Base;
using TamilTv.Contracts;
using Xamarin.Forms;

namespace TamilSerial.ViewModels
{
    /// <inheritdoc />
    public class HomePageViewModel : ViewModelBase
    {
        private readonly ICachedBigbossService _cachedBigbossService;
        private readonly IArticlesHandler _articlesHandler;
        private readonly ILogger _logger;
        private ObservableCollection<CategoryGrouping<string, Categories>> _categories;
        private ObservableCollection<ProgramInformationModel> _programInformationListList = ProgramInformationModel.GenerateDummyProgramInformationModel(12).ToObservableCollection();

        private string _title;
        private bool _isFlyOutOpen;
        private int _thresholdNumber = 2;
        private bool _isDummyLoaded = true;
        private bool _endOfItem;
        private Categories _selectedCategory = null;
        private bool _isRefreshing;

        public HomePageViewModel(
            ICachedBigbossService cachedBigbossService,
            IArticlesHandler articlesHandler,
            ILogger logger)
        {
            _cachedBigbossService = cachedBigbossService;
            _articlesHandler = articlesHandler;
            _logger = logger;

            Title = "Home";

            MenuCommand = new Command<Categories>(async (o) => await ExecuteMenuCommand(o));
            RefreshArticlesCommand = new Command(async () => await ExecuteRefreshArticlesCommand());
            ThresholdReachedCommand = new Command(ExecuteThresholdReachedCommand);
            NavigateToArticleCommand = new Command<ProgramInformationModel>(ExecuteNavigateToArticleCommand);
            RefreshCommand = new Command(ExecuteRefreshCommand);
        }

        public ICommand MenuCommand { get; }

        public ICommand RefreshArticlesCommand { get; }

        public ICommand ThresholdReachedCommand { get; }

        public ICommand NavigateToArticleCommand { get; }

        public ICommand RefreshCommand { get; }

        public string Title
        {
            get => _title;
            set
            {
                _title = value;
                RaisePropertyChanged(() => Title);
            }
        }

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

        public ObservableCollection<CategoryGrouping<string, Categories>> Categories
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

        public override void OnNavigatedFrom(INavigationParameters navigationParameters)
        {
            base.OnNavigatedFrom(navigationParameters);
        }

        public override async void OnNavigatedTo(INavigationParameters navigationParameters)
        {
            base.OnNavigatedTo(navigationParameters);

            await InitializeData();
        }

        private Task InitializeData(bool showLoading = true)
        {
            if (showLoading)
            {

                return DialogService.LoadingAsync(async () =>
                {
                    IsBusy = !IsBusy;

                    await LoadHomePage();

                    _isDummyLoaded = false;

                    IsBusy = !IsBusy;
                });
            }
            else
            {
                return LoadHomePage();
            }
        }

        private async Task LoadHomePage()
        {
            try
            {
                var result = _cachedBigbossService.GetCategories();
                var articlesResponse = _articlesHandler.LoadArticles(AppConstants.HostUrl);

                await Task.WhenAll(result, articlesResponse);

                Categories = new ObservableCollection<CategoryGrouping<string, Categories>>(result.Result
                    .GroupBy(categories => categories.CategoryName).Select(grouping =>
                        new CategoryGrouping<string, Categories>(grouping.Key, grouping)));
                ProgramInformationList = articlesResponse.Result.ProgramInformations
                    .Select(ProgramInformationModel.Transform).ToObservableCollection();
            }
            catch (System.Exception exception)
            {
                _logger.Error(exception, exception.Message);
                Categories = new ObservableCollection<CategoryGrouping<string, Categories>>();
                ProgramInformationList = new ObservableCollection<ProgramInformationModel>();
            }
        }

        private void LoadDummyData()
        {
            ProgramInformationList = ProgramInformationModel
                .GenerateDummyProgramInformationModel(12).ToObservableCollection();

            _isDummyLoaded = true;
        }

        private async Task ExecuteMenuCommand(Categories categoryUrl, bool showLoading = true)
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

        private async Task LoadCategoryPrograms(Categories categoryUrl)
        {
            try
            {
                Title = categoryUrl.Title;
                var articlesResponse = await _articlesHandler.LoadArticles(categoryUrl.Url).ConfigureAwait(false);
                ProgramInformationList = articlesResponse.ProgramInformations
                    .Select(ProgramInformationModel.Transform).ToObservableCollection();
            }
            catch (System.Exception exception)
            {
                _logger.Error(exception, exception.Message);
                ProgramInformationList = new ObservableCollection<ProgramInformationModel>();
            }
        }

        private async Task ExecuteRefreshArticlesCommand()
        {
            await _articlesHandler.ExecuteGetNextPage();
        }

        private async void ExecuteThresholdReachedCommand()
        {
            try
            {
                if (IsBusy || _isDummyLoaded)
                {
                    await Task.Delay(1000);
                    return;
                }

                await DialogService.LoadingAsync(async () =>
                {
                    if (_articlesHandler.HasNextPage)
                    {
                        var result = await _articlesHandler.ExecuteGetNextPage();
                        if (result.PaginationDetail.Any() || result.ProgramInformations.Any())
                        {
                            foreach (var information in result.ProgramInformations)
                            {
                                ProgramInformationList.Add(ProgramInformationModel.Transform(information));
                            }
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
                }, "Downloading \n new episodes");
            }
            catch (System.Exception exception)
            {
                _logger.Error(exception, exception.Message);
            }
        }

        private async void ExecuteNavigateToArticleCommand(ProgramInformationModel programInformation)
        {
            if (string.IsNullOrEmpty(programInformation.Url))
            {
                await DialogService.ShowAlertAsync(
                    $"Sorry !! Invalid content information for {programInformation.Title}", "Warning",
                    "Ok");
            }

            await NavigationService.NavigateAsync("ArticlePage",
                new NavigationParameters() { { NavigationParameterKeys.ArticleUrl, programInformation } });
        }

        private async void ExecuteRefreshCommand()
        {
            await _cachedBigbossService.InvalidateCacheToRefresh();
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
    }
}
