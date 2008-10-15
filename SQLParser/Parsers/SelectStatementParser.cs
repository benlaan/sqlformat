using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Collections;

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
        private const string WHERE = "WHERE";
        private const string BY = "BY";
        private const string ORDER = "ORDER";
        private const string GROUP = "GROUP";

        private string[] FieldTerminatorSet = { FROM, Constants.COMMA };
        
        private string[] FromTerminatorSet = { 
            INNER, JOIN, LEFT, RIGHT, FULL, Constants.COMMA, Constants.CLOSE_BRACKET, ORDER, GROUP 
        };

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

        private void ProcessFields( List<Field> fieldList )
        {
            do
            {
                ProcessField( fieldList );

            } while ( Tokenizer.TokenEquals( Constants.COMMA ) );
        }

        private void ProcessField( List<Field> fieldList )
        {
            Expression token = ProcessExpression();
            string alias = null;

            Expression expression = null;

            if ( token is CriteriaExpression )
            {
                // this handles the non-stadard syntax of: Alias = Expression
                CriteriaExpression criteriaExpression = ( CriteriaExpression )token;
                alias = criteriaExpression.Left.Value;
                expression = criteriaExpression.Right;
            }
            else if ( Tokenizer.TokenEquals( AS ) )
            {
                alias = CurrentToken;
                expression = token;
                ReadNextToken();
            }
            else if ( !IsNextToken( FieldTerminatorSet ) )
            {
                alias = CurrentToken;
                expression = token;
                ReadNextToken();
            }
            else
                expression = token;

            fieldList.Add( new Field() { Expression = expression, Alias = alias } );
        }

        private void ProcessFrom()
        {
            if ( !Tokenizer.TokenEquals( FROM ) )
                return;

            do
            {
                Table table = null;
                if ( Tokenizer.TokenEquals( Constants.OPEN_BRACKET ) )
                {
                    DerivedTable derivedTable = new DerivedTable();

                    if ( IsNextToken( "SELECT" ) )
                    {
                        ReadNextToken();
                        var parser = new SelectStatementParser( Tokenizer );
                        derivedTable.SelectStatement = ( SelectStatement )parser.Execute();
                    }
                    table = derivedTable;

                    ExpectToken( Constants.CLOSE_BRACKET );
                }
                else
                    table = new Table() { Name = GetTableName() };

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

                CriteriaExpression criteriaExpression = ProcessExpression() as CriteriaExpression;
                if ( criteriaExpression == null )
                    throw new SyntaxException( "Expected Criteria Expression" );

                join.Condition = criteriaExpression;

                _statement.Joins.Add( join );
            }
            while ( Tokenizer.HasMoreTokens && !IsNextToken( ORDER, GROUP ) );
        }

        private void ProcessWhere()
        {
            if ( Tokenizer.TokenEquals( WHERE ) )
                _statement.Where = ProcessExpression();
        }

        private void ProcessOrderBy()
        {
            if ( Tokenizer.TokenEquals( ORDER ) )
            {
                ExpectToken( BY );
                ProcessFields( _statement.OrderBy );
            }
        }

        private void ProcessGroupBy()
        {
            if ( Tokenizer.TokenEquals( GROUP ) )
            {
                ExpectToken( BY );
                ProcessFields( _statement.GroupBy );
            }
        }

        public override IStatement Execute()
        {
            _statement = new SelectStatement();

            ProcessDistinct();
            ProcessTop();
            ProcessFields( _statement.Fields );
            ProcessFrom();
            ProcessWhere();
            ProcessOrderBy();
            ProcessGroupBy();

            return _statement;
        }


    }
}
