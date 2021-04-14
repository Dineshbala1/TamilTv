using System.Collections.Generic;
using System.Linq;
using TamilTv.Extensions;

namespace TamilTv.Models
{
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
                ProgramInformationList = new List<ProgramInformationModel>
                {
                    ProgramInformationModel.GetDummyProgramInformationModel(),
                    ProgramInformationModel.GetDummyProgramInformationModel(),
                    ProgramInformationModel.GetDummyProgramInformationModel(),
                    ProgramInformationModel.GetDummyProgramInformationModel()
                }
            };
        }
    }
}