using System;

namespace Laan.Sql.Parser.Entities
{
    internal static class Extensions
    {
        public static string WithBrackets( this string field )
        {
            return String.Format( "[{0}]", field.Trim( new char[] { '[', ']' } ) );
        }
    }
}
