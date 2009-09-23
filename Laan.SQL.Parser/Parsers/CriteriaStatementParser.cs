using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Collections;

using Laan.SQL.Parser.Expressions;

namespace Laan.SQL.Parser
{
    public abstract class CriteriaStatementParser<T> : StatementParser where T : CustomStatement
    {
        protected const string SELECT = "SELECT";
        protected const string DISTINCT = "DISTINCT";
        protected const string TOP = "TOP";
        protected const string FROM = "FROM";
        protected const string AS = "AS";
        protected const string INNER = "INNER";
        protected const string JOIN = "JOIN";
        protected const string LEFT = "LEFT";
        protected const string RIGHT = "RIGHT";
        protected const string OUTER = "OUTER";
        protected const string FULL = "FULL";
        protected const string EQUALS = "=";
        protected const string WHERE = "WHERE";
        protected const string BY = "BY";
        protected const string ORDER = "ORDER";
        protected const string GROUP = "GROUP";
        protected const string HAVING = "HAVING";

        protected string[] FieldTerminatorSet = { FROM, Constants.Comma, HAVING };
        protected string[] FromTerminatorSet = { INNER, JOIN, LEFT, RIGHT, FULL, Constants.Comma, Constants.CloseBracket, ORDER, GROUP, WHERE };

        protected T _statement;

        public CriteriaStatementParser( ITokenizer tokenizer ) : base( tokenizer ) { }

        protected enum FieldType
        {
            Select,
            Update,
            OrderBy,
            GroupBy
        }

        private SortedField GetOrderByField( Expression token )
        {
            SortOrder sortOrder = SortOrder.Implicit;
            if ( Tokenizer.TokenEquals( "ASC" ) )
                sortOrder = SortOrder.Ascending;
            if ( Tokenizer.TokenEquals( "DESC" ) )
                sortOrder = SortOrder.Descending;
            return new SortedField { Expression = token, SortOrder = sortOrder };
        }

        private Field GetSelectField( Expression token )
        {
            Expression expression = null;
            Alias alias = new Alias( token );
            if ( token is CriteriaExpression )
            {
                // this handles the non-standard syntax of: Alias = Expression
                CriteriaExpression criteriaExpression = ( CriteriaExpression )token;
                alias.Name = criteriaExpression.Left.Value;
                alias.Type = AliasType.Equals;
                expression = criteriaExpression.Right;
            }
            else
                if ( Tokenizer.TokenEquals( AS ) )
                {
                    alias.Name = CurrentToken;
                    alias.Type = AliasType.As;
                    expression = token;
                    ReadNextToken();
                }
                else
                {
                    if ( !Tokenizer.IsNextToken( FieldTerminatorSet ) )
                    {
                        if ( Tokenizer.HasMoreTokens )
                        {
                            alias.Name = CurrentToken;
                            alias.Type = AliasType.Implicit;
                            ReadNextToken();
                        }
                        else
                            alias.Type = AliasType.None;
                    }
                    expression = token;
                }

            return new Field { Expression = expression, Alias = alias };
        }

        private Field GetUpdateField( Expression token )
        {
            Alias alias = new Alias( token );
            Expression expression = null;

            if ( token is CriteriaExpression )
            {
                CriteriaExpression criteriaExpression = ( CriteriaExpression )token;
                alias.Name = criteriaExpression.Left.Value;
                alias.Type = AliasType.Equals;
                expression = criteriaExpression.Right;
            }
            else
                throw new SyntaxException( String.Format( "Expected field assignment at {0}", Tokenizer.Position.ToString() ) );

            return new Field() { Expression = expression, Alias = alias };
        }

        private void ProcessField( FieldType fieldType, List<Field> fieldList )
        {
            Expression token = ProcessExpression();
            Field field = null;
            switch ( fieldType )
            {
                case FieldType.Select:
                    field = GetSelectField( token );
                    break;
                case FieldType.Update:
                    field = GetUpdateField( token );
                    break;
                case FieldType.OrderBy:
                    field = GetOrderByField( token );
                    break;
                case FieldType.GroupBy:
                    field = new Field { Expression = token };
                    break;
            }
            if ( field != null )
                fieldList.Add( field );
        }

        protected void ProcessFields( FieldType fieldType, List<Field> fieldList )
        {
            do
                ProcessField( fieldType, fieldList );
            while ( Tokenizer.TokenEquals( Constants.Comma ) );
        }

