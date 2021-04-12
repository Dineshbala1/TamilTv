using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Polly;
using Serilog;
using TamilSerial.Contracts;
using TamilSerial.Models;

namespace TamilSerial.Services
{
    public class BigBossService : IBigBossService
    {
        private readonly IApiService _apiService;

        public BigBossService(IApiService apiService)
        {
            _apiService = apiService;
        }

        private void FallbackAction()
        {
            throw new NotImplementedException();
        }

        public async Task<IList<Categories>> GetCategories()
        {
            IList<Categories> categories = new List<Categories>();
            var response = await GetCategoriesFromService(AppConstants.HostUrl);
            if (response!= null && response.Any())
            {
                foreach (var kvp in response)
                {
                    foreach (var menuItem in kvp.Value)
                    {
                        categories.Add(new Categories
                        {
                            CategoryName = kvp.Key,
                            Title = menuItem.Title,
                            Url = menuItem.Url
                        });
                    }
                }
            }

            return categories;
        }

        public async Task<PagedArticle> GetArticles(string categoryUrl)
        {
            return await GetFallback(() => _apiService.GetArticles(categoryUrl));
        }

        public Task<Article> GetArticle(string articleUrl)
        {
            return GetFallback(() => _apiService.GetArticle(articleUrl));
        }

        public Task<PagedArticle> Search(string searchString)
        {
            return GetFallback(() => _apiService.GetArticles(searchString));
        }

        private async Task<IDictionary<string, IList<MenuItem>>> GetCategoriesFromService(string category)
        {
            return await GetFallback(() => _apiService.GetCategories(category));
        }

        private async Task<T> GetFallback<T>(Func<Task<T>> exeFunc)
        {
            var fallback = Policy<T>
                .Handle<Exception>()
                .FallbackAsync(default(T));
            var retry = Policy<T>
                .Handle<HttpRequestException>()
                .WaitAndRetryAsync(3, i => TimeSpan.FromSeconds(3), onRetry: (result, span, retryAttempt, context) =>
                {
                    Log.Logger.Error(result.Exception, result.Exception.Message);

                    if (retryAttempt >= 3)
                    {
                        throw new TimeoutException("Http request timed out exception");
                    }
                });

            return await fallback.WrapAsync(retry)
                .ExecuteAsync(exeFunc);
        }
    }
}
