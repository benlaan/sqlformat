using System;
using System.Collections.Generic;
using System.Text;

using Laan.Sql.Parser;
using Laan.Sql.Parser.Entities;
using Laan.Sql.Parser.Exceptions;

namespace Laan.Sql.Formatter
{
    public class StatementFormatterFactory
    {
        public static IStatementFormatter GetFormatter(IIndentable indentable, StringBuilder outSql, IStatement statement)
        {
            switch (statement)
            {
                case SelectStatement selectStatement:
                    return new SelectStatementFormatter(indentable, outSql, selectStatement);

                case UpdateStatement updateStatement:
                    return new UpdateStatementFormatter(indentable, outSql, updateStatement);

                case CreateIndexStatement createIndexStatement:
                    return new CreateIndexStatementFormatter(indentable, outSql, createIndexStatement);

                case DeleteStatement deleteStatement:
                    return new DeleteStatementFormatter(indentable, outSql, deleteStatement);

                case InsertStatement insertStatement:
                    return new InsertStatementFormatter(indentable, outSql, insertStatement);

                case DeclareStatement declareStatement:
                    return new DeclareStatementFormatter(indentable, outSql, declareStatement);

                case GoTerminator goTerminator:
                    return new GoTerminatorFormatter(indentable, outSql, goTerminator);

                case IfStatement ifStatement:
                    return new IfStatementFormatter(indentable, outSql, ifStatement);

                case BeginTransactionStatement beginTransactionStatement:
                    return new BeginTransactionStatementFormatter(indentable, outSql, beginTransactionStatement);

                case RollbackTransactionStatement rollbackTransactionStatement:
                    return new RollbackTransactionStatementFormatter(indentable, outSql, rollbackTransactionStatement);

                case CommitTransactionStatement commitTransactionStatement:
                    return new CommitTransactionStatementFormatter(indentable, outSql, commitTransactionStatement);

                case BlockStatement blockStatement:
                    return new BlockStatementFormatter(indentable, outSql, blockStatement);

                case ExecuteSqlStatement executeSqlStatement:
                    return new ExecuteSqlStatementFormatter(indentable, outSql, executeSqlStatement);

                case ExecStatement execStatement:
                    return new ExecStatementFormatter(indentable, outSql, execStatement);

                case CreateViewStatement createViewStatement:
                    return new CreateViewStatementFormatter(indentable, outSql, createViewStatement);

                case CreateProcedureStatement createProcedureStatement:
                    return new CreateProcedureStatementFormatter(indentable, outSql, createProcedureStatement);

                case CommonTableExpressionStatement commonTableExpressionStatement:
                    return new CommonTableExpressionStatementFormatter(indentable, outSql, commonTableExpressionStatement);

				case UseStatement useStatement:
                    return new UseStatementFormatter(indentable, outSql, useStatement);

                default:
                    throw new FormatterNotImplementedException("Formatter not implemented for statement: " + statement.GetType().Name);
            }
        }
    }
}
