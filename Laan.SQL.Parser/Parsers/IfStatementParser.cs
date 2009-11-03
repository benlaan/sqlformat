using System;

using Laan.Sql.Parser.Expressions;
using Laan.Sql.Parser.Entities;
using Laan.Sql.Parser.Exceptions;

namespace Laan.Sql.Parser.Parsers
{
    public class IfStatementParser : StatementParser<IfStatement>
    {
        public IfStatementParser( ITokenizer tokenizer ) : base( tokenizer )
        {
        }

        private void ProcessIfCondition()
        {
            var condition = new ExpressionParser( Tokenizer ).Execute();
            if ( !( condition is CriteriaExpression || condition is FunctionExpression || condition is NegationExpression ) )
                throw new SyntaxException( "IF expects a boolean result expression" );

            _statement.Condition = condition;
        }

        private void ProcessTrueBlock()
        {
            if ( !Tokenizer.HasMoreTokens )
                throw new SyntaxException( "missing success block for IF" );

            _statement.If = ParserFactory
                .GetParser( Tokenizer )
                .Execute();
        }

        private void ProcessFalseBlock()
        {
            if ( Tokenizer.TokenEquals( Constants.Else ) )
            {
                if ( !Tokenizer.HasMoreTokens )
                    throw new SyntaxException( "missing else block for IF" );

                _statement.Else = ParserFactory
                    .GetParser( Tokenizer )
                    .Execute();
            }
        }

        public override IfStatement Execute()
        {
            if ( !Tokenizer.HasMoreTokens )
                throw new SyntaxException( "missing condition for IF" );

            _statement = new IfStatement();

            ProcessIfCondition();
            ProcessTrueBlock();
            ProcessFalseBlock();
            
            return _statement;
        }
    }
}
