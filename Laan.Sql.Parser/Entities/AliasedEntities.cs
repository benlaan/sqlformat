using System;
using System.Collections.Generic;

using Laan.Sql.Parser.Expressions;

namespace Laan.Sql.Parser.Entities
{
    public class AliasedEntity : Expression
    {
        public AliasedEntity() : base(null)
        {
            Alias = new Alias(this);
        }

        public Alias Alias { get; set; }
    }

    public enum AliasType { None, Implicit, As, Equals }

    public class Alias : Expression
    {
        public Alias(Expression parent) : base(parent)
        {
            Type = AliasType.Implicit;
        }

        public string Name { get; set; }
        public AliasType Type { get; set; }

        public override string Value
        {
            get
            {
                switch (Type)
                {
                    case AliasType.Implicit:
                        return !String.IsNullOrEmpty(Name) ? String.Format(" {0}", Name) : String.Empty;

                    case AliasType.Equals:
                        return !String.IsNullOrEmpty(Name) ? String.Format("{0} = ", Name) : String.Empty;

                    case AliasType.As:
                        return String.Format(" AS {0}", Name);

                    default:
                        return String.Empty;
                }
            }
            protected set { base.Value = value; }
        }
    }

}
