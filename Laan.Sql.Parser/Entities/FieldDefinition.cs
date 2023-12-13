using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using Laan.Sql.Parser.Expressions;

namespace Laan.Sql.Parser.Entities
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

    [DebuggerDisplay( "{Name}: {Type} {Description}" )]
    public class FieldDefinition
    {
        public Nullability Nullability { get; set; }

        public SqlType Type { get; set; }
        public string Name { get; set; }
        public bool IsPrimaryKey { get; set; }
        public Identity Identity { get; set; }
        public Expression CalculatedValue { get; set; }
        public bool IsCalculated { get { return CalculatedValue != null; } }

        private string Description
        {
            get
            {
                string result = "";
                if ( Identity != null )
                    result = Identity.ToString() + " ";

                if ( IsPrimaryKey )
                    return result + "PRIMARY KEY ";
                else
                {
                    if ( Nullability == Nullability.NotNullable )
                        result += "NOT ";

                    return result += "NULL";
                }
            }
        }

        public override string ToString()
        {
            return String.Format( "{0} {1} {2}", Name, Type, Description );
        }

        public override int GetHashCode()
        {
            return
                Type.GetHashCode() +
                Name.GetHashCode() +
                IsPrimaryKey.GetHashCode() +
                Nullability.GetHashCode();
        }

        private bool CompareIdentity( FieldDefinition other )
        {
            if ( Identity == null && other.Identity == null )
                return true;

            if ( ( Identity != null && other.Identity == null ) || ( other.Identity != null && Identity == null ) )
                return false;

            return other.Identity.Equals( Identity );
        }

        public override bool Equals( object fieldDefinition )
        {
            var other = (FieldDefinition) fieldDefinition;
            return
                other.Type.Name.WithBrackets() == Type.Name.WithBrackets() &&
                other.Name.WithBrackets() == Name.WithBrackets() &&
                other.Nullability == Nullability &&
                other.IsPrimaryKey == IsPrimaryKey &&
                CompareIdentity( other );
        }
    }
}
