using System;
using System.Text;

using Laan.Sql.Parser;
using Laan.Sql.Parser.Entities;

namespace Laan.Sql.Formatter
{
    public class RollbackTransactionStatementFormatter : TransactionStatementFormatter<RollbackTransactionStatement>, IStatementFormatter
    {
        public RollbackTransactionStatementFormatter( IIndentable indentable, StringBuilder sql, RollbackTransactionStatement statement )
            : base( indentable, sql, statement )
        {

        }

        #region IStatementFormatter Members

        public void Execute()
        {
            DecreaseIndent();
            IndentAppendFormat(
                "ROLLBACK{0}{1}",
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
