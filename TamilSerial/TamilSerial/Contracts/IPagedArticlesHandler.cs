using System.Collections.Generic;
using System.Threading.Tasks;
using TamilTv.Models;

namespace TamilTv.Contracts
{
    public interface IPagedArticlesHandler
    {
        bool HasNextPage { get; }

        bool HasPreviousPage { get; }

        Task <IList<ProgramInformationModel>> LoadArticles(string categoryUrl);

        Task<IList<ProgramInformationModel>> ExecuteGetNextPage();

        Task ExecuteGetPreviousPage();
    }
}