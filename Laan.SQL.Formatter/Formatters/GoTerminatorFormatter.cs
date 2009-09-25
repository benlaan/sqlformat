using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Laan.SQL.Parser;

namespace Laan.SQL.Formatter
{
    public class GoTerminatorFormatter : IStatementFormatter
    {
        protected int _indentStep;
        protected StringBuilder _sql;
        protected GoTerminator _statement;
        protected string _indent;

        public GoTerminatorFormatter( string indent, int indentStep, StringBuilder sql, GoTerminator statement )
        {
            _indent = indent;
            _indentStep = indentStep;
            _sql = sql;
            _statement = statement;
        }

        #region IStatementFormatter Members

        public void Execute()
        {
            _sql.Append( Constants.Go );
        }

        #endregion
    }
}
