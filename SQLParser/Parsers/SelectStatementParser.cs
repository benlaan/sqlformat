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
        private const string OUTER = "OUTER";
        private const string FULL = "FULL";
        private const string EQUALS = "=";
        private const string WHERE = "WHERE";
        private const string BY = "BY";
        private const string ORDER = "ORDER";
        private const string GROUP = "GROUP";
        private const string HAVING = "HAVING";

        private string[] FieldTerminatorSet = { FROM, Constants.Comma, HAVING };
        private string[] FromTerminatorSet = { INNER, JOIN, LEFT, RIGHT, FULL, Constants.Comma, Constants.CloseBracket, ORDER, GROUP };

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

            } while ( Tokenizer.TokenEquals( Constants.Comma ) );
        }

        private void ProcessField( List<Field> fieldList )
        {
            Expression token = ProcessExpression();
            Alias alias = new Alias();

            Expression expression = null;

            if ( token is CriteriaExpression )
            {
                // this handles the non-standard syntax of: Alias = Expression
                CriteriaExpression criteriaExpression = (CriteriaExpression) token;
                alias.Name = criteriaExpression.Left.Value;
                alias.Type = AliasType.Equals;
                expression = criteriaExpression.Right;
            }
            else if ( Tokenizer.TokenEquals( AS ) )
            {
                alias.Name = CurrentToken;
                alias.Type = AliasType.As;
                expression = token;
                ReadNextToken();
            }
            else if ( !Tokenizer.IsNextToken( FieldTerminatorSet ) )
            {
                alias.Name = CurrentToken;
                alias.Type = AliasType.Implicit;
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
                if (Tokenizer.TokenEquals( Constants.OpenBracket ))
                {
                    DerivedTable derivedTable = new DerivedTable();

                    if ( Tokenizer.IsNextToken( "SELECT" ) )
                    {
                        ReadNextToken();
                        var parser = new SelectStatementParser( Tokenizer );
                        derivedTable.SelectStatement = ( SelectStatement )parser.Execute();
                    }
                    table = derivedTable;

                    ExpectToken( Constants.CloseBracket );
                }
                else
                    table = new Table() { Name = GetTableName() };

                _statement.From.Add( table );

                Alias alias = new Alias();
                if ( Tokenizer.IsNextToken( AS ) )
                {
                    alias.Type = AliasType.As;
                    Tokenizer.ReadNextToken();
                }

                if ( alias.Type != AliasType.Implicit || !Tokenizer.IsNextToken( FromTerminatorSet ) )
                {
                    alias.Name = CurrentToken;
                    table.Alias = alias;
                    ReadNextToken();
                }

            } while ( Tokenizer.TokenEquals( Constants.Comma ) );

            ProcessJoins();
        }

        private JoinType? GetJoinType()
        {
            JoinType? joinType = null;

            if ( Tokenizer.TokenEquals( INNER ) || Tokenizer.IsNextToken( JOIN ) )
            {
                joinType = JoinType.InnerJoin;
            }
            else if ( Tokenizer.TokenEquals( FULL ) )
            {
                joinType = JoinType.FullJoin;
                if ( Tokenizer.TokenEquals( OUTER ) )
                {
                    // just consume this 'legacy' token without using it..
                }
            }
            else if ( Tokenizer.TokenEquals( LEFT ) )
            {
                joinType = JoinType.LeftJoin;
                if ( Tokenizer.TokenEquals( OUTER ) )
                {
                    // just consume this 'legacy' token without using it..
                }
            }
            else if ( Tokenizer.TokenEquals( RIGHT ) )
            {
                joinType = JoinType.RightJoin;
                if ( Tokenizer.TokenEquals( OUTER ) )
                {
                    // consume this redundant token..
                }
            }
            return joinType;
        }

        private void ProcessJoins()
        {
            do
            {
                JoinType? joinType = GetJoinType();
                if ( joinType == null )
                    return;

                ExpectToken( JOIN );
                Join join = new Join() { Type = ( JoinType )joinType };

                join.Name = GetTableName();

                Alias alias = new Alias();
                if ( Tokenizer.IsNextToken( AS ) )
                {
                    alias.Type = AliasType.As;
                    Tokenizer.ReadNextToken();
                }

                if ( alias.Type != AliasType.Implicit || !Tokenizer.TokenEquals( Constants.On ) )
                {
                    alias.Name = GetIdentifier();
                    join.Alias = alias;
                }

                ExpectToken( Constants.On );

                CriteriaExpression criteriaExpression = ProcessExpression() as CriteriaExpression;
                if ( criteriaExpression == null )
                    throw new SyntaxException( "Expected Criteria Expression" );

                join.Condition = criteriaExpression;

                _statement.Joins.Add( join );
            }
            while ( Tokenizer.HasMoreTokens && !Tokenizer.IsNextToken( ORDER, GROUP ) );
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

                if ( Tokenizer.TokenEquals( HAVING ) )
                    _statement.Having = ProcessExpression();
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
