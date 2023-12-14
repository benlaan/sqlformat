using System;

namespace Laan.Sql.Parser.Expressions
{
    public class NegationExpression : Expression
    {
        /// <summary>
        /// Initializes a new instance of the NegationExpression class.
        /// </summary>
        public NegationExpression( Expression parent ) : base ( parent )
        {
        }

        public Expression Expression { get; set; }

        public override string Value
        {
            get { return String.Format( "{0} {1}", Constants.Not, Expression.Value ); }
        }

        public override bool CanInline
        {
            get { return Expression.CanInline; }
        }
    }
}
