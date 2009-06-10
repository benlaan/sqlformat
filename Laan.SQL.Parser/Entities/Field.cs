using System.Diagnostics;

using Laan.SQL.Parser.Expressions;

namespace Laan.SQL.Parser
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