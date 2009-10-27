using System;
using System.Collections.Generic;
using System.Text;

using Laan.SQL.Parser;

namespace Laan.SQL.Formatter
{
    public abstract class TransactionStatementFormatter<T> : CustomStatementFormatter<T> where T : TransactionStatement
    {
        public TransactionStatementFormatter( IIndentable indentable, StringBuilder sql, T statement )
            : base( indentable, sql, statement )
        {

        }

        protected string GetDescription()
        {
            var description = new Dictionary<TransactionDescriptor, string>
            {
                { TransactionDescriptor.Work, "WORK" },
                { TransactionDescriptor.Tran, "TRAN" },
                { TransactionDescriptor.Transaction, "TRANSACTION" }
            };
            string value = "";
            if ( description.TryGetValue( _statement.Descriptor, out value ) )
                return " " + value;
            else
                return "";
        }
    }
}
