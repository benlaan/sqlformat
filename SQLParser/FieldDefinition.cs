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

    public class FieldDefinitions : List<FieldDefinition>
    {
        public FieldDefinition FindByName( string name )
        {
            return base.Find( field => field.Name.WithBrackets() == name.WithBrackets() );
        }
    }

    [DebuggerDisplay( "{Name}: {Type} {Descriptor}" )]
    public class FieldDefinition
    {
        public Nullability Nullability { get; set; }

        // TODO: This type needs to be converted to a complex type, to record the type name, length, precision, etc.
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
            return 
                Type.GetHashCode() + 
                Name.GetHashCode() + 
                IsPrimaryKey.GetHashCode() + 
                Nullability.GetHashCode();
        }

        public override bool Equals( object fieldDefinition )
        {
            var other = (FieldDefinition) fieldDefinition;
            return
                other.Type == Type &&
                other.Name.WithBrackets() == Name.WithBrackets() &&
                other.Nullability == Nullability &&
                other.IsPrimaryKey == IsPrimaryKey;
        }
    }
}
