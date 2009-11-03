using System;

namespace Laan.Sql.Parser.Expressions
{
    public class BetweenExpression : Expression
    {
        public BetweenExpression( Expression parent ) : base( parent )
        {

        }

        public Expression Expression { get; set; }
        public Expression From { get; set; }
        public Expression To { get; set; }
        public bool Negated { get; set; }

        public override string Value
        {
            get { 
                return String.Format(
                    "{0} {1}BETWEEN {2} AND {3}",
                    Expression.Value,
                    Negated ? "NOT " : "",
                    From.Value,
                    To.Value
                );
            }
        }

        public override bool CanInline
        {
            get { return Expression.CanInline && From.CanInline && To.CanInline && Value.Length < 80; }
        }
    }
}
