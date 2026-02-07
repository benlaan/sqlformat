using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Laan.Sql.Parser;
using Laan.Sql.Parser.Entities;
using Laan.Sql.Parser.Expressions;

namespace Laan.Sql.Formatter
{
    public class StatementFormatter<T> : BaseFormatter, IIndentable where T : Statement
    {
        protected T _statement;

        static StatementFormatter()
        {
            _bracketFormats = new Dictionary<BracketFormatOption, string>()
            {
                { BracketFormatOption.NoSpaces, "({0})" },
                { BracketFormatOption.SpacesWithinBrackets, "( {0} )" },
            };
        }

        public StatementFormatter(IIndentable indentable, StringBuilder sql, T statement) : base(indentable, sql)
        {
            _statement = statement;
        }

        protected void FormatTop(Top top)
        {
            if (top == null)
                return;

            string format = " {0} {1}{2}";

            Append(
                String.Format(
                    format,
                    Keyword(Constants.Top),
                    top.Expression.FormattedValue(0, this),
                    top.Percent ? " " + Keyword(Constants.Percent) : ""
                )
            );
        }

        private void FormatDerivedJoin(DerivedJoin derivedJoin)
        {
            NewLine(2);
            IndentAppend(derivedJoin.Value);
            NewLine(2);
            using (new IndentScope(this))
            {
                var formatter = new SelectStatementFormatter(this, _sql, derivedJoin.SelectStatement);
                formatter.Execute();
            }
            NewLine(2);
            IndentAppend(String.Format("){0}", derivedJoin.Alias.Value));
            NewLine();
            IndentAppendFormat(
                "  {0} {1}",
                Keyword(Constants.On),
                derivedJoin.Condition.FormattedValue(4, this)
            );
        }

        protected void FormatJoins(Table table, bool multipleFroms, bool isLastFrom)
        {
            if (table.Joins == null || !table.Joins.Any())
                return;

            foreach (var join in table.Joins)
            {
                if (join is DerivedJoin)
                    FormatDerivedJoin((DerivedJoin)join);
                else
                {
                    using (new IndentScope(this, multipleFroms))
                    {
                        NewLine(2);
                        IndentAppendFormat(
                            "{0} {1}{2}{3}",
                            Keyword(join.Type.GetDescription()),
                            join.Name,
                            join.Alias.Value,
                            FormatHints(join)
                        );
                        NewLine();

                        var isLastJoin = join == table.Joins.Last();

                        IndentAppendFormat(
                            "{0}{1} {2}{3}",
                            new string(' ', join.Length - Constants.On.Length),
                            Keyword(Constants.On),
                            join.Condition.FormattedValue(join.Length, this),
                            (!isLastFrom && isLastJoin) ? Constants.Comma + "\n" : ""
                        );
                    }
                }
            }
        }

        protected void FormatTerminator()
        {
            if (_statement.Terminated)
                Append(Constants.SemiColon);
        }

        protected void FormatStatement(IStatement statement)
        {
            var formatter = StatementFormatterFactory.GetFormatter(this, _sql, statement);
            formatter.Execute();
        }

        protected void FormatBlock(IStatement statement)
        {
            // do not increase indent when the child statement is a block.. 
            // this results in IF <> BEGIN END working as expected
            if (statement is BlockStatement)
                FormatStatement(statement);
            else
                using (new IndentScope(this))
                    FormatStatement(statement);
        }

        protected string FormatHints(ITableHints hinting)
        {
            if (!hinting.TableHints.Any())
                return "";

            var withPrefix = hinting.ExplicitWith ? " WITH" : String.Empty;
            return String.Format("{0} ({1})", withPrefix, String.Join(", ", hinting.TableHints.Select(t => t.Hint).ToArray()));
        }

        protected bool FitsOnRow(string text)
        {
            return text.Length <= (Options.MaxLineLength - (IndentLevel * Indent.Length + CurrentColumn));
        }

        protected int CurrentColumn
        {
            get { return _sql.ToString().Split('\n').Last().Length; }
        }

        protected bool IsExpressionOperatorAndOr(Expression expression)
        {
            CriteriaExpression where = expression as CriteriaExpression;
            return where == null
                || (
                    !String.Equals(where.Operator, Constants.And, StringComparison.CurrentCultureIgnoreCase)
                    &&
                    !String.Equals(where.Operator, Constants.Or, StringComparison.CurrentCultureIgnoreCase)
                );
        }
    }
}
