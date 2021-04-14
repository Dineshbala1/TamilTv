using System.Collections.Generic;
using System.Threading.Tasks;
using TamilTv.Models;

namespace TamilSerial.Contracts
{
    public interface IBigBossService
    {
        Task<IList<Category>> GetCategories();

        Task<PagedArticle> GetArticles(string categoryUrl);

        Task<Article> GetArticle(string articleUrl);

        Task<PagedArticle> Search(string searchString);
    }
}