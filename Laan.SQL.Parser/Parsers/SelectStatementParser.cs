using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Collections;

namespace Laan.SQL.Parser
{
    public class SelectStatementParser : CriteriaStatementParser<SelectStatement>
    {

        public SelectStatementParser( Tokenizer tokenizer ) : base( tokenizer ) { }

        private void ProcessDistinct()
        {
            _statement.Distinct = Tokenizer.TokenEquals( DISTINCT );
        }

        private void ProcessTop()
        {
            if ( Tokenizer.TokenEquals( TOP ) )
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
            if ( Tokenizer.TokenEquals( ORDER ) )
            {
                ExpectToken( BY );
                ProcessFields( FieldType.OrderBy, _statement.OrderBy );
            }
        }

        private void ProcessGroupBy()
        {
            if ( Tokenizer.TokenEquals( GROUP ) )
            {
                ExpectToken( BY );
                ProcessFields( FieldType.GroupBy, _statement.GroupBy );

                if ( Tokenizer.TokenEquals( HAVING ) )
                    _statement.Having = ProcessExpression();
            }
        }

        public override IStatement Execute()
        {
            _statement = new SelectStatement();

            ProcessDistinct();
            ProcessTop();
            ProcessFields( FieldType.Select, _statement.Fields );
            ProcessFrom();
            ProcessWhere();
            ProcessOrderBy();
            ProcessGroupBy();

            return _statement;
        }
    }
}
