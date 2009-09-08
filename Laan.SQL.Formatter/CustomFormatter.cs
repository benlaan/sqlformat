using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Laan.SQL.Parser;

namespace Laan.SQL.Formatter
{
    public class CustomFormatter
    {
        public CustomFormatter()
        {
            
        }

        protected string _indent;
        protected int _indentStep;
        protected StringBuilder _sql;

        protected void IndentedAppend( string text )
        {
            for ( int count = 0; count < _indentStep; count++ )
                _sql.Append( _indent );
            _sql.Append( text );
        }

        protected void NewLine( int times )
        {
            for ( int index = 0; index < times; index++ )
                _sql.AppendLine();
        }

        protected void NewLine()
        {
            NewLine( 1 );
        }

        protected void IndentedAppendFormat( string text, params object[] args )
        {
            IndentedAppend( String.Format( text, args ) );
        }
    }
}
