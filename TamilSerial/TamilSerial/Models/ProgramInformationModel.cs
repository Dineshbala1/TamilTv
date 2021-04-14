using System.Collections.Generic;

namespace TamilTv.Models
{
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