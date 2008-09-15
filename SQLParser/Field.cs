using System.Diagnostics;

namespace Laan.SQLParser
{
    [DebuggerDisplay( "{Name} ({Alias})" )]
    public class Field
    {
        public string Alias { get; set; }
        public string Name { get; set; }
    }
}
