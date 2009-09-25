using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Collections;

namespace Laan.SQL.Parser
{
    public class SelectStatementParser : CriteriaStatementParser<SelectStatement>
    {

        public SelectStatementParser( ITokenizer tokenizer ) : base( tokenizer ) { }

        private void ProcessDistinct()
        {
            _statement.Distinct = Tokenizer.TokenEquals( Constants.Distinct );
        }

        private void ProcessTop()
        {
            if ( Tokenizer.TokenEquals( Constants.Top ) )
            {
                int top;
                if ( !Int32.TryParse( CurrentToken, out top ) )
                    throw new SyntaxException( String.Format( "Expected integer but found: '{0}'", CurrentToken ) );

                _statement.Top = ( int )top;
                ReadNextToken();
            }
        }

        private void ProcessOrderBy()
        {
            if ( Tokenizer.TokenEquals( Constants.Order ) )
            {
                ExpectToken( Constants.By );
                ProcessFields( FieldType.OrderBy, _statement.OrderBy );
            }
        }

        private void ProcessGroupBy()
        {
            if ( Tokenizer.TokenEquals( Constants.Group ) )
            {
                ExpectToken( Constants.By );
                ProcessFields( FieldType.GroupBy, _statement.GroupBy );

                if ( Tokenizer.TokenEquals( Constants.Having ) )
                    _statement.Having = ProcessExpression();
            }
        }

        public override SelectStatement Execute()
        {
            _statement = new SelectStatement();

            ProcessDistinct();
            ProcessTop();
            ProcessFields( FieldType.Select, _statement.Fields );
            ProcessFrom();
            ProcessWhere();
            ProcessGroupBy();
            ProcessOrderBy();
            ProcessTerminator();

            return _statement;
        }
    }
}
