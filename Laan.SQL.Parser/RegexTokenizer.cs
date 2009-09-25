using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;
using System.Diagnostics;

namespace Laan.SQL.Parser
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
        QuotedText,
        BlockedText,
        Operator,
        Symbol,
        InLineComment,
        MultiLineComment
    }

    public class TokenDefinition
    {
        public Regex Regex;
        public bool Skip;
        public TokenType Type;

        public TokenDefinition( TokenType type, bool skip, string regex )
        {
            Skip = skip;
            Regex = new Regex( regex, RegexOptions.Compiled );
            Type = type;
        }
    }

    [DebuggerDisplay( "Current: {Current} Position: {Position} [{HasMoreTokens ? \"Y\" : \"N\"}]" )]
    public abstract class RegexTokenizer : CustomTokenizer, IDisposable
    {
        private TextReader _reader;
        private Token _current;

        public RegexTokenizer( string input )
        {
            TokenDefinitions = new List<TokenDefinition>();
            Position = new Position( this );

            _reader = new StringReader( input );
        }

        private int ReadChar()
        {
            int read = _reader.Read();
            if ( read == '\n' )
                Position.NewRow();
            else
                Position.Column++;

            return read;
        }
        
        /// <summary>
        /// Returns the number of matches that the current token could be
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        private int MatchCount( string token, out TokenDefinition definition )
        {
            var matches = TokenDefinitions.Where(
                tokenDefinition => 
                    tokenDefinition.Regex.Matches( token ).Cast<Match>().Any( match => match.Value == token )
            );

            definition = matches.FirstOrDefault();
            return matches.Count();
        }

        public override void ReadNextToken()
        {
            _current = null;
            var current = new StringBuilder();
            TokenDefinition definition;
            TokenDefinition last;

            int next = -1;
            while ( ( next = _reader.Peek() ) != -1 )
            {
                int matchCount = MatchCount( current.ToString() + (char) next, out definition );
                last = definition;
                int lastMatchCount = 0;

                while ( _reader.Peek() != -1 && ( matchCount > 0 || ( lastMatchCount == 0 ) ) )
                {
                    last = definition;
                    ReadChar();
                    current.Append( (char) next );

                    next = _reader.Peek();
                    if ( next == -1 )
                        break;

                    lastMatchCount = matchCount;
                    matchCount = MatchCount( current.ToString() + (char) next, out definition );
                }

                if ( last != null && last.Skip )
                {
                    ReadNextToken();
                    return;
                }

                // if we get here, we either have a good token, or the token is not recognised
                if ( MatchCount( current.ToString(), out definition ) == 0 )
                    throw new UnknownTokenException( current.ToString() );

                _current = new Token( current.ToString(), last != null ? last.Type : TokenType.None );
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
