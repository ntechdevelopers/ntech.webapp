using System.Collections.Generic;
using System.Linq;

namespace Ntech.NetStandard.Utilities
{
    /// <summary>
    /// Define additional APIs for collection
    /// </summary>
    public static class CollectionExtension
    {

        /// <summary>
        /// Check current collection is null or empty
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collection">The collection.</param>
        /// <returns><c>true</c> if [is null or empty] [the specified collection]; otherwise, <c>false</c>.</returns>
        public static bool IsNullOrEmpty<T>(this ICollection<T> collection)
        {
            return collection == null || collection.Count == 0;
        }

        /// <summary>
        /// Check current collection is null or empty
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collection">The collection.</param>
        /// <returns><c>true</c> if [is null or empty] [the specified collection]; otherwise, <c>false</c>.</returns>
        public static bool IsNullOrEmpty<T>(this IEnumerable<T> collection)
        {
            return collection == null || !collection.Any();
        }

        /// <summary>
        /// Convert a list to hash set
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source">The source.</param>
        /// <returns>HashSet&lt;T&gt;.</returns>
        public static HashSet<T> ToHashSet<T>(this IEnumerable<T> source)
        {
            return new HashSet<T>(source);
        }

        /// <summary>
        /// Adds the range.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="target">The target.</param>
        /// <param name="source">The source.</param>
        /// <exception cref="ArgumentNullException">target
        /// or
        /// source</exception>
        public static void AddRange<T>(this ICollection<T> target, IEnumerable<T> source)
        {
            // Check Condition
            Preconditions.CheckNotNull(target);
            Preconditions.CheckNotNull(source);

            // Execute add element of source to target
            foreach (var element in source)
            {
                target.Add(element);
            }

        }
    }
}
