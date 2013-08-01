using System;
using System.Linq;
using System.Collections.Generic;

using Laan.Sql.Parser.Entities;
using Laan.Sql.Parser.Parsers;

namespace Laan.Sql.Parser
{
    public class ExecStatementParser : StatementParser<ExecStatement>
    {
        public ExecStatementParser(ITokenizer tokenizer)
            : base(tokenizer)
        {
        }

        private ExecStatement ParseExecuteSql(string functionName)
        {
            var statement = new ExecuteSqlStatement() { FunctionName = functionName };
            if (!Tokenizer.IsNextToken("N"))
                return statement;

            var parts = new List<string>();
            ReadNextToken();
            do
            {
                string body = "";
                bool withinQuotes = false;
                bool canContinue;
                do
                {
                    withinQuotes ^= Tokenizer.Current.Type == TokenType.SingleQuote;

                    canContinue = withinQuotes || !Tokenizer.TokenEquals(Constants.Comma);
                    if (canContinue)
                    {
                        body += CurrentToken;
                        ReadNextToken();
                    }
                }
                while (canContinue && Tokenizer.HasMoreTokens);

                parts.Add(body);
            }
            while (Tokenizer.HasMoreTokens);

            if (!parts.Any())
                return statement;

            var args = parts.Skip(1).First().Trim('\'').Split(',');
            int index = 0;
            foreach (string arg in parts.Skip(2))
            {
                var assignment = arg.Split('=');

                statement.Arguments.Add(new Argument
                {
                    Name = assignment[0],
                    Value = assignment[1],
                    Type = args[index++].Split(' ').Last()
                });
            }

            string sql = parts.First().Replace("''", "'").Trim('\'');
            var parser = ParserFactory.Execute(sql);
            statement.InnerStatement = parser.FirstOrDefault();
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
