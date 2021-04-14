using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace TamilTv.Models
{
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
}