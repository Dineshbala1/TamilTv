using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using TamilSerial.Contracts;

namespace TamilSerial.Models
{
    public class MenuItem
    {
        public string Title { get; set; }
        public string Url { get; set; }
        public string Id { get; set; }
    }

    public class Categories
    {
        public string CategoryName { get; set; }
        public string Title { get; set; }
        public string Url { get; set; }
    }

    public class CategoryGrouping<K, T> : ObservableCollection<T>
    {
        public K Key { get; }

        public CategoryGrouping(K categoryName, IEnumerable<T> categories)
        {
            Key = categoryName;
            foreach (var category in categories)
            {
                this.Add(category);
            }
        }
    }

    public class PaginationDetail
    {
        public string PageNumber { get; set; }
        public string PageUrl { get; set; }
        public bool IsCurrent { get; set; }
    }

    public class ProgramInformation
    {
        public string Title { get; set; }
        public string Url { get; set; }
        public string Image { get; set; }
        public string ImageAlternative { get; set; }
        public string CategoryId { get; set; }
        public string Id { get; set; }
    }

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

    public class Article
    {
        public string Title { get; set; }
        public string EpisodeDate { get; set; }
        public string Content { get; set; }
        public List<string> VideoUrl { get; set; }
        public string VideoBanner { get; set; }
        public string ProgramId { get; set; }
        public string id { get; set; }

        public List<ProgramInformation> ProgramInformations { get; set; }

        public static Article Default()
        {
            return new Article
            {
                Title = string.Empty,
                Content = string.Empty,
                ProgramInformations = new List<ProgramInformation>()
                {
                    new ProgramInformation {Title = "", Image = null, Url = null},
                    new ProgramInformation {Title = "", Image = null, Url = null},
                    new ProgramInformation {Title = "", Image = null, Url = null},
                    new ProgramInformation {Title = "", Image = null, Url = null}
                }
            };
        }
    }

    public class ArticleModel
    {
        public string Title { get; set; }
        public string EpisodeDate { get; set; }
        public string Content { get; set; }
        public List<string> VideoUrl { get; set; }
        public string VideoBanner { get; set; }

        public List<ProgramInformationModel> ProgramInformationList { get; set; }

        public static ArticleModel Default()
        {
            return new ArticleModel
            {
                Title = string.Empty,
                Content = string.Empty,
                ProgramInformationList = new List<ProgramInformationModel>()
                {
                   ProgramInformationModel.GetDummyProgramInformationModel(),
                   ProgramInformationModel.GetDummyProgramInformationModel(),
                   ProgramInformationModel.GetDummyProgramInformationModel(),
                   ProgramInformationModel.GetDummyProgramInformationModel()
                }
            };
        }

        public static ArticleModel Transform(Article article)
        {
            return new ArticleModel
            {
                Title = article.Title,
                Content = article.Content,
                VideoUrl = article.VideoUrl,
                VideoBanner = article.VideoBanner,
                ProgramInformationList =
                    article.ProgramInformations.Select(x => ProgramInformationModel.Transform(x)).ToList()
            };
        }
    }

    public class ProgramInformationModel
    {
        public string Title { get; set; }
        public string Url { get; set; }
        public string Image { get; set; }
        public string ImageAlternative { get; set; }
        public string Date { get; set; }


        public static ProgramInformationModel GetDummyProgramInformationModel()
        {
            return new ProgramInformationModel
            {
                Title = string.Empty,
                Image = null,
                Url = string.Empty,
                Date = string.Empty,
                ImageAlternative = string.Empty
            };
        }

        public static ProgramInformationModel Transform(ProgramInformation program)
        {
            string titleTrimmed = null;
            var dateOfProgram = program.Title?.GetFirstDateFromString()?.ToString("dd-MM-yyyy");
            if (!string.IsNullOrEmpty(dateOfProgram))
            {
                var inde = program?.Title?.IndexOf(dateOfProgram);
                if (inde.HasValue && inde.Value > 0)
                {
                    titleTrimmed = program.Title.Remove(inde.Value);
                }
            }

            return new ProgramInformationModel
            {
                Date = dateOfProgram,
                Title = titleTrimmed ?? program.Title,
                Image = program.Image,
                ImageAlternative = program.ImageAlternative,
                Url = program.Url
            };
        }

        public static List<ProgramInformationModel> GenerateDummyProgramInformationModel(int count)
        {
            var list = new List<ProgramInformationModel>();
            for (int i = 0; i < count; i++)
            {
                list.Add(GetDummyProgramInformationModel());
            }

            return list;
        }
    }

}
