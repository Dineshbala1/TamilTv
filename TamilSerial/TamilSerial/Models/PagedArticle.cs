using System.Collections.Generic;

namespace TamilTv.Models
{
    public class PagedArticle
    {
        public List<PaginationDetail> PaginationDetail { get; set; }
        public List<ProgramInformation> ProgramInformations { get; set; }

        public static PagedArticle Default()
        {
            return new PagedArticle
            {
                PaginationDetail = new List<PaginationDetail>(),
                ProgramInformations = new List<ProgramInformation>()
            };
        }
    }
}