        private JoinType? GetJoinType()
        {
            JoinType? joinType = null;
            if ( Tokenizer.TokenEquals( INNER ) )
                joinType = JoinType.InnerJoin;
            else
                if ( Tokenizer.IsNextToken( JOIN ) )
                    // don't consume - it is checked after here
                    joinType = JoinType.Join;
                else
                    if ( Tokenizer.TokenEquals( FULL ) )
                    {
                        joinType = JoinType.FullJoin;
                        if ( Tokenizer.TokenEquals( OUTER ) )
                            joinType = JoinType.FullOuterJoin;
                    }
                    else
                        if ( Tokenizer.TokenEquals( LEFT ) )
                        {
                            joinType = JoinType.LeftJoin;
                            if ( Tokenizer.TokenEquals( OUTER ) )
                                joinType = JoinType.LeftOuterJoin;
                        }
                        else
                            if ( Tokenizer.TokenEquals( RIGHT ) )
                            {
                                joinType = JoinType.RightJoin;
                                if ( Tokenizer.TokenEquals( OUTER ) )
                                    joinType = JoinType.RightOuterJoin;
                            }

            return joinType;
        }

        protected void ProcessFrom()
        {
            if ( !Tokenizer.TokenEquals( FROM ) )
                return;
            do
            {
                Table table = null;
                if ( Tokenizer.IsNextToken( Constants.OpenBracket ) )
                    using ( Tokenizer.ExpectBrackets() )
                    {
                        DerivedTable derivedTable = new DerivedTable();
                        Tokenizer.ExpectToken( SELECT );
                        var parser = new SelectStatementParser( Tokenizer );
                        derivedTable.SelectStatement = ( SelectStatement )parser.Execute();
                        table = derivedTable;
                    }
                else
                    table = new Table { Name = GetTableName() };

                _statement.From.Add( table );

                // if a new statement is initiated here, do not process the alias
                if ( Tokenizer.IsNextToken( ";", "GO", "SELECT", "INSERT", "UPDATE", "DELETE", "CREATE", "ALTER", "UNION" ) )
                    return;

                Alias alias = new Alias( null );
                if ( Tokenizer.IsNextToken( AS ) )
                {
                    alias.Type = AliasType.As;
                    Tokenizer.ReadNextToken();
                }
                if ( alias.Type != AliasType.Implicit || !Tokenizer.IsNextToken( FromTerminatorSet ) )
                {
                    if ( Tokenizer.HasMoreTokens)
                     {
                        if( !Tokenizer.Current.IsTypeIn( 
                                TokenType.Alpha, TokenType.AlphaNumeric, TokenType.BlockedText, TokenType.QuotedText 
                            ) 
                        )
                            throw new SyntaxException( String.Format( "Incorrect syntax near '{0}'", CurrentToken ) );
                       
                        alias.Name = CurrentToken;
                        table.Alias = alias;
                        ReadNextToken();
                     }
                }
            }
            while ( Tokenizer.TokenEquals( Constants.Comma ) );

            ProcessJoins();
        }

        private void ProcessJoins()
        {
            do
            {
                JoinType? joinType = GetJoinType();
                if ( joinType == null )
                    return;
                ExpectToken( JOIN );
                Join join = null;
                if ( Tokenizer.IsNextToken( Constants.OpenBracket ) )
                    using ( Tokenizer.ExpectBrackets() )
                    {
                        join = new DerivedJoin { Type = ( JoinType )joinType };
                        Tokenizer.ExpectToken( SELECT );
                        var parser = new SelectStatementParser( Tokenizer );
                        ( ( DerivedJoin )join ).SelectStatement = ( SelectStatement )parser.Execute();
                    }
                else
                {
                    join = new Join { Type = ( JoinType )joinType };
                    join.Name = GetTableName();
                }
                Debug.Assert( join != null );
                Alias alias = new Alias( null );
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
                Expression expr = ProcessExpression();

                if ( !( expr is CriteriaExpression ) && !( expr is NestedExpression && ( expr as NestedExpression ).Expression is CriteriaExpression ) )
                    throw new SyntaxException( "Expected Criteria Expression" );
                
                join.Condition = expr;

                _statement.Joins.Add( join );
            }
            while ( Tokenizer.HasMoreTokens && !Tokenizer.IsNextToken( ORDER, GROUP ) );
        }

        protected void ProcessWhere()
        {
            if ( Tokenizer.TokenEquals( WHERE ) )
                _statement.Where = ProcessExpression();
        }
    }
}
