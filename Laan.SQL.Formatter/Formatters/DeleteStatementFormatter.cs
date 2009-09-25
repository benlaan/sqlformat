using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Laan.SQL.Parser;

namespace Laan.SQL.Formatter
{
    public class DeleteStatementFormatter : CustomFormatter<DeleteStatement>, IStatementFormatter
    {
        public DeleteStatementFormatter( string indent, int indentStep, StringBuilder sql, DeleteStatement statement )
            : base( indent, indentStep, sql, statement )
        {
        }

        #region IStatementFormatter Members

        public void Execute()
        {
            FormatDelete();
            FormatFrom();
            FormatJoins();
            FormatWhere();
            FormatTerminator();
        }

        private void FormatDelete()
        {
            _sql.Append( Constants.Delete );
            FormatTop( _statement.Top );
            _sql.Append( _statement.TableName != null ? " " + _statement.TableName : "" );
        }

        #endregion
    }
}
