﻿using System.Collections.Generic;
using System.Threading.Tasks;
using Refit;
using TamilSerial.Models;

namespace TamilSerial.Contracts
{
    public interface IApiService
    {
        [Get("/OnDemand/{category}/categories")]
        Task<Dictionary<string, IList<MenuItem>>> GetCategories(string category);

        [Get("/OnDemand/{categoryUrl}/articles")]
        Task<PagedArticle> GetArticles(string categoryUrl);

        [Get("/OnDemand/{articleUrl}/article")]
        Task<Article> GetArticle(string articleUrl);
    }
}
