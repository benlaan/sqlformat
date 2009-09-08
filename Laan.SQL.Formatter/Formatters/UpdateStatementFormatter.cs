using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Laan.SQL.Parser;

namespace Laan.SQL.Formatter
{
    public class UpdateStatementFormatter : CustomFormatter
    {

        private int _indentStep;
        private StringBuilder _sql;
        private UpdateStatement _statement;
        private string _indent;

        public UpdateStatementFormatter( string indent, int indentStep, StringBuilder sql, UpdateStatement statement )
        {
            _statement = statement;
            _sql = sql;
            _indentStep = indentStep;
            _indent = indent;
        }

        #region IStatementFormatter Members

        public void Execute()
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
