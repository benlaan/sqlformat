using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;

namespace Laan.Sql.Parser
{
    [DebuggerDisplay("{Type} : {Regex}")]
    public class TokenDefinition
    {
        public TokenDefinition(TokenType type, bool skip, string regex)
            : this(type, skip, regex, null)
        {
        }

        public TokenDefinition(TokenType type, bool skip, string regex, string multiLineTerminator)
        {
            if (!String.IsNullOrEmpty(multiLineTerminator))
            {
                MultiLineTerminator = new Regex(multiLineTerminator, RegexOptions.Compiled | RegexOptions.ExplicitCapture);
                MultiLineContinuation = new Regex(String.Format("[^({0})]*", multiLineTerminator), RegexOptions.Compiled | RegexOptions.ExplicitCapture);
            }

            Skip = skip;
            Regex = new Regex(regex, RegexOptions.Compiled | RegexOptions.ExplicitCapture);
            Type = type;
        }

        public Regex Regex { get; private set; }
        public bool Skip { get; set; }
        public TokenType Type { get; private set; }
        public bool WithinQuotesOnly { get; private set; }
        public bool IsMultiLine { get { return MultiLineTerminator != null; } }
        public Regex MultiLineTerminator { get; private set; }
        public Regex MultiLineContinuation { get; private set; }
    }
}
