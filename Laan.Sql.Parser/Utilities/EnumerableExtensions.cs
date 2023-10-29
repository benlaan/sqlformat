using System;
using System.Collections.Generic;
using System.Linq;

namespace Laan.Sql.Parser
{
    public static class EnumerableExtensions
    {
        public static string Join<T>(this IEnumerable<T> enumerable, string separator)
        {
            return String.Join(separator, enumerable.Select(e => e.ToString()));
        }

        public static string ToCsv<T>(this IEnumerable<T> enumerable)
        {
            return enumerable.Join(", ");
        }
    }
}
