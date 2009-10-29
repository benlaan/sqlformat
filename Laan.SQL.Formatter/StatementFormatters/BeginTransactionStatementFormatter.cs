using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Laan.SQL.Parser;

namespace Laan.SQL.Formatter
{
    public class BeginTransactionStatementFormatter : TransactionStatementFormatter<BeginTransactionStatement>, IStatementFormatter
    {
        public BeginTransactionStatementFormatter( IIndentable indentable, StringBuilder sql, BeginTransactionStatement statement )
            : base( indentable, sql, statement )
        {

        }

        #region IStatementFormatter Members

        public void Execute()
        {
            IndentAppendFormat(
                "BEGIN{0}{1}",
                ( _statement.Distributed ? " DISTRIBUTED" : "" ),
                GetDescription(),
                ( !String.IsNullOrEmpty( _statement.Name ) ? " " + _statement.Name : "" )
            );
            IncreaseIndent();
        }

        public override bool CanInline
        {
            get { return false; }
        }

        #endregion
    }
}