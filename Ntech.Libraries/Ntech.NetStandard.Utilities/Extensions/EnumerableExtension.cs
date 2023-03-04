using System;
using System.Collections.Generic;
using System.Linq;

namespace Ntech.NetStandard.Utilities
{
    public static class EnumerableExtension
    {
        [System.Diagnostics.DebuggerStepThrough]
        public static IEnumerable<T> Empty<T>()
        {
            return Enumerable.Empty<T>();
        }
        public static IEnumerable<T> Singleton<T>(T value)
        {
            return new[] { value };
        }
        /// <summary>
        /// Returns true if the seq is empty
        /// </summary>
        /// <typeparam name="T">Seq item type</typeparam>
        /// <param name="source">the sequence</param>
        /// <returns>true if the seq is empty, false otherwise</returns>
        [System.Diagnostics.DebuggerStepThrough]
        public static bool IsEmpty<T>(this IEnumerable<T> source)
        {
            return !source.EmptyIfNull().Any();
        }
        /// <summary>
        /// Returns true if the seq is not empty, handles nulls
        /// </summary>
        /// <typeparam name="T">Seq item type</typeparam>
        /// <param name="source">the sequence</param>
        /// <param name="_">always true</param>
        /// <returns>true if the seq is not empty, false otherwise</returns>
        [System.Diagnostics.DebuggerStepThrough, System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "handleNull")]
        public static bool Any<T>(this IEnumerable<T> source, bool handleNull)
        {
            return source.EmptyIfNull().Any();
        }
        /// <summary>
        /// Returns given items seq or empty seq if the items argument is null
        /// </summary>
        /// <typeparam name="T">seq item type</typeparam>
        /// <param name="items">seq to check for null</param>
        /// <returns>given items seq or empty seq if items is null</returns>
        [System.Diagnostics.DebuggerStepThrough]
        public static IEnumerable<T> EmptyIfNull<T>(this IEnumerable<T> items)
        {
            return items ?? Enumerable.Empty<T>();
        }

        /// <summary>
        /// A shortcut to get a sequence of static list of values, as in: Enumerable.OfValues(1, 2, 3) which returns sequence of 1, 2, 3
        /// </summary>
        /// <typeparam name="TValue">Value type</typeparam>
        /// <param name="values">list of values</param>
        /// <returns>sequence of the given values</returns>
        [System.Diagnostics.DebuggerStepThrough]
        public static IEnumerable<TValue> OfValues<TValue>(params TValue[] values)
        {
            return values;
        }

        /// <summary>
        /// Iterates the sequence and calls the given action with the item
        /// </summary>
        [System.Diagnostics.DebuggerStepThrough]
        public static void Iter<TItem>(this IEnumerable<TItem> items, Action<TItem> action)
        {
            items.Iteri((item, _) => action(item));
        }

        /// <summary>
        /// Iterates the sequence and calls the given action with the item and its index in the sequence
        /// </summary>
        [System.Diagnostics.DebuggerStepThrough]
        public static void Iteri<TItem>(this IEnumerable<TItem> items, Action<TItem, int> action)
        {
            var index = 0;
            foreach (var item in items.EmptyIfNull())
                action(item, index++);
        }

        [System.Diagnostics.DebuggerStepThrough]
        public static IEnumerable<TRes> Selecti<TItem, TRes>(this IEnumerable<TItem> items, Func<TItem, int, TRes> action)
        {
            var index = 0;
            foreach (var item in items.EmptyIfNull())
                yield return action(item, index++);
        }

        [System.Diagnostics.DebuggerStepThrough]
        public static IEnumerable<T> Flatten<T>(this IEnumerable<T> items, Func<T, IEnumerable<T>> children)
        {
            return items.EmptyIfNull().SelectMany(item => EnumerableExtension.Singleton(item).Concat(children(item).Flatten(children)));
        }
        /// <summary>
        /// Converts to ToList() and adds the new item into the existing collection and returns back the List.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="items"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        [System.Diagnostics.DebuggerStepThrough]
        public static IList<T> AddEx<T>(this IEnumerable<T> items, T value)
        {
            var list = items.ToList();
            list.Add(value);

            return list;
        }

        /// <summary>
        /// Converts to ToList() and adds the list of items to the existing list of item
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="items"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        //[System.Diagnostics.DebuggerStepThrough]
        public static IEnumerable<T> AddRangeEx<T>(this IEnumerable<T> items, IEnumerable<T> value)
        {
            var list = items.ToList();
            list.AddRange(value);
            return list;
        }

        public static List<List<T>> ChunkBy<T>(this IEnumerable<T> source, int chunkSize)
        {
            return source
                .Select((x, i) => new { Index = i, Value = x })
                .GroupBy(x => x.Index / chunkSize)
                .Select(x => x.Select(v => v.Value).ToList())
                .ToList();
        }

        [System.Diagnostics.DebuggerStepThrough]
        public static bool Contains<T>(this IEnumerable<T> items, Func<T, bool> predicate)
        {
            return !items.EmptyIfNull().Where(predicate).IsEmpty();
        }

        [System.Diagnostics.DebuggerStepThrough]
        public static bool ContainsEx<T>(this IEnumerable<T> items, T value, StringComparison compareOption = StringComparison.CurrentCulture)
        {
            if (compareOption != StringComparison.CurrentCulture)
            {
                return items.Contains(i => i.ToString().Equals(value.ToString(), compareOption));
            }
            else
            {
                return items.Contains(i => i.Equals(value));
            }
        }
        [System.Diagnostics.DebuggerStepThrough]
        public static bool ContainsIgnoreCase<T>(this IEnumerable<T> items, T value)
        {
            return items.Contains(i => i.ToString().Equals(value.ToString(),StringComparison.InvariantCultureIgnoreCase));
        }
    }
}
