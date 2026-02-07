using System;
using System.Linq;
using System.Text;

using Laan.Sql.Parser.Entities;
using Laan.Sql.Parser;

namespace Laan.Sql.Formatter
{
    public class CreateProcedureStatementFormatter : StatementFormatter<CreateProcedureStatement>, IStatementFormatter
    {
        public CreateProcedureStatementFormatter(IIndentable indentable, StringBuilder sql, CreateProcedureStatement statement)
            : base(indentable, sql, statement)
        {
        }

        public void Execute()
        {
            _sql.AppendFormat(
                "{0} {1} {2}{3}",
                _statement.IsAlter ? Keyword(Constants.Alter) : Keyword(Constants.Create),
                _statement.IsShortForm ? Keyword(Constants.Proc) : Keyword(Constants.Procedure),
                _statement.Name,
                _statement.HasBracketedArguments ? " (" : String.Empty
            );

            if (_statement.Arguments.Any())
            {
                var formatter = new VariableDefinitionFormatter(_statement.Arguments, this, _sql);
                formatter.Execute();
            }

            _sql.AppendLine();

            if (_statement.HasBracketedArguments)
                _sql.AppendLine(")");

            _sql.Append(Keyword(Constants.As));
            _sql.AppendLine();

            FormatStatement(_statement.Definition);
        }
    }
}
