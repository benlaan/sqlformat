using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Collections;
using System.Linq;

using Laan.Sql.Parser.Expressions;
using Laan.Sql.Parser.Entities;
using Laan.Sql.Parser.Exceptions;

namespace Laan.Sql.Parser.Parsers
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
            if ( Tokenizer.IsNextToken( Constants.Top ) )
                _statement.Top = GetTop();
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

        private void ProcessSetOperation()
        {
            SetType type = SetType.None;
            if ( Tokenizer.TokenEquals( Constants.Union ) )
            {
                type = SetType.Union;
                if ( Tokenizer.TokenEquals( Constants.All ) )
                    type = SetType.UnionAll;
            }
            if ( Tokenizer.TokenEquals( Constants.Except ) )
                type = SetType.Except;

            if ( Tokenizer.TokenEquals( Constants.Intersect ) )
                type = SetType.Intersect;

            if ( type == SetType.None )
                return;

            _statement.SetOperation = new SetOperation() { Type = type };
            Tokenizer.ExpectToken( Constants.Select );
            _statement.SetOperation.Statement = new SelectStatementParser( Tokenizer).Execute();
        }

        private void ProcessInto()
        {
            if ( Tokenizer.TokenEquals( Constants.Into ) )
            {
                Expression expression = ProcessExpression();
                if ( !( expression is IdentifierExpression ) )
                    throw new SyntaxException( "INTO expects identifier expression" );

                _statement.Into = expression.Value;
            }
        }

        public override SelectStatement Execute()
        {
            _statement = new SelectStatement();

            ProcessDistinct();
            ProcessTop();
            ProcessFields( FieldType.Select, _statement.Fields );
            ProcessInto();
            ProcessFrom();
            ProcessWhere();
            ProcessGroupBy();

            if ( Tokenizer.IsNextToken( Constants.Union, Constants.Intersect, Constants.Except ) )
                ProcessSetOperation();
            else
            {
                ProcessOrderBy();
                ProcessTerminator();
            }

            return _statement;
        }
    }
}
