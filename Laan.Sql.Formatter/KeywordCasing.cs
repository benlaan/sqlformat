namespace Laan.Sql.Formatter
{
    /// <summary>
    /// Specifies the casing style for SQL keywords
    /// </summary>
    public enum KeywordCasing
    {
        /// <summary>
        /// All keywords in UPPERCASE (e.g., SELECT, FROM, WHERE)
        /// </summary>
        Upper,

        /// <summary>
        /// All keywords in lowercase (e.g., select, from, where)
        /// </summary>
        Lower,

        /// <summary>
        /// Keywords in PascalCase (e.g., Select, From, Where)
        /// </summary>
        Pascal
    }
}
