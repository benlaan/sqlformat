using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

using Laan.SQL.Parser;

namespace Laan.SQL.Formatter
{
    public class CommitTransactionStatementFormatter : TransactionStatementFormatter<CommitTransactionStatement>, IStatementFormatter
    {
        public CommitTransactionStatementFormatter( IIndentable indentable, StringBuilder sql, CommitTransactionStatement statement )
            : base( indentable, sql, statement )
        {

        }

        #region IStatementFormatter Members

        public void Execute()
        {
            DecreaseIndent();
            IndentAppendFormat(
                "COMMIT {1}",
                GetDescription(),
                ( !String.IsNullOrEmpty( _statement.Name ) ? " " + _statement.Name : "" )
            );
        }

        public override bool CanInline
        {
            get { return false; }
        }

        #endregion
    }
}