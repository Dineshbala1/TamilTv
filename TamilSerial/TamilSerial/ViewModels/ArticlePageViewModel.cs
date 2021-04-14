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
using Xamarin.Forms;

namespace TamilSerial.ViewModels
{
    public class ArticlePageViewModel : ViewModelBase
    {
        private readonly ICachedBigbossService _cachedBigBossService;
        private readonly ILogger _logger;

        private ArticleModel _article = ArticleModel.Default();
        private string _url;
        private string _title;
        private bool _pause;
        private ObservableCollection<SelectedPlayable> _selectedPlayables = new ObservableCollection<SelectedPlayable>();

        public ArticlePageViewModel(
            ICachedBigbossService cachedBigBossService,
            ILogger logger)
        {
            _cachedBigBossService = cachedBigBossService;
            _logger = logger;

            NavigateToArticleCommand = new AsyncCommand<ProgramInformationModel>(ExecuteNavigateToArticleCommand);
            UpdatedPlayingVideoUrlCommand = new AsyncCommand<SelectedPlayable>(ExecuteUpdatedPlayingVideoUrlCommand);
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
                var programInformationModel =
                    navigationParameters.GetValue<ProgramInformationModel>(NavigationParameterKeys.ArticleUrl);
                Title = programInformationModel?.Title;

                await LoadData(programInformationModel?.Url);
            }
        }

        private Task LoadData(string url)
        {
            return DialogService.LoadingAsync(async () => { await LoadProgramInformation(url); }, "Loading episode");
        }

        private async Task LoadProgramInformation(string url)
        {
            IsBusy = !IsBusy;
            try
            {
                if (!string.IsNullOrEmpty(url))
                {
                    var article = await _cachedBigBossService.GetArticle(url);

                    var articleModel = article.TransformToArticleModel();
                    var collection = new ObservableCollection<SelectedPlayable>();

                    articleModel.Content = articleModel.Title;
                    if (articleModel.VideoUrl.Count <= 1)
                    {
                        Url = articleModel.VideoUrl[0] + "&img" + articleModel.VideoBanner;
                    }

                    CreatePartitionedVideo(articleModel, collection);

                    Article = articleModel;
                    SelectedPlayables = collection;
                }
            }
            catch (System.Exception exception)
            {
                _logger.Error(exception, exception.Message);
                Article = ArticleModel.Default();
            }
            finally
            {
                IsBusy = !IsBusy;
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
                return;
            }

            await NavigationService.NavigateAsync(NavigationKeys.ArticlePage,
                new NavigationParameters {{NavigationParameterKeys.ArticleUrl, programInformation}});
        }

        private Task ExecuteUpdatedPlayingVideoUrlCommand(SelectedPlayable playable)
        {
            SelectedPlayables.ResetAll(x => x.IsSelected = false);

            playable.IsSelected = true;
            Url = playable.Url;

            return Task.CompletedTask;
        }

        private void CreatePartitionedVideo(ArticleModel articleModel, ObservableCollection<SelectedPlayable> collection)
        {
            for (int i = 0; i < articleModel.VideoUrl.Count - 1; i++)
            {
                if (i == 0)
                {
                    Url = articleModel.VideoUrl[i] + "&img" + articleModel.VideoBanner;
                }

                collection.Add(new SelectedPlayable
                {
                    Title = $"Part - {i + 1}",
                    Url = articleModel.VideoUrl[i] + "&img" + articleModel.VideoBanner,
                    IsSelected = i == 0
                });
            }
        }
    }
}
