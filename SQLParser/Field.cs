using System.Diagnostics;

namespace SQLParser
{
    public enum Nullability
    {
        NotNullable,
        Nullable
    }

    [DebuggerDisplay( "{Name} ({Alias})" )]
    public class Field
    {
        public string Alias { get; set; }
        public string Name { get; set; }
    }
}
