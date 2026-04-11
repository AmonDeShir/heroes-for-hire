using System.Collections.Generic;
using System.Linq;

namespace Registry
{
    public delegate T SelectionStrategy<T>(IEnumerable<T> items);
    
    public static class Registry<T> where T : class
    {
        private static HashSet<T> items = new();

        public static bool TryAdd(T item)
        {
            return item != null && items.Add(item);
        }
        
        public static bool Remove(T item)
        {
            return items.Remove(item);
        }

        public static T GetFirst()
        {
            return items.FirstOrDefault();
        }

        public static T Get(SelectionStrategy<T> selectionStrategy)
        {
            return selectionStrategy(items);
        }

        public static IEnumerable<T> All()
        {
            return items;
        }
    }
}