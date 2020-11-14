using System;
using System.Linq;
using h73.Elastic.Core.Search.Results;

namespace h73.Elastic.Search.Helpers
{
    /// <summary>
    /// Helpers for Type
    /// </summary>
    public static class TypeHelpers
    {
        private static readonly char[] InvalidChars = { '\'', '\\', '\"', '`' };

        /// <summary>
        /// Type string to index string.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>Index name</returns>
        public static string TypeStringToIndexString(string type)
        {
            return Remove(type.ToLower().Replace(".", "_"), InvalidChars);
        }

        /// <summary>
        /// Removes the specified source.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="oldChar">The old character.</param>
        /// <returns>String with replaced chars</returns>
        public static string Remove(string source, char[] oldChar)
        {
            return string.Join(string.Empty, source.ToCharArray().Where(a => !oldChar.Contains(a)).ToArray());
        }

        public static bool IsSubclassOfRawGeneric(this object obj, Type generic)
        {
            var toCheck = obj.GetType();
            while (toCheck != null && toCheck != typeof(object)) {
                var cur = toCheck.IsGenericType ? toCheck.GetGenericTypeDefinition() : toCheck;
                if (generic == cur) {
                    return true;
                }
                toCheck = toCheck.BaseType;
            }
            return false;
        }

        public static TopHitsResult<T> TopHits<T>(this Bucket buckets)
        {
            return ((TopHitsResult<T>) buckets.TopHits);
        }

    }
}
