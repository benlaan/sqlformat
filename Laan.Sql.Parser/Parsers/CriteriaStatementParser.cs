using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Collections;

using Laan.Sql.Parser.Expressions;
using Laan.Sql.Parser.Entities;
using Laan.Sql.Parser.Exceptions;

namespace Laan.Sql.Parser.Parsers
{
    internal enum FieldType
    {
        Select,
        Update,
        OrderBy,
        GroupBy
    }

    public abstract class CriteriaStatementParser<T> : StatementParser<T> where T : CustomStatement
    {
        protected string[] FieldTerminatorSet = { Constants.From, Constants.Comma, Constants.Having, Constants.Go, Constants.SemiColon, Constants.End, Constants.Into, Constants.Union, Constants.Intersect, Constants.Except, Constants.CloseBracket };
        protected string[] FromTerminatorSet = { Constants.Inner, Constants.Join, Constants.Left, Constants.Right, Constants.Full, Constants.Comma, Constants.CloseBracket, Constants.Order, Constants.Group, Constants.Where, Constants.Cross };

        protected CriteriaStatementParser(ITokenizer tokenizer) : base(tokenizer) { }

        private SortedField GetOrderByField(Expression token)
        {
            SortOrder sortOrder = SortOrder.Implicit;
            if (Tokenizer.TokenEquals("ASC"))
                sortOrder = SortOrder.Ascending;
            if (Tokenizer.TokenEquals("DESC"))
                sortOrder = SortOrder.Descending;
            return new SortedField { Expression = token, SortOrder = sortOrder };
        }

