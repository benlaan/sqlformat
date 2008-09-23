using System.Diagnostics;

namespace Laan.SQL.Parser
{
    [DebuggerDisplay( "{Expression.Value} ({Alias})" )]
    public class Field
    {
        public string Alias { get; set; }
        public Expression Expression { get; set; }
    }
}
