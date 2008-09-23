using System;

namespace Laan.SQL.Parser
{
    public class Expression
    {
        public virtual string Value { get; set; }
    }

    public class NestedSelectExpression
    {
    }

    public class BinaryExpression
    {
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