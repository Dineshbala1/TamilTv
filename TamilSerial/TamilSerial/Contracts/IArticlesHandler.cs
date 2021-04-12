using System.Threading.Tasks;
using TamilSerial.Models;

namespace TamilSerial.Contracts
{
    public interface IArticlesHandler
    {
        bool HasNextPage { get; }

        bool HasPreviousPage { get; }

        Task <PagedArticle> LoadArticles(string categoryUrl);

        Task<PagedArticle> ExecuteGetNextPage();

        Task<PagedArticle> ExecuteGetPreviousPage();
    }
}