using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Laan.SQL.Parser;

namespace Laan.SQL.Formatter
{
    public class GoTerminatorFormatter : IStatementFormatter
    {
        protected int IndentStep;
        protected StringBuilder _sql;
        protected GoTerminator _statement;
        protected string Indent;

        public GoTerminatorFormatter( IIndentable indentable, StringBuilder sql, GoTerminator statement )
        {
            Indent = indentable.Indent;
            IndentStep = indentable.IndentLevel;
            _sql = sql;
            _statement = statement;
        }

        #region IStatementFormatter Members

        public void Execute()
        {
            _sql.Append( Constants.Go );
        }

        public bool CanInline
        {
            get { return false; }
        }

        #endregion
    }
}
