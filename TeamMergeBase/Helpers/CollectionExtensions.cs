using System.Collections.Generic;

namespace TeamMergeBase.Helpers
{
    public static class CollectionExtensions
    {
        public static void AddRange<T>(this ICollection<T> list, IEnumerable<T> itemsToAdd)
        {
            foreach (var itemToAdd in itemsToAdd)
            {
                list.Add(itemToAdd);
            }
        }
    }
}
