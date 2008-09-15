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

        // TODO: This type needs to be converted to a complex type, to 
        //       record
        public string Type { get; set; }
        public string Name { get; set; }
        public bool IsPrimaryKey { get; set; }

        public string Descriptor
        {
            get
            {
                if ( IsPrimaryKey )
                    return "(PK)";
                else
                {
                    string result = "";
                    if ( Nullability == Nullability.NotNullable )
                        result = "NOT ";

                    return result + "NULL";
                }
            }
        }

        public override int GetHashCode()
        {
            return Type.GetHashCode() + Name.GetHashCode() + IsPrimaryKey.GetHashCode() + Nullability.GetHashCode();
        }

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
