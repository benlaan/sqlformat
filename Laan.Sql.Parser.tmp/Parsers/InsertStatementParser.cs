using System;
using System.Collections.Generic;

using Laan.Sql.Parser.Entities;
using Laan.Sql.Parser.Exceptions;
using Laan.Sql.Parser.Expressions;

namespace Laan.Sql.Parser.Parsers
{
    class InsertStatementParser : StatementParser<InsertStatement>
    {

    // INSERT [INTO] table_name [(column_list)] { 
    //      {
    //        VALUES ( { DEFAULT | NULL | expression }[,...n] )[,...n]
    //        | derived_table
    //        | execute_statement    
    //      }

        public InsertStatementParser( ITokenizer tokenizer ) : base( tokenizer ) { }

        private void ProcessColumnList()
        {
            if ( Tokenizer.IsNextToken( Constants.OpenBracket ) )
            {
                using ( Tokenizer.ExpectBrackets() )
                {
                    _statement.Columns = ( GetIdentifierList() );
                }
            }
        }

        private void ProcessValues()
        {
            do
            {
                using (Tokenizer.ExpectBrackets())
                {
                    var identifiers = new List<Expression>();
                    do
                    {
                        identifiers.Add(ProcessExpression());
                    }
                    while (Tokenizer.TokenEquals(Constants.Comma));
                    
                    _statement.Values.Add(identifiers);
                }
            }
            while (Tokenizer.TokenEquals(Constants.Comma));
        }

        private void ProcessSelect()
        {
            ReadNextToken();
            SelectExpression selectExpression = new SelectExpression();

            var parser = new SelectStatementParser( Tokenizer );
            _statement.SourceStatement = parser.Execute();
        }

        private void ProcessExec()
        {
            throw new NotImplementedException();
            //var parser = new ExecuteProcedureStatementParser( Tokenizer );
            //_statement.Procedure = parser.Execute<ExecuteProcuedreStatement>();
        }

        public override InsertStatement Execute()
        {
            _statement = new InsertStatement();

            if ( Tokenizer.TokenEquals( Constants.Into ) )
                _statement.HasInto = true;

            _statement.TableName = GetTableName();
            ProcessTableHints(_statement, With.Required);

            ProcessColumnList();

            if ( Tokenizer.TokenEquals( Constants.Values ) )
                ProcessValues();
            else if ( Tokenizer.IsNextToken( Constants.Select ) )
                ProcessSelect();
            else if ( Tokenizer.IsNextToken( Constants.Exec ) )
                ProcessExec();
            else
                throw new SyntaxException( String.Format("syntax error after 'INSERT INTO {0} '", _statement.TableName ) );

            ProcessTerminator();

            return _statement;
        }
    }
}
