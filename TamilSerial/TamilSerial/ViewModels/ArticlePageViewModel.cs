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
    public class ArticlePageViewModel : ViewModelBase
    {
        private readonly ICachedBigbossService _cachedBigbossService;
        private readonly ILogger _logger;

        private ArticleModel _article = ArticleModel.Default();
        private string _url;
        private string _title;
        private bool _pause;
        private ObservableCollection<SelectedPlayable> _selectedPlayables = new ObservableCollection<SelectedPlayable>();

        public ArticlePageViewModel(
            ICachedBigbossService cachedBigbossService,
            ILogger logger)
        {
            _cachedBigbossService = cachedBigbossService;
            _logger = logger;

            NavigateToArticleCommand = new Command<ProgramInformationModel>(ExecuteNavigateToArticleCommand);
            UpdatedPlayingVideoUrlCommand = new Command<SelectedPlayable>(ExecuteUpdatedPlayingVideoUrlCommand);
        }

        public ICommand NavigateToArticleCommand { get; }

        public ICommand UpdatedPlayingVideoUrlCommand { get; }

        public ArticleModel Article
        {
            get => _article;
            set
            {
                _article = value;
                RaisePropertyChanged(() => Article);
            }
        }

        public string Title
        {
            get => _title;
            set { _title = value; RaisePropertyChanged(() => Title); }
        }

        public string Url
        {
            get => _url;
            set
            {
                _url = value;
                RaisePropertyChanged(() => Url);
            }
        }

        public bool Pause
        {
            get => _pause;
            set
            {
                _pause = value;
                RaisePropertyChanged(() => Pause);
            }
        }

        public ObservableCollection<SelectedPlayable> SelectedPlayables
        {
            get => _selectedPlayables;
            set
            {
                _selectedPlayables = value;
                RaisePropertyChanged(() => SelectedPlayables);
            }
        }

        public override void OnNavigatedFrom(INavigationParameters navigationParameters)
        {
            base.OnNavigatedFrom(navigationParameters);

            Pause = true;
        }

        public override async void OnNavigatedTo(INavigationParameters navigationParameters)
        {
            base.OnNavigatedTo(navigationParameters);
            if (navigationParameters.ContainsKey(NavigationParameterKeys.__NavigationMode) &&
                navigationParameters.GetValue<NavigationMode>(NavigationParameterKeys.__NavigationMode) ==
                NavigationMode.New)
            {
                await LoadData(navigationParameters);
            }
        }

        private Task LoadData(INavigationParameters navigationParameters)
        {
            return DialogService.LoadingAsync(async () =>
            {
                IsBusy = !IsBusy;

                _logger.Information("Loading");

                try
                {
                    var articleUrl =
                        navigationParameters.GetValue<ProgramInformationModel>(NavigationParameterKeys.ArticleUrl);
                    Title = articleUrl?.Title;
                    var collection = new ObservableCollection<SelectedPlayable>();
                    if (!string.IsNullOrEmpty(articleUrl.Url))
                    {
                        var article = await _cachedBigbossService.GetArticle(articleUrl.Url);
                        Article = ArticleModel.Transform(article);
                        Article.Content = Article.Title;

                        if (Article.VideoUrl.Count <= 1)
                        {
                            Url = Article.VideoUrl[0] + "&img" + Article.VideoBanner;
                        }

                        for (int i = 0; i < Article.VideoUrl.Count - 1; i++)
                        {
                            if (i == 0)
                            {
                                Url = Article.VideoUrl[i] + "&img" + Article.VideoBanner;
                            }

                            collection.Add(new SelectedPlayable()
                            {
                                Title = $"Part - {i + 1}",
                                Url = Article.VideoUrl[i] + "&img" + Article.VideoBanner,
                                IsSelected = i == 0
                            });
                        }
                    }

                    SelectedPlayables = collection;
                }
                catch (System.Exception exception)
                {
                    _logger.Error(exception, exception.Message);
                    Article = ArticleModel.Default();
                }

                IsBusy = !IsBusy;
            }, "Loading episode");
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

        private void ExecuteUpdatedPlayingVideoUrlCommand(SelectedPlayable playable)
        {
            SelectedPlayables.All(x =>
            {
                x.IsSelected = false;
                return true;
            });

            playable.IsSelected = true;
            Url = playable.Url;
        }
    }
}
