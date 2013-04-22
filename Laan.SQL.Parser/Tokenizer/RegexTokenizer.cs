using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;
using System.Diagnostics;

using Laan.Sql.Parser.Exceptions;

namespace Laan.Sql.Parser
{
    public enum TokenType
    {
        None,
        WhiteSpace,
        Alpha,
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
        QuotedText
    }

    public class TokenDefinition
    {
        public Regex Regex;
        public bool Skip;
        public TokenType Type;
        public bool WithinQuotesOnly;

        public TokenDefinition(TokenType type, bool skip, string regex)
        {
            Skip = skip;
            Regex = new Regex(regex, RegexOptions.Compiled);//| RegexOptions.Multiline | RegexOptions.Singleline );
            Type = type;
        }
    }

    [DebuggerDisplay("Current: {Current} Position: {Position} [{HasMoreTokens ? \"Y\" : \"N\"}]")]
    public abstract class RegexTokenizer : CustomTokenizer, IDisposable
    {
        private TokenType _currentQuoteType = TokenType.None;
        private readonly TextReader _reader;
        private Token _current;

        protected RegexTokenizer( string input )
        {
            TokenDefinitions = new List<TokenDefinition>();
            Position = new Position(this);
            _reader = new StringReader(input);
        }

        private void ReadChar()
        {
            int read = _reader.Read();
            if (read == '\n')
                Position.NewRow();
            else
                Position.Column++;

            return;
        }

        /// <summary>
        /// Returns the number of matches that the current token could be
        /// </summary>
        /// <param name="token"></param>
        /// <param name="definition"></param>
        /// <returns></returns>
        private int MatchCount(string token, out TokenDefinition definition)
        {
            var definitions = TokenDefinitions.ToList();
            if (!IsQuote(_currentQuoteType))
                definitions.RemoveAll(d => d.WithinQuotesOnly);
            
            var matches = definitions.Where(
                tokenDefinition =>
                    tokenDefinition.Regex.Matches(token).Cast<Match>().Any(match => match.Value == token)
            );

            definition = matches.FirstOrDefault();
            return matches.Count();
        }

        private bool IsQuote(TokenType type)
        {
            return type == TokenType.SingleQuote || type == TokenType.DoubleQuote; ;
        }

        public override void ReadNextToken()
        {
            _current = null;
            var current = new StringBuilder();
            TokenDefinition definition;
            TokenDefinition last;

            int next;
            while ((next = _reader.Peek()) != -1)
            {
                int matchCount = MatchCount(current.ToString() + (char)next, out definition);
                last = definition;
                int lastMatchCount = 0;

                while (_reader.Peek() != -1 && (matchCount > 0 || (lastMatchCount == 0)))
                {
                    last = definition;
                    ReadChar();
                    current.Append((char)next);

                    next = _reader.Peek();
                    if (next == -1)
                        break;

                    lastMatchCount = matchCount;
                    matchCount = MatchCount(current.ToString() + (char)next, out definition);
                }

                if (last != null && last.Skip && !IsQuote(_currentQuoteType))
                {
                    ReadNextToken();
                    return;
                }

                // if we get here, we either have a good token, or the token is not recognised
                if (MatchCount(current.ToString(), out definition) == 0)
                    throw new UnknownTokenException(current.ToString());

                _current = new Token(current.ToString(), last != null ? last.Type : TokenType.None);
                if (IsQuote(_current.Type))
                {
                    if (_currentQuoteType != _current.Type) 
                        _currentQuoteType = _current.Type;
                    else
                        _currentQuoteType = TokenType.None;
                }
                return;
            }
        }

        public override Token Current
        {
            get { return _current; }
        }

        public override bool HasMoreTokens
        {
            get { return _reader.Peek() != -1 || Current != (Token)null; }
        }

        public List<TokenDefinition> TokenDefinitions { get; set; }

        #region IDisposable Members

        public void Dispose()
        {
            _reader.Dispose();
        }

        #endregion
    }
}
