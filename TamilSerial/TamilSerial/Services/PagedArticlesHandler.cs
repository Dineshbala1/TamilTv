using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TamilSerial.Contracts;
using TamilTv.Contracts;
using TamilTv.Extensions;
using TamilTv.Models;

namespace TamilTv.Services
{
    public class PagedArticlesHandler : IPagedArticlesHandler
    {
        private readonly ICachedBigbossService _cachedBigbossService;

        public PagedArticlesHandler(ICachedBigbossService cachedBigbossService)
        {
            _cachedBigbossService = cachedBigbossService;
        }

        private PagedArticle _pagedArticle;

        public bool HasNextPage => _pagedArticle.PaginationDetail.Any() &&
                                   _pagedArticle?.PaginationDetail?.SingleOrDefault(x => x.IsCurrent)?.PageNumber
                                       ?.GetIntFromString() >= 1;

        public bool HasPreviousPage => _pagedArticle.PaginationDetail.Any() &&
                                       _pagedArticle?.PaginationDetail?.SingleOrDefault(x => x.IsCurrent)?.PageNumber
                                           ?.GetIntFromString() > 1;

        public async Task<IList<ProgramInformationModel>> LoadArticles(string categoryUrl)
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

            return _pagedArticle.ProgramInformations.Select(x => x.TransformToProgramInformationModel()).ToList();
        }

        public Task<IList<ProgramInformationModel>> ExecuteGetNextPage()
        {
            var currentPage = _pagedArticle.PaginationDetail.SingleOrDefault(x => x.IsCurrent);
            return LoadArticles(currentPage?.PageUrl.Replace("page/" + currentPage.PageNumber,
                "page/" + (currentPage.PageNumber.GetIntFromString() + 1)));
        }

        public Task ExecuteGetPreviousPage()
        {
            var currentPage = _pagedArticle.PaginationDetail.SingleOrDefault(x => x.IsCurrent);
            if (currentPage != null && currentPage.PageNumber.GetIntFromString() <= 1)
            {
                return null;
            }

            return LoadArticles(currentPage.PageUrl.Replace(currentPage.PageNumber,
                (currentPage.PageNumber.GetIntFromString() - 1).ToString()));
        }
    }
}