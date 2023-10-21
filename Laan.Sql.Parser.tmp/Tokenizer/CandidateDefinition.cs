using System;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;

namespace Laan.Sql.Parser
{
    [DebuggerDisplay("\\{ Position = {Position}, Match = {Match}, Definition = {Definition} \\}")]
    public class CandidateDefinition
    {
        public CandidateDefinition(int position, Match match, TokenDefinition definition)
        {
            Position = position;
            Match = match;
            Definition = definition;
        }

        public override string ToString()
        {
            return $"{{ Position = {Position}, Match = {Match}, Definition = {Definition} }}";
        }

        public int Position { get; private set; }
        public Match Match { get; private set; }
        public TokenDefinition Definition { get; private set; }
    }
}
