using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Text.RegularExpressions;

namespace TamilSerial.Contracts
{
    public static class CommonExtensions
    {
        public static int ToInt(this string content)
        {
            return Int32.Parse(content);
        }

        public static DateTime? GetFirstDateFromString(this string inputText)
        {
            var regex = new Regex(@"\b\d{2}\-\d{2}-\d{4}\b");
            foreach (Match m in regex.Matches(inputText))
            {
                DateTime dt;
                if (DateTime.TryParseExact(m.Value, "dd-MM-yyyy", null, DateTimeStyles.None, out dt))
                    return dt;
            }
            return null;
        }

        public static ObservableCollection<T> ToObservableCollection<T>(this IList<T> collection)
        {
            return new ObservableCollection<T>(collection);
        }

        public static ObservableCollection<T> ToObservableCollection<T>(this IEnumerable<T> collection)
        {
            return new ObservableCollection<T>(collection);
        }
    }
}
