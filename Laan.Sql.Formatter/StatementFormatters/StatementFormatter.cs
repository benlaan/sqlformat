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
        protected const int WrapMarginColumn = 80;
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

            AppendFormat(
                " TOP {0}{1}",
                top.Expression.FormattedValue(0, this),
                top.Percent ? " PERCENT" : String.Empty
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
                "  ON {0}",
                derivedJoin.Condition.FormattedValue(4, this)
            );
        }

        private void FormatApplyJoin(ApplyJoin applyJoin)
        {
            switch (applyJoin.Expression)
            {
                case SelectExpression selectExpression:
                    NewLine(2);
                    IndentAppendFormat("{0} (", applyJoin.GetJoinType());
                    NewLine(2);

                    using (new IndentScope(this))
                    {
                        var formatter = new SelectStatementFormatter(this, _sql, selectExpression.Statement);
                        formatter.Execute();
                    }

                    NewLine(2);
                    IndentAppendFormat("){0}", applyJoin.Alias.Value);
                    break;

                case FunctionExpression functionExpression:
                    NewLine(2);
                    IndentAppendFormat(
                        "{0} {1}{2}",
                        applyJoin.GetJoinType(),
                        functionExpression.FormattedValue(0, _indentable),
                        applyJoin.Alias.Value
                    );
                    break;
            }
        }

        protected void FormatJoins(Table table, bool multipleFroms, bool isLastFrom)
        {
            if (table.Joins == null || !table.Joins.Any())
                return;

            foreach (var join in table.Joins)
            {
                switch (join)
                {
                    case DerivedJoin derivedJoin:
                        FormatDerivedJoin(derivedJoin);
                        break;

                    case ApplyJoin applyJoin:
                        FormatApplyJoin(applyJoin);
                        break;

                    default:
                        using (new IndentScope(this, multipleFroms))
                        {
                            NewLine(2);
                            IndentAppend(join.Value + FormatHints(join));
                            NewLine();

                            var isLastJoin = join == table.Joins.Last();

                            IndentAppendFormat(
                                "{0}ON {1}{2}",
                                new string(' ', join.Length - Constants.On.Length),
                                join.Condition.FormattedValue(join.Length, this),
                                (!isLastFrom && isLastJoin) ? Constants.Comma + "\n" : String.Empty
                            );
                        }

                        break;
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
                return String.Empty;

            var withPrefix = hinting.ExplicitWith ? " WITH" : String.Empty;
            return String.Format("{0} ({1})", withPrefix, hinting.TableHints.ToCsv(t => t.Hint));
        }

        protected bool FitsOnRow(string text)
        {
            return text.Length <= (WrapMarginColumn - (IndentLevel * Indent.Length + CurrentColumn));
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
