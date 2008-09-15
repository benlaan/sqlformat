using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace Laan.SQLParser
{
    public enum Nullability
    {
        NotNullable,
        Nullable
    }

    [DebuggerDisplay( "{Name}: {Type} {Descriptor}" )]
    public class FieldDefinition
    {
        public Nullability Nullability { get; set; }
        public string Type { get; set; }
        public string Name { get; set; }
        public bool IsPrimaryKey { get; set; }

        public string Descriptor { get { return IsPrimaryKey ? "(PK)" : ( Nullability == Nullability.NotNullable ? "NOT " : "") + "NULL"; } }

        public override bool Equals( object obj )
        {
            FieldDefinition def = (FieldDefinition) obj;
            return 
                def.Type == Type && 
                def.Name == Name && 
                def.Nullability == Nullability && 
                def.IsPrimaryKey == IsPrimaryKey;
        }
    }
}
