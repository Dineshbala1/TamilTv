using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using TamilTv.Models;

namespace TamilTv.Extensions
{
    public static class CommonExtensions
    {
        public static int GetIntFromString(this string content)
        {
            return Int32.Parse(content);
        }

        public static DateTime? GetDateFromString(this string inputText)
        {
            var regex = new Regex(@"\b\d{2}\-\d{2}-\d{4}\b");
            foreach (Match m in regex.Matches(inputText))
            {
                if (DateTime.TryParseExact(m.Value, "dd-MM-yyyy", null, DateTimeStyles.None, out var dt))
                    return dt;
            }
            return null;
        }

        public static ObservableCollection<CategoryGrouping<string, Category>> GetCategoryGrouping(
            this IList<Category> categories)
        {
            return categories
                .GroupBy(x => x.CategoryName)
                .Select(grouping => new CategoryGrouping<string, Category>(grouping.Key, grouping))
                .ToObservableCollection();
        }

        public static ObservableCollection<T> ToObservableCollection<T>(this IList<T> collection)
        {
            return new ObservableCollection<T>(collection);
        }

        public static ObservableCollection<T> ToObservableCollection<T>(this IEnumerable<T> collection)
        {
            return new ObservableCollection<T>(collection);
        }

        public static void AddBulk<T>(this ObservableCollection<T> collection, IList<T> itemsToAdd)
        {
            foreach (var item in itemsToAdd)
            {
                collection.Add(item);
            }
        }

        public static void ResetAll<T>(this IList<T> collection, Func<T, bool> predicate)
        {
            collection.All(predicate);
        }

        public static ProgramInformationModel TransformToProgramInformationModel(this ProgramInformation program)
        {
            string titleTrimmed = null;
            var dateOfProgram = program.Title?.GetDateFromString()?.ToString("dd-MM-yyyy");
            if (!string.IsNullOrEmpty(dateOfProgram))
            {
                var dateIndex = program?.Title?.IndexOf(dateOfProgram);
                if (dateIndex.HasValue && dateIndex.Value > 0)
                {
                    titleTrimmed = program.Title.Remove(dateIndex.Value);
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

        public static ArticleModel TransformToArticleModel(this Article article)
        {
            return new ArticleModel
            {
                Title = article.Title,
                Content = article.Content,
                VideoUrl = article.VideoUrl,
                VideoBanner = article.VideoBanner,
                ProgramInformationList =
                    article.ProgramInformations.Select(x => x.TransformToProgramInformationModel()).ToList()
            };
        }
    }
}
