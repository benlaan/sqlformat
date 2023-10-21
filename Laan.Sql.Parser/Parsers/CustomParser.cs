using System;
using System.Collections.Generic;

using Laan.Sql.Parser.Entities;
using Laan.Sql.Parser.Exceptions;
using Laan.Sql.Parser.Expressions;

namespace Laan.Sql.Parser.Parsers
{
    public abstract class CustomParser
    {
        public CustomParser(ITokenizer tokenizer)
        {
            Tokenizer = tokenizer;
        }

        protected void ExpectToken(string token)
        {
            var current = Tokenizer.Current;

            if (current == Token.Null || !String.Equals(CurrentToken, token, StringComparison.OrdinalIgnoreCase))
            {
                throw new ExpectedTokenNotFoundException(
                    token,
                    current != Token.Null ? current.ToString() : "EOF",
                    Tokenizer.Position
                );
            }

            ReadNextToken();
        }

        protected void ExpectTokens(params string[] tokens)
        {
            foreach (var token in tokens)
                ExpectToken(token);
        }

        protected void ReadNextToken()
        {
            Tokenizer.ReadNextToken();
        }

        protected string CurrentToken
        {
            get { return Tokenizer.Current.Value; }
        }

        protected ITokenizer Tokenizer { get; }

        protected Expression ProcessExpression()
        {
            var parser = new ExpressionParser(Tokenizer);
            return parser.Execute();
        }

        private string GetOperator()
        {
            var comparisonOperators = new[] { "=", ">=", "<=", "!=", "<>", Constants.In, Constants.Any, Constants.Like };

            if (!Tokenizer.IsNextToken(comparisonOperators))
                throw new ExpectedTokenNotFoundException(comparisonOperators.ToCsv(), CurrentToken, Tokenizer.Position);

            var token = Tokenizer.Current.Value;
            ReadNextToken();
            return token;
        }

        protected CriteriaExpression ProcessCriteriaExpression(Expression parent)
        {
            var expression = new CriteriaExpression(parent);
            expression.Left = ProcessExpression();
            expression.Operator = GetOperator();
            expression.Right = ProcessExpression();

            return expression;
        }

        protected string GetIdentifier()
        {
            if (!Tokenizer.HasMoreTokens)
                throw new SyntaxException("Identifier expected");

            var identifier = String.Empty;

            switch (Tokenizer.Current.Type)
            {
                case TokenType.SingleQuote:
                    var parser = new ExpressionParser(Tokenizer);
                    var expression = parser.Execute();

                    if (!(expression is StringExpression))
                        throw new SyntaxException("Transaction name must be a string");

                    identifier = expression.Value;
                    break;

                default:
                    identifier = CurrentToken;
                    ReadNextToken();
                    break;
            }

            return identifier;
        }

        protected List<string> GetIdentifierList()
        {
            var identifiers = new List<string>();
            do
            {
                identifiers.Add(GetIdentifier());
            }
            while (Tokenizer.TokenEquals(Constants.Comma));

            return identifiers;
        }

        protected string GetDotNotationIdentifier()
        {
            string token;
            token = String.Empty;

            do
            {
                token += (token != String.Empty ? Constants.Dot : String.Empty) + GetIdentifier();
            }
            while (Tokenizer.TokenEquals(Constants.Dot));
            return token;
        }

        protected bool HasTerminator()
        {
            var result = Tokenizer.IsNextToken(Constants.SemiColon);
            if (result)
                Tokenizer.ReadNextToken();
            return result;
        }

        protected Top GetTop()
        {
            Tokenizer.ExpectToken(Constants.Top);

            Top top;
            if (Tokenizer.IsNextToken(Constants.OpenBracket))
            {
                using (Tokenizer.ExpectBrackets())
                {
                    var parser = new ExpressionParser(Tokenizer);
                    var expression = parser.Execute();
                    if (expression == null)
                        throw new SyntaxException("TOP clause requires an expression");

                    var brackets = new NestedExpression(null);
                    brackets.Expression = expression;
                    top = new Top(brackets);
                }
            }
            else
            {
                if (Tokenizer.Current.Type != TokenType.Numeric || Tokenizer.Current.Value.Contains("."))
                    throw new SyntaxException(String.Format("Expected integer but found: '{0}'", Tokenizer.Current.Value));

                top = new Top(new StringExpression(Tokenizer.Current.Value, null));
                ReadNextToken();
            }

            if (Tokenizer.TokenEquals(Constants.Percent))
                top.Percent = true;

            return top;
        }
    }
}
