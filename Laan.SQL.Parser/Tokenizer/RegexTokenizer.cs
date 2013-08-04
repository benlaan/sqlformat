using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

using Laan.Sql.Parser.Exceptions;

namespace Laan.Sql.Parser
{
    public enum TokenType
    {
        None,
        WhiteSpace,
        Numeric,
        AlphaNumeric,
        Variable,
        OpenBrace,
        CloseBrace,
        OpenBracket,
        CloseBracket,
        SingleQuote,
        DoubleQuote,
        BlockedText,
        Operator,
        Symbol,
        InLineComment,
        MultiLineComment,
        String,
        QuotedText
    }

    [DebuggerDisplay("{Type} : {Regex}")]
    public class TokenDefinition
    {
        public Regex Regex;
        public bool Skip;
        public TokenType Type;
        public bool WithinQuotesOnly;

        public TokenDefinition(TokenType type, bool skip, string regex, bool multiLine = false)
        {
            Skip = skip;
            RegexOptions options = RegexOptions.Compiled | RegexOptions.ExplicitCapture;
            if (multiLine)
                options |= RegexOptions.Multiline;

            Regex = new Regex(regex, options);
            Type = type;
        }
    }

    [DebuggerDisplay("Current: {Current} Position: {Position} [{HasMoreTokens ? \"Y\" : \"N\"}]")]
    public abstract class RegexTokenizer : CustomTokenizer, IDisposable
    {
        private Token _current;
        private readonly TextReader _reader;
        private string _line;

        protected RegexTokenizer(string input)
        {
            TokenDefinitions = new List<TokenDefinition>();

            Position = new Position(this);
            _reader = new StringReader(input);
            _line = String.Empty;
        }

        private void ReadNextLine()
        {
            while (_line.Length == 0 && _reader.Peek() != -1)
            {
                _line = _reader.ReadLine();
                Position.Row++;
                Position.Column = 1;
            }
        }

        public override Token Current
        {
            get { return _current; }
        }

        public override bool HasMoreTokens
        {
            get 
            { 
                return (_line != null && _line.Length > 0) 
                    || _reader.Peek() != -1
                    || Current != (Token)null; 
            }
        }

        public List<TokenDefinition> TokenDefinitions { get; set; }

        public override void ReadNextToken()
        {
            _current = null;
            ReadNextLine();

            if (!HasMoreTokens)
                return;

            var candidateDefinitions = TokenDefinitions
                .Select((td, i) => new { Position = i, Match = td.Regex.Match(_line), Definition = td })
                .Where(m => m.Match.Success)
                .OrderBy(m => m.Match.Captures[0].Index)
                .ThenBy(td => td.Position)
                .ToList();

            var matchingToken = candidateDefinitions.FirstOrDefault();

            if (matchingToken == null)
                throw new SyntaxException();

            Match match = matchingToken.Match;
            _current = new Token(match.Value, matchingToken.Definition.Type);
            _line = _line.Remove(match.Captures[0].Index, match.Captures[0].Length);
            Position.Column += match.Value.Length;

            if (matchingToken.Definition.Skip)
                ReadNextToken();
        }

        public void Dispose()
        {
            _reader.Dispose();
        }
    }
}
