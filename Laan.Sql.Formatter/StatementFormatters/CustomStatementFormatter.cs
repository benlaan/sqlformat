using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Laan.Sql.Parser;
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
        private const int _whereLength = 5;

        public CustomStatementFormatter(IIndentable indentable, StringBuilder sql, T statement)
            : base(indentable, sql, statement)
        {
        }

        protected virtual bool CanCompactFormat()
        {
            return IsExpressionOperatorAndOr(_statement.Where);
        }

        protected void FormatWhere()
        {
            if (_statement.Where == null)
                return;

            NewLine(CanCompactFormat() ? 1 : 2);
            IndentAppendFormat(
                "{0} {1}",
                Constants.Where,
                _statement.Where.FormattedValue(_whereLength, this)
            );
        }

        protected void FormatFrom()
        {
            var multipleFroms = _statement.From.Count > 1;
            if (_statement.From == null || !_statement.From.Any())
                return;

            var canCompactFormat = CanCompactFormat();
            NewLine(canCompactFormat ? 0 : 1);

            var fromIndex = 0;
            foreach (var from in _statement.From)
            {
                var fromText = !multipleFroms || from == _statement.From.First() ? "FROM " : String.Empty;

                if (from is DerivedTable derivedTable)
                {
                    NewLine();
                    IndentAppendFormat("{0}(", fromText);
                    NewLine(canCompactFormat ? 1 : 2);

                    using (new IndentScope(this))
                    {
                        var formatter = new SelectStatementFormatter(this, _sql, derivedTable.SelectStatement);
                        formatter.Execute();
                    }

                    NewLine(canCompactFormat ? 1 : 2);
                    IndentAppendFormat("){0}", from.Alias.Value);
                }
                else
                {
                    var isLast = from == _statement.From.Last();

                    NewLine(1);
                    IndentAppend(String.Concat(
                        fromIndex > 0 ? Indent + " " : String.Empty, fromText,
                        from.Name,
                        from.Alias.Value,
                        FormatHints(from),
                        !isLast && !from.Joins.Any() ? Constants.Comma + "\n" : String.Empty
                    ));
                }

                FormatJoins(from, multipleFroms, from == _statement.From.Last());
                fromIndex++;
            }

            FormatPivot();
        }

        private void FormatPivot()
        {
            if (_statement.Pivot == null)
                return;

            NewLine(2);
            IndentAppendLine("PIVOT (");
            NewLine(1);

            using (new IndentScope(this))
            {
                IndentAppendLine(_statement.Pivot.Field.Expression.FormattedValue(0, _indentable));
                IndentAppendFormat("FOR {0} IN ({1})", _statement.Pivot.For, _statement.Pivot.In.ToCsv());
            }

            NewLine(2);
            IndentAppendFormat("){0}", _statement.Pivot.Alias.Value);
        }
    }
}
