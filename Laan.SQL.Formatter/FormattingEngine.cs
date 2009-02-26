using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Laan.SQL.Parser;

namespace Laan.SQL.Formatter
{
    public class FormattingEngine
    {
        public string Execute( string sql )
        {
            string padding = new string( ' ', 4 );

            // Exercise
            var statement = ParserFactory.Execute<SelectStatement>( sql );

            string result = "SELECT";

            const string NEW_LINE = "\r\n";

            if ( statement.Fields.Count == 1 )
                result += " " + statement.Fields[ 0 ].Expression.Value;
            else
                foreach ( var field in statement.Fields )
                    result += NEW_LINE + padding + field.Expression.Value;

            if ( statement.From != null )
            {
                result += String.Format( 
                    "{0}{0}FROM {1} {2}", 
                    NEW_LINE, statement.From[ 0 ].Name, statement.From[ 0 ].Alias 
                );
            }

            if ( statement.Where != null )
            {
                result += String.Format(
                    "{0}{0}WHERE {1}",
                    NEW_LINE,
                    statement.Where.Value
                );
            }
            return result;
        }
    }
}
