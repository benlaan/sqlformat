using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Laan.Sql.Parser;

namespace Laan.Sql.Formatter
{
    public class FormattingEngine : IFormattingEngine
    {
        /// <summary>
        /// Initializes a new instance of the FormattingEngine class.
        /// </summary>
        public FormattingEngine()
        {
            UseTabChar = false;
        }

        public string Execute( string sql )
        {
            var outSql = new StringBuilder( (int)( sql.Length * 1.5 ) );
            var statements = ParserFactory.Execute( sql );

            var indentation = new Indentation();
            foreach ( var statement in statements )
            {
                var formatter = StatementFormatterFactory.GetFormatter( indentation, outSql, statement );
                formatter.Execute();

                if ( statement != statements.Last() )
                    outSql.AppendLine( "\n" );
            }
            return outSql.ToString();
        }

        public int IndentStep { get; set; }
        public int TabSize { get; set; }
        public bool UseTabChar { get; set; }
    }
}
