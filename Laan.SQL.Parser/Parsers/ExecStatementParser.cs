using System;
using System.Linq;
using System.Collections.Generic;

using Laan.Sql.Parser.Entities;
using Laan.Sql.Parser.Parsers;
using Laan.Sql.Parser.Expressions;

namespace Laan.Sql.Parser
{
    public class ExecStatementParser : StatementParser<ExecStatement>
    {
        public ExecStatementParser(ITokenizer tokenizer) : base(tokenizer)
        {
        }

        private ExecStatement ParseExecuteSql(string functionName)
        {
            var statement = new ExecuteSqlStatement() { FunctionName = functionName };
            var parts = new List<string>();
            
            var parser = new ExpressionParser(Tokenizer);

            var sqlStringExpression = parser.Execute<StringExpression>();
            statement.InnerStatement = ParserFactory.Execute(sqlStringExpression.Content).First();

            if (!Tokenizer.IsNextToken(Constants.Comma))
                return statement;

            Tokenizer.ReadNextToken();

            var parameters = parser.Execute<StringExpression>()
                .Content.Split(',')
                .Select(p => p.Split(' ').Last())
                .ToList();

            Tokenizer.ExpectToken(Constants.Comma);
            int parameterIndex = 0;
            do
            {
                var argumentValue = parser.Execute<CriteriaExpression>();

                statement.Arguments.Add(new Argument 
                { 
                    Name = argumentValue.Left.Value, 
                    Value = argumentValue.Right.Value,
                    Type = parameters[parameterIndex]
                });
                parameterIndex++;
            }
            while (Tokenizer.TokenEquals(Constants.Comma));

            return statement;
        }

        private ExecStatement ParseExec(string functionName)
        {
            ExecStatement statement = new ExecStatement() { FunctionName = functionName };

            if (Tokenizer.HasMoreTokens)
            {
                do
                {
                    var name = "";
                    string value = GetIdentifier();
                    if (Tokenizer.IsNextToken(Constants.Assignment))
                    {
                        ReadNextToken();
                        var identifier = GetIdentifier();
                        name = value;
                        value = identifier;
                    }

                    statement.Arguments.Add(new Argument { Name = name, Value = value });
                }
                while (Tokenizer.TokenEquals(Constants.Comma));
            }

            return statement;
        }

        public override ExecStatement Execute()
        {
            string name = GetDotNotationIdentifier();

            // Add custom handlers for named stored procedures
            switch (name)
            {
                case "sp_executesql":
                    return ParseExecuteSql(name);

                default:
                    return ParseExec(name);
            }
        }

    }
}
