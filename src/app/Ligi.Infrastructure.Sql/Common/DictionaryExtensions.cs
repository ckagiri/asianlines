using System.Collections.Generic;

namespace Ligi.Infrastructure.Sql.Common
{
    /// <summary>
    /// Usability extensions for dictionaries.
    /// </summary>
    internal static class DictionaryExtensions
    {
        /// <summary>
        /// Gets an item from the dictionary, if it's found.
        /// </summary>
        public static TValue TryGetValue<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key)
        {
            return dictionary.TryGetValue(key, default(TValue));
        }

        /// <summary>
        /// Gets an item from the dictionary, if it's found. Otherwise, 
        /// returns the specified default value.
        /// </summary>
        public static TValue TryGetValue<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, TValue defaultValue)
        {
            var result = defaultValue;
            if (!dictionary.TryGetValue(key, out result))
                return defaultValue;

            return result;
        }
    }
}
