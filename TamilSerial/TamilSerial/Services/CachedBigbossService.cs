using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MonkeyCache;
using TamilSerial.Contracts;
using TamilTv.Models;
using Xamarin.Essentials;

namespace TamilTv.Services
{
    public class CachedBigbossService : ICachedBigbossService
    {
        private readonly IBigBossService _bigBossService;
        private readonly IBarrel _barrel;

        public CachedBigbossService(IBigBossService bigBossService, IBarrel barrel)
        {
            _bigBossService = bigBossService;
            _barrel = barrel;
        }

        public Task InvalidateCacheToRefresh()
        {
            _barrel.EmptyAll();
            return Task.CompletedTask;
        }

        public async Task<IList<Category>> GetCategories()
        {
            if (Connectivity.NetworkAccess == NetworkAccess.None)
            {
                _barrel.EmptyAll();
            }

            if (!_barrel.IsExpired(AppConstants.CategoryCacheKey))
            {
                return _barrel.Get<IList<Category>>(AppConstants.CategoryCacheKey);
            }

            var response = await _bigBossService.GetCategories();
            if (response.Any())
            {
                _barrel.Add(AppConstants.CategoryCacheKey, response, TimeSpan.FromDays(2));
                return _barrel.Get<IList<Category>>(AppConstants.CategoryCacheKey);
            }

            return new List<Category>();
        }

        public async Task<PagedArticle> GetArticles(string categoryUrl)
        {
            if (Connectivity.NetworkAccess == NetworkAccess.None)
            {
                _barrel.Empty(categoryUrl);
            }

            if (!_barrel.IsExpired(categoryUrl))
            {
                return _barrel.Get<PagedArticle>(categoryUrl);
            }

            var response = await _bigBossService.GetArticles(categoryUrl);
            if (response != null && response.ProgramInformations.Any())
            {
                _barrel.Add(categoryUrl, response, TimeSpan.FromDays(2));
                return _barrel.Get<PagedArticle>(categoryUrl);
            }

            return PagedArticle.Default();
        }

        public async Task<Article> GetArticle(string articleUrl)
        {
            if (Connectivity.NetworkAccess == NetworkAccess.None)
            {
                _barrel.Empty(articleUrl);
            }

            if (!_barrel.IsExpired(articleUrl))
            {
                return _barrel.Get<Article>(articleUrl);
            }

            var response = await _bigBossService.GetArticle(articleUrl);
            if (response != null)
            {
                _barrel.Add(articleUrl, response, TimeSpan.FromDays(2));
                return _barrel.Get<Article>(articleUrl);
            }

            return Article.Default();
        }

        public Task<PagedArticle> Search(string searchString)
        {
            return _bigBossService.Search(string.Format(AppConstants.SearchUrl, searchString));
        }
    }
}