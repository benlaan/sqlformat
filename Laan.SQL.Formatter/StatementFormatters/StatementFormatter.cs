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
        protected StringBuilder _sql;
        protected T _statement;
        protected const int WrapMarginColumn = 80;
        protected IIndentable _indentable;

        static StatementFormatter()
        {
            _bracketFormats = new Dictionary<BracketFormatOption, string>()
            {
                { BracketFormatOption.NoSpaces, "({0})" },
                { BracketFormatOption.SpacesWithinBrackets, "( {0} )" },
            };
        }

        public StatementFormatter(IIndentable indentable, StringBuilder sql, T statement)
        {
            _sql = sql;
            _indentable = indentable;
            _statement = statement;
        }

        #region Rendering Utilities

        protected void IndentAppend(string text)
        {
            for (int count = 0; count < IndentLevel; count++)
                _sql.Append(Indent);
            _sql.Append(text);
        }
        protected void IndentAppendFormat(string text, params object[] args)
        {
            IndentAppend(String.Format(text, args));
        }

        protected void IndentAppendLine(string text)
        {
            IndentAppend(text);
            NewLine();
        }

        protected void IndentAppendLineFormat(string text, params object[] args)
        {
            IndentAppendLine(String.Format(text, args));
        }

        protected void Append(string text)
        {
            _sql.Append(text);
        }

        protected void NewLine(int times)
        {
            for (int index = 0; index < times; index++)
                _sql.AppendLine();
        }

        protected void NewLine()
        {
            NewLine(1);
        }

        #endregion
        protected void FormatTop(Top top)
        {
            if (top == null)
                return;

            string format = top.Brackets ? " TOP ({0}){1}" : " TOP {0}{1}";

            Append(
                String.Format(
                    format,
                    top.Expression.FormattedValue(0, this),
                    top.Percent ? " PERCENT" : ""
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
                "  ON {0}",
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
                        IndentAppend(join.Value);
                        NewLine();

                        bool isLastJoin = join == table.Joins.Last();

                        IndentAppendFormat(
                            "{0}ON {1}{2}",
                            new string(' ', join.Length - Constants.On.Length),
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
            return where == null || (!String.Equals( where.Operator, Constants.And, StringComparison.InvariantCultureIgnoreCase ) && 
                                     !String.Equals( where.Operator, Constants.Or, StringComparison.InvariantCultureIgnoreCase ) );
        }

        public string Indent
        {
            get { return _indentable.Indent; }
            set { _indentable.Indent = value; }
        }

        public int IndentLevel
        {
            get { return _indentable.IndentLevel; }
            set { _indentable.IndentLevel = value; }
        }

        public void IncreaseIndent()
        {
            IndentLevel++;
        }

        public void DecreaseIndent()
        {
            IndentLevel--;
        }
    }
}
