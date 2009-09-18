using System;

namespace Laan.SQL.Parser
{
    public class SqlTokenizer : RegexTokenizer
    {
        public SqlTokenizer( string input ) : base( input )
        {
            TokenDefinitions.AddRange(
                new[]
                {
                    new TokenDefinition( TokenType.WhiteSpace,       true,  @"^\s+$" ),
                    new TokenDefinition( TokenType.Alpha,            false, @"^[a-zA-Z_]+$" ),
                    new TokenDefinition( TokenType.Variable,         false, @"^(:|@{1,2})[A-Za-z_]+[[A-Za-z_0-9]*$" ),
                    new TokenDefinition( TokenType.AlphaNumeric,     false, @"^[A-Za-z_]+\w+$" ),
                    new TokenDefinition( TokenType.Numeric,          false, @"^[-]?\d*[0-9](|.\d*[0-9]|,\d*[0-9])?$" ),
                    new TokenDefinition( TokenType.OpenBracket,      false, @"^\($" ),
                    new TokenDefinition( TokenType.CloseBracket,     false, @"^\)$" ),
                    new TokenDefinition( TokenType.QuotedText,       false, @"^(?:([""'])|(“)|‘).*?(?<!\\)(?(1)\1|(?(2)”|’))$" ),
                    new TokenDefinition( TokenType.BlockedText,      false, @"^\[.*\]$" ),
                    new TokenDefinition( TokenType.Symbol,           false, @"^[,.;]$" ),
                    new TokenDefinition( TokenType.Operator,         false, @"^([\+\-\*/\^\%]|<?>?|[!<>]?=)$" ),
                    new TokenDefinition( TokenType.InLineComment,    true,  @"^\-\-[^\n\r]*$" ),
                    new TokenDefinition( TokenType.MultiLineComment, true,  @"\/\*(\s|.)*?(\*\/)|\/\*(\s|.)*" ),
                }
            );
        }
    }
}
