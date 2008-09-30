using System;

namespace Laan.SQL.Parser
{
    public class FieldExpression : Expression
    {
        internal FieldExpression()
        {
        }

        public override string Value { get; set; }
    }

    public class Expression
    {
        public virtual string Value { get; set; }
    }

    public class CriteriaExpression
    {
        public CriteriaExpression()
        {
            Left = new Expression();
            Right = new Expression();
        }

        public Expression Left { get; set; }
        public Expression Right { get; set; }
        public string Operator { get; set; }

        public string Value
        {
            get
            {
                return String.Format( "{0} {1} {2}", Left.Value, Operator, Right.Value );
            }
        }

    }
}