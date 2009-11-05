using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Laan.Sql.Parser;
using Laan.Sql.Parser.Entities;

namespace Laan.Sql.Formatter
{
    public class DeleteStatementFormatter : CustomStatementFormatter<DeleteStatement>, IStatementFormatter
    {
        public DeleteStatementFormatter( IIndentable indentable, StringBuilder sql, DeleteStatement statement )
            : base( indentable, sql, statement )
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
            IndentAppend( Constants.Delete );
            FormatTop( _statement.Top );
            Append( _statement.TableName != null ? " " + _statement.TableName : "" );
        }

        #endregion
    }
}
