using System.Collections.Generic;
using System.Threading.Tasks;
using TamilSerial.Models;

namespace TamilSerial.Contracts
{
    public interface IBigBossService
    {
        Task<IList<Categories>> GetCategories();

        Task<PagedArticle> GetArticles(string categoryUrl);

        Task<Article> GetArticle(string articleUrl);

        Task<PagedArticle> Search(string searchString);
    }
}