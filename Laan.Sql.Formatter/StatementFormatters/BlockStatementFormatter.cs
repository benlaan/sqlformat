using System;
using System.Text;

using Laan.Sql.Parser;
using Laan.Sql.Parser.Entities;

namespace Laan.Sql.Formatter
{
    public class BlockStatementFormatter : CustomStatementFormatter<BlockStatement>, IStatementFormatter
    {

        public BlockStatementFormatter(IIndentable indentable, StringBuilder sql, BlockStatement statement)
            : base(indentable, sql, statement)
        {

        }

        public void Execute()
        {
            IndentAppendLine("BEGIN");
            IndentAppendLine("");
            using (new IndentScope(this))
            {
                foreach (IStatement statement in _statement.Statements)
                {
                    FormatStatement(statement);
                    NewLine(2);
                }
            }
            IndentAppend("END");
        }
    }
}
