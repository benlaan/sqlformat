using System;
using System.Collections.Generic;

using Laan.SQL.Parser.Expressions;

namespace Laan.SQL.Parser
{
    public class AliasedEntity : Expression
    {
        public AliasedEntity() : base ( null )
        {
            Alias = new Alias( this );
        }

        public Alias Alias { get; set; }
    }

    public enum AliasType { None, Implicit, As, Equals }

    public class Alias : Expression
    {
        public Alias( Expression parent ) : base ( parent )
        {
            Type = AliasType.Implicit;
        }

        public string Name { get; set; }
        public AliasType Type { get; set; }

        public override string Value
        {
            get
            {
                string format = "";
                switch ( Type )
                {
                    case AliasType.Implicit:
                        format = !String.IsNullOrEmpty( Name ) ? String.Format( " {0}", Name ) : "";
                        break;

                    case AliasType.Equals:
                        format = !String.IsNullOrEmpty( Name ) ? String.Format( "{0} = ", Name ) : "";
                        break;

                    case AliasType.As:
                        format = String.Format( " AS {0}", Name );
                        break;
                }
                return format;
            }
            protected set { base.Value = value; }
        }
    }

}
