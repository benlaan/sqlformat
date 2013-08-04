using System;
using System.Linq;
using System.Collections.Generic;

namespace Laan.Sql.Parser
{
    public class SqlTokenizer : RegexTokenizer, ITokenizer
    {
        public SqlTokenizer(string input) : base(input)
        {
            TokenDefinitions.AddRange(new[]
            {
                new TokenDefinition( TokenType.InLineComment,      true,  @"\-\-[^\n^\r]*" ),
                new TokenDefinition( TokenType.MultiLineComment,   true,  @"\/\*(\s|.)*?(\*\/)|\/\*(\s|.)*", true),
                new TokenDefinition( TokenType.String,             false, @"N?'(''|[^'])*'"),
                new TokenDefinition( TokenType.Variable,           false, @"(:|@{1,2}|#{1,2})[A-Za-z_]+[[A-Za-z_0-9]*" ),
                new TokenDefinition( TokenType.AlphaNumeric,       false, @"[A-Za-z_]+\w*" ),
                new TokenDefinition( TokenType.Numeric,            false, @"[-]?\d*[0-9](|.\d*[0-9]|,\d*[0-9])?" ),
                new TokenDefinition( TokenType.OpenBracket,        false, @"\(" ),
                new TokenDefinition( TokenType.CloseBracket,       false, @"\)" ),
                //new TokenDefinition( TokenType.SingleQuote,        false, @"'"),
                //new TokenDefinition( TokenType.NSingleQuote,       false, @"N'") { ExceptInQuotes = true },
                //new TokenDefinition( TokenType.EscapedSingleQuote, false, @"''.*''") { WithinQuotesOnly = true },
                new TokenDefinition( TokenType.DoubleQuote,        false, @""".*"""),
                new TokenDefinition( TokenType.BlockedText,        false, @"\[.*?\]" ),
                new TokenDefinition( TokenType.Symbol,             false, @"[,.;:]" ),
                new TokenDefinition( TokenType.Operator,           false, @"(\<\>)|[\*\\+\-!\<\>]?=|[\<\>]|[\+\-\*/\\%\^]" ),
                new TokenDefinition( TokenType.WhiteSpace,         true,  @"\s+" )
                //new TokenDefinition( TokenType.QuotedText,         false, @"['\""]+" ) { WithinQuotesOnly = true },
            });
        }

        public bool SkipComments
        {
            set
            {
                TokenDefinitions
                    .Where(tdef => tdef.Type == TokenType.InLineComment || tdef.Type == TokenType.MultiLineComment)
                    .ToList()
                    .ForEach(tdef => tdef.Skip = value);
            }
        }

        public bool SkipWhiteSpace
        {
            set
            {
                var definitions = TokenDefinitions
                    .Where(tdef => tdef.Type == TokenType.WhiteSpace)
                    .ToList();

                foreach (var definition in definitions)
                    definition.Skip = value;
            }
        }
    }
}
