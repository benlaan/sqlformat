using System;
using System.Linq;

namespace Laan.Sql.Parser.Expressions
{
    public class CountExpression : FunctionExpression
    {
        public CountExpression(Expression parent) : base(parent)
        {
            Name = Constants.Count;
        }

        public CountExpression(Expression parent, bool distinct) : this(parent)
        {
            Distinct = distinct;
        }

        public override bool CanInline
        {
            get { return true; }
        }

        public bool Distinct { get; set; }

        public override string Value
        {
            get
            {
                return String.Format("COUNT({0}{1})", Distinct ? "DISTINCT " : "", Arguments.First().Value);
            }
        }
    }
}
