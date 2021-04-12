using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TamilSerial.Contracts;
using TamilSerial.Models;

namespace TamilSerial.Services
{
    public class ArticlesHandler : IArticlesHandler
    {
        private readonly ICachedBigbossService _cachedBigbossService;

        public ArticlesHandler(ICachedBigbossService cachedBigbossService)
        {
            _cachedBigbossService = cachedBigbossService;
        }

        private PagedArticle _pagedArticle;

        public bool HasNextPage => _pagedArticle.PaginationDetail.Any() &&
                                   _pagedArticle?.PaginationDetail?.SingleOrDefault(x => x.IsCurrent)?.PageNumber
                                       ?.ToInt() >= 1;

        public bool HasPreviousPage => _pagedArticle.PaginationDetail.Any() &&
                                       _pagedArticle?.PaginationDetail?.SingleOrDefault(x => x.IsCurrent)?.PageNumber
                                           ?.ToInt() > 1;

        public async Task<PagedArticle> LoadArticles(string categoryUrl)
        {
            _pagedArticle = await _cachedBigbossService.GetArticles(categoryUrl) ?? new PagedArticle
            {
                PaginationDetail = new List<PaginationDetail>(),
                ProgramInformations = new List<ProgramInformation>()
            };

            var currentPage = _pagedArticle.PaginationDetail.SingleOrDefault(x => x.IsCurrent);
            if (currentPage != null && !currentPage.PageUrl.Contains("/page/"))
            {
                currentPage.PageUrl = currentPage.PageUrl + "/page/1";
            }

            return _pagedArticle;
        }

        public Task<PagedArticle> ExecuteGetNextPage()
        {
            var currentPage = _pagedArticle.PaginationDetail.SingleOrDefault(x => x.IsCurrent);
            return LoadArticles(currentPage?.PageUrl.Replace("page/" + currentPage.PageNumber,
                "page/" + (currentPage.PageNumber.ToInt() + 1)));
        }

        public Task<PagedArticle> ExecuteGetPreviousPage()
        {
            var currentPage = _pagedArticle.PaginationDetail.SingleOrDefault(x => x.IsCurrent);
            if (currentPage != null && currentPage.PageNumber.ToInt() <= 1)
            {
                return null;
            }

            return LoadArticles(currentPage.PageUrl.Replace(currentPage.PageNumber,
                (currentPage.PageNumber.ToInt() - 1).ToString()));
        }
    }
}