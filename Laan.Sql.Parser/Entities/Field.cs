using System.Diagnostics;

using Laan.Sql.Parser.Expressions;

namespace Laan.Sql.Parser.Entities
{
    [DebuggerDisplay( "{Expression.Value} ({Alias})" )]
    public class Field : AliasedEntity
    {
        /// <summary>
        /// Initializes a new instance of the Field class.
        /// </summary>
        public Field()
        {
        }

        public Expression Expression { get; set; }
    }
}