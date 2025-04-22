using System;
using System.Collections.Generic;
using System.Linq;

namespace ReadingRoomApp.Common.Extensions
{
    public static class CollectionExtensions
    {
        public static bool IsNullOrEmpty<T>(this IEnumerable<T> collection)
        {
            return collection == null || !collection.Any();
        }

        public static IEnumerable<T> EmptyIfNull<T>(this IEnumerable<T> collection)
        {
            return collection ?? Enumerable.Empty<T>();
        }

        public static void ForEach<T>(this IEnumerable<T> collection, Action<T> action)
        {
            if (collection == null) return;

            foreach (var item in collection)
            {
                action(item);
            }
        }

        public static List<TResult> ConvertAll<TSource, TResult>(this IEnumerable<TSource> collection, Func<TSource, TResult> converter)
        {
            if (collection == null) return new List<TResult>();

            return collection.Select(converter).ToList();
        }
    }
}