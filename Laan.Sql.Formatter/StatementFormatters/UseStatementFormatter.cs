using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Laan.Sql.Parser;
using Laan.Sql.Parser.Entities;

namespace Laan.Sql.Formatter
{
    public class UseStatementFormatter : IStatementFormatter
    {
        protected int IndentStep;
        protected StringBuilder _sql;
        protected UseStatement _statement;
        protected string Indent;
        protected IIndentable _indentable;

        public UseStatementFormatter( IIndentable indentable, StringBuilder sql, UseStatement statement )
        {
            Indent = indentable.Indent;
            IndentStep = indentable.IndentLevel;
            _sql = sql;
            _statement = statement;
            _indentable = indentable;
        }

        #region IStatementFormatter Members

        public void Execute()
        {
            _sql.Append(KeywordTransform.Apply(Constants.Use, _indentable.Options.KeywordCasing));
            _sql.Append(" ");
            _sql.Append(_statement.DatabaseName);
        }

        public bool CanInline
        {
            get { return true; }
        }

        #endregion
    }
}
