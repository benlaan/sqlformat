using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Laan.Sql.Parser;
using Laan.Sql.Parser.Entities;

namespace Laan.Sql.Formatter
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
            IndentAppendLineFormat( "{0} {1}", Constants.If, _statement.Condition.FormattedValue( 0, this ) );
            FormatBlock( _statement.If );
            if ( _statement.Else != null )
            {
                NewLine();
                IndentAppendLine( Constants.Else );
                FormatBlock( _statement.Else );
            }
        }

        #endregion
    }
}
