using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Core
{
    public static class EnumerableHelper
    {
        public static IEnumerable<string> GetLines(this StreamReader reader)
        {
            while (!reader.EndOfStream)
            {
                yield return reader.ReadLine();
            }
        }

        public static IEnumerable<IGrouping<TKey,T>>  
            GroupBySequentually<T, TKey>(this IEnumerable<T> source, Func<T, TKey> selector) where TKey : struct
        {
            Grouping<TKey, T> currentGroup = null;
            foreach (var s in source)
            {
                var key = selector(s);
                if (currentGroup != null && !currentGroup.Key.Equals(key))
                {
                    yield return currentGroup;
                    currentGroup = new Grouping<TKey, T>(key);
                }
                if (currentGroup == null)
                {
                    currentGroup = new Grouping<TKey, T>(key);
                }
                currentGroup.Add(s);
            }
            yield return currentGroup;
        }

        public static int CollectionCount<T>(this IList<T> list, Func<T, bool> predicate)
        {
            var count = 0;
            for (var i = 0; i < list.Count; i++)
            {
                if (predicate(list[i]))
                {
                    count++;
                }
            }
            return count;
        }
    }

    public class Grouping<TKey, T> : IGrouping<TKey, T>
    {
        List<T> values;
        public Grouping(TKey key)
        {
            Key = key;
            values = new List<T>();
        }

        public void Add(T t)
        {
            values.Add(t);
        }

        public IEnumerator<T> GetEnumerator()
        {
            return values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public TKey Key { get; private set; }
    }
}
