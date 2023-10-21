using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

using Laan.Sql.Parser.Exceptions;

namespace Laan.Sql.Parser
{
    public enum TokenType
    {
        None, WhiteSpace, Numeric, AlphaNumeric, Variable, OpenBrace, CloseBrace, OpenBracket, CloseBracket,
        SingleQuote, DoubleQuote, BlockedText, Operator, Symbol, InLineComment, MultiLineComment, String, QuotedText
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

        private int ReadNextLine()
        {
            var linesConsumed = 0;

            while (_line.Length == 0 && _reader.Peek() != -1)
            {
                _line = _reader.ReadLine();
                Position.Row++;
                Position.Column = 1;

                linesConsumed++;
            }

            return linesConsumed;
        }

        private void AdvanceCurrentToken(Match match, TokenType type)
        {
            _current = new Token(match.Value, type);

            if (match.Success)
            {
                _line = _line.Remove(match.Captures[0].Index, match.Captures[0].Length);
                Position.Column += match.Value.Length;
            }
        }

        private CandidateDefinition GetCandidateDefinition(IList<CandidateDefinition> definitions)
        {
            var candidateDefinitions = definitions
                .Where(m => m.Match.Success)
                .OrderBy(m => m.Match.Captures[0].Index)
                .ThenBy(td => td.Position)
                .ToList();

            var matchingToken = candidateDefinitions.FirstOrDefault();

            if (matchingToken != null)
                AdvanceCurrentToken(matchingToken.Match, matchingToken.Definition.Type);

            return matchingToken;
        }

        private void ProcessMultiLine(CandidateDefinition matchingToken)
        {
            var multiLineType = _current.Type;
            var multiLineToken = new System.Text.StringBuilder(_current.Value);

            Match continuation;
            do
            {
                continuation = matchingToken.Definition.MultiLineContinuation.Match(_line);
                if (continuation.Value.Length > 0)
                {
                    AdvanceCurrentToken(
                        continuation,
                        matchingToken.Definition.Type
                    );

                    var linesConsumed = ReadNextLine();
                    multiLineToken.Append(_current.Value);
                    for (int i = 0; i < linesConsumed; i++)
                        multiLineToken.AppendLine();
                }
            }
            while (HasMoreTokens && continuation != null && continuation.Value.Length > 0);

            Match terminationMatch = matchingToken.Definition.MultiLineTerminator.Match(_line);
            if (terminationMatch.Value.Length == 0)
                throw new SyntaxException(String.Format("Failed to find terminal for {0}", matchingToken.Definition.Type));

            AdvanceCurrentToken(terminationMatch, matchingToken.Definition.Type);
            multiLineToken.Append(_current.Value);

            _current = new Token(multiLineToken.ToString(), multiLineType);
        }

        public override void ReadNextToken()
        {
            _current = null;
            ReadNextLine();

            if (!HasMoreTokens)
                return;

            var m = TokenDefinitions
                .Select((td, i) => new CandidateDefinition(i, td.Regex.Match(_line), td))
                .ToList();

            var matchingToken = GetCandidateDefinition(m);
            if (matchingToken == null)
                throw new SyntaxException();

            if (matchingToken.Definition.IsMultiLine)
                ProcessMultiLine(matchingToken);

            if (matchingToken.Definition.Skip)
                ReadNextToken();
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

        public void Dispose()
        {
            _reader.Dispose();
        }
    }
}
