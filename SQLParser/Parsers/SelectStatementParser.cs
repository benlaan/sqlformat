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
            else if ( !IsNextToken( FieldTerminatorSet ) )
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

                if ( Tokenizer.TokenEquals( AS ) || !IsNextToken( FromTerminatorSet ) )
                {
                    table.Alias = CurrentToken;
                    ReadNextToken();
                }

            } while ( Tokenizer.TokenEquals( Constants.COMMA ) );

            ProcessJoins();
        }

        private void ProcessJoins()
        {
            do
            {
                JoinType? joinType = null;

                if ( Tokenizer.TokenEquals( "INNER" ) || IsNextToken( "JOIN" ) )
                {
                    joinType = JoinType.InnerJoin;
                }
                if ( Tokenizer.TokenEquals( "LEFT" ) )
                {
                    joinType = JoinType.LeftJoin;
                    if ( Tokenizer.TokenEquals( "OUTER" ) )
                        ; // consume this redundant token..
                }
                if ( Tokenizer.TokenEquals( "RIGHT" ) )
                {
                    joinType = JoinType.RightJoin;
                    if ( Tokenizer.TokenEquals( "OUTER" ) )
                        ; // consume this redundant token..
                }
                if ( joinType == null )
                    return;

                ExpectToken( "JOIN" );
                Join join = new Join() { Type = ( JoinType )joinType };

                join.Name = GetTableName();

                if ( Tokenizer.TokenEquals( "AS" ) || !Tokenizer.TokenEquals( "ON" ) )
                    join.Alias = GetIdentifier();

                ExpectToken( "ON" );

                join.Condition = ProcessCriteriaExpression();

                _statement.Joins.Add( join );
            }
            while ( Tokenizer.HasMoreTokens && !IsNextToken( "ORDER", "GROUP" ) );
            
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
