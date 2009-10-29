using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Laan.SQL.Parser;

namespace Laan.SQL.Formatter
{
    public class UpdateStatementFormatter : CustomStatementFormatter<UpdateStatement>, IStatementFormatter
    {
        public UpdateStatementFormatter( IIndentable indentable, StringBuilder sql, UpdateStatement statement )
            : base( indentable, sql, statement )
        {
        }

        #region IStatementFormatter Members

        public void Execute()
        {
            FormatUpdate();
            FormatFieldAssignment(); 
            FormatFrom();
            FormatJoins();
            FormatWhere();
            FormatTerminator();
        }

        #endregion

        private void FormatFieldAssignment()
        {
            string format = String.Format(
                "   {{0}} {{1,{0}}} = {{2}}{{3}}",
                -1 * _statement.Fields.Max( f => f.Alias.Name.Length ) 
            );

            foreach ( Field field in _statement.Fields )
            {
                string separator = field != _statement.Fields.Last() ? Constants.Comma + Environment.NewLine : "";
                string set = field == _statement.Fields.First() ? "SET" : "   ";

                IndentAppendFormat(
                    format,
                    set,
                    field.Alias.Name,
                    field.Expression.FormattedValue( 0, this ),
                    separator
                );
            }
        }

        private void FormatUpdate()
        {
            IndentAppend( Constants.Update );
            FormatTop( _statement.Top );
            _sql.AppendFormat( " {0}\n", _statement.TableName );
        }

        protected override bool CanCompactFormat()
        {
            return false;
        }
    }
}