        private Field GetSelectField(Expression token)
        {
            Expression expression = null;
            Alias alias = new Alias(token);
            if (token is CriteriaExpression)
            {
                // this handles the non-standard syntax of: Alias = Expression
                CriteriaExpression criteriaExpression = (CriteriaExpression)token;
                alias.Name = criteriaExpression.Left.Value;
                alias.Type = AliasType.Equals;
                expression = criteriaExpression.Right;
            }
            else
                if (Tokenizer.TokenEquals(Constants.As))
                {
                    alias.Name = GetIdentifier();
                    alias.Type = AliasType.As;
                    expression = token;
                }
                else
                {
                    if (!Tokenizer.IsNextToken(FieldTerminatorSet))
                    {
                        if (Tokenizer.HasMoreTokens)
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

        private Field GetUpdateField(Expression token)
        {
            Alias alias = new Alias(token);
            Expression expression = null;

            if (token is CriteriaExpression)
            {
                CriteriaExpression criteriaExpression = (CriteriaExpression)token;
                alias.Name = criteriaExpression.Left.Value;
                alias.Type = AliasType.Equals;
                expression = criteriaExpression.Right;
            }
            else
                throw new SyntaxException(String.Format("Expected field assignment at {0}", Tokenizer.Position.ToString()));

            return new Field() { Expression = expression, Alias = alias };
        }

        private void ProcessField(FieldType fieldType, List<Field> fieldList)
        {
            Expression token = ProcessExpression();
            Field field = null;
            switch (fieldType)
            {
                case FieldType.Select:
                    field = GetSelectField(token);
                    break;
                case FieldType.Update:
                    field = GetUpdateField(token);
                    break;
                case FieldType.OrderBy:
                    field = GetOrderByField(token);
                    break;
                case FieldType.GroupBy:
                    field = new Field { Expression = token };
                    break;
            }
            if (field != null)
                fieldList.Add(field);
        }

        internal void ProcessFields(FieldType fieldType, List<Field> fieldList)
        {
            do
            {
                ProcessField(fieldType, fieldList);
            }
            while (Tokenizer.TokenEquals(Constants.Comma));

            if (fieldList.Count == 0)
                throw new SyntaxException("field list can not be empty");
        }

        private JoinType? GetJoinType()
        {
            JoinType? joinType = null;
            if (Tokenizer.TokenEquals(Constants.Inner))
                joinType = JoinType.InnerJoin;
            else
                if (Tokenizer.IsNextToken(Constants.Join))
                    // don't consume - it is checked after here
                    joinType = JoinType.Join;
                else
                    if (Tokenizer.TokenEquals(Constants.Full))
                    {
                        joinType = JoinType.FullJoin;
                        if (Tokenizer.TokenEquals(Constants.Outer))
                            joinType = JoinType.FullOuterJoin;
                    }
                    else
                        if (Tokenizer.TokenEquals(Constants.Left))
                        {
                            joinType = JoinType.LeftJoin;
                            if (Tokenizer.TokenEquals(Constants.Outer))
                                joinType = JoinType.LeftOuterJoin;
                        }
                        else
                            if (Tokenizer.TokenEquals(Constants.Right))
                            {
                                joinType = JoinType.RightJoin;
                                if (Tokenizer.TokenEquals(Constants.Outer))
                                    joinType = JoinType.RightOuterJoin;
                            }
                            else
                                if (Tokenizer.TokenEquals(Constants.Cross))
                                    joinType = JoinType.CrossJoin;

                return joinType;
        }

        private bool IsTerminatingFromExpression()
        {
            return Tokenizer.IsNextToken(
                Constants.SemiColon, Constants.Go, Constants.Select, Constants.Insert,
                Constants.Update, Constants.Delete, Constants.Create, Constants.Alter,
                Constants.Union, Constants.Else, Constants.Commit, Constants.Rollback,
                Constants.End, Constants.Except, Constants.Intersect
            );
        }

        protected void ProcessFrom()
        {
            if (!Tokenizer.TokenEquals(Constants.From))
                return;

            do
            {
                Table table = null;

                if (Tokenizer.IsNextToken(Constants.OpenBracket))
                    using (Tokenizer.ExpectBrackets())
                    {
                        DerivedTable derivedTable = new DerivedTable();
                        Tokenizer.ExpectToken(Constants.Select);
                        var parser = new SelectStatementParser(Tokenizer);
                        derivedTable.SelectStatement = (SelectStatement)parser.Execute();
                        table = derivedTable;
                    }
                else
                    table = new Table { Name = GetTableName() };

                _statement.From.Add(table);

                // TODO: This needs to be changed to test Tokenizer.Token.Current.TokenType for TokenType.Keyword
                // if a new statement is initiated here, do not process the alias
                if (IsTerminatingFromExpression())
                    return;

                Alias alias = new Alias(null);
                if (Tokenizer.IsNextToken(Constants.As))
                {
                    alias.Type = AliasType.As;
                    Tokenizer.ReadNextToken();
                }

                if (!Tokenizer.IsNextToken(Constants.OpenBracket) && (alias.Type != AliasType.Implicit || !Tokenizer.IsNextToken(FromTerminatorSet)))
                {
                    if (Tokenizer.HasMoreTokens)
                    {
                        if (!Tokenizer.Current.IsTypeIn(
                                TokenType.AlphaNumeric, TokenType.AlphaNumeric, TokenType.BlockedText, TokenType.SingleQuote
                            )
                        )
                            throw new SyntaxException(String.Format("Incorrect syntax near '{0}'", CurrentToken));

                        alias.Name = CurrentToken;
                        table.Alias = alias;
                        ReadNextToken();
                    }
                }
                ProcessTableHints(table);
                ProcessJoins(table);
            }
            while (Tokenizer.HasMoreTokens && Tokenizer.TokenEquals(Constants.Comma));
        }

        private void ProcessJoins(Table table)
        {
            if (!Tokenizer.HasMoreTokens)
                return;
            do
            {
                JoinType? joinType = GetJoinType();
                if (joinType == null)
                    return;

                ExpectToken(Constants.Join);

                Join join = null;
                if (Tokenizer.IsNextToken(Constants.OpenBracket))
                    using (Tokenizer.ExpectBrackets())
                    {
                        join = new DerivedJoin { Type = joinType.Value };
                        Tokenizer.ExpectToken(Constants.Select);
                        var parser = new SelectStatementParser(Tokenizer);
                        ((DerivedJoin)join).SelectStatement = (SelectStatement)parser.Execute();
                    }
                else
                {
                    join = new Join { Type = joinType.Value };
                    join.Name = GetTableName();
                }

                Debug.Assert(join != null);

                Alias alias = new Alias(null);
                if (Tokenizer.IsNextToken(Constants.As))
                {
                    alias.Type = AliasType.As;
                    Tokenizer.ReadNextToken();
                }

                if (alias.Type != AliasType.Implicit || !Tokenizer.IsNextToken(Constants.On))
                {
                    alias.Name = GetIdentifier();
                    join.Alias = alias;
                }

                ProcessTableHints(join);
                ExpectToken(Constants.On);
                Expression expr = ProcessExpression();

                if (!(expr is CriteriaExpression) && !(expr is NestedExpression && (expr as NestedExpression).Expression is CriteriaExpression))
                    throw new SyntaxException("Expected Criteria Expression");

                join.Condition = expr;

                table.Joins.Add(join);
            }
            while (Tokenizer.HasMoreTokens && !Tokenizer.IsNextToken(Constants.Order, Constants.Group));
        }

        protected void ProcessWhere()
        {
            if (Tokenizer.TokenEquals(Constants.Where))
                _statement.Where = ProcessExpression();
        }
    }
}
