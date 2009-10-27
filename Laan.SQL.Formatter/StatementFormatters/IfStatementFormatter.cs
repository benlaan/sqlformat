using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Laan.SQL.Parser;

namespace Laan.SQL.Formatter
{
    public class IfStatementFormatter : CustomStatementFormatter<IfStatement>, IStatementFormatter
    {
        public IfStatementFormatter( IIndentable indentable, StringBuilder sql, IfStatement statement )
            : base( indentable, sql, statement )
        {

        }

        #region IStatementFormatter Members

        public void Execute()
        {
            AppendLineFormat( "{0} {1}", Constants.If, _statement.Condition.FormattedValue( 0, this ) );
            using ( new IndentScope( this ) )
            {
                FormatStatement( _statement.If );
            }
            if ( _statement.Else != null )
            {
                AppendLine( Constants.Else );
                using ( new IndentScope( this ) )
                {
                    FormatStatement( _statement.Else );
                }
            }
        }

        #endregion
    }
}
