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

        private ArticleModel _article;
        private string _url;
        private string _title;

        public ArticlePageViewModel(ICachedBigbossService cachedBigbossService, ILogger logger)
        {
            _cachedBigbossService = cachedBigbossService;
            _logger = logger;

            NavigateToArticleCommand = new Command<ProgramInformationModel>(ExecuteNavigateToArticleCommand);
        }

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

        public ICommand NavigateToArticleCommand { get; }

        public override async void OnNavigatedTo(INavigationParameters navigationParameters)
        {
            base.OnNavigatedTo(navigationParameters);

            LoadDummyData();

            await Task.Run(async () => await LoadData(navigationParameters)).ConfigureAwait(false);
        }

        private void LoadDummyData()
        {
            Article = ArticleModel.Default();
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

                    await Task.Delay(1000);

                    if (!string.IsNullOrEmpty(articleUrl.Url))
                    {
                        var article = await _cachedBigbossService.GetArticle(articleUrl.Url);
                        Article = ArticleModel.Transform(article);
                        Article.Content = Article.Title;

                        foreach (var videorUrl in Article.VideoUrl)
                        {
                            Url = videorUrl + "&img" + Article.VideoBanner;
                        }
                    }
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
                new NavigationParameters() {{NavigationParameterKeys.ArticleUrl, programInformation}});
        }
    }
}
