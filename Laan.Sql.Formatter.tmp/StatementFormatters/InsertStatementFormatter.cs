using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Laan.Sql.Parser;
using Laan.Sql.Parser.Entities;
using Laan.Sql.Parser.Expressions;

namespace Laan.Sql.Formatter
{
    public class InsertStatementFormatter : CustomStatementFormatter<InsertStatement>, IStatementFormatter
    {
        private const int MaxOneLineColumnCount = 4;

        public InsertStatementFormatter(IIndentable indentable, StringBuilder sql, InsertStatement statement)
            : base(indentable, sql, statement)
        {
        }

        #region IStatementFormatter Members

        public void Execute()
        {
            FormatInsert();
            FormatColumns();
            FormatInputData();
            FormatTerminator();
        }

        #endregion

        private void FormatInsert()
        {
            IndentAppendFormat("{0} {1} {2}", Constants.Insert, Constants.Into, _statement.TableName);
        }

        private string FormatColumnWithSeparator(int index)
        {
            return _statement.Columns[index] + (index < _statement.Columns.Count - 1 ? ", " : "");
        }

        private void FormatColumns()
        {
            if (_statement.Columns.Count == 0)
                NewLine();
            else
            {
                string text = _statement.Columns.ToCsv();
                var lines = new List<string>();
                if (_statement.Columns.Count <= MaxOneLineColumnCount && FitsOnRow(text))
                    _sql.AppendFormat(" {0}\n", FormatBrackets(text));
                else
                {
                    if (text.Length > WrapMarginColumn)
                    {
                        string line = "";
                        for (int index = 0; index < _statement.Columns.Count; index++)
                        {
                            if (line.Length + _statement.Columns[index].Length >= WrapMarginColumn)
                            {
                                lines.Add(line);
                                line = "";
                            }
                            line += FormatColumnWithSeparator(index);
                        }
                        lines.Add(line);
                    }
                    else
                    {
                        lines.Add(String.Join(", ", _statement.Columns));
                    }

                    _sql.Append(" (");
                    NewLine();

                    using (new IndentScope(this))
                    {
                        foreach (string line in lines)
                            IndentAppendLine(line);

                        IndentAppendLine(")");
                    }
                }
            }
        }

        private string GetValues(List<Expression> values)
        {
            StringBuilder result = new StringBuilder();
            int size = 0;
            var count = values.Count;

            bool multiline = false;

            foreach (var item in values)
            {
                var value = item.Value;
                size += value.Length;

                result.Append(value + (count > 1 ? ", " : String.Empty));
                if (size > 120)
                {
                    result.Append(Environment.NewLine + Indent + Indent);
                    size = 0;
                    multiline = true;
                }
                count--;
            }

            if (multiline)
                return string.Format("{0}{1}{1}{2}{0}{1}", Environment.NewLine, Indent, result);

            return result.ToString();
        }

        private void FormatInputData()
        {
            if (_statement.Values.Any())
            {
                using (new IndentScope(this))
                {
                    var first = _statement.Values.First();
                    var last = _statement.Values.Last();

                    foreach (var values in _statement.Values)
                    {
                        IndentAppendFormat(
                            "{0} {1}{2}",
                            values == first ? Constants.Values : new string(' ', Constants.Values.Length),
                            FormatBrackets(GetValues(values)),
                            values == last ? String.Empty : ",\n"
                        );
                    }
                }
            }
            else if (_statement.SourceStatement != null)
            {
                using (new IndentScope(this))
                {
                    var formatter = new SelectStatementFormatter(this, _sql, _statement.SourceStatement);
                    formatter.Execute();
                }
            }
            //else if ( _statement.ExecuteProcudure != null )
            //{
            //    using ( new IndentScope( this ) )
            //    {
            //        var formatter = new ExecuteProcedureFormatter( this, _sql, _statement.ExecuteProcudure );
            //        formatter.Execute();
            //    }
            //}
        }
    }
}
