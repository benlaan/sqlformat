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

    public class SqlType
    {
        public SqlType( string name, int length, int scale ) : this ( name, length )
        {
            Scale = scale;
        }

        public SqlType( string name, int length ) : this ( name )
        {
            Length = length;
        }

        public SqlType( string name )
        {
            Name = name;
        }

        public string Name { get; set; }
        public int? Length { get; set; }
        public int? Scale { get; set; }

        public override string ToString()
        {
            string lengthDisplay = Length.HasValue ? String.Format( "({0})", Length ) : null;
            string precisionDisplay = Scale.HasValue ? String.Format( "({0}, {1})", Length, Scale ) : null;
            return String.Format( "{0}{1}", Name, precisionDisplay ?? lengthDisplay ?? "" );
        }
    }

    [DebuggerDisplay( "{Name}: {Type} {Descriptor}" )]
    public class FieldDefinition
    {
        public Nullability Nullability { get; set; }

        // TODO: This type needs to be converted to a complex type, to record the type name, length, precision, etc.
        public SqlType Type { get; set; }
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
                other.Type.Name.WithBrackets() == Type.Name.WithBrackets() &&
                other.Name.WithBrackets() == Name.WithBrackets() &&
                other.Nullability == Nullability &&
                other.IsPrimaryKey == IsPrimaryKey;
        }
    }
}
