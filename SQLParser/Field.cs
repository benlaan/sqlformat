using System.Diagnostics;

namespace Laan.SQL.Parser
{
    [DebuggerDisplay( "{Name} ({Alias})" )]
    public class Field
    {
        public string Alias { get; set; }
        public string Name { get; set; }
    }
}
