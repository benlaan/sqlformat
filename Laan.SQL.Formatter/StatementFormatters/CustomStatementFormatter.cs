using System;
using System.Text;
using System.Linq;
using System.Collections.Generic;

using Laan.Sql.Parser;
using Laan.Sql.Parser.Expressions;
using Laan.Sql.Parser.Entities;

namespace Laan.Sql.Formatter
{
    public enum BracketFormatOption
    {
        NoSpaces,
        SpacesWithinBrackets
    }

    public class CustomStatementFormatter<T> : StatementFormatter<T> where T : CustomStatement
    {
        private const int WhereLength = 5;

        public CustomStatementFormatter(IIndentable indentable, StringBuilder sql, T statement) 
            : base(indentable, sql, statement)
        {
            
        }

        protected virtual bool CanCompactFormat()
        {
            return IsExpressionOperatorAndOr( _statement.Where );
        }

        protected void FormatWhere()
        {
            if ( _statement.Where == null )
                return;

            NewLine( CanCompactFormat() ? 1 : 2 );
            IndentAppendFormat(
                "{0} {1}",
                Constants.Where,
                _statement.Where.FormattedValue( WhereLength, this )
            );
        }

        protected void FormatFrom()
        {
            bool multipleFroms = _statement.From.Count > 1;
            if (_statement.From == null || !_statement.From.Any())
                return;

            bool canCompactFormat = CanCompactFormat();

            NewLine(canCompactFormat ? 0 : 1);
            int fromIndex = 0;
            foreach (var from in _statement.From)
            {
                string fromText = !multipleFroms || from == _statement.From.First() ? "FROM " : "";

                DerivedTable derivedTable = from as DerivedTable;
                if (derivedTable != null)
                {
                    NewLine();
                    IndentAppend(String.Format("{0}(", fromText));
                    NewLine(canCompactFormat ? 1 : 2);

                    using (new IndentScope(this))
                    {
                        var formatter = new SelectStatementFormatter(this, _sql, derivedTable.SelectStatement);
                        formatter.Execute();
                    }
                    NewLine(canCompactFormat ? 1 : 2);
                    IndentAppend(String.Format("){0}", from.Alias.Value));
                }
                else
                {
                    bool isLast = from == _statement.From.Last();
                    NewLine(1);
                    IndentAppendFormat(
                        "{0}{1}{2}{3}{4}",
                        fromIndex > 0 ? Indent + " " : "", fromText, from.Name, from.Alias.Value, !isLast && !from.Joins.Any() ? Constants.Comma + "\n" : ""
                    );
                }
                FormatJoins(from, multipleFroms, from == _statement.From.Last());
                fromIndex++;
            }

        }
    }
}
