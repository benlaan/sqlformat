using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Laan.SQL.Parser;

namespace Laan.SQL.Formatter
{
    public class UpdateStatementFormatter : CustomFormatter<UpdateStatement>, IStatementFormatter
    {
        public UpdateStatementFormatter( string indent, int indentStep, StringBuilder sql, UpdateStatement statement )
            : base( indent, indentStep, sql, statement )
        {
        }

        #region IStatementFormatter Members

        public void Execute()
        {
            FormatUpdate();
            FormatFrom();
            FormatJoins();
            FormatWhere();
            FormatTerminator();
        }

        #endregion

        private void FormatUpdate()
        {
            _sql.Append( "UPDATE " + ( _statement.Top.HasValue ? String.Format( "TOP ({0}) ", _statement.Top ) : "" ) + _statement.TableName );
            NewLine();
            string format = String.Format( "   {{0}} {{1,{0}}} = {{2}}{{3}}", -1 * _statement.Fields.Max( f => f.Alias.Name.Length ) );
            foreach ( Field field in _statement.Fields )
            {
                string separator = field != _statement.Fields.Last() ? Constants.Comma + Environment.NewLine : "";
                string set = field == _statement.Fields.First() ? "SET" : "   ";

                _sql.AppendFormat(
                    format,
                    set,
                    field.Alias.Name,
                    field.Expression.FormattedValue( 0, _indent, _indentStep ),
                    separator
                );
            }
        }

    }
}