using System;
using System.Linq;

namespace Laan.Sql.Formatter
{
    /// <summary>
    /// Helper class for transforming SQL keyword casing
    /// </summary>
    public static class KeywordTransform
    {
        /// <summary>
        /// Applies the specified casing transform to a SQL keyword
        /// </summary>
        public static string Apply(string keyword, KeywordCasing casing)
        {
            if (string.IsNullOrEmpty(keyword))
                return keyword;

            switch (casing)
            {
                case KeywordCasing.Upper:
                    return keyword.ToUpperInvariant();

                case KeywordCasing.Lower:
                    return keyword.ToLowerInvariant();

                case KeywordCasing.Pascal:
                    return ToPascalCase(keyword);

                default:
                    return keyword;
            }
        }

        private static string ToPascalCase(string keyword)
        {
            if (string.IsNullOrEmpty(keyword))
                return keyword;

            // Handle single word
            if (!keyword.Contains(" ") && !keyword.Contains("_"))
            {
                return char.ToUpperInvariant(keyword[0]) + keyword.Substring(1).ToLowerInvariant();
            }

            // Handle multi-word keywords like "ORDER BY"
            var words = keyword.Split(new[] { ' ', '_' }, StringSplitOptions.RemoveEmptyEntries);
            return string.Join(" ", words.Select(w => 
                char.ToUpperInvariant(w[0]) + w.Substring(1).ToLowerInvariant()));
        }
    }
}
