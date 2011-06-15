using System.Linq;
using System.Collections.Generic;
using Laan.Sql.Parser.Entities;
using Laan.Sql.Parser.Parsers;

namespace Laan.Sql.Parser
{
    public class ExecuteSqlStatementParser : StatementParser<ExecuteSqlStatement>
    {
        public ExecuteSqlStatementParser(ITokenizer tokenizer)
            : base(tokenizer)
        {

        }

        public override ExecuteSqlStatement Execute()
        {
            var statement = new ExecuteSqlStatement();
            string name = GetDotNotationIdentifier();
            statement.FunctionName = name;
            switch (name)
            {
                case "sp_executesql":
                    if (Tokenizer.IsNextToken("N"))
                    {
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
                            statement.Arguments.Add(
                                new Argument
                                    { 
                                    Name = assignment[0],
                                    Value = assignment[1],
                                    Type = args[index++].Split(' ').Last()
                                }
                            );
                        }
                        
                        string sql = parts.First().Replace("''", "'").Trim('\'');
                        var parser = ParserFactory.Execute(sql);
                        statement.InnerStatement = parser.FirstOrDefault();
                    }
                    break;
            }
            return statement;
        }

    }
}
