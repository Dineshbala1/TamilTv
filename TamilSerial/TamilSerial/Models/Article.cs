using System.Collections.Generic;

namespace TamilTv.Models
{
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
                ProgramInformations = new List<ProgramInformation>
                {
                    new ProgramInformation {Title = "", Image = null, Url = null},
                    new ProgramInformation {Title = "", Image = null, Url = null},
                    new ProgramInformation {Title = "", Image = null, Url = null},
                    new ProgramInformation {Title = "", Image = null, Url = null}
                }
            };
        }
    }
}