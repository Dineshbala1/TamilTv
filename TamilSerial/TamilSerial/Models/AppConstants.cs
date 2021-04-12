using Xamarin.Essentials;

namespace TamilSerial.Models
{
    public class AppConstants
    {
        public const string ApiUrl = "http://bigboss.centralus.cloudapp.azure.com:5000/api/";
        public const string HostUrl = "https://www.biggboss4.net/";
        public const string SearchUrl = "https://www.biggboss4.net/?s={0}";

        public const string CategoryCacheKey = nameof(CategoryCacheKey);
        public const string ArticlesCacheKey = nameof(ArticlesCacheKey);
    }

    public class NavigationKeys
    {
        public const string HomePage = nameof(HomePage);
        public const string ArticlePage = nameof(ArticlePage);
        public const string SearchPage = nameof(SearchPage);
        public const string NoInternetPage = nameof(NoInternetPage);
    }

    public class NavigationParameterKeys
    {
        public const string _NavigationMode = nameof(_NavigationMode);
        public const string ArticleUrl = nameof(ArticleUrl);
    }
}
