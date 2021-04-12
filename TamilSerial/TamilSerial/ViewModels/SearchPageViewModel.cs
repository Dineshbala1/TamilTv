using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using TamilSerial.Contracts;
using TamilSerial.Models;
using TamilSerial.Presentation.Navigation;
using TamilSerial.ViewModels.Base;
using TamilTv.Contracts;
using Xamarin.Forms;

namespace TamilSerial.ViewModels
{
    public class SearchPageViewModel : ViewModelBase
    {
        private readonly ICachedBigbossService _cachedBigbossService;
        private readonly ILogger _logger;

        private string _searchText;
        private ObservableCollection<ProgramInformationModel> _programInformationList;

        public SearchPageViewModel(ICachedBigbossService cachedBigbossService, ILogger logger)
        {
            _cachedBigbossService = cachedBigbossService;
            _logger = logger;

            SearchCommand = new Command<string>(ExecuteSearchCommand);
            NavigateToArticleCommand = new Command<ProgramInformationModel>(ExecuteNavigateToArticleCommand);
            SearchPlaceHolder = "Enter your keywords to search";
        }

        public ICommand SearchCommand { get; }

        public ICommand NavigateToArticleCommand { get; }

        public string SearchText
        {
            get => _searchText;
            set
            {
                _searchText = value;
                RaisePropertyChanged(() => SearchText);
            }
        }

        public string SearchPlaceHolder { get; }

        public ObservableCollection<ProgramInformationModel> ProgramInformationList
        {
            get => _programInformationList;
            set
            {
                _programInformationList = value;
                RaisePropertyChanged(() => ProgramInformationList);
            }
        }

        private async void ExecuteSearchCommand(string searchText)
        {
            try
            {
                await DialogService.LoadingAsync(async () =>
                {
                    SearchText = searchText;
                    if (!string.IsNullOrEmpty(SearchText))
                    {
                        var response = await _cachedBigbossService.Search(SearchText);
                        ProgramInformationList =
                            new ObservableCollection<ProgramInformationModel>(
                                response.ProgramInformations.Select(x => ProgramInformationModel.Transform(x)));
                    }

                }, $"Searching {searchText}");
            }
            catch (System.Exception exception)
            {
                _logger.Error(exception, exception.Message);
                ProgramInformationList = new ObservableCollection<ProgramInformationModel>();
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
            await NavigationService.NavigateAsync(
                NavigationKeys.ArticlePage,
                new NavigationParameters() {{NavigationParameterKeys.ArticleUrl, programInformation}});
        }
    }
}
