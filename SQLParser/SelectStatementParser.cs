using System;
using System.Diagnostics;

namespace Laan.SQL.Parser
{
    public class SelectStatementParser : StatementParser
    {
        private const string DISTINCT = "DISTINCT";
        private const string TOP = "TOP";
        private const string FROM = "FROM";
        private const string AS = "AS";
        private const string INNER = "INNER";
        private const string JOIN = "JOIN";
        private const string LEFT = "LEFT";
        private const string RIGHT = "RIGHT";
        private const string FULL = "FULL";
        private const string EQUALS = "=";

        private string[] FieldTerminatorSet = { FROM, Constants.COMMA };
        private string[] FromTerminatorSet = { INNER, JOIN, LEFT, RIGHT, FULL, Constants.COMMA };

        SelectStatement _statement;

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

        private void ProcessFields()
        {
            do
            {
                ProcessField();

            } while ( Tokenizer.TokenEquals( Constants.COMMA ) );
        }

        private void ProcessField()
        {
            Expression token = ProcessExpression();
            string alias = null;

            Expression expression = null;

            if ( Tokenizer.TokenEquals( AS ) )
            {
                alias = CurrentToken;
                expression = token;
                ReadNextToken();
            }
            else if ( Tokenizer.TokenEquals( EQUALS ) )
            {
                alias = token.Value;
                expression = ProcessExpression();
            }
            else if ( !IsTokenInSet( FieldTerminatorSet ) )
            {
                alias = CurrentToken;
                expression = token;
                ReadNextToken();
            }
            else
                expression = token;

            _statement.Fields.Add( new Field() { Expression = expression, Alias = alias } );
        }

        private void ProcessFrom()
        {
            Tokenizer.ExpectToken( FROM );
            do
            {
                Table table = new Table() { Name = ProcessTableName() };
                _statement.From.Add( table );

                if ( Tokenizer.TokenEquals( AS ) || !IsTokenInSet( FromTerminatorSet ) )
                {
                    table.Alias = CurrentToken;
                    ReadNextToken();
                }

            } while ( Tokenizer.TokenEquals( Constants.COMMA ) );

            // ProcessJoins
        }

        private string ProcessTableName()
        {
            //TODO: Allow processing of the full format: database.owner.table
            string table = CurrentToken;
            ReadNextToken();
            return table;
        }

        public override IStatement Execute()
        {
            _statement = new SelectStatement();

            ProcessDistinct();
            ProcessTop();
            ProcessFields();
            ProcessFrom();

            return _statement;
        }
    }
